using System;
using System.Drawing;
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

    internal EventHandler NavigationCompleted;

    private readonly Control _webBrowser;

    internal string Html { get; private set; }

    internal static async Task<CoreWebView2Environment> InitWebView2()
    {
        if (_environment == null)
        {
            var options = new CoreWebView2EnvironmentOptions()
            {
                Language = "en-US",
            };

            _environment = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Path.GetTempPath(), "CCE2browser"), options);
        }

        return _environment;
    }

    public BrowserForm(string url)
    {
        _url = url;

        this.InitializeComponent();

        this.Icon = Properties.Resource.djdsoft;

        _webBrowser = this.InitWebBrowser();
    }

    #region InitWebBrowser

    private Control InitWebBrowser()
    {
        Control webBrowser;
        //switch (Program.SelectedBrowserControl)
        //{
        //    case BrowserControlSelection.FormsDefault:
        //        {
        //            webBrowser = this.InitWebBrowserFormsDefault();

        //            break;
        //        }
        //    case BrowserControlSelection.WebView:
        //        {
        //            webBrowser = this.InitWebBrowserWebView();

        //            break;
        //        }
        //    case BrowserControlSelection.WebView2:
        //        {
        webBrowser = this.InitWebBrowserWebView2();

        //            break;
        //        }
        //    default:
        //        {
        //            webBrowser = null;

        //            break;
        //        }
        //}

        if (webBrowser != null)
        {
            var supportInitialize = webBrowser as System.ComponentModel.ISupportInitialize;

            supportInitialize?.BeginInit();

            webBrowser.Name = "WebBrowser";
            webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webBrowser.Location = new Point(9, 64);
            webBrowser.Size = new Size(this.Size.Width - 15, this.Size.Height - 40);

            supportInitialize?.EndInit();

            this.Controls.Add(webBrowser);
        }

        return webBrowser;
    }

    //private Control InitWebBrowserFormsDefault()
    //{
    //    var webBrowser = new System.Windows.Forms.WebBrowser()
    //    {
    //        AllowWebBrowserDrop = false,
    //        ScriptErrorsSuppressed = true,
    //    };

    //    webBrowser.Navigated += this.OnWebBrowserNavigated;

    //    return webBrowser;
    //}

    //private Control InitWebBrowserWebView()
    //{
    //    var webBrowser = new Microsoft.Toolkit.Forms.UI.Controls.WebView();

    //    webBrowser.NavigationCompleted += this.OnWebViewNavigationCompleted;

    //    return webBrowser;
    //}

    private Control InitWebBrowserWebView2()
    {
        var webBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();

        webBrowser.NavigationCompleted += this.OnWebView2NavigationCompleted;

        return webBrowser;
    }

    #endregion

    #region OnWebBrowser

    //private async void OnWebBrowserNavigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
    //{
    //    if (_webBrowser is System.Windows.Forms.WebBrowser webBrowserForms)
    //    {
    //        for (var scrollIndex = 0; scrollIndex < 5; scrollIndex++)
    //        {
    //            await Task.Delay(1000);
    //        }

    //        try
    //        {
    //            var document = webBrowserForms.Document;

    //            var html = document.Body?.OuterHtml ?? string.Empty;

    //            this.Html = html;
    //        }
    //        catch
    //        {
    //            this.Html = string.Empty;
    //        }
    //    }

    //    NavigationCompleted?.Invoke(this, EventArgs.Empty);
    //}

    //private async void OnWebViewNavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
    //{
    //    if (_webBrowser is Microsoft.Toolkit.Forms.UI.Controls.WebView webView)
    //    {
    //        for (var scrollIndex = 0; scrollIndex < 5; scrollIndex++)
    //        {
    //            await Task.Delay(1000);

    //            await webView.InvokeScriptAsync("eval", ["window.scroll(0, document.body.scrollHeight);"]);
    //        }

    //        try
    //        {
    //            var html = await webView.InvokeScriptAsync("eval", ["document.documentElement.outerHTML;"]);

    //            this.Html = html;
    //        }
    //        catch
    //        {
    //            this.Html = string.Empty;
    //        }
    //    }

    //    NavigationCompleted?.Invoke(this, EventArgs.Empty);
    //}

    #endregion

    private async void OnWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (_webBrowser is Microsoft.Web.WebView2.WinForms.WebView2 webView2)
        {
            for (var scrollIndex = 0; scrollIndex < 5; scrollIndex++)
            {
                await Task.Delay(1000);

                await webView2.ExecuteScriptAsync("window.scroll(0, document.body.scrollHeight);");
            }

            try
            {
                var html = await webView2.ExecuteScriptAsync("document.documentElement.outerHTML");

                html = Regex.Unescape(html);

                html = html.Remove(0, 1);

                html = html.Remove(html.Length - 1, 1);

                this.Html = html;
            }
            catch
            {
                this.Html = string.Empty;
            }

            NavigationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void OnBrowserFormLoad(object sender, EventArgs e)
    {
        await this.InitialNavigate();
    }

    private async Task InitialNavigate()
    {
        //if (_webBrowser is System.Windows.Forms.WebBrowser webBrowserForms)
        //{
        //    webBrowserForms.Navigate(_url);
        //}
        //else if (_webBrowser is Microsoft.Toolkit.Forms.UI.Controls.WebView webView)
        //{
        //    webView.Source = new Uri(_url);
        //}
        //else
        if (_webBrowser is Microsoft.Web.WebView2.WinForms.WebView2 webView2)
        {
            var environment = await InitWebView2();

            await webView2.EnsureCoreWebView2Async(environment);

            webView2.Source = new Uri(_url);
        }
    }

    private void OnBrowserFormClosed(object sender, FormClosedEventArgs e)
    {
        //if (_webBrowser is System.Windows.Forms.WebBrowser webBrowserForms)
        //{
        //    webBrowserForms.Navigated -= this.OnWebBrowserNavigated;
        //}
        //else if (_webBrowser is Microsoft.Toolkit.Forms.UI.Controls.WebView webView)
        //{
        //    webView.NavigationCompleted -= this.OnWebViewNavigationCompleted;
        //}
        //else
        if (_webBrowser is Microsoft.Web.WebView2.WinForms.WebView2 webView2)
        {
            webView2.NavigationCompleted -= this.OnWebView2NavigationCompleted;
        }

        Dispose(_webBrowser);
    }

    private static void Dispose(object toDispose)
    {
        try
        {
            if (toDispose is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        catch
        {
        }
    }
}