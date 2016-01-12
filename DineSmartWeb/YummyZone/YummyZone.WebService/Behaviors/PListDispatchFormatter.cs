using System;
using System.Xml;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace YummyZone.WebService
{
    public class PListDispatchFormatter : IDispatchMessageFormatter
    {
        private OperationDescription operationDescription;

        public PListDispatchFormatter(OperationDescription operationDescription)
        {
            this.operationDescription = operationDescription;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.DeserializeRequest", Guid.Empty, "Method started", null);

                if (OperationContext.Current == null ||
                    OperationContext.Current.RequestContext == null ||
                    OperationContext.Current.RequestContext.RequestMessage == null ||
                    OperationContext.Current.RequestContext.RequestMessage.Headers == null ||
                    OperationContext.Current.RequestContext.RequestMessage.Headers.To == null)
                {
                    Logger.LogAsInfo("DeserializeRequest", Guid.Empty, "URL couldn't be retrieved from OperationContext");
                    return;
                }

                if (parameters == null || parameters.Length == 0)
                {
                    Logger.LogAsInfo("DeserializeRequest", Guid.Empty, "No parameters passed to the DeserializeRequest");
                    return;
                }

                XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
                if (bodyReader == null)
                {
                    Logger.LogAsInfo("DeserializeRequest", Guid.Empty, "GetReaderAtBodyContents returned NULL");
                    return;
                }

                using (bodyReader)
                {
                    MessagePartDescriptionCollection inputs = this.operationDescription.Messages[0].Body.Parts;

                    if (inputs.Count != parameters.Length)
                    {
                        string error = String.Format(
                            CultureInfo.InvariantCulture,
                            "Input count ('{0}') and parameter count ('{1}') are different",
                            inputs.Count,
                            parameters.Length);

                        Logger.LogAsError("DeserializeRequest", Guid.Empty, error);

                        throw new YummyZoneException(error);
                    }

                    for (int x = 0; x < inputs.Count; x++)
                    {
                        MessagePartDescription input = inputs[x];
                        XmlObjectSerializer serializer = null;

                        Uri uri = OperationContext.Current.RequestContext.RequestMessage.Headers.To;
                        string pathAndQuery = uri.PathAndQuery.ToLower();

                        if (pathAndQuery.Contains("format=plist"))
                        {
                            serializer = new PListSerializer(input.Type);
                        }
                        else if (pathAndQuery.Contains("format=json"))
                        {
                            serializer = new DataContractJsonSerializer(input.Type);
                        }
                        else
                        {
                            serializer = new DataContractSerializer(input.Type);
                        }

                        parameters[x] = serializer.ReadObject(bodyReader);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListDispatchFormatter.DeserializeRequest", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.SerializeReply", Guid.Empty, "Method started", null);

                if (OperationContext.Current == null ||
                    OperationContext.Current.RequestContext == null ||
                    OperationContext.Current.RequestContext.RequestMessage == null ||
                    OperationContext.Current.RequestContext.RequestMessage.Headers == null ||
                    OperationContext.Current.RequestContext.RequestMessage.Headers.To == null)
                {
                    Logger.LogAsInfo("SerializeReply", Guid.Empty, "URL couldn't be retrieved from OperationContext");
                    return null;
                }

                if (this.operationDescription == null || this.operationDescription.Messages.Count < 2)
                {
                    Logger.LogAsInfo("SerializeReply", Guid.Empty, "No reply message in the OperationDescription");
                    return null;
                }

                XmlObjectSerializer serializer = null;
                MessageDescription reply = this.operationDescription.Messages[1];
                Type resultType = reply.Body.ReturnValue.Type;

                Uri uri = OperationContext.Current.RequestContext.RequestMessage.Headers.To;
                string pathAndQuery = uri.PathAndQuery.ToLower();

                if (pathAndQuery.Contains("format=plist"))
                {
                    serializer = new PListSerializer(resultType);
                }
                else if (pathAndQuery.Contains("format=json"))
                {
                    serializer = new DataContractJsonSerializer(resultType);
                }
                else
                {
                    serializer = new DataContractSerializer(resultType);
                }

                return Message.CreateMessage(messageVersion, reply.Action, result, serializer);
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListDispatchFormatter.SerializeReply", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }
    }
}