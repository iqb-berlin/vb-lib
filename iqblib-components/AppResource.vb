Imports System
Imports Microsoft.Win32


Public Class AppResource
    ''' <summary>
    ''' Liefert ein Grafikobject, das als Resource in der Assembly liegt, als ImageResource
    ''' </summary>
    ''' <remarks>
    ''' Beispielaufruf: CopyButton.Content = New Image With {.Source = ImageSourceHelper.GetResourceImage("Resources/cut.png"), .Width = 18.0, .Height = 18.0}
    ''' </remarks>
    Public Shared Function GetResourceImage(resourcePath As String) As ImageSource
        Return GetResourceImage(Reflection.Assembly.GetCallingAssembly(), resourcePath)
    End Function

    Private Shared Function GetResourceImage(resourceAssembly As Reflection.Assembly, resourcePath As String) As ImageSource
        If String.IsNullOrEmpty(resourcePath) Then
            Return Nothing
        Else
            Return BitmapFrame.Create(New Uri(String.Format("pack://application:,,,/{0};component/{1}", resourceAssembly.GetName().Name, resourcePath), UriKind.RelativeOrAbsolute))
        End If
    End Function

End Class
