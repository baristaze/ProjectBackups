using System;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace YummyZone.WebService
{
    public class PListBehaviorAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) { }
        public void Validate(OperationDescription operationDescription) { }
    }
}