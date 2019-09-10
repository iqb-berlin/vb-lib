Public Class TextInputDialog
    Public Enum DialogMode
        TextSingleLine
        TextMultiLine
    End Enum

    Public TitleStr As String
    Public MessageStr As String
    Public TipText As String
    Public HelpTopic As Integer
    Public InputStr As String
    Public DlgMode As DialogMode

    Private Sub Me_Loaded() Handles Me.Loaded
        Me.Title = TitleStr

        If String.IsNullOrEmpty(MessageStr) Then
            Me.TBMessage.Visibility = Windows.Visibility.Collapsed
        Else
            Me.TBMessage.Text = MessageStr
        End If

        If HelpTopic > 0 Then
            Me.SetValue(HelpProvider.HelpTopicIdProperty, HelpTopic.ToString)
        Else
            BtnHelp.Visibility = Windows.Visibility.Collapsed
        End If

        If String.IsNullOrEmpty(TipText) Then
            LbTip.Visibility = Windows.Visibility.Collapsed
        Else
            LbTip.Content = TipText
        End If

        If DlgMode = DialogMode.TextMultiLine Then
            TBInput.Height = 120
            TBInput.TextWrapping = TextWrapping.Wrap
            TBInput.AcceptsReturn = True
            TBInput.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled)
            TBInput.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto)
        End If
        TBInput.Text = InputStr
        TBInput.SelectAll()
        TBInput.Focus()
    End Sub

    Private Sub TBInput_TextChanged() Handles TBInput.TextChanged
        BtnOK.IsEnabled = Not String.IsNullOrEmpty(TBInput.Text)
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        If Not String.IsNullOrEmpty(TBInput.Text) Then
            InputStr = TBInput.Text
            Me.DialogResult = True
        End If
    End Sub

End Class
