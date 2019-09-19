Public Class DialogFactory
    Public Shared MainWindow As Window = Nothing
    Public Shared MainStatusMessageControl As MessageTextUserControl = Nothing
    Public Shared MainProgressBar As AsynchProgressBarUserControl = Nothing

    Public Shared Sub Msg(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.Message}
        d.ShowDialog()
    End Sub

    Public Shared Sub MsgLong(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageLongDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.Message}
        d.ShowDialog()
    End Sub

    Public Shared Sub MsgError(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.ErrorMessage}
        d.ShowDialog()
    End Sub

    Public Shared Sub MsgErrorLong(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageLongDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.ErrorMessage}
        d.ShowDialog()
    End Sub

    Public Shared Sub MsgWarning(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.WarningMessage}
        d.ShowDialog()
    End Sub

    Public Shared Sub MsgWarningLong(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0)
        Dim d As New MessageLongDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.WarningMessage}
        d.ShowDialog()
    End Sub

    Public Shared Function YesNoCancel(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0) As MessageBoxResult
        Dim d As New MessageDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.YesNoCancel}
        d.ShowDialog()
        Return d.ButtonClicked
    End Function

    Public Shared Function YesNo(parent As FrameworkElement, Title As String, Message As String, Optional HelpTopic As Integer = 0) As Boolean
        Dim d As New MessageDialog With {.TitleStr = Title, .MessageStr = Message, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .DlgMode = MessageDialog.DialogMode.YesNo}
        Return d.ShowDialog()
    End Function

    Public Shared Function InputText(parent As FrameworkElement, Title As String, Prompt As String, DefaultText As String, TipText As String, Optional HelpTopic As Integer = 0) As String
        Dim d As New TextInputDialog With {.TitleStr = Title, .MessageStr = Prompt, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .TipText = TipText,
                                       .DlgMode = TextInputDialog.DialogMode.TextSingleLine, .InputStr = DefaultText}
        If d.ShowDialog() Then
            Return d.InputStr
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function InputTextMultiLine(parent As FrameworkElement, Title As String, Prompt As String, DefaultText As String, TipText As String, Optional HelpTopic As Integer = 0) As String
        Dim d As New TextInputDialog With {.TitleStr = Title, .MessageStr = Prompt, .Owner = GetParentWindow(parent), .HelpTopic = HelpTopic, .TipText = TipText,
                                       .DlgMode = TextInputDialog.DialogMode.TextMultiLine, .InputStr = DefaultText}
        If d.ShowDialog() Then
            Return d.InputStr
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetParentWindow(fe As FrameworkElement) As Window
        Do While fe IsNot Nothing AndAlso Not TypeOf (fe) Is Window
            fe = VisualTreeHelper.GetParent(fe)
        Loop
        If fe Is Nothing Then fe = DialogFactory.MainWindow
        Return fe
    End Function
End Class
