Imports System.Windows.Threading
Imports System.Threading

Public Class simpleMessageBox

    Private _hideRequest As Boolean
    Private _result As MessageBoxResult
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Function Show(ByVal Text As String, Optional ByVal MsgboxStyle As MsgBoxStyle = vbOKCancel, Optional ByVal Title As String = "") As MessageBoxResult
        txt.Text = Text
        Me.Title.Text = Title
        cancelButton.Visibility = Visibility.Collapsed
        OkButton.Visibility = Visibility.Collapsed
        YesButton.Visibility = Visibility.Collapsed
        NoButton.Visibility = Visibility.Collapsed

        If MsgboxStyle = vbOKOnly Then
            OkButton.Visibility = Visibility.Visible
            OkButton.IsDefault = True
        End If
        If MsgboxStyle = vbOKCancel Then
            OkButton.Visibility = Visibility.Visible
            cancelButton.Visibility = Visibility.Visible
            cancelButton.IsDefault = True
        End If
        If MsgboxStyle = vbYesNoCancel Then
            YesButton.Visibility = Visibility.Visible
            NoButton.Visibility = Visibility.Visible
            cancelButton.Visibility = Visibility.Visible
            cancelButton.IsDefault = True
        End If
        If MsgboxStyle = vbYesNo Then
            YesButton.Visibility = Visibility.Visible
            NoButton.Visibility = Visibility.Visible
            NoButton.IsDefault = True
        End If

        _hideRequest = False
        While (Not _hideRequest)
            If Me.Dispatcher.HasShutdownStarted Or Me.Dispatcher.HasShutdownFinished Then
                Exit While
            End If

            Me.Dispatcher.Invoke(DispatcherPriority.Background,
            New ThreadStart(AddressOf doNothing))
        End While
        Return _result
    End Function

    Public Sub doNothing()

    End Sub

    Private Sub hide()

        _hideRequest = True

    End Sub



    Private Sub NoButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        _result = MessageBoxResult.No
        hide()
    End Sub

    Private Sub cancelButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        _result = MessageBoxResult.Cancel
        hide()
    End Sub

    Private Sub OkButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        _result = MessageBoxResult.OK
        hide()
    End Sub

    Private Sub YesButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        _result = MessageBoxResult.Yes
        hide()
    End Sub
End Class
