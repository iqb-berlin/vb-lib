'####################################################################
Public Class PickLabelFromXElementConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing OrElse Not TypeOf (value) Is XElement Then
            Return Nothing
        Else
            Dim xe As XElement = value
            Return xe.@lb
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in PickLabelFromXElementConverter")
    End Function
End Class

'####################################################################
Public Class CheckListListLabelConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim AllChecklists As IEnumerable(Of XElement) = values(0)
        If values(1) Is Nothing OrElse values(1) Is DependencyProperty.UnsetValue Then
            Return Nothing
        Else
            Dim myreturn As New List(Of XElement)
            For Each xe As XElement In CType(values(1), IEnumerable)
                Dim Label As String = (From xc As XElement In AllChecklists Where xc.@id = xe.@id Select xc.Value).FirstOrDefault
                If String.IsNullOrEmpty(Label) Then
                    myreturn.Add(<cl id=<%= xe.@id %>><%= "?? " + xe.@id %></cl>)
                Else
                    myreturn.Add(<cl id=<%= xe.@id %>><%= Label %></cl>)
                End If
            Next
            Return From xe As XElement In myreturn Order By xe.Value
        End If
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in CheckListListLabelConverter")
    End Function
End Class

'####################################################################
Public Class BoolTextVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso CType(value, String) = "True" Then
            Return Visibility.Visible
        Else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("BoolTextVisibilityConverter ConvertBack")
    End Function
End Class

'####################################################################
Public Class PickValueFromKeyConverter
    Implements IMultiValueConverter

    Public Function Convert(ByVal values() As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IMultiValueConverter.Convert
        Dim Key As String
        If values(0) Is DependencyProperty.UnsetValue Then
            Key = Nothing
        Else
            Key = values(0)
        End If
        If String.IsNullOrEmpty(Key) AndAlso values.Length > 2 AndAlso Not values(2) Is DependencyProperty.UnsetValue Then Key = values(2) 'Kompatibilität mit altem @status
        If Not String.IsNullOrEmpty(Key) Then
            If values(1) IsNot DependencyProperty.UnsetValue AndAlso values(1) IsNot Nothing AndAlso TypeOf values(1) Is Dictionary(Of String, String) Then
                Dim Dict As Dictionary(Of String, String) = values(1)
                If Dict.ContainsKey(Key) Then
                    Return Dict.Item(Key)
                Else
                    Return "??? " + Key
                End If
            Else
                Return "?? " + Key
            End If
        End If
        Return "??"
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetTypes() As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object() Implements System.Windows.Data.IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("PickValueFromKeyConverter ConvertBack")
    End Function
End Class

Public Class MDKeyListLabelListConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As New List(Of XElement)
        If Not String.IsNullOrEmpty(value) AndAlso TypeOf (value) Is String Then
            For Each mddef As String In CType(value, String).Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                Dim mddefsplits As String() = mddef.Split({"##"}, StringSplitOptions.RemoveEmptyEntries)
                If mddefsplits.Count = 2 Then
                    myreturn.Add(<MD cat=<%= mddefsplits(0) %> def=<%= mddefsplits(1) %>><%= md.xml.MDCFactory.GetMDLabel(mddefsplits(0), mddefsplits(1)) %></MD>)
                End If
            Next

            Return Visibility.Visible
        Else
            Return Visibility.Collapsed
        End If
        Return myreturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("MDKeyListLabelListConverter ConvertBack")
    End Function
End Class
