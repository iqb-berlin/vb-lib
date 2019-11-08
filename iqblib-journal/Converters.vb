'###########################################################
Public Class JournalEntryCategoryBrushConverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If String.IsNullOrEmpty(value) OrElse Not JournalFactory.JournalEntryCategories.ContainsKey(value) Then
            Return Brushes.Turquoise
        Else
            Return JournalFactory.JournalEntryCategories.Item(value)
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("JournalEntryCategoryBrushConverter ConvertBack")
    End Function
End Class

'####################################################################
Public Class XLogEntriesConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing OrElse Not TypeOf (value) Is XElement Then
            Return Nothing
        Else
            Return (From xe As XElement In CType(value, XElement).Elements
                    Order By xe.@date + xe.@sortstr Descending).ToList
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in XLogEntriesConverter")
    End Function
End Class

'####################################################################
Public Class LogDateStringConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If String.IsNullOrEmpty(value) Then
            Return "??"
        Else
            Dim ParseDate As Date
            If Date.TryParse(value, ParseDate) Then
                Return ParseDate.ToString("dd.MM.yyyy")
            Else
                Return "??..."
            End If
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in LogDateStringConverter")
    End Function
End Class
