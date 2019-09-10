Public Class MouseWheelPass
    Public Shared ReadOnly ThroughProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("Through", GetType(Boolean), GetType(MouseWheelPass),
                                            New FrameworkPropertyMetadata(False, FrameworkPropertyMetadataOptions.None, AddressOf ThroughPropertyChanged))

    Public Shared Function GetThrough(obj As DependencyObject) As Boolean
        Return obj.GetValue(ThroughProperty)
    End Function

    Public Shared Sub SetThrough(obj As DependencyObject, value As Boolean)
        obj.SetValue(ThroughProperty, value)
    End Sub

    Private Shared Sub ThroughPropertyChanged(depObj As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim item As UIElement = depObj
        If item Is Nothing OrElse Not TypeOf (e.NewValue) Is Boolean Then
            Return
        Else
            If CType(e.NewValue, Boolean) = True Then
                AddHandler item.PreviewMouseWheel, AddressOf OnPreviewMouseWheel
            Else
                RemoveHandler item.PreviewMouseWheel, AddressOf OnPreviewMouseWheel
            End If
        End If
    End Sub

    Private Shared Sub OnPreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        e.Handled = True

        Dim e2 As New MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) With {.RoutedEvent = UIElement.MouseWheelEvent}
        Dim gv As UIElement = sender
        If gv IsNot Nothing Then gv.RaiseEvent(e2)
    End Sub
End Class
