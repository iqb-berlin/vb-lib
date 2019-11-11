Public Class ReminderConverter
    Implements IMultiValueConverter


    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        If Not values(0) Is DependencyProperty.UnsetValue AndAlso Not values(1) Is DependencyProperty.UnsetValue AndAlso
            values(0) IsNot Nothing AndAlso values(1) IsNot Nothing AndAlso
            TypeOf (values(0)) Is Integer AndAlso TypeOf (values(1)) Is Integer Then
            If CType(values(0), Integer) = CType(values(1), Integer) Then Return Brushes.Blue
        End If
        Return Brushes.LightGray
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("ReminderConverter ConvertBack")
    End Function
End Class

Public Class ReminderExpanderHeaderBackgroundConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If Not value Is DependencyProperty.UnsetValue AndAlso value IsNot Nothing Then
            Dim myCVS As ListCollectionView = value
            If myCVS.Count > 0 Then
                Dim r As Reminder = myCVS.GetItemAt(0)
                Return r.MaturityBrush
            End If
        End If
        Return Brushes.Transparent
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("ReminderExpanderHeaderBackgroundConverter ConvertBack")
    End Function
End Class