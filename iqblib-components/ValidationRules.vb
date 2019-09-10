'################################################
Public Class DigitValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse String.IsNullOrEmpty(value) Then
            myreturn = New ValidationResult(False, "Darf nicht leer sein!")
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If Text.RegularExpressions.Regex.IsMatch(myInputString, "^[0-9]$") Then
                myreturn = ValidationResult.ValidResult
            Else
                myreturn = New ValidationResult(False, "Darf nur eine Ziffer enthalten!")
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class

'################################################
Public Class PosIntegerValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse String.IsNullOrEmpty(value) Then
            myreturn = New ValidationResult(False, "Darf nicht leer sein!")
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If Text.RegularExpressions.Regex.IsMatch(myInputString, "^[0-9]+$") Then
                myreturn = ValidationResult.ValidResult
            Else
                myreturn = New ValidationResult(False, "Darf nur eine Zahl enthalten!")
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class

'################################################
Public Class TimeValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing Then
            myreturn = ValidationResult.ValidResult
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If String.IsNullOrEmpty(myInputString) Then
                myreturn = ValidationResult.ValidResult
            Else
                If Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, "^[0-9]$") OrElse
                    Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, "^[0-5]{0,1}[0-9]$") OrElse
                    Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, "^[0-5]{0,1}[0-9]:[0-9]{0,1}$") OrElse
                    Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, "^:[0-5]{0,1}[0-9]$") OrElse
                    Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, "^[0-5]{0,1}[0-9]:[0-5]{0,1}[0-9]$") Then

                    myreturn = ValidationResult.ValidResult
                Else
                    myreturn = New ValidationResult(False, "Bitte eine Zeitangabe in der Form 'mm:ss' eingeben!")
                End If
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class
