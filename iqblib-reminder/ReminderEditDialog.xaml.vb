Imports iqb.lib.components
Public Class ReminderEditDialog
    Private _R As Reminder
    Public ReadOnly Property ReminderObject As Reminder
        Get
            Return _R
        End Get
    End Property

    Public ReadOnly Property IsClosed As Boolean
        Get
            Return _IsClosed
        End Get
    End Property

    Private _IsNew As Boolean
    Private _IsClosed As Boolean

    Public Sub New(Remi As Reminder, IsNew As Boolean)
        InitializeComponent()
        _R = New Reminder(Remi.ObjectId, Remi.ObjectLabel, Remi.ReminderId) With {.Maturity = Remi.Maturity, .Text = Remi.Text}
        _IsNew = IsNew
        _IsClosed = False
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        TBText.Text = _R.Text
        DPMaturity.SelectedDate = _R.Maturity
        If _IsNew Then BtnClose.Visibility = Windows.Visibility.Collapsed
    End Sub


    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrEmpty(TBText.Text) Then
            DialogFactory.MsgError(Me, Me.Title, "Bitte einen Text für die Erinnerung eingeben!")
        ElseIf DPMaturity.SelectedDate <= Date.Now Then
            DialogFactory.MsgError(Me, Me.Title, "Bitte ein Datum in der Zukunft eingeben!")
        Else
            _R.Text = TBText.Text
            _R.Maturity = DPMaturity.SelectedDate
            Me.DialogResult = True
        End If
    End Sub

    Private Sub Btn1Week_Click(sender As Object, e As RoutedEventArgs)
        Dim d As Date = Date.Now
        DPMaturity.SelectedDate = d.AddDays(7)
    End Sub

    Private Sub Btn1Month_Click(sender As Object, e As RoutedEventArgs)
        Dim d As Date = Date.Now
        DPMaturity.SelectedDate = d.AddMonths(1)
    End Sub

    Private Sub Btn3Months_Click(sender As Object, e As RoutedEventArgs)
        Dim d As Date = Date.Now
        DPMaturity.SelectedDate = d.AddMonths(3)
    End Sub

    Private Sub BtnClose_Clicked(sender As Object, e As RoutedEventArgs)
        If DialogFactory.YesNoCancel(Me, "Entfernen Erinnerung", "Die Erinnerung für '" + _R.ObjectLabel + "' wird entfernt." + vbNewLine + vbNewLine + "Fortsetzen?") Then
            _IsClosed = True
            Me.DialogResult = True
        End If
    End Sub
End Class
