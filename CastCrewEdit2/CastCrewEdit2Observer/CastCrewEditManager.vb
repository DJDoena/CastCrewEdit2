Imports System
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports DoenaSoft.DVDProfiler.CastCrewEdit2

''' <summary>
''' Manages the connection to the Cast/Crew Edit 2 (CCE2) interface.
''' </summary>
''' <remarks>
''' The code in this class needs to be kept in sync with potential changes in the CCE2 code.
''' </remarks>
Public Class CastCrewEditManager
    Implements IDisposable

    ''' <summary>
    ''' CCE2 v2.1.0.0 and higher will support the CastCrewEdit2Adapter.dll.
    ''' </summary>
    Private ReadOnly MinimumVersion As New Version(2, 1, 0, 0)

    ''' <summary>
    ''' CCE2 interface.
    ''' </summary>
    Private AdapterEventHandler As ICastCrewEditAdapterEventHandler

    ''' <summary>
    ''' CCE2 entry method.
    ''' </summary>
    Private ProgramMainMethodInfo As MethodInfo

    Private IsDisposed As Boolean

#Region "Public"

    ''' <summary>
    ''' Is raised when CCE2 signals a completion of cast scan.
    ''' </summary>
    Public Event CastCompleted As EventHandler(Of XmlEventArgs)

    ''' <summary>
    ''' Is raised when CCE2 signals a completion of crew scan.
    ''' </summary>
    Public Event CrewCompleted As EventHandler(Of XmlEventArgs)

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="exe">path to CastCrewEdit2.exe</param>
    Public Sub New(exe As FileInfo)
        Const Flags As BindingFlags = BindingFlags.Public Or BindingFlags.Static

        CheckInterfaceAssembly(exe)

        Dim assembly As Assembly = GetAssembly(exe)

        Dim programType As Type = GetProgramTye(assembly)

        InitializeProgram(programType, exe.DirectoryName)

        RegisterEvents(programType)

        ProgramMainMethodInfo = programType.GetMethod("Main", Flags)

        If ProgramMainMethodInfo Is Nothing Then
            Throw New NullReferenceException(NameOf(ProgramMainMethodInfo))
        End If

        IsDisposed = False
    End Sub

    ''' <summary>
    ''' Starts CCE2 in embedded mode.
    ''' </summary>
    Public Sub Run()
        ProgramMainMethodInfo.Invoke(Nothing, New Object() {New String() {"embedded"}})
    End Sub

    ''' <summary>
    ''' Ends CCE2.
    ''' </summary>
    Public Sub Kill()
        AdapterEventHandler.Close()
    End Sub

    ''' <summary>
    ''' <see cref="IDisposable.Dispose()"/>
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        If (Not IsDisposed) Then
            If (Not AdapterEventHandler Is Nothing) Then

                RemoveHandler AdapterEventHandler.CastCompleted, AddressOf HandleCastCompleted
                RemoveHandler AdapterEventHandler.CrewCompleted, AddressOf HandleCrewCompleted

                AdapterEventHandler = Nothing
            End If

            IsDisposed = True
        End If
    End Sub

#End Region

#Region "Initialization"

    ''' <summary>
    ''' Checks if CCE2 brings the adapter DLL or if an older version is used.
    ''' </summary>
    ''' <param name="exe">path to CastCrewEdit2.exe</param>
    Private Sub CheckInterfaceAssembly(exe As FileInfo)
        Const DllName As String = "CastCrewEdit2Adapter.dll"

        Dim file As String = Path.Combine(exe.DirectoryName, DllName)

        If (Not IO.File.Exists(file)) Then
            Throw New ApplicationException($"Cast/Crew Edit 2 is missing {DllName} and cannot be used. Please download the newest version of Cast/Crew Edit 2.")
        End If
    End Sub

    ''' <summary>
    ''' Loads the CCE2 exe into memory and checks if the version is suitable.
    ''' </summary>
    ''' <param name="exe">path to CastCrewEdit2.exe</param>
    ''' <returns>the CCE2 exe</returns>
    Private Function GetAssembly(exe As FileInfo) As Assembly
        Dim assembly As Assembly = Assembly.LoadFrom(exe.FullName)

        Dim assemblyVersion As Version = assembly.GetName().Version

        If (assemblyVersion < MinimumVersion) Then
            Throw New ApplicationException($"Cast/Crew Edit 2 must be at least version {MinimumVersion}. Your version is only {assemblyVersion}")
        End If

        Return assembly
    End Function

    ''' <summary>
    ''' Returns the type for the CCE2 CCE2 program class.
    ''' </summary>
    ''' <param name="assembly">the CCE2 exe</param>
    ''' <returns>the type for the CCE2 program class</returns>
    Private Shared Function GetProgramTye(assembly As Assembly) As Type
        Dim types As Type() = assembly.GetTypes()

        Dim programType As Type = types.Where(Function(t As Type) t.FullName = "DoenaSoft.DVDProfiler.CastCrewEdit2.Program").First()

        Return programType
    End Function

    ''' <summary>
    ''' Initializes the CCE2 program.
    ''' </summary>
    ''' <param name="programType">the type for the CCE2 program class</param>
    ''' <param name="directory">the folder of the CastCrewEdit2.exe</param>
    Private Shared Sub InitializeProgram(programType As Type, directory As String)
        Const Flags As BindingFlags = BindingFlags.NonPublic Or BindingFlags.Static

        Dim rootPathFieldInfo As FieldInfo = programType.GetField("RootPath", Flags)

        rootPathFieldInfo.SetValue(Nothing, directory)

        Dim initDataPathsMethodInfo As MethodInfo = programType.GetMethod("InitDataPaths", Flags)

        initDataPathsMethodInfo.Invoke(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Registers for the CCE2 interface events.
    ''' </summary>
    ''' <param name="programType">the type for the CCE2 program class</param>
    Private Sub RegisterEvents(programType As Type)
        AdapterEventHandler = GetAdapterHandler(programType)

        AddHandler AdapterEventHandler.CastCompleted, AddressOf HandleCastCompleted
        AddHandler AdapterEventHandler.CrewCompleted, AddressOf HandleCrewCompleted
    End Sub

    ''' <summary>
    ''' Returns the CCE2 interface.
    ''' </summary>
    ''' <param name="programType">the type for the CCE2 program class</param>
    ''' <returns>the CCE2 interface</returns>
    Private Shared Function GetAdapterHandler(programType As Type) As ICastCrewEditAdapterEventHandler
        Const Flags As BindingFlags = BindingFlags.Public Or BindingFlags.Static

        Dim adapterEventHandlerFieldInfo As FieldInfo = programType.GetField("AdapterEventHandler", Flags)

        Dim value As Object = adapterEventHandlerFieldInfo.GetValue(Nothing)

        Dim adapterEventHandler As ICastCrewEditAdapterEventHandler = CType(value, ICastCrewEditAdapterEventHandler)

        Return adapterEventHandler
    End Function

#End Region

#Region "EventHandler"

    ''' <summary>
    ''' Is called when a cast scan is completed in CCE2.
    ''' </summary>
    ''' <param name="sender">the sender</param>
    ''' <param name="args">the cast information</param>
    Private Sub HandleCastCompleted(sender As Object, args As XmlEventArgs)
        RaiseEvent CastCompleted(Me, args)
    End Sub

    ''' <summary>
    ''' Is called when a crew scan is completed in CCE2.
    ''' </summary>
    ''' <param name="sender">the sender</param>
    ''' <param name="args">the cast information</param>
    Private Sub HandleCrewCompleted(sender As Object, args As XmlEventArgs)
        RaiseEvent CrewCompleted(Me, args)
    End Sub

#End Region
End Class
