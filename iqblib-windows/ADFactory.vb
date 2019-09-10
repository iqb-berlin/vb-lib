Imports System.Security.Principal
Imports System.DirectoryServices
Imports System.Security.AccessControl

Public Class ADFactory
    Private Shared GroupMembers As Dictionary(Of String, List(Of String)) = Nothing
    Private Shared GroupDescriptions As Dictionary(Of String, String) = Nothing

    Public Shared Function GetGroupMembers(GroupId As String, Optional includeDisabled As Boolean = False,
                                           Optional myBackgroundWorker As ComponentModel.BackgroundWorker = Nothing) As List(Of String)
        Dim pos As Integer = GroupId.IndexOf("\")
        If pos > 0 Then GroupId = GroupId.Substring(pos + 1)

        If GroupMembers Is Nothing Then GroupMembers = New Dictionary(Of String, List(Of String))
        If GroupMembers.ContainsKey(GroupId) Then
            Return GroupMembers.Item(GroupId)

        Else
            Dim myUsers As New List(Of String)
            Dim GroupUsers As SortedDictionary(Of String, UserData) = GetUsers(GroupId, myBackgroundWorker)

            For Each u As KeyValuePair(Of String, UserData) In GroupUsers
                Dim tmpstr As String = u.Value.Name
                If String.IsNullOrEmpty(u.Value.Description) Then
                    tmpstr += " (" + u.Key + ")"
                Else
                    If u.Value.Description.IndexOf(u.Key) >= 0 Then
                        tmpstr += " (" + u.Value.Description + ")"
                    Else
                        tmpstr += " (" + u.Key + " - " + u.Value.Description + ")"
                    End If
                End If
                If u.Value.gesperrt Then
                    If includeDisabled Then myUsers.Add("- " + tmpstr)
                Else
                    myUsers.Add(tmpstr)
                End If
            Next
            GroupMembers.Add(GroupId, myUsers)

            Return myUsers
        End If
    End Function

    ''' <summary>
    ''' liest alle User einer bestimmten Gruppe
    ''' </summary>
    ''' <param name="GroupId">Domänenangabe unnötig</param>
    ''' <param name="myBackgroundWorker">zur Forstschrittsanzeige</param>
    ''' <returns>Key: SID, Value: Name + Vorname</returns>
    Public Shared Function GetGroupMembersDict(GroupId As String, Optional myBackgroundWorker As ComponentModel.BackgroundWorker = Nothing) As Dictionary(Of String, String)
        Dim pos As Integer = GroupId.IndexOf("\")
        If pos > 0 Then GroupId = GroupId.Substring(pos + 1)

        Dim myreturn As New Dictionary(Of String, String)
        For Each u As KeyValuePair(Of String, UserData) In GetUsers(GroupId, myBackgroundWorker)
            If Not u.Value.gesperrt Then myreturn.Add(u.Value.SID, u.Value.LastName + ", " + u.Value.FirstName)
        Next

        Return myreturn
    End Function

    'cached
    Public Shared Function GetGroupDescription(GroupId As String) As String
        Dim pos As Integer = GroupId.IndexOf("\")
        If pos > 0 Then GroupId = GroupId.Substring(pos + 1)

        If GroupDescriptions Is Nothing Then GroupDescriptions = New Dictionary(Of String, String)
        If GroupDescriptions.ContainsKey(GroupId) Then
            Return GroupDescriptions.Item(GroupId)
        Else
            Dim myDescription As String = ""
            Try
                Dim domain As ActiveDirectory.Domain = ActiveDirectory.Domain.GetCurrentDomain
                Dim deSearch As DirectorySearcher = New DirectorySearcher()
                deSearch.SearchRoot = domain.GetDirectoryEntry
                deSearch.Filter = "(&(objectClass=group) (cn=" & GroupId & "))"
                Dim results As SearchResultCollection = deSearch.FindAll()
                If results.Count > 0 Then
                    Dim dbgroup As New DirectoryEntry(results(0).Path)
                    Dim DescrQ = From p As PropertyValueCollection In dbgroup.Properties Where p.PropertyName = "description" Select p.Value
                    If DescrQ.Count > 0 Then myDescription = DescrQ.First.ToString
                End If
            Catch ex As Exception
                Return ex.ToString
            End Try

            GroupDescriptions.Add(GroupId, myDescription)
            Return myDescription
        End If
    End Function

    ''' <remarks>Holt einen nach Konventionenen abgelegten String ab</remarks>
    Public Shared Function GetIQBEntry(EntryKey As String) As String
        Dim descrtext As String = GetGroupInfo("grUsrHueIqbUsers")
        For Each descr As String In descrtext.Split({vbNewLine}, StringSplitOptions.RemoveEmptyEntries)
            Dim markerpos As Integer = descr.IndexOf("::")
            If markerpos > 1 AndAlso descr.Substring(0, markerpos) = EntryKey Then Return descr.Substring(markerpos + 2)
        Next
        Return ""
    End Function

    'stellt fest, ob ausführender Nutzer Mitglied einer bestimmten Gruppe ist
    Public Shared Function IAmMemberOf(GroupId As String) As Boolean
        For Each ir As Security.Principal.IdentityReference In System.Security.Principal.WindowsIdentity.GetCurrent().Groups
            If ir.Translate(GetType(System.Security.Principal.NTAccount)).ToString = "USER\" + GroupId Then
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' liefert den System-Anmeldenamen des aktuell angemeldeten Users
    ''' </summary>
    Public Shared Function GetMyName() As String
        Dim myCurrentUser As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent
        Return myCurrentUser.Name.Substring(myCurrentUser.Name.IndexOf("\") + 1)
    End Function

    ''' <summary>
    ''' wenn LastNameFirst dann 'Name, Vorname' des aktuell angemeldeten Users
    ''' </summary>
    Public Shared Function GetMyNameLong(Optional LastNameFirst As Boolean = False) As String
        Dim mySysName As String = GetMyName()

        Dim domain As ActiveDirectory.Domain = ActiveDirectory.Domain.GetCurrentDomain
        Dim deSearch As DirectorySearcher = New DirectorySearcher()
        deSearch.SearchRoot = domain.GetDirectoryEntry
        deSearch.Filter = String.Format("(&(objectCategory=person)(objectClass=user)(SAMAccountname={0}))", mySysName)

        Dim result As SearchResult = deSearch.FindOne()
        If result IsNot Nothing Then
            Dim userEntry As New DirectoryEntry(result.Path)
            Dim LastName As String = ""
            Dim FirstName As String = ""
            For Each p As PropertyValueCollection In userEntry.Properties
                Select Case p.PropertyName
                    Case "givenName"
                        FirstName = p.Value
                        If Not String.IsNullOrEmpty(LastName) Then Exit For
                    Case "sn"
                        LastName = p.Value
                        If Not String.IsNullOrEmpty(FirstName) Then Exit For

                End Select
            Next
            If LastNameFirst Then
                Return LastName + ", " + FirstName
            Else
                Return FirstName + " " + LastName
            End If
            'Old: Return userEntry.Name.Substring(3)

        End If
        Return mySysName
    End Function

    Public Shared Function GetMyID() As String
        Dim myCurrentUser As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent
        Return myCurrentUser.User.ToString
    End Function


    'not chached
    Public Shared Function GetGroupInfo(GroupId As String) As String
        Dim pos As Integer = GroupId.IndexOf("\")
        If pos > 0 Then GroupId = GroupId.Substring(pos + 1)

        Dim myDescription As String = ""
        Try
            Dim domain As ActiveDirectory.Domain = ActiveDirectory.Domain.GetCurrentDomain
            Dim deSearch As DirectorySearcher = New DirectorySearcher()
            deSearch.SearchRoot = domain.GetDirectoryEntry
            deSearch.Filter = "(&(objectClass=group) (cn=" & GroupId & "))"
            Dim results As SearchResultCollection = deSearch.FindAll()
            If results.Count > 0 Then
                Dim dbgroup As New DirectoryEntry(results(0).Path)
                Dim DescrQ = From p As PropertyValueCollection In dbgroup.Properties Where p.PropertyName = "info" Select p.Value
                If DescrQ.Count > 0 Then myDescription = DescrQ.First.ToString
            End If
        Catch ex As Exception
            myDescription = ""
        End Try

        Return myDescription
    End Function

    Public Shared Function GetXGroup(Rule As FileSystemAccessRule) As XElement
        'Dim RightsMask As Integer = FileSystemRights.WriteData +
        '                            FileSystemRights.CreateFiles +
        '                            FileSystemRights.CreateDirectories +
        '                            FileSystemRights.AppendData +
        '                            FileSystemRights.WriteExtendedAttributes +
        '                            FileSystemRights.DeleteSubdirectoriesAndFiles +
        '                            FileSystemRights.WriteAttributes +
        '                            FileSystemRights.Write +
        '                            FileSystemRights.Delete +
        '                            FileSystemRights.Modify +
        '                            FileSystemRights.ChangePermissions +
        '                            FileSystemRights.TakeOwnership +
        '                            FileSystemRights.Synchronize +
        '                            FileSystemRights.FullControl
        Dim myGroup As String = Rule.IdentityReference.Value.ToString()
        Dim myreturn As XElement = <Group id=<%= myGroup %> description=<%= ADFactory.GetGroupDescription(myGroup) %> oldrw="False">
                                       <%= Rule.FileSystemRights.ToString() %></Group>

        Dim AccessType As String = IIf((Rule.FileSystemRights And FileSystemRights.Modify) = FileSystemRights.Modify, "RW", "RO")
        If AccessType = "RW" Then
            myreturn.@oldrw = "True"
        ElseIf AccessType = "RO" AndAlso (Rule.FileSystemRights And FileSystemRights.DeleteSubdirectoriesAndFiles) = FileSystemRights.DeleteSubdirectoriesAndFiles Then
            AccessType = "RW"
        End If
        myreturn.@type = AccessType
        Return myreturn
    End Function

    Public Shared Function GetIQBUsers() As SortedDictionary(Of String, UserData)
        Return GetUsers("grUsrHueIqbUsers")
    End Function

    Public Shared Function GetUsers(GroupName As String, Optional myBackgroundWorker As ComponentModel.BackgroundWorker = Nothing) As SortedDictionary(Of String, UserData)
        Dim domain As ActiveDirectory.Domain = ActiveDirectory.Domain.GetCurrentDomain
        Dim deSearch As DirectorySearcher = New DirectorySearcher()
        deSearch.SearchRoot = domain.GetDirectoryEntry
        deSearch.Filter = "(&(objectClass=group) (cn=" & GroupName & "))"
        Dim results As SearchResultCollection = deSearch.FindAll()
        Dim users As New SortedDictionary(Of String, UserData)
        Const IqbGroupNamePrefix = "grUsrHueIqb"
        If results.Count > 0 Then
            Dim group As New DirectoryEntry(results(0).Path)
            Dim members As Object = group.Invoke("Members", Nothing)
            Dim maxProgressVal As Integer = 130
            Dim ProgressVal As Integer = 0

            For Each o In CType(members, IEnumerable)
                If myBackgroundWorker IsNot Nothing Then
                    myBackgroundWorker.ReportProgress(ProgressVal * 100 / maxProgressVal)
                    ProgressVal += 1
                End If

                Dim u As New DirectoryEntry(o)
                Dim ud As New UserData With {.Name = u.Name.Substring(3), .gesperrt = False, .Groups = New List(Of String)}
                Dim usysname As String = ""

                For Each p As PropertyValueCollection In u.Properties
                    Select Case p.PropertyName
                        Case "description" : ud.Description = p.Value
                        Case "sAMAccountName"
                            usysname = p.Value
                            ud.Systemname = p.Value
                        Case "givenName" : ud.FirstName = p.Value
                        Case "sn" : ud.LastName = p.Value
                        Case "objectSid"
                            Dim mySID As New SecurityIdentifier(CType(p.Value, Byte()), 0)
                            ud.SID = mySID.ToString
                        Case "userAccountControl"
                            ud.gesperrt = (p.Value And &H2) <> 0
                        Case "lastLogonTimestamp"
                            Dim intLastLogonTime As Int64 = p.Value.HighPart * (2 ^ 32) + p.Value.LowPart
                            intLastLogonTime = intLastLogonTime / (60 * 10000000)
                            intLastLogonTime = intLastLogonTime / 1440

                            'Dim RawLastLogon As Int64 = CLng(Fix(p.Value))
                            Dim LastLogonDate As New Date(1601, 1, 1)
                            LastLogonDate = LastLogonDate.AddDays(intLastLogonTime)
                            Dim tsp As TimeSpan = Date.Now - LastLogonDate
                            If tsp.Days > 60 Then ud.LastLogon = LastLogonDate.ToString("MMM yyyy")

                        Case "memberOf"
                            'Dim de As DirectoryEntry = New DirectoryEntry("LDAP://" + p.ToString())
                            'ud.Groups.Add(de.Properties("name").Value.ToString())
                            If p.Value IsNot Nothing Then
                                For Each s As String In p.Value
                                    If Not String.IsNullOrEmpty(s) AndAlso s.Length > 10 Then
                                        Dim g As String = Text.RegularExpressions.Regex.Match(s, "\w{5,}").Value
                                        If g.Substring(0, IqbGroupNamePrefix.Length) = IqbGroupNamePrefix Then
                                            ud.Groups.Add(g.Substring(IqbGroupNamePrefix.Length))
                                        Else
                                            ud.Groups.Add(g)
                                        End If
                                    End If

                                Next
                            End If
                    End Select
                Next

                If Not String.IsNullOrEmpty(usysname) Then users.Add(usysname, ud)
            Next
        End If
        Return users
    End Function

    Public Shared ReadOnly Property AssemblyDirectory() As String
        Get
            Dim codeBase As String = Reflection.Assembly.GetExecutingAssembly().CodeBase
            Dim uriBuilder As New UriBuilder(codeBase)
            Dim assemblyPath As String = Uri.UnescapeDataString(uriBuilder.Path)
            Return IO.Path.GetDirectoryName(assemblyPath)
        End Get
    End Property

    Public Shared Function canWriteToFolder(folderPath As String) As Boolean
        'mehrfach wegen möglichen Delays
        Dim myreturn As Boolean = False
        Dim loopcounter As Integer = 0
        Do
            Try
                Dim ds As System.Security.AccessControl.DirectorySecurity = IO.Directory.GetAccessControl(folderPath)
                myreturn = True
            Catch ex As Exception
                myreturn = False
            End Try
            loopcounter += 1
        Loop Until myreturn OrElse loopcounter > 3
        Return myreturn
    End Function

    Public Shared Function IsValidFileName(name As String) As Boolean
        If Not String.IsNullOrEmpty(name) Then
            Dim containsABadCharacter As New Text.RegularExpressions.Regex("[" + Text.RegularExpressions.Regex.Escape(IO.Path.GetInvalidFileNameChars) + "]")
            Return Not containsABadCharacter.IsMatch(Trim(name))
        Else
            Return False
        End If
    End Function

    Private Shared Function PropValueFromLDAP(de As DirectoryEntry, PropName As String) As List(Of String)
        Dim PropValue
        Try
            PropValue = de.Properties.Item(PropName).Value
        Catch ex As Exception
            PropValue = Nothing
        End Try

        If PropValue Is Nothing Then
            Return Nothing
        ElseIf TypeOf (PropValue) Is String Then
            Dim myreturn As New List(Of String)
            myreturn.Add(PropValue)
            Return myreturn
        ElseIf TypeOf (PropValue) Is IEnumerable(Of Object) Then
            Dim myreturn As New List(Of String)
            For Each s In CType(PropValue, IEnumerable(Of Object))
                Try
                    myreturn.Add(s)
                Catch ex As Exception
                    Debug.Print("failed (Common.ADFactory.PropValueFromLDAP)")
                End Try
            Next
            Return myreturn
        Else
            Return Nothing
        End If
    End Function

    Private Shared Function GetUserDataFromLDAP(de As DirectoryEntry) As LDAPUser
        Dim myreturn As New LDAPUser
        Dim s As List(Of String) = PropValueFromLDAP(de, "uid")
        If s IsNot Nothing AndAlso s.Count > 0 Then myreturn.uid = s.First
        s = PropValueFromLDAP(de, "sn")
        If s IsNot Nothing AndAlso s.Count > 0 Then myreturn.LastName = s.First
        s = PropValueFromLDAP(de, "givenName")
        If s IsNot Nothing AndAlso s.Count > 0 Then myreturn.FirstName = s.First
        s = PropValueFromLDAP(de, "cn")
        If s IsNot Nothing AndAlso s.Count > 0 Then myreturn.Name = s

        If myreturn.Name Is Nothing OrElse myreturn.Name.Count = 0 Then
            myreturn.Name = New List(Of String)
            If Not String.IsNullOrEmpty(myreturn.LastName) Then
                If Not String.IsNullOrEmpty(myreturn.FirstName) Then
                    myreturn.Name.Add(myreturn.FirstName + " " + myreturn.LastName)
                Else
                    myreturn.Name.Add(myreturn.LastName)
                End If
            End If
        End If

        If myreturn.Name Is Nothing OrElse myreturn.Name.Count = 0 Then
            Return Nothing
        Else
            Return myreturn
        End If
    End Function
    ''' <summary>
    ''' Liefert Liste von passenden Usern aus dem HU-LDAP
    ''' </summary>
    ''' <param name="SearchName">Wird mit *SearchName* in cn gesucht</param>
    ''' <returns>Systemname=uid, Lastname=sn, FirstName=givenName, Name=cn (Vor- und Zuname), Description=mail</returns>
    Public Shared Function LookForNameInLdap(SearchName As String) As List(Of LDAPUser)
        Dim myreturn As New List(Of LDAPUser)

        Dim oRoot As DirectoryEntry = New DirectoryEntry("LDAP://ldap.hu-berlin.de:389/o=Humboldt-Universitaet zu Berlin,c=de")
        oRoot.AuthenticationType = AuthenticationTypes.Anonymous

        Dim oSearcher As DirectorySearcher = New DirectorySearcher(oRoot)
        oSearcher.Filter = "(&(objectClass=person)(cn=*" + SearchName + "*))"

        Dim oResults As SearchResultCollection = oSearcher.FindAll

        If oResults IsNot Nothing AndAlso oResults.Count > 0 Then
            For Each oResult As SearchResult In oResults
                Dim de As DirectoryEntry = oResult.GetDirectoryEntry()
                Dim ud As LDAPUser = GetUserDataFromLDAP(de)
                If ud IsNot Nothing Then myreturn.Add(ud)
            Next
        End If

        Return myreturn
    End Function

    ''' <summary>
    ''' Liefert passenden User aus dem HU-LDAP oder nothing
    ''' </summary>
    ''' <returns>Systemname=uid, Lastname=sn, FirstName=givenName, Name=cn (Vor- und Zuname), Description=mail</returns>
    Public Shared Function LookForIDInLdap(UserID As String) As LDAPUser
        Dim oRoot As DirectoryEntry = New DirectoryEntry("LDAP://ldap1.cms.hu-berlin.de:389/o=Humboldt-Universitaet zu Berlin,c=de")
        oRoot.AuthenticationType = AuthenticationTypes.Anonymous

        Dim oSearcher As DirectorySearcher = New DirectorySearcher(oRoot)
        oSearcher.Filter = "(&(objectClass=person)(uid=" + UserID + "))"

        Dim oResults As SearchResultCollection = oSearcher.FindAll

        If oResults IsNot Nothing AndAlso oResults.Count > 0 Then
            Dim oResult As SearchResult = oResults.Item(0)

            Dim de As DirectoryEntry = oResult.GetDirectoryEntry()
            Return GetUserDataFromLDAP(de)
        End If

        Return Nothing
    End Function
End Class

Public Class UserData
        Public SID As String
        Public Name As String
        Public FirstName As String
        Public LastName As String
        Public Systemname As String
        Public Description As String
        Public LastLogon As String
        Public gesperrt As Boolean
        Public Groups As List(Of String)
End Class

Public Class LDAPUser
    Public uid As String
    Public Name As List(Of String)
    Public FirstName As String
    Public LastName As String
End Class

'######################################################################################################

Public Class OtherUser
    Implements IDisposable

    'Nutzung 
    ' Using (New OtherUser("testName", "testPwd"))
    ' …
    ' End Using

    Sub New(ByVal username As String, ByVal pwd As String)
        impersonateValidUser(username, pwd, "user")
        If impersonationContext Is Nothing Then Throw New Exception("no valid user")
    End Sub

    Private Function impersonateValidUser(ByVal userName As String, _
            ByVal password As String, ByVal domain As String) As Boolean

        Dim tempWindowsIdentity As WindowsIdentity
        Dim token As IntPtr = IntPtr.Zero
        Dim tokenDuplicate As IntPtr = IntPtr.Zero
        impersonateValidUser = False

        If RevertToSelf() Then
            If LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                         LOGON32_PROVIDER_DEFAULT, token) <> 0 Then
                If DuplicateToken(token, 2, tokenDuplicate) <> 0 Then
                    tempWindowsIdentity = New WindowsIdentity(tokenDuplicate)
                    impersonationContext = tempWindowsIdentity.Impersonate()
                    If Not impersonationContext Is Nothing Then
                        impersonateValidUser = True
                    End If
                End If
            End If
        End If
        If Not tokenDuplicate.Equals(IntPtr.Zero) Then
            CloseHandle(tokenDuplicate)
        End If
        If Not token.Equals(IntPtr.Zero) Then
            CloseHandle(token)
        End If
    End Function

    Private Sub undoImpersonation()
        impersonationContext.Undo()
    End Sub


    ' #region Interop imports/constants
    Dim LOGON32_LOGON_INTERACTIVE As Integer = 2
    Dim LOGON32_PROVIDER_DEFAULT As Integer = 0

    Dim impersonationContext As WindowsImpersonationContext

    Declare Function LogonUserA Lib "advapi32.dll" (ByVal lpszUsername As String, _
                            ByVal lpszDomain As String, _
                            ByVal lpszPassword As String, _
                            ByVal dwLogonType As Integer, _
                            ByVal dwLogonProvider As Integer, _
                            ByRef phToken As IntPtr) As Integer

    Declare Auto Function DuplicateToken Lib "advapi32.dll" ( _
                            ByVal ExistingTokenHandle As IntPtr, _
                            ByVal ImpersonationLevel As Integer, _
                            ByRef DuplicateTokenHandle As IntPtr) As Integer

    Declare Auto Function RevertToSelf Lib "advapi32.dll" () As Long
    Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal handle As IntPtr) As Long

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If

            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
            Me.undoImpersonation()
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
