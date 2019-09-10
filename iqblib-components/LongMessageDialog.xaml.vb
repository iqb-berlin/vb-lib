Public Class LongMessageDialog
    Public TitleStr As String
    Public MessageStr As String
    Public HelpTopic As Integer


    Private Sub Me_Loaded() Handles Me.Loaded
        Me.Title = TitleStr
        Me.MBUC.AddMessage(MessageStr)
        If HelpTopic > 0 Then
            Me.SetValue(HelpProvider.HelpTopicIdProperty, HelpTopic.ToString)
        Else
            BtnHelp.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub BtnClose_Click() Handles BtnClose.Click
        DialogResult = False
    End Sub


    Private Sub BtnEditor_Click() Handles BtnEditor.Click
        Try
            Dim txtFN As String = IO.Path.GetTempPath + IO.Path.DirectorySeparatorChar + "COCO" + Guid.NewGuid.ToString + ".txt"
            IO.File.WriteAllBytes(txtFN, System.Text.Encoding.Unicode.GetBytes(MBUC.Text))

            Dim proc As New Process
            With proc.StartInfo
                .FileName = txtFN
                .WindowStyle = ProcessWindowStyle.Normal
            End With
            proc.Start()

            Me.DialogResult = True
        Catch ex As Exception
            Dim msg As String = ex.Message
            If ex.InnerException IsNot Nothing Then msg += vbNewLine + ex.InnerException.Message
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Übertragen Meldungen in Texteditor", msg)
        End Try
    End Sub

End Class
