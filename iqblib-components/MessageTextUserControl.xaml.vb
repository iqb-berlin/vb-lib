Public Class MessageTextUserControl
    Private Delegate Sub AddMessageDelegate(MsgType As MessageType, Message As String)
    Private MyAddMessageDelegate As AddMessageDelegate = Nothing

    Private Enum MessageType
        ParagraphOnly
        NoBreakBefore
        Info
        Warning
        ErrorMsg
        title
        header
    End Enum

    Private Shared Property MessageTypeList As New Dictionary(Of String, MessageType) From {
                                                            {"e", MessageType.ErrorMsg},
                                                            {"w", MessageType.Warning},
                                                            {"i", MessageType.Info},
                                                            {"t", MessageType.title},
                                                            {"h", MessageType.header}}


    Private Sub Me_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        MyAddMessageDelegate = AddressOf InternalSetMessage
    End Sub

    Public Sub SetMessage(Optional Message As String = "")
        Dim myMsgType As MessageType = MessageType.Info
        If String.IsNullOrEmpty(Message) Then
            myMsgType = MessageType.ParagraphOnly
        Else
            If Message.Length > 3 Then
                If Message.Substring(1, 1) = ":" Then
                    If MessageTypeList.ContainsKey(Message.Substring(0, 1).ToLower) Then
                        myMsgType = MessageTypeList.Item(Message.Substring(0, 1).ToLower)
                        If Message.Substring(2, 1) = " " Then
                            Message = Message.Substring(3)
                        Else
                            Message = Message.Substring(2)
                        End If
                    End If
                End If
            Else
                myMsgType = MessageType.NoBreakBefore
            End If
        End If

        If MyAddMessageDelegate Is Nothing Then
            InternalSetMessage(myMsgType, Message)
        Else
            Me.MyTB.Dispatcher.Invoke(MyAddMessageDelegate, myMsgType, Message)
        End If
    End Sub

    Private Sub InternalSetMessage(MsgType As MessageType, Message As String)
        MyTB.Text = Message
        Select Case MsgType
            Case MessageType.ParagraphOnly : MyTB.Foreground = Brushes.Black
            Case MessageType.ErrorMsg : MyTB.Foreground = Brushes.Crimson
            Case MessageType.Warning : MyTB.Foreground = Brushes.Orange
            Case MessageType.header : MyTB.Foreground = Brushes.LightSteelBlue
            Case MessageType.title : MyTB.Foreground = Brushes.Khaki
            Case Else : MyTB.Foreground = Brushes.MidnightBlue
        End Select
    End Sub
End Class
