Public Class AppCommands
    Public Shared ReadOnly NewConfig As RoutedUICommand = New RoutedUICommand("Neue Anwendungskonfiguration", "NewConfig", GetType(FrameworkElement))
    Public Shared ReadOnly NewOption As RoutedUICommand = New RoutedUICommand("Neue Option", "NewOption", GetType(FrameworkElement))
    Public Shared ReadOnly DeleteOption As RoutedUICommand = New RoutedUICommand("Option löschen", "DeleteOption", GetType(FrameworkElement))
    Public Shared ReadOnly EncryptFile As RoutedUICommand = New RoutedUICommand("Verschlüsselung festlegen", "EncryptFile", GetType(FrameworkElement))
    Public Shared ReadOnly EncryptFileNo As RoutedUICommand = New RoutedUICommand("Keine Verschlüsselung", "EncryptFileNo", GetType(FrameworkElement))
End Class
