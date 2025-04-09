namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;

partial class BrowserForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.WebBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.WebBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // WebBrowser
            // 
            this.WebBrowser.AllowExternalDrop = true;
            this.WebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebBrowser.CreationProperties = null;
            this.WebBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebBrowser.Location = new System.Drawing.Point(12, 12);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.Size = new System.Drawing.Size(776, 426);
            this.WebBrowser.TabIndex = 1;
            this.WebBrowser.ZoomFactor = 1D;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.WebBrowser);
            this.Name = "BrowserForm";
            this.Text = "BrowserForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnBrowserFormClosed);
            this.Load += new System.EventHandler(this.OnBrowserFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.WebBrowser)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private Microsoft.Web.WebView2.WinForms.WebView2 WebBrowser;
}