Public Class IQBCommands
    Public Shared ReadOnly Filter As RoutedUICommand = New RoutedUICommand("Filtern", "Filter", GetType(FrameworkElement))
    Public Shared ReadOnly FilterRemove As RoutedUICommand = New RoutedUICommand("Filter entfernen", "FilterRemove", GetType(FrameworkElement))
    Public Shared ReadOnly Report As RoutedUICommand = New RoutedUICommand("Bericht", "Report", GetType(FrameworkElement))
    Public Shared ReadOnly Download As RoutedUICommand = New RoutedUICommand("Download", "Download", GetType(FrameworkElement))
    Public Shared ReadOnly Table As RoutedUICommand = New RoutedUICommand("Tabelle", "Table", GetType(FrameworkElement))
    Public Shared ReadOnly RegisteredCommand As RoutedUICommand = New RoutedUICommand("Registrierte Funktion", "RegisteredCommand", GetType(FrameworkElement))

    Public Shared ReadOnly InsertObject As RoutedUICommand = New RoutedUICommand("Einfügen", "InsertObject",
                                                                        GetType(FrameworkElement),
                                                                        New InputGestureCollection From {New KeyGesture(Key.Insert, ModifierKeys.Control, "Ctrl+Einfg")})

    Public Shared ReadOnly EditObject As RoutedUICommand = New RoutedUICommand("Ändern", "EditObject",
                                                                        GetType(FrameworkElement),
                                                                        New InputGestureCollection From {New KeyGesture(Key.E, ModifierKeys.Control, "Ctrl+E")})

    Public Shared ReadOnly Options As RoutedUICommand = New RoutedUICommand("Optionen", "Options",
                                                                        GetType(FrameworkElement),
                                                                        New InputGestureCollection From {New KeyGesture(Key.Y, ModifierKeys.Control, "Ctrl+Y")})
    Public Shared ReadOnly ReloadObject As RoutedUICommand = New RoutedUICommand("Neu laden", "ReloadObject",
                                                                        GetType(FrameworkElement),
                                                                        New InputGestureCollection From {New KeyGesture(Key.R, ModifierKeys.Control, "Ctrl+R")})


    '###################################################################
    Public Delegate Sub RoutedCommandExecuteDelegate(Parameter As Object)
    Private Shared _RoutingCommandsList As Dictionary(Of String, RoutedCommandExecuteDelegate)

    Shared Sub New()
        _RoutingCommandsList = New Dictionary(Of String, RoutedCommandExecuteDelegate)
    End Sub

    Public Shared Sub RegisterCommand(CommandKey As String, ExecuteDelegate As RoutedCommandExecuteDelegate)
        If _RoutingCommandsList.ContainsKey(CommandKey) Then
            If ExecuteDelegate Is Nothing Then
                _RoutingCommandsList.Remove(CommandKey)
            Else
                _RoutingCommandsList.Item(CommandKey) = ExecuteDelegate
            End If
        Else
            _RoutingCommandsList.Add(CommandKey, ExecuteDelegate)
        End If
    End Sub


    Public Shared Function ExecuteCommand(CommandKey As String, CommandParameter As Object) As Boolean
        If _RoutingCommandsList.ContainsKey(CommandKey) AndAlso _RoutingCommandsList.Item(CommandKey) IsNot Nothing Then
            Try
                _RoutingCommandsList.Item(CommandKey).Invoke(CommandParameter)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End If
        Return False
    End Function
End Class
