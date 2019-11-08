Public Class ToDoListEntryDataTemplateSelector
    Inherits DataTemplateSelector

    Private _ToDoTemplate As DataTemplate
    Public Property ToDoTemplate() As DataTemplate
        Get
            Return _ToDoTemplate
        End Get
        Set(ByVal value As DataTemplate)
            _ToDoTemplate = value
        End Set
    End Property

    Private _ConfirmedTemplate As DataTemplate
    Public Property ConfirmedTemplate() As DataTemplate
        Get
            Return _ConfirmedTemplate
        End Get
        Set(ByVal value As DataTemplate)
            _ConfirmedTemplate = value
        End Set
    End Property

    Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate
        Dim XItem As XElement = item
        If XItem Is Nothing OrElse Not String.IsNullOrEmpty(XItem.@donedate) Then
            Return _ConfirmedTemplate
        Else
            Return _ToDoTemplate
        End If
    End Function

End Class
