using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Core
{
    /// <summary>
    /// 通信载体接口
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMessagesInterface
    {
        [OperationContract]
        string DoMessage(string text);
    }
}
