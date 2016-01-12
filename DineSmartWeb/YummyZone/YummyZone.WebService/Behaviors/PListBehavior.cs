using System;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;

namespace YummyZone.WebService
{
    public class PListBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        private static PListWebHttpBehavior Instance { get; set; }

        public PListBehavior()
        {
            Instance = new PListWebHttpBehavior();
        }

        public override Type BehaviorType
        {
            get 
            { 
                return typeof(PListBehavior); 
            }
        }

        protected override object CreateBehavior()
        {
            return new PListBehavior();
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            Instance.AddBindingParameters(endpoint, bindingParameters);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            Instance.ApplyClientBehavior(endpoint, clientRuntime);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Instance.ApplyDispatchBehavior(endpoint, endpointDispatcher);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}