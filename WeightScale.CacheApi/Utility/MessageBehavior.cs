namespace WeightScale.CacheApi.Utility
{
    using System.ServiceModel.Description;

    public class MessageBehavior : IEndpointBehavior
    {
        string _username;
        string _password;

        public MessageBehavior(string username, string password)
        {
            _username = username;
            _password = password;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageInspector(_username, _password));
        }

        void IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
        }
    }
}