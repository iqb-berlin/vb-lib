Public Class InfoTextUserControl
    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(InfoTextUserControl))
    Public Property Text As String
        Get
            Return GetValue(TextProperty)
        End Get
        Set(ByVal value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

End Class
