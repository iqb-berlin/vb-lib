Imports iqb.lib.components

Public Class EditJournalEntryDialog
    Private _XJournalEntry As XElement = Nothing
    Private _XJournalRoot As XElement = Nothing
    Private _RefLabel As String
    Private _UserName As String
    Private _RefList As List(Of String)
    Private _CanEditRefDate As Boolean

    Public Sub New(XJournalRoot As XElement, XJournalEntry As XElement,
                   RefLabel As String, RefList As List(Of String), UserName As String,
                   CanEditRefDate As Boolean)
        InitializeComponent()
        _XJournalEntry = XJournalEntry
        If RefList Is Nothing Then
            _RefList = New List(Of String)
        Else
            _RefList = RefList
        End If
        _RefLabel = RefLabel
        _XJournalRoot = XJournalRoot
        _CanEditRefDate = CanEditRefDate
        _UserName = UserName
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        Try
            Dim EntryTypeKey As String = Nothing
            If _XJournalEntry IsNot Nothing Then EntryTypeKey = _XJournalEntry.@category
            For Each JournalEntryType As KeyValuePair(Of String, Brush) In JournalFactory.JournalEntryCategories
                If String.IsNullOrEmpty(EntryTypeKey) OrElse JournalEntryType.Key <> EntryTypeKey Then
                    ICCategories.Items.Add(<t checked="False"><%= JournalEntryType.Key %></t>)
                Else
                    ICCategories.Items.Add(<t checked="True"><%= JournalEntryType.Key %></t>)
                End If
            Next
            If Not String.IsNullOrEmpty(_RefLabel) Then
                ICApplicantsLabel.Text = "Bezug zu " + _RefLabel + ":"
                Dim ApplicantsNameList As New List(Of String)
                If _XJournalEntry IsNot Nothing Then
                    For Each xp As XElement In _XJournalEntry.<ref>
                        If Not _RefList.Contains(xp.Value) Then _RefList.Add(xp.Value)
                        ApplicantsNameList.Add(xp.Value)
                    Next
                End If
                For Each p As String In From a As String In _RefList Order By a
                    ICApplicants.Items.Add(<a checked=<%= ApplicantsNameList.Contains(p).ToString %>><%= p %></a>)
                Next
            End If
            If String.IsNullOrEmpty(_RefLabel) OrElse ICApplicants.Items.Count = 0 Then
                ICApplicantsLabel.Visibility = Windows.Visibility.Collapsed
                ICApplicants.Visibility = Windows.Visibility.Collapsed
            End If

            If _XJournalEntry IsNot Nothing Then
                TBText.Text = _XJournalEntry.<text>.First.Value
                TBLabel.Text = _XJournalEntry.@lb
                DPiRefDate.SelectedDate = Date.Parse(_XJournalEntry.@date)
                Me.Title = "Ändern Journaleintrag"
            Else
                DPiRefDate.SelectedDate = Date.Now
            End If

            If Not _CanEditRefDate Then DPiRefDate.IsEnabled = False
        Catch ex As Exception
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "New/Edit Journaleintrag", ex.ToString)
            Me.DialogResult = False
        End Try
    End Sub

    Private Sub BtnCancel_Click() Handles BtnCancel.Click
        DialogResult = False
    End Sub

    Private Sub BtnSave_Click() Handles BtnSave.Click
        Dim SelectedCategory As String = Nothing
        For Each xc As XElement In ICCategories.Items
            If xc.@checked = "True" Then
                SelectedCategory = xc.Value
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(SelectedCategory) Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, "Bitte Kategorie des Eintrags wählen!")
        ElseIf String.IsNullOrEmpty(TBLabel.Text) Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, "Bitte einen Titel für den Eintrag eingeben!")
        ElseIf String.IsNullOrEmpty(TBText.Text) Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, "Bitte Text eingeben!")
        Else
            If _XJournalRoot IsNot Nothing Then
                If _XJournalEntry IsNot Nothing Then _XJournalEntry.Remove()
                Dim SelectedDate As Date = DPiRefDate.SelectedDate

                Dim RefStringList As List(Of String) = (From xe As XElement In ICApplicants.Items Where xe.@checked = "True" Select xe.Value).ToList
                _XJournalRoot.Add(JournalFactory.NewLogEntry(_UserName, SelectedCategory, TBLabel.Text, RefStringList, TBText.Text, False, SelectedDate))
            End If

            DialogResult = True
        End If

    End Sub
End Class
