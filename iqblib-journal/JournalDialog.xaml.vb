Imports iqb.lib.components

Public Class JournalDialog
    'Achtung: Modul wurde aus dem FDZ-Antragsmanager extrahiert, daher wird RefString als Applicant (Antragsteller) geführt
    Private _XJournalRoot As XElement
    Private _RefLabel As String
    Private _RefList As List(Of String)
    Private _CanEditRefDate As Boolean
    Private _UserName As String

    Private FilterValue_Type As String = Nothing
    Private FilterValue_Ref As String = Nothing
    Private Const AlleString = "- alle -"
    Private Const ForRefPrefix = "nur für "
    Private Const ForRefPrefixLen = 8

    Public Shared ReadOnly EnableSysEditProperty As DependencyProperty =
        DependencyProperty.Register("EnableSysEdit", GetType(Boolean), GetType(JournalDialog))

    Public Property EnableSysEdit As Boolean
        Get
            Return GetValue(EnableSysEditProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(EnableSysEditProperty, value)
        End Set
    End Property

    Public Sub New(XLogRoot As XElement, RefLabel As String, RefList As List(Of String),
                   CanEditRefDate As Boolean, UserName As String)

        InitializeComponent()
        _XJournalRoot = XLogRoot
        _RefLabel = RefLabel
        _RefList = RefList
        _CanEditRefDate = CanEditRefDate
        _UserName = UserName
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        If _XJournalRoot Is Nothing Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, "Fehler beim Laden des Journals")
            DialogResult = False
        Else
            Me.DataContext = _XJournalRoot

            Dim TypeList As New List(Of String)
            TypeList.Add(AlleString)

            Dim ApplicantList As New List(Of String)
            ApplicantList.Add(AlleString)
            For Each xe As XElement In _XJournalRoot.Elements
                If Not String.IsNullOrEmpty(xe.@category) AndAlso Not TypeList.Contains(xe.@category) Then TypeList.Add(xe.@category)
                For Each xp As XElement In xe.<ref>
                    If Not String.IsNullOrEmpty(xp.Value) AndAlso Not ApplicantList.Contains(xp.Value) Then ApplicantList.Add(xp.Value)
                Next
            Next

            CbBType.ItemsSource = From t As String In TypeList Order By t
            If TypeList.Count = 2 Then
                CbBType.SelectedIndex = 1
                CbBType.IsEnabled = False
            Else
                CbBType.SelectedIndex = 0
            End If

            If String.IsNullOrEmpty(_RefLabel) Then
                StPRefString.Visibility = Windows.Visibility.Collapsed
            Else
                TBRefString.Text = "Filter " + _RefLabel + ":"
                CbBApplicant.ItemsSource = From t As String In ApplicantList Order By t Select IIf(t = AlleString, AlleString, ForRefPrefix + t)
                CbBApplicant.SelectedIndex = 0
                If ApplicantList.Count = 1 Then CbBApplicant.IsEnabled = False
            End If
        End If
    End Sub

    '############################################################################################
    Public Function FilterList(item As Object) As Boolean
        Dim XLogEntry As XElement = item
        If String.IsNullOrEmpty(FilterValue_Type) OrElse FilterValue_Type = AlleString Then
            If String.IsNullOrEmpty(FilterValue_Ref) OrElse FilterValue_Ref = AlleString Then
                Return True
            Else
                Dim ApptList As List(Of String) = (From xp As XElement In XLogEntry.<ref> Select xp.Value Distinct).ToList
                Return ApptList.Contains(FilterValue_Ref)
            End If
        Else
            If String.IsNullOrEmpty(FilterValue_Ref) OrElse FilterValue_Ref = AlleString Then
                Return XLogEntry.@category = FilterValue_Type
            Else
                Dim ApptList As List(Of String) = (From xp As XElement In XLogEntry.<ref> Select xp.Value Distinct).ToList
                Return ApptList.Contains(FilterValue_Ref) AndAlso XLogEntry.@category = FilterValue_Type
            End If
        End If
    End Function

    Private Sub FilterChanged(sender As Object, e As SelectionChangedEventArgs) Handles CbBApplicant.SelectionChanged, CbBType.SelectionChanged
        If sender.Equals(CbBApplicant) Then
            FilterValue_Ref = CbBApplicant.SelectedValue
            FilterValue_Ref = FilterValue_Ref.Substring(ForRefPrefixLen)
        Else
            FilterValue_Type = CbBType.SelectedValue
        End If

        RefreshJournalList()
    End Sub

    Private Sub RefreshJournalList()
        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(ICXJournal.ItemsSource)
        If cv IsNot Nothing Then cv.Refresh()
    End Sub

    Private Sub BtnCancel_Click() Handles BtnCancel.Click
        DialogResult = False
    End Sub

    Private Sub BtnSave_Click() Handles BtnSave.Click
        DialogResult = True
    End Sub

    Private Sub BtnEdit_Clicked(sender As Object, e As RoutedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim XJournalEntry As XElement = fe.GetValue(DataContextProperty)
        If XJournalEntry IsNot Nothing AndAlso XJournalEntry.@sys <> "True" Then
            Dim myDlg As New EditJournalEntryDialog(_XJournalRoot, XJournalEntry, _RefLabel, _RefList, _UserName, _CanEditRefDate)
            If myDlg.ShowDialog() Then RefreshJournalList()
        End If
    End Sub

    Private Sub FilterJournalList(sender As Object, e As FilterEventArgs)
        e.Accepted = FilterList(e.Item)
    End Sub

    Private Sub BtnDelete_Clicked(sender As Object, e As RoutedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim XJournalEntry As XElement = fe.GetValue(DataContextProperty)
        If XJournalEntry IsNot Nothing AndAlso XJournalEntry.@sys <> "True" Then
            If DialogFactory.YesNoCancel(DialogFactory.GetParentWindow(Me), "Löschen Journal-Eintrag", "Möchten Sie den Eintrag '" + XJournalEntry.@lb + "' löschen?") = MessageBoxResult.OK Then
                XJournalEntry.Remove()
                RefreshJournalList()
            End If
        End If
    End Sub
End Class
