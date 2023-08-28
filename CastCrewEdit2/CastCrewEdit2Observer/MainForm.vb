Imports System.IO
Imports System.Windows.Forms
Imports DoenaSoft.DVDProfiler.CastCrewEdit2

Public Class MainForm

    Private CastCrewEditManager As CastCrewEditManager = Nothing

    Private Sub HandleFormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim path As String = My.Settings.Default.CastCrewEditPath

        If File.Exists(path) Then
            CastCrewEditExe.Text = path
        End If

        LanguageComboBox.SelectedIndex = 0
        BrowserComboBox.SelectedIndex = 1
    End Sub

    Private Sub HandleFormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If (CastCrewEditManager IsNot Nothing) Then
            RemoveHandler CastCrewEditManager.CastCompleted, AddressOf HandleCastCompleted
            RemoveHandler CastCrewEditManager.CrewCompleted, AddressOf HandleCrewCompleted

            CastCrewEditManager.Dispose()

            CastCrewEditManager = Nothing
        End If

        My.Settings.Default.CastCrewEditPath = CastCrewEditExe.Text
        My.Settings.Default.Save()
    End Sub

    Private Sub HandleStartCastCrewButtonClick(sender As Object, e As EventArgs) Handles StartCastCrewButton.Click
        If String.IsNullOrWhiteSpace(CastCrewEditExe.Text) OrElse Not File.Exists(CastCrewEditExe.Text) Then
            MessageBox.Show("Cast/Crew Edit 2 does not exist in selected location", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If
        If (CastCrewEditManager Is Nothing AndAlso CastCrewEditExe.Text <> String.Empty) Then
            Dim exe As New FileInfo(CastCrewEditExe.Text)

            If exe.Exists Then
                Try
                    CastCrewEditManager = New CastCrewEditManager(exe)

                    AddHandler CastCrewEditManager.CastCompleted, AddressOf HandleCastCompleted
                    AddHandler CastCrewEditManager.CrewCompleted, AddressOf HandleCrewCompleted
                Catch ex As ApplicationException
                    MessageBox.Show(ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Exit Sub
                Catch ex As Exception
                    MessageBox.Show("Could not properly start Cast/Crew Edit 2" + Environment.NewLine + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Exit Sub
                End Try
            End If
        End If

        Try
            CastCrewEditManager.Run(LanguageComboBox.Text, BrowserComboBox.Text)
        Catch ex As Exception
            MessageBox.Show(ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HandleCrewCompleted(sender As Object, e As XmlEventArgs)
        MessageBox.Show(e.Xml)

        If (CastCrewEditManager IsNot Nothing) Then
            CastCrewEditManager.Kill()
        End If
    End Sub

    Private Sub HandleCastCompleted(sender As Object, e As XmlEventArgs)
        MessageBox.Show(e.Xml)
    End Sub
End Class
