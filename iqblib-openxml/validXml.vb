Public Class validXml
    Public Shared Function GetValidXmlcodedChar(c As Char) As Char
        Dim i As Integer = Asc(c)
        If i = &H9 OrElse i = &HA OrElse i = &HD OrElse
            (i >= &H20 AndAlso i <= &HD7FF) OrElse
            (i >= &HE000 AndAlso i <= &HFFFD) OrElse
            (i >= &H10000 AndAlso i <= &H10FFFF) Then
            Return c
        Else
            Return "`"
        End If
    End Function

    Public Shared Function GetValidXmlcodedString(s As String) As String
        Dim myreturn As String = ""
        For Each c As Char In s
            myreturn += GetValidXmlcodedChar(c)
        Next
        Return myreturn
    End Function
End Class
