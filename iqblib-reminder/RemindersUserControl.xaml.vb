Imports iqb.lib.components

Public Class RemindersUserControl
    Public Shared ReadOnly CurrentObjectIdProperty As DependencyProperty =
        DependencyProperty.Register("CurrentObjectId", GetType(Integer), GetType(RemindersUserControl))

    Public Property CurrentObjectId As Integer
        Get
            Return GetValue(CurrentObjectIdProperty)
        End Get
        Set(ByVal value As Integer)
            SetValue(CurrentObjectIdProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CanEditProperty As DependencyProperty =
        DependencyProperty.Register("CanEdit", GetType(Boolean), GetType(RemindersUserControl))

    Public Property CanEdit As Boolean
        Get
            Return GetValue(CanEditProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(CanEditProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RemindersProperty As DependencyProperty =
        DependencyProperty.Register("Reminders", GetType(List(Of Reminder)), GetType(RemindersUserControl))

    Public Property Reminders As List(Of Reminder)
        Get
            Return GetValue(RemindersProperty)
        End Get
        Set(ByVal value As List(Of Reminder))
            SetValue(RemindersProperty, value)
        End Set
    End Property

    Private Sub BtnNew_Click(sender As Object, e As RoutedEventArgs)
        Reminder.AddReminder.Execute(Nothing, Me)
    End Sub

    Private Sub DPReminder_Click(sender As Object, e As MouseButtonEventArgs)
        If CanEdit Then
            Dim fe As FrameworkElement = sender
            Dim myReminder As Reminder = fe.GetValue(DataContextProperty)
            Dim myDlg As New ReminderEditDialog(myReminder, False) With {.Owner = DialogFactory.GetParentWindow(Me), .Title = "Erinnerung ändern"}
            If myDlg.ShowDialog Then
                If myDlg.IsClosed Then
                    Reminder.RemoveReminder.Execute(myDlg.ReminderObject, Me)
                Else
                    Reminder.ChangeReminder.Execute(myDlg.ReminderObject, Me)
                End If
            End If
        End If
    End Sub
End Class
