Imports HUBerlin.IQB.Common.Controls

Public Class ToDoListUserControl
    Public Shared ReadOnly XToDoListProperty As DependencyProperty =
        DependencyProperty.Register("XToDoList", GetType(List(Of XElement)), GetType(ToDoListUserControl))

    Public Property XToDoList As List(Of XElement)
        Get
            Return GetValue(XToDoListProperty)
        End Get
        Set(ByVal value As List(Of XElement))
            SetValue(XToDoListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CanAddDeleteProperty As DependencyProperty =
        DependencyProperty.Register("CanAddDelete", GetType(Boolean), GetType(ToDoListUserControl))

    Public Property CanAddDelete As Boolean
        Get
            Return GetValue(CanAddDeleteProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(CanAddDeleteProperty, value)
        End Set
    End Property

    Public Sub Refresh()
        Dim be As BindingExpression = BindingOperations.GetBindingExpression(Me, ToDoListUserControl.XToDoListProperty)
        If be IsNot Nothing Then be.UpdateTarget()
    End Sub

End Class
