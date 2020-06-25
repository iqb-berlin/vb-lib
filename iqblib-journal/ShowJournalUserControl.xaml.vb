Imports iqb.lib.components

Public Class ShowJournalUserControl
    Public Shared ReadOnly XLogProperty As DependencyProperty =
        DependencyProperty.Register("XLog", GetType(XElement), GetType(ShowJournalUserControl))

    Public Property XLog As XElement
        Get
            Return GetValue(XLogProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XLogProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RefStringLabelProperty As DependencyProperty =
        DependencyProperty.Register("RefStringLabel", GetType(String), GetType(ShowJournalUserControl))

    Public Property RefStringLabel As String
        Get
            Return GetValue(RefStringLabelProperty)
        End Get
        Set(ByVal value As String)
            SetValue(RefStringLabelProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RefStringListProperty As DependencyProperty =
    DependencyProperty.Register("RefStringList", GetType(IEnumerable(Of String)), GetType(ShowJournalUserControl))

    Public Property RefStringList As List(Of String)
        Get
            Return GetValue(RefStringListProperty)
        End Get
        Set(ByVal value As List(Of String))
            SetValue(RefStringListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CanAddProperty As DependencyProperty =
        DependencyProperty.Register("CanAdd", GetType(Boolean), GetType(ShowJournalUserControl))

    Public Property CanAdd As Boolean
        Get
            Return GetValue(CanAddProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(CanAddProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CanEditRefDateProperty As DependencyProperty =
        DependencyProperty.Register("CanEditRefDate", GetType(Boolean), GetType(ShowJournalUserControl))

    Public Property CanEditRefDate As Boolean
        Get
            Return GetValue(CanEditRefDateProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(CanEditRefDateProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ShowDetailsButtonProperty As DependencyProperty =
    DependencyProperty.Register("ShowDetailsButton", GetType(Boolean), GetType(ShowJournalUserControl))

    Public Property ShowDetailsButton As Boolean
        Get
            Return GetValue(ShowDetailsButtonProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(ShowDetailsButtonProperty, value)
        End Set
    End Property

    Public Shared ReadOnly EnableSysEditProperty As DependencyProperty =
    DependencyProperty.Register("EnableSysEdit", GetType(Boolean), GetType(ShowJournalUserControl))

    Public Property EnableSysEdit As Boolean
        Get
            Return GetValue(EnableSysEditProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(EnableSysEditProperty, value)
        End Set
    End Property

    Public Shared ReadOnly UserNameProperty As DependencyProperty =
        DependencyProperty.Register("UserName", GetType(String), GetType(ShowJournalUserControl))

    Public Property UserName As String
        Get
            Return GetValue(UserNameProperty)
        End Get
        Set(ByVal value As String)
            SetValue(UserNameProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LabelProperty As DependencyProperty =
        DependencyProperty.Register("Label", GetType(String), GetType(ShowJournalUserControl))

    Public Property Label As String
        Get
            Return GetValue(LabelProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property


    '######################################################################
    Public Sub RefreshJournal()
        Dim be As BindingExpression = ICJournal.GetBindingExpression(ItemsControl.ItemsSourceProperty)
        If be IsNot Nothing Then be.UpdateTarget()
    End Sub

    Private Sub BtnFullViewAll_Click(sender As Object, e As RoutedEventArgs)
        Dim XLogRoot As XElement = Me.XLog
        If XLogRoot Is Nothing Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Anzeige Journal", "Bitte wählen Sie ein Objekt aus!")
        Else
            Dim MyDlg As New JournalDialog(XLogRoot, RefStringLabel, RefStringList, CanEditRefDate, UserName) With {.Owner = DialogFactory.GetParentWindow(Me), .EnableSysEdit = Me.EnableSysEdit}
            If MyDlg.ShowDialog() Then RefreshJournal()
        End If
    End Sub

    Private Sub BtnNewJournalEntry_Click(sender As Object, e As RoutedEventArgs)
        Dim XLogRoot As XElement = Me.XLog
        If XLogRoot Is Nothing Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neuer Journaleintrag", "Bitte wählen Sie ein Objekt aus!")
        Else
            Dim MyDlg As New EditJournalEntryDialog(XLogRoot, Nothing, RefStringLabel, RefStringList, UserName, CanEditRefDate) With {.Owner = DialogFactory.GetParentWindow(Me)}
            If MyDlg.ShowDialog() Then RefreshJournal()
        End If
    End Sub

End Class
