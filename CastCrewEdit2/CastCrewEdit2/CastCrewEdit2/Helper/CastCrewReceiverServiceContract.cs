﻿using System.ServiceModel;

namespace DoenaSoft.DVDProfiler.CastCrewCopyPaste
{
    [ServiceContract(Namespace = "http://DoenaSoft.CastCrewReceiver")]
    public interface ICastCrewReceiver
    {
        [OperationContract]
        string Receive(string xml);
    }

    public static class CastCrewReceiverServiceContract
    {
        public const string TcpAddress = "net.tcp://localhost:10001/castcrewreceiver";
    }
}