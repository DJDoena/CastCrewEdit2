Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.CastCrewEditExe = New System.Windows.Forms.TextBox()
        Me.StartCastCrewButton = New System.Windows.Forms.Button()
        Me.LanguageComboBox = New System.Windows.Forms.ComboBox()
        Me.BrowserComboBox = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'CastCrewEditExe
        '
        Me.CastCrewEditExe.Location = New System.Drawing.Point(12, 12)
        Me.CastCrewEditExe.Name = "CastCrewEditExe"
        Me.CastCrewEditExe.Size = New System.Drawing.Size(897, 20)
        Me.CastCrewEditExe.TabIndex = 0
        '
        'StartCastCrewButton
        '
        Me.StartCastCrewButton.Location = New System.Drawing.Point(772, 38)
        Me.StartCastCrewButton.Name = "StartCastCrewButton"
        Me.StartCastCrewButton.Size = New System.Drawing.Size(137, 23)
        Me.StartCastCrewButton.TabIndex = 1
        Me.StartCastCrewButton.Text = "Run Cast/Crew Edit 2"
        Me.StartCastCrewButton.UseVisualStyleBackColor = True
        '
        'LanguageComboBox
        '
        Me.LanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.LanguageComboBox.FormattingEnabled = True
        Me.LanguageComboBox.Items.AddRange(New Object() {"/lang=en", "/lang=de"})
        Me.LanguageComboBox.Location = New System.Drawing.Point(12, 40)
        Me.LanguageComboBox.Name = "LanguageComboBox"
        Me.LanguageComboBox.Size = New System.Drawing.Size(180, 21)
        Me.LanguageComboBox.TabIndex = 2
        '
        'BrowserComboBox
        '
        Me.BrowserComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BrowserComboBox.FormattingEnabled = True
        Me.BrowserComboBox.Items.AddRange(New Object() {"/browser=ie6", "/browser=webview", "/browser=webviewcompatible", "/browser=webview2"})
        Me.BrowserComboBox.Location = New System.Drawing.Point(12, 67)
        Me.BrowserComboBox.Name = "BrowserComboBox"
        Me.BrowserComboBox.Size = New System.Drawing.Size(180, 21)
        Me.BrowserComboBox.TabIndex = 3
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(921, 225)
        Me.Controls.Add(Me.BrowserComboBox)
        Me.Controls.Add(Me.LanguageComboBox)
        Me.Controls.Add(Me.StartCastCrewButton)
        Me.Controls.Add(Me.CastCrewEditExe)
        Me.Name = "MainForm"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CastCrewEditExe As TextBox
    Friend WithEvents StartCastCrewButton As Button
    Friend WithEvents LanguageComboBox As ComboBox
    Friend WithEvents BrowserComboBox As ComboBox
End Class
