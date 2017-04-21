
Public Class XmlEventArgs
    Inherits EventArgs

    Private m_Xml As String

    Public ReadOnly Property Xml As String
        Get
            Return m_Xml
        End Get
    End Property

    Public Sub New(xml As String)
        m_Xml = xml
    End Sub
End Class
