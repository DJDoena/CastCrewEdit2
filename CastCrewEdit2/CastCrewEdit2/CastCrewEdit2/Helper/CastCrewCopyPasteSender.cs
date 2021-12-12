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

            var json = EncodeJson(xml);

            await TrySend(json);
        }

        private static string EncodeJson(string xml)
        {
            var json = xml.Replace("\r", "\\r");

            json = json.Replace("\n", "\\n");

            json = json.Replace("\t", "\\t");

            json = json.Replace("\"", "\\\"");

            return $"\"{json}\"";
        }

        private static async Task TrySend(string json)
        {
            try
            {
                using (var content = new StringContent(json, Encoding.GetEncoding("utf-8")))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                    await Send(content);
                }
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

        private static async Task Send(StringContent content)
        {
            using (var request = new HttpRequestMessage()
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri("http://localhost:10001/api/Receiver/Receive"),
                Content = content,
            })
            {
                var response = await HttpClient.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    message = message?.Trim().Trim('{', '}').Trim();

                    throw new Exception($"Response Code: {response.StatusCode}, {message}");
                }
            }
        }
    }
}