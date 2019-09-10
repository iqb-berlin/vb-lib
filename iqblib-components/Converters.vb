'####################################################################
Public Class IntegerBooleanExact0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Integer) = 0
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerBooleanExact0Converter")
    End Function
End Class

'####################################################################
Public Class IntegerBooleanExact1Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Integer) = 1
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerBooleanExact1Converter")
    End Function
End Class

'####################################################################
Public Class IntegerBooleanGreaterThen1Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Integer) > 1
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerBooleanGreaterThen1Converter")
    End Function
End Class

'####################################################################
Public Class IntegerBooleanGreaterThen0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Integer) > 0
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerBooleanGreaterThen0Converter")
    End Function
End Class

'####################################################################
Public Class ObjectBooleanNotNothingConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return value IsNot Nothing
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in ObjectBooleanNotNothingConverter")
    End Function
End Class

'####################################################################
Public Class ObjectVisibilityNotNothingConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing OrElse value Is DependencyProperty.UnsetValue OrElse (TypeOf (value) Is String AndAlso String.IsNullOrEmpty(value)) Then
            Return Visibility.Collapsed
        Else
            Return Visibility.Visible
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack ObjectVisibilityNotNothingConverter")
    End Function
End Class

'####################################################################
Public Class ObjectVisibilityNothingConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf (value) Is String AndAlso String.IsNullOrEmpty(value) Then
            Return Visibility.Visible
        ElseIf value Is Nothing Then
            Return Visibility.Visible
        Else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack ObjectVisibilityNothingConverter")
    End Function
End Class

'####################################################################
Public Class BooleanXmlStringConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is DependencyProperty.UnsetValue OrElse String.IsNullOrEmpty(value) Then
            Return False
        ElseIf CType(value, String).ToLower = "true" Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        If TypeOf (value) Is Boolean AndAlso CType(value, Boolean) = True Then
            Return "true"
        Else
            Return "false"
        End If
    End Function
End Class

'####################################################################
Public Class BooleanVisibilityConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot Nothing Then
            If TypeOf value Is Boolean AndAlso CType(value, Boolean) Then
                Return Visibility.Visible
            ElseIf TypeOf value Is String AndAlso CType(value, String) = "True" OrElse CType(value, String) = "true" Then
                Return Visibility.Visible
            End If
        End If
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanVisibilityConverter")
    End Function
End Class

'####################################################################
Public Class BooleanNotVisibilityConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot Nothing Then
            If TypeOf value Is Boolean AndAlso CType(value, Boolean) Then
                Return Visibility.Collapsed
            ElseIf TypeOf value Is String AndAlso CType(value, String) = "True" OrElse CType(value, String) = "true" Then
                Return Visibility.Collapsed
            End If
        End If
        Return Visibility.Visible
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanNotVisibilityConverter")
    End Function
End Class

'####################################################################
Public Class TextBooleanNotEmptyConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TextBooleanNotEmptyConverter")
    End Function
End Class

'####################################################################
Public Class TextVisibilityNotEmptyConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value) Then
                Return Visibility.Visible
            Else
                Return Visibility.Collapsed
            End If
        Catch ex As Exception
            Return Visibility.Collapsed
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TextVisibilityNotEmptyConverter")
    End Function
End Class

'####################################################################
Public Class TextLengthVisibilityGreaterThen1Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso CType(value, String).Length > 1 Then
                Return Visibility.Visible
            Else
                Return Visibility.Collapsed
            End If
        Catch ex As Exception
            Return Visibility.Collapsed
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TextLengthVisibilityGreaterThen1Converter")
    End Function
End Class

'####################################################################
Public Class TextVisibilityNotEmptyHiddenConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value) Then
                Return Visibility.Visible
            Else
                Return Visibility.Hidden
            End If
        Catch ex As Exception
            Return Visibility.Hidden
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TextVisibilityNotEmptyHiddenConverter")
    End Function
End Class

'####################################################################
Public Class TextVisibilityEmptyConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If value IsNot Nothing AndAlso TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value) Then
                Return Visibility.Collapsed
            Else
                Return Visibility.Visible
            End If
        Catch ex As Exception
            Return Visibility.Visible
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TextVisibilityEmptyConverter")
    End Function
End Class

'####################################################################
Public Class BooleanReverseConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Return CType(value, Boolean) = False
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanReverseConverter")
    End Function
End Class

'####################################################################
Public Class BooleanOrVisibilityConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        For Each v As Boolean In values
            If v = True Then
                Return Visibility.Visible
                Exit For
            End If
        Next
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanOrVisibilityConverter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityGreaterThen0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 0 Then
                Return Visibility.Visible
            Else
                Return Visibility.Hidden
            End If
        Catch ex As Exception
            Return Visibility.Hidden
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityGreaterThen0Converter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityCollapsedGreaterThen0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 0 Then
                Return Visibility.Visible
            Else
                Return Visibility.Collapsed
            End If
        Catch ex As Exception
            Return Visibility.Collapsed
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityGreaterThen0Converter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityExact0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 0 Then
                Return Visibility.Hidden
            Else
                Return Visibility.Visible
            End If
        Catch ex As Exception
            Return Visibility.Visible
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityExact0Converter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityGreaterThen1Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 1 Then
                Return Visibility.Visible
            Else
                Return Visibility.Hidden
            End If
        Catch ex As Exception
            Return Visibility.Hidden
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityGreaterThen1Converter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityCollapsedGreaterThen1Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 1 Then
                Return Visibility.Visible
            Else
                Return Visibility.Collapsed
            End If
        Catch ex As Exception
            Return Visibility.Collapsed
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityGreaterThen1Converter")
    End Function
End Class

'####################################################################
Public Class IntegerVisibilityCollapsedNotGreaterThen0Converter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If CType(value, Integer) > 0 Then
                Return Visibility.Collapsed
            Else
                Return Visibility.Visible
            End If
        Catch ex As Exception
            Return Visibility.Visible
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in IntegerVisibilityCollapsedNotGreaterThen0Converter")
    End Function
End Class

'####################################################################
Public Class VisibilityBooleanConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Visibility) = Visibility.Visible
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack VisibilityBooleanConverter")
    End Function
End Class

'####################################################################
Public Class VisibilityBooleanNotConverter
    Implements IValueConverter


    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            Return CType(value, Visibility) <> Visibility.Visible
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack VisibilityBooleanNotConverter")
    End Function
End Class

'####################################################################
Public Class TimeStringIntegerConverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse Not TypeOf (value) Is Integer OrElse CType(value, Integer) = 0 Then
            Return ""
        Else
            Dim ts As TimeSpan = TimeSpan.FromSeconds(CType(value, Integer))
            Return ts.ToString("m\:ss")
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing OrElse Not TypeOf (value) Is String OrElse String.IsNullOrEmpty(value) Then
            Return 0
        Else
            Dim myInput As String = CType(value, String).Trim
            If Text.RegularExpressions.Regex.IsMatch(myInput, "^[0-9]$") OrElse Text.RegularExpressions.Regex.IsMatch(myInput, "^[0-5]{0,1}[0-9]$") Then
                Return Integer.Parse(myInput) * 60
            ElseIf Text.RegularExpressions.Regex.IsMatch(myInput, "^[0-5]{0,1}[0-9]:$") Then
                Return Integer.Parse(myInput.Substring(0, myInput.Length - 1)) * 60
            ElseIf Text.RegularExpressions.Regex.IsMatch(myInput, "^:[0-5]{0,1}[0-9]$") Then
                Return Integer.Parse(myInput.Substring(1))
            ElseIf Text.RegularExpressions.Regex.IsMatch(myInput, "^[0-5]{0,1}[0-9]:[0-5]{0,1}[0-9]$") Then
                Return Integer.Parse(myInput.Substring(0, myInput.IndexOf(":"))) * 60 + Integer.Parse(myInput.Substring(myInput.IndexOf(":") + 1))
            Else
                Return 0
            End If
        End If
    End Function
End Class

'####################################################################
Public Class StringIntegerConverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As Integer = 0
        If Not Integer.TryParse(value, myreturn) Then
            Return 0
        Else
            Return myreturn
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing OrElse Not TypeOf (value) Is Integer Then
            Return "0"
        Else
            Return CType(value, Integer).ToString
        End If
    End Function
End Class

'###############################################################
''' <summary>
''' value: XElement, dessen Children sortiert werden
''' parameter: p1#p2 wobei p1: TagName der Children, p2: Name des Attributes (Standard: id), das Sortierkriterium enthält; optional #p3 ein zweites Attribute
''' </summary>
Public Class XElementNumericSorterConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value IsNot Nothing AndAlso TypeOf (value) Is XElement AndAlso Not String.IsNullOrEmpty(parameter) Then
            Dim TagName As System.Xml.Linq.XName = Nothing
            Dim AttrName1 As System.Xml.Linq.XName = Nothing
            Dim AttrName2 As System.Xml.Linq.XName = Nothing
            Dim dummy_i As Integer = 0

            'aufwändige Prüfung, weil LINQ-/RegEx-Ausdrücke zu empfindlich sind
            If String.IsNullOrEmpty(parameter) Then
                AttrName1 = "id"
            Else
                Dim parameters As String() = CType(parameter, String).Split({"#"}, StringSplitOptions.RemoveEmptyEntries)
                TagName = parameters(0)
                If parameters.Length < 2 Then
                    AttrName1 = "id"
                Else
                    AttrName1 = parameters(1)
                End If
                For Each xc As XElement In CType(value, XElement).Elements(TagName)
                    If Text.RegularExpressions.Regex.IsMatch(xc.Attribute(AttrName1).Value, "\d+") Then
                        If Not Integer.TryParse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName1).Value, "\d+").Value, dummy_i) Then
                            AttrName1 = Nothing
                            Exit For
                        End If
                    Else
                        AttrName1 = Nothing
                        Exit For
                    End If
                Next
                If AttrName1 IsNot Nothing AndAlso parameters.Length > 2 Then
                    AttrName2 = parameters(1)
                    For Each xc As XElement In CType(value, XElement).Elements(TagName)
                        If Text.RegularExpressions.Regex.IsMatch(xc.Attribute(AttrName2).Value, "\d+") Then
                            If Not Integer.TryParse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName2).Value, "\d+").Value, dummy_i) Then
                                AttrName2 = Nothing
                                Exit For
                            End If
                        Else
                            AttrName2 = Nothing
                            Exit For
                        End If
                    Next
                End If
            End If
            '####################

            If TagName Is Nothing Then
                If AttrName1 Is Nothing Then
                    Return From xc As XElement In CType(value, XElement).Elements Select xc
                Else
                    Return From xc As XElement In CType(value, XElement).Elements
                           Let i As Integer = Integer.Parse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName1).Value, "\d+").Value)
                           Order By i Select xc
                End If
            Else
                If AttrName1 Is Nothing Then
                    Return From xc As XElement In CType(value, XElement).Elements(TagName) Select xc
                ElseIf AttrName2 Is Nothing Then
                    Return From xc As XElement In CType(value, XElement).Elements(TagName)
                           Let i As Integer = Integer.Parse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName1).Value, "\d+").Value)
                           Order By i Select xc
                Else
                    Return From xc As XElement In CType(value, XElement).Elements(TagName)
                           Let i As Integer = Integer.Parse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName1).Value, "\d+").Value),
                            i2 As Integer = Integer.Parse(Text.RegularExpressions.Regex.Match(xc.Attribute(AttrName2).Value, "\d+").Value)
                           Order By i, i2 Select xc
                End If
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("XElementNumericSorterConverter ConvertBack")
    End Function
End Class

'####################################
Public Class PathExtractDirConverter
    Implements IValueConverter

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("PathExtractDirConverter ConvertBack")
    End Function

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value) Then
            Dim d As String = IO.Path.GetDirectoryName(value)
            If d.EndsWith("\") Then
                Return d.Substring(0, d.Length - 1)
            Else
                Return d
            End If
        Else
            Return "?"
        End If
    End Function

End Class

'####################################
Public Class PathExtractFilenameConverter
    Implements IValueConverter

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("PathExtractFilenameConverter ConvertBack")
    End Function

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf (value) Is String AndAlso Not String.IsNullOrEmpty(value) Then
            If TypeOf (parameter) Is String Then
                Return CType(parameter, String) + IO.Path.GetFileName(value)
            Else
                Return IO.Path.GetFileName(value)
            End If
        Else
            Return "?"
        End If
    End Function

End Class

'####################################
Public Class CollectionViewConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Try
            If value Is Nothing Then Return Nothing
            If TypeOf (value) Is IEnumerable Then
                Return (From entry In CType(value, IEnumerable) Select entry).ToList
            End If

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class