Public Class XRenameDialog
    Public XSelectionList As List(Of XElement) = Nothing
    Public HelpTopic As Integer
    Public TipText As String
    Public Prompt As String
    Public TitleStr As String
    Public UniqueNames As Boolean = True

    Private Sub Me_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If Not String.IsNullOrEmpty(TitleStr) Then Me.Title = TitleStr

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

        If String.IsNullOrEmpty(Prompt) Then
            TBPrompt.Visibility = Windows.Visibility.Collapsed
        Else
            TBPrompt.Text = Prompt
        End If

        ICElements.ItemsSource = XSelectionList
        ICElements.Focus()
    End Sub

    Private Sub OK_Click() Handles BtnOK.Click
        If UniqueNames Then
            Dim Names As New List(Of String)
            Dim DoppeltName As String = Nothing
            For Each xe As XElement In XSelectionList
                If Names.Contains(xe.Value.ToUpper) Then
                    DoppeltName = xe.Value
                    Exit For
                Else
                    Names.Add(xe.Value.ToUpper)
                End If
            Next
            If String.IsNullOrEmpty(DoppeltName) Then
                Me.DialogResult = True
            Else
                DialogFactory.MsgError(Me, TitleStr, "'" + DoppeltName + "' kommt mehrfach vor.")
            End If
        Else
            Me.DialogResult = True
        End If
    End Sub

    Private Sub TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim myX As XElement = fe.GetValue(DataContextProperty)
        myX.@changed = "True"
    End Sub
End Class
