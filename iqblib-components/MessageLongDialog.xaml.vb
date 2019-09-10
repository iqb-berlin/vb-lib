Public Class MessageLongDialog
    Public TitleStr As String
    Public MessageStr As String
    Public HelpTopic As Integer
    Public DlgMode As MessageDialog.DialogMode

    Private Sub Me_Loaded() Handles Me.Loaded
        Me.Title = TitleStr
        Me.TBMessage.Text = MessageStr
        If HelpTopic > 0 Then
            Me.SetValue(HelpProvider.HelpTopicIdProperty, HelpTopic.ToString)
        Else
            BtnHelp.Visibility = Windows.Visibility.Collapsed
        End If

        Select Case DlgMode
            Case MessageDialog.DialogMode.Message
                ImgError.Visibility = Windows.Visibility.Collapsed
                ImgWarning.Visibility = Windows.Visibility.Collapsed

            Case MessageDialog.DialogMode.ErrorMessage
                ImgWarning.Visibility = Windows.Visibility.Collapsed

            Case MessageDialog.DialogMode.WarningMessage
                ImgError.Visibility = Windows.Visibility.Collapsed

            Case Else
                Throw New ArgumentNullException("MessageDialog: DialogMode??")
        End Select
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        Me.DialogResult = True
    End Sub

End Class
