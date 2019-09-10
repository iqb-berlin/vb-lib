Public Class TimeShowUserControl
    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(Integer), GetType(TimeShowUserControl), New FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))
    Public Property Value As Integer
        Get
            Return GetValue(ValueProperty)
        End Get
        Set(ByVal value As Integer)
            SetValue(ValueProperty, value)
        End Set
    End Property

End Class
