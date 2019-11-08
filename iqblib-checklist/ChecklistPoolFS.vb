Public Class ChecklistPoolFS
    Inherits ChecklistPool

    Private _ChecklistPoolFile As String
    Private NextChecklistId As Integer = 0

    ''' <summary>
    ''' Legt Checklistpool an, der Dateibasiert ist (XML)
    ''' </summary>
    ''' <param name="CLFileName">Name und Pfad der Datei, die den Pool enthält</param>
    ''' <param name="Label">Bezeichnung des Pools (wird bei Dialogboxen angezeigt)</param>
    ''' <param name="PPLabel">Zusatzmerkmal - Bezeichnung</param>
    ''' <param name="PPList">Zusatzmerkmal - Werteliste (Reihenfolge wird nicht verändert)</param>
    ''' <remarks></remarks>
    Public Sub New(CLFileName As String, Label As String, PPLabel As String, PPList As Dictionary(Of String, String))
        _ChecklistPoolFile = CLFileName
        _PlusPropLabel = PPLabel
        _PlusPropValues = New Dictionary(Of String, String)(PPList)
    End Sub



    Private _PlusPropLabel As String
    Public Overrides Property PlusPropLabel As String
        Get
            Return _PlusPropLabel
        End Get
        Set(value As String)
            Throw New NotImplementedException("Setting ChecklistPoolFS.PlusPropLabel")
        End Set
    End Property

    Private _PlusPropValues As Dictionary(Of String, String)
    Public Overrides Property PlusPropValues As Dictionary(Of String, String)
        Get
            Return _PlusPropValues
        End Get
        Set(value As Dictionary(Of String, String))
            Throw New NotImplementedException("Setting ChecklistPoolFS.PlusPropValues")
        End Set
    End Property

    Private _Pool As Dictionary(Of String, XElement) = Nothing
    Public Overrides Property Pool As Dictionary(Of String, XElement)
        Get
            If _Pool Is Nothing Then
                _Pool = New Dictionary(Of String, XElement)
                Try
                    Dim XPool As XDocument = XDocument.Load(_ChecklistPoolFile)
                    Dim NextChecklistIdStr As String = XPool.Root.@nextid
                    Me.NextChecklistId = Integer.Parse(NextChecklistIdStr)
                    For Each xe As XElement In XPool.Root.Elements
                        _Pool.Add(xe.@id, xe)
                    Next
                Catch ex As Exception
                    Debug.Print(ex.ToString)
                End Try
            End If
            Return _Pool
        End Get
        Set(value As Dictionary(Of String, XElement))
            Throw New NotImplementedException("Setting ChecklistPoolFS.Pool")
        End Set
    End Property

    Private _PoolLabel As String
    Public Overrides Property PoolLabel As String
        Get
            Return _PoolLabel
        End Get
        Set(value As String)
            Throw New NotImplementedException("Setting ChecklistPoolFS.PoolLabel")
        End Set
    End Property

    '##############################################
    Public Overrides Function AddChecklist(Name As String) As String
        Dim NewId As String
        Try
            NewId = NextChecklistId.ToString
            Dim NewCL As XElement = <checklist id=<%= NewId %> lb=<%= Name %> nextid="1"/>
            _Pool.Add(NewId, NewCL)

            NextChecklistId += 1
            SaveXDoc()

        Catch ex As Exception
            NewId = Nothing
        End Try
        Return NewId
    End Function

    Public Overrides Sub AutoSave(sender As Object, e As XObjectChangeEventArgs)
        SaveXDoc()
    End Sub

    Private Sub SaveXDoc()
        Dim myXDoc As XDocument = <?xml version="1.0" encoding="utf-8"?><checklistpool nextid=<%= Me.NextChecklistId.ToString %>/>
        If _Pool IsNot Nothing Then
            For Each xe As KeyValuePair(Of String, XElement) In Me._Pool
                myXDoc.Root.Add(xe.Value)
            Next
        End If
        myXDoc.Save(_ChecklistPoolFile)
    End Sub

    Public Overrides Function DeleteChecklist(Id As String) As Boolean
        Dim ok As Boolean = False
        If _Pool IsNot Nothing AndAlso _Pool.ContainsKey(Id) Then
            If _Pool.Remove(Id) Then
                SaveXDoc()
                ok = True
            End If
        End If
        Return ok
    End Function
End Class
