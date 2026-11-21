using Microsoft.Web.WebView2.Core;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    internal static bool IsRuntimeMissing(Exception exception)
        => exception is WebView2RuntimeNotFoundException or FileNotFoundException;

    internal static void ShowRuntimeMissingMessage(IWin32Window owner)
    {
        MessageBox.Show(owner,
            "The Microsoft Edge WebView2 Runtime could not be found." + Environment.NewLine
            + "Please install the \"Evergreen\" runtime from https://developer.microsoft.com/microsoft-edge/webview2/ and restart the application.",
            "WebView2 Runtime missing",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
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
            try
            {
                await this.ScrollUntilStable(webView2);

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

    #region Network activity tracking

    private int _pendingRequests;

    private long _lastNetworkActivityTicks = Environment.TickCount;

    private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        _pendingRequests++;

        _lastNetworkActivityTicks = Environment.TickCount;
    }

    private void OnWebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        if (_pendingRequests > 0)
        {
            _pendingRequests--;
        }

        _lastNetworkActivityTicks = Environment.TickCount;
    }

    /// <summary>
    /// Requests that never produce a response (failed, cancelled, long-polling) would keep the counter above zero forever,
    /// so a silent network for a while is also treated as idle.
    /// </summary>
    private bool IsNetworkIdle()
    {
        if (_pendingRequests <= 0)
        {
            return true;
        }

        return unchecked(Environment.TickCount - _lastNetworkActivityTicks) > 3000;
    }

    #endregion

    /// <summary>
    /// Scrolls down viewport by viewport (so that content that loads when it enters the viewport gets triggered),
    /// and finishes once the bottom is reached, the document is complete, the page has stopped growing
    /// and no requests are in flight, or until a timeout is hit.
    /// </summary>
    private async Task ScrollUntilStable(Microsoft.Web.WebView2.WinForms.WebView2 webView2)
    {
        const string scrollScript = "(function(){var e=document.scrollingElement||document.documentElement;"
            + "var step=Math.max(window.innerHeight*0.8,200);"
            + "window.scrollBy(0,step);"
            + "var atBottom=(window.scrollY+window.innerHeight)>=(e.scrollHeight-2);"
            + "return document.readyState+'|'+e.scrollHeight+'|'+document.querySelectorAll('*').length+'|'+(atBottom?1:0);})()";

        var timeout = TimeSpan.FromSeconds(60);
        var pollInterval = TimeSpan.FromMilliseconds(300);
        var requiredStableChecks = 3;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        string lastState = null;
        var stableChecks = 0;

        while (stopwatch.Elapsed < timeout)
        {
            await Task.Delay(pollInterval);

            // "readyState|height|elementCount|atBottom" - height or DOM growth means more content arrived
            var state = await webView2.ExecuteScriptAsync(scrollScript);

            var isReady = state != null && state.StartsWith("\"complete|", StringComparison.Ordinal);

            var atBottom = state != null && state.EndsWith("|1\"", StringComparison.Ordinal);

            if (isReady && atBottom && state == lastState && this.IsNetworkIdle())
            {
                stableChecks++;

                if (stableChecks >= requiredStableChecks)
                {
                    return;
                }
            }
            else
            {
                stableChecks = 0;
            }

            lastState = state;
        }
    }

    private async void OnBrowserFormLoad(object sender, EventArgs e)
    {
        try
        {
            await this.InitialNavigate();
        }
        catch
        {
            // Without this the caller would wait forever for NavigationCompleted
            this.Html = string.Empty;

            NavigationCompleted?.Invoke(this, EventArgs.Empty);
        }
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

            webView2.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView2.CoreWebView2.WebResourceRequested += this.OnWebResourceRequested;
            webView2.CoreWebView2.WebResourceResponseReceived += this.OnWebResourceResponseReceived;

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

            if (webView2.CoreWebView2 != null)
            {
                webView2.CoreWebView2.WebResourceRequested -= this.OnWebResourceRequested;
                webView2.CoreWebView2.WebResourceResponseReceived -= this.OnWebResourceResponseReceived;
            }
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