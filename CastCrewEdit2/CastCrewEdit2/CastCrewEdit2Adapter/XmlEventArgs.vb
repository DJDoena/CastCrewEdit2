Imports System

''' <summary>
''' Contains the cast or crew information as XML text.
''' </summary>
Public Class XmlEventArgs
    Inherits EventArgs

    Private m_Xml As String

    ''' <summary>
    ''' Returns the cast or crew information.
    ''' </summary>
    Public ReadOnly Property Xml As String
        Get
            Return m_Xml
        End Get
    End Property

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="xml">the cast or crew information</param>
    Public Sub New(xml As String)
        m_Xml = xml
    End Sub
End Class