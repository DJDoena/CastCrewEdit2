using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;

internal partial class BrowserForm : Form
{
    private readonly string _url;

    private static CoreWebView2Environment _environment;

    internal EventHandler NavidationCompleted;

    internal string Html { get; private set; }

    internal static async Task<CoreWebView2Environment> InitWebView2()
    {
        if (_environment == null)
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--lang=en-US");

            _environment = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Path.GetTempPath(), "CCE2browser"));
        }

        return _environment;
    }

    public BrowserForm(string url)
    {
        _url = url;

        this.InitializeComponent();

        WebBrowser.NavigationCompleted += this.OnWebView2NavigationCompleted;
    }

    private async Task<string> GetHtml()
    {
        try
        {
            var html = await WebBrowser.ExecuteScriptAsync("document.documentElement.outerHTML");

            html = Regex.Unescape(html);

            html = html.Remove(0, 1);

            html = html.Remove(html.Length - 1, 1);

            return html;
        }
        catch
        {
            return string.Empty;
        }
    }


    private async void OnWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        await WebBrowser.ExecuteScriptAsync("window.scroll(0, document.body.scrollHeight);");

        await Task.Delay(1000);

        await WebBrowser.ExecuteScriptAsync("window.scroll(0, document.body.scrollHeight);");

        await Task.Delay(1000);

        await WebBrowser.ExecuteScriptAsync("window.scroll(0, document.body.scrollHeight);");

        this.Html = await this.GetHtml();

        NavidationCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async void OnBrowserFormLoad(object sender, EventArgs e)
    {
        await this.InitialNavigate();
    }

    private async Task InitialNavigate()
    {
        var environment = await InitWebView2();

        await WebBrowser.EnsureCoreWebView2Async(environment);

        WebBrowser.Source = new Uri(_url);
    }

    private void OnBrowserFormClosed(object sender, FormClosedEventArgs e)
    {
        WebBrowser.NavigationCompleted -= this.OnWebView2NavigationCompleted;

        try
        {
            WebBrowser.Dispose();
        }
        catch
        {
        }
    }
}
