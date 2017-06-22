using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Core
{
    public class NamedPipeServer
    {
        private readonly ServiceHost serverHost;

        public string SelfName { get; set; }
        public List<string> ClientNames { get; set; }

        public NamedPipeServer(IMessagesInterface messageInterface, ClientNames selfName)
        {
            SelfName = selfName.ToString();
            ClientNames = new List<string>();
            RegisterClientName();
            serverHost = new ServiceHost(messageInterface);
            serverHost.AddServiceEndpoint((typeof(IMessagesInterface)), new NetNamedPipeBinding(), "net.pipe://localhost/Server/" + SelfName);
            serverHost.Open();
        }


        /// <summary>
        /// 调用远程 DoMessage，发送消息给远程
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="text">消息正文</param>
        /// <returns>发送结果</returns>
        public void SendMessage(string clientName, string text)
        {
            var msgInterface = GetMessagesInterface(clientName);
            if (msgInterface == null) return;
            try
            {
                msgInterface.DoMessage(text);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CloseChannel((ICommunicationObject)msgInterface);
            }
        }

        /// <summary>
        /// 打开通道 并 获取可供调用的远程接口
        /// </summary>
        /// <param name="clientKey"></param>
        /// <returns></returns>
        public IMessagesInterface GetMessagesInterface(string clientKey)
        {
            try
            {
                var factory = new ChannelFactory<IMessagesInterface>(new NetNamedPipeBinding(),
                    new EndpointAddress("net.pipe://localhost/Server/" + clientKey));
                return factory.CreateChannel();
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 关闭通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private string CloseChannel(ICommunicationObject channel)
        {
            try
            {
                channel.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                channel.Abort();
            }
            return null;
        }

        private void RegisterClientName()
        {
            foreach (var name in Enum.GetNames(typeof(ClientNames)))
            {
                ClientNames.Add(name);
            }
        }
    }

    public enum ClientNames
    {
        win1, win2, win3, win4
    }
}
