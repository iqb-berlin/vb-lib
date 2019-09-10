Public Class FilenameShowUserControl
    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(FilenameShowUserControl))
    Public Property Text As String
        Get
            Return GetValue(TextProperty)
        End Get
        Set(ByVal value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

End Class

Public Class FilenameShowUserControlConverter1
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse value Is DependencyProperty.UnsetValue OrElse String.IsNullOrEmpty(value) Then
            Return ""
        Else
            Return IO.Path.GetDirectoryName(value)
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("FilenameShowUserControlConverter1 ConvertBack")
    End Function
End Class

Public Class FilenameShowUserControlConverter2
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse value Is DependencyProperty.UnsetValue OrElse String.IsNullOrEmpty(value) Then
            Return ""
        Else
            Return IO.Path.DirectorySeparatorChar + IO.Path.GetFileName(value)
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("FilenameShowUserControlConverter2 ConvertBack")
    End Function
End Class
