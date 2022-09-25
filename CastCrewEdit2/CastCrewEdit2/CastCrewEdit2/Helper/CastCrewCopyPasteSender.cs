namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DoenaSoft.DVDProfiler.CastCrewCopyPaste;
    using Resources;

    internal static class CastCrewCopyPasteSender
    {
        private static HttpClient _httpClient;

        private static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }

                return _httpClient;
            }
        }

        internal static Task Send(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return Task.CompletedTask;
            }

            TrySend(xml);

            return Task.CompletedTask;
        }

        private static void TrySend(string xml)
        {
            try
            {
                ExecuteSend(xml);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                if (ex is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    MessageBox.Show(MessageBoxTexts.CastCrewCopyPasteUnreachable, MessageBoxTexts.CastCrewCopyPasteUnreachableHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, MessageBoxTexts.CastCrewCopyPasteUnreachableHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static void ExecuteSend(string xml)
        {
            var binding = new NetTcpBinding();

            binding.Security.Mode = SecurityMode.None;

            var channel = new ChannelFactory<ICastCrewReceiver>(binding, new EndpointAddress(CastCrewReceiverServiceContract.TcpAddress));

            var serviceClient = channel.CreateChannel();

            var response = serviceClient.Receive(xml);

            if (response != "OK")
            {
                throw new Exception(response);
            }

            (serviceClient as IDisposable)?.Dispose();
        }
    }
}