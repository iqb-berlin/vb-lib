Public Class Commands
    Public Shared ReadOnly AddToDoList As RoutedUICommand = New RoutedUICommand("ToDo-Liste hinzufügen", "AddToDoList", GetType(FrameworkElement))
    Public Shared ReadOnly RemoveToDoList As RoutedUICommand = New RoutedUICommand("ToDo-Liste entfernen", "RemoveToDoList", GetType(FrameworkElement))
    Public Shared ReadOnly Confirm As RoutedUICommand = New RoutedUICommand("ToDo bestätigen", "Confirm", GetType(FrameworkElement))

End Class
