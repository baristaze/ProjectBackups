using System;
using System.Xml;
using System.Globalization;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace YummyZone.WebService
{
    public class PListWebHttpBehavior : WebHttpBehavior
    {
        /// <summary>
        /// This method is called only once when the Service is initialized first time.
        /// It simply determines which method will be using what dispatcher in Reply.
        /// Dispatchers will be responsible to serialize/de-serialize of the inputs.
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            try
            {
                Logger.LogAsVerbose("IDispatchMessageFormatter.GetReplyDispatchFormatter", Guid.Empty, "Method started", null);

                if (operationDescription.Behaviors.Find<PListBehaviorAttribute>() != null)
                {
                    // Messages[1] is the reply message
                    MessageDescription replyDesc = operationDescription.Messages[1];
                    Type returnType = replyDesc.Body.ReturnValue.Type;

                    // if the return type is void, then we don't need to involve the plist
                    if (returnType != typeof(void))
                    {
                        // if this is a primitive type, then we don't need to involve the plist
                        if (returnType.IsClass && returnType != typeof(string))
                        {
                            Logger.LogAsVerbose(
                                "GetReplyDispatchFormatter",
                                Guid.Empty,
                                "PListDispatchFormatter will be used for the Reply of Method {0}",
                                operationDescription.SyncMethod.Name);

                            return new PListDispatchFormatter(operationDescription);
                        }
                    }
                }

                return base.GetReplyDispatchFormatter(operationDescription, endpoint);
            }
            catch (Exception ex)
            {
                Logger.LogAsError("IDispatchMessageFormatter.GetReplyDispatchFormatter", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        /// <summary>
        /// This method is called only once when the Service is initialized first time.
        /// It simply determines which method will be using what dispatcher in Request.
        /// Dispatchers will be responsible to serialize/de-serialize of the inputs.
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            try
            {
                Logger.LogAsVerbose("IDispatchMessageFormatter.GetRequestDispatchFormatter", Guid.Empty, "Method started", null);

                if (operationDescription.Behaviors.Find<PListBehaviorAttribute>() != null)
                {
                    // Messages[0] is the request message
                    MessageDescription requestDesc = operationDescription.Messages[0];
                    MessagePartDescriptionCollection inputs = requestDesc.Body.Parts;

                    // if there is not any input, then we don't need to de-serialize anything
                    if (inputs.Count > 0)
                    {
                        bool hasComplexInput = false;
                        foreach (MessagePartDescription input in inputs)
                        {
                            if (input.Type.IsClass && input.Type != typeof(string))
                            {
                                hasComplexInput = true;
                                break;
                            }
                        }

                        // if all inputs are primitive, then we don't need to involve plist
                        if (hasComplexInput)
                        {
                            Logger.LogAsVerbose(
                                "GetRequestDispatchFormatter",
                                Guid.Empty,
                                "PListDispatcher will be used for the Request of Method {0}",
                                operationDescription.SyncMethod.Name);

                            return new PListDispatchFormatter(operationDescription);
                        }
                    }
                }

                return base.GetRequestDispatchFormatter(operationDescription, endpoint);
            }
            catch (Exception ex)
            {
                Logger.LogAsError("IDispatchMessageFormatter.GetRequestDispatchFormatter", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }
    }
}