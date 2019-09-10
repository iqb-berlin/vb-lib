Public Class TimeEditUserControl
    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(Integer), GetType(TimeEditUserControl), New FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))
    Public Property Value As Integer
        Get
            Return GetValue(ValueProperty)
        End Get
        Set(ByVal value As Integer)
            SetValue(ValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register("IsReadOnly", GetType(Boolean), GetType(TimeEditUserControl),
                                                                                                  New PropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return GetValue(IsReadOnlyProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property


    Private Sub TBTime_LostFocus(sender As Object, e As Windows.Input.KeyEventArgs) Handles TBTime.KeyDown
        Dim tb As TextBox = sender
        If e.Key = Key.Escape Then
            Dim be As BindingExpression = tb.GetBindingExpression(TextBox.TextProperty)
            If be IsNot Nothing Then be.UpdateTarget()
        End If
    End Sub

End Class
