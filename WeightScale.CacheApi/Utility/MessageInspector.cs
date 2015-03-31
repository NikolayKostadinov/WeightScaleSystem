namespace WeightScale.CacheApi.Utility
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    public class MessageInspector : IClientMessageInspector
    {
        string _username;
        string _password;

        public MessageInspector(string username, string password)
        {
            _username = username;
            _password = password;
        }


        void IClientMessageInspector.AfterReceiveReply(ref System.ServiceModel.Channels.Message reply,
            Object correlationState)
        {
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string headerText = "<wsse:UsernameToken xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">" +
                                        "<wsse:Username>{0}</wsse:Username>" +
                                        "<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{1}</wsse:Password>" +
                                    "</wsse:UsernameToken>";// +

            headerText = string.Format(headerText, _username, _password);

            XmlDocument MyDoc = new XmlDocument();
            MyDoc.LoadXml(headerText);
            XmlElement myElement = MyDoc.DocumentElement;

            System.ServiceModel.Channels.MessageHeader myHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", myElement, false);
            request.Headers.Add(myHeader);
            return Convert.DBNull;
        }
    }
}

