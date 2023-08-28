''' <summary>
''' Represents the eventing interface between Cast/Crew Edit 2 and a hosting process.
''' </summary>
Public Interface ICastCrewEditAdapterEventHandler
    ''' <summary>
    ''' Is raised when cast is completely scanned.
    ''' </summary>
    Event CastCompleted As EventHandler(Of XmlEventArgs)

    ''' <summary>
    ''' Is raised when crew is completely scanned.
    ''' </summary>
    Event CrewCompleted As EventHandler(Of XmlEventArgs)

    ''' <summary>
    ''' Is raised when Cast/Crew Edit 2 is closed.
    ''' </summary>
    Event ProgramClosed As EventHandler

    Sub Close()
End Interface