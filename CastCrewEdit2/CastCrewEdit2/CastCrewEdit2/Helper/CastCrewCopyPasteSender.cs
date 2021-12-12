namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

        internal static async Task Send(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return;
            }

            var json = JsonConvert.SerializeObject(xml);

            await TrySend(json);
        }

        private static async Task TrySend(string json)
        {
            try
            {
                await ExecuteSend(json);
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

        private static async Task ExecuteSend(string json)
        {
            using (var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                using (var request = new HttpRequestMessage()
                {
                    Method = new HttpMethod("POST"),
                    RequestUri = new Uri("http://localhost:10001/api/Receiver/Receive"),
                    Content = content,
                })
                {
                    using (var response = await HttpClient.SendAsync(request))
                    {
                        await CheckResponse(response);
                    }
                }
            }
        }

        private static async Task CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = await response.Content.ReadAsStringAsync();

                try
                {
                    var jo = JObject.Parse(message);

                    var innerMessage = jo.GetValue("Message").ToString();

                    if (!string.IsNullOrEmpty(innerMessage))
                    {
                        message = innerMessage;
                    }
                }
                catch
                { }

                throw new Exception($"Response Code: {response.StatusCode}, {message}");
            }
        }
    }
}