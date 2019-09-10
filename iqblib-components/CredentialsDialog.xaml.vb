Public Class CredentialsDialog

    Public Property UserCredentials() As System.Net.NetworkCredential
        Get
            Return CCCUC.UserCredentials
        End Get
        Set(ByVal value As System.Net.NetworkCredential)
            CCCUC.UserCredentials = value
        End Set
    End Property

    Private Sub MeLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        Me.CCCUC.Focus()
    End Sub

    Private Sub BtnOK_Click()
        DialogResult = True
    End Sub

    Private Sub BtnCancel_Click()
        DialogResult = False
    End Sub

End Class
