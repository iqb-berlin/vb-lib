'##############################################
Public Class MessagePopupBackgroundConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If String.IsNullOrEmpty(value) Then
            Return Brushes.PaleGoldenrod
        Else
            Dim myMsg As String = value
            If myMsg.Length > 3 AndAlso myMsg.Substring(0, 2).ToUpper = "E:" Then
                myMsg = myMsg.Substring(2).TrimStart
                Return Brushes.LightPink
            Else
                Return Brushes.PaleGoldenrod
            End If
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("MessagePopupBackgroundConverter ConvertBack")
    End Function
End Class


'##############################################
Public Class MessagePopupTextConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If String.IsNullOrEmpty(value) Then
            Return ""
        Else
            Dim myMsg As String = value
            If myMsg.Length > 3 AndAlso myMsg.Substring(0, 2).ToUpper = "E:" Then
                Return myMsg.Substring(2).TrimStart
            Else
                Return myMsg
            End If
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("MessagePopupTextConverter ConvertBack")
    End Function
End Class


'##############################################
Public Class CheckBoxXValueMultiConverter
    Implements IMultiValueConverter

    Public Function Convert(ByVal values() As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IMultiValueConverter.Convert
        If TypeOf values(0) Is String AndAlso Not String.IsNullOrEmpty(values(0)) AndAlso
            TypeOf values(1) Is String AndAlso Not String.IsNullOrEmpty(values(1)) AndAlso TypeOf parameter Is ItemsControl Then
            Dim XValue As XElement = CType(parameter, ItemsControl).DataContext
            If XValue IsNot Nothing Then
                Return XValue.Value.Split({" "}, System.StringSplitOptions.RemoveEmptyEntries).Contains(CType(values(1), String))
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetTypes() As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object() Implements System.Windows.Data.IMultiValueConverter.ConvertBack
        If TypeOf (parameter) Is ItemsControl Then
            Dim myItemzControl As ItemsControl = parameter
            Dim myNewValue As String = ""
            For i = 0 To myItemzControl.Items.Count - 1
                Dim mycheckbox As CheckBox = CType(VisualTreeHelper.GetChild(
                        CType(myItemzControl.ItemContainerGenerator.ContainerFromIndex(i), ContentPresenter), 0), CheckBox)
                If mycheckbox.IsChecked Then
                    If Not String.IsNullOrEmpty(myNewValue) Then myNewValue += " "
                    myNewValue += CType(mycheckbox.DataContext, KeyValuePair(Of String, String)).Key
                End If
            Next
            Return {myNewValue}
        Else
            Throw New ArgumentException("CheckBoxStringMultiConverter - ConvertBack")
            Return Nothing
        End If
    End Function
End Class


'##############################################
Public Class CheckBoxXAttributeMultiConverter
    Implements IMultiValueConverter

    Public Function Convert(ByVal values() As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IMultiValueConverter.Convert
        If TypeOf values(0) Is String AndAlso Not String.IsNullOrEmpty(values(0)) AndAlso
            TypeOf values(1) Is String AndAlso Not String.IsNullOrEmpty(values(1)) AndAlso TypeOf parameter Is ItemsControl Then
            Dim XValue As XAttribute = CType(parameter, ItemsControl).DataContext
            If XValue IsNot Nothing Then
                Return XValue.Value.Split({" "}, System.StringSplitOptions.RemoveEmptyEntries).Contains(CType(values(1), String))
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetTypes() As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object() Implements System.Windows.Data.IMultiValueConverter.ConvertBack
        If TypeOf (parameter) Is ItemsControl Then
            Dim myItemzControl As ItemsControl = parameter
            Dim myNewValue As String = ""
            For i = 0 To myItemzControl.Items.Count - 1
                Dim mycheckbox As CheckBox = CType(VisualTreeHelper.GetChild(
                        CType(myItemzControl.ItemContainerGenerator.ContainerFromIndex(i), ContentPresenter), 0), CheckBox)
                If mycheckbox.IsChecked Then
                    If Not String.IsNullOrEmpty(myNewValue) Then myNewValue += " "
                    myNewValue += CType(mycheckbox.DataContext, KeyValuePair(Of String, String)).Key
                End If
            Next
            Return {myNewValue}
        Else
            Throw New ArgumentException("CheckBoxStringMultiConverter - ConvertBack")
            Return Nothing
        End If
    End Function
End Class

'##############################################
Public Class CheckBoxListDictStringConverter
    Implements IMultiValueConverter

    Public Function Convert(ByVal values() As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IMultiValueConverter.Convert
        If values(0) IsNot Nothing AndAlso TypeOf values(0) Is String AndAlso TypeOf values(1) Is String AndAlso Not String.IsNullOrEmpty(values(1)) AndAlso TypeOf parameter Is ItemsControl Then
            Dim PropValue As String = values(0)
            Return PropValue.Split({" "}, System.StringSplitOptions.RemoveEmptyEntries).Contains(CType(values(1), String))
        End If
        Return False
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetTypes() As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object() Implements System.Windows.Data.IMultiValueConverter.ConvertBack
        If TypeOf (parameter) Is ItemsControl Then
            Dim myItemzControl As ItemsControl = parameter
            Dim myCheckboxListControl As FrameworkElement = parameter
            Do
                myCheckboxListControl = myCheckboxListControl.Parent
            Loop Until myCheckboxListControl Is Nothing OrElse TypeOf (myCheckboxListControl) Is CheckBoxListDictStringUserControl

            If myCheckboxListControl IsNot Nothing Then
                Dim myNewValue As String = ""
                For i = 0 To myItemzControl.Items.Count - 1
                    Dim mycheckbox As CheckBox = CType(VisualTreeHelper.GetChild(
                            CType(myItemzControl.ItemContainerGenerator.ContainerFromIndex(i), ContentPresenter), 0), CheckBox)
                    If mycheckbox.IsChecked Then
                        If Not String.IsNullOrEmpty(myNewValue) Then myNewValue += " "
                        myNewValue += CType(mycheckbox.DataContext, KeyValuePair(Of String, String)).Key
                    End If
                Next

                Return {myNewValue}
            Else
                Throw New ArgumentException("CheckBoxListDictStringConverter - ConvertBack")
                Return Nothing
            End If
        End If
        Return Nothing
    End Function
End Class
