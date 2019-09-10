Public Class CredentialsUserControl
    Public Shared ReadOnly IsValidProperty As DependencyProperty =
             DependencyProperty.Register("IsValid", GetType(Boolean), GetType(CredentialsUserControl))

    Private _UserCredentials As System.Net.NetworkCredential = Nothing
    Public Property UserCredentials() As System.Net.NetworkCredential
        Get
            If IsValid Then
                Return New Net.NetworkCredential(TBName.Text, PBKennwort.Password)
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As System.Net.NetworkCredential)
            _UserCredentials = value
        End Set
    End Property

    Private Sub Me_Loaded() Handles Me.Loaded
        If _UserCredentials IsNot Nothing Then
            TBName.Text = _UserCredentials.UserName
            PBKennwort.Password = _UserCredentials.Password
        End If
        If String.IsNullOrEmpty(_UserCredentials.UserName) Then
            Me.TBName.Focus()
        Else
            Me.PBKennwort.Focus()
        End If
    End Sub


    Public Property IsValid() As Boolean
        Get
            Return GetValue(IsValidProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsValidProperty, value)
        End Set
    End Property

    Private Sub PasswordChanged() Handles PBKennwort.PasswordChanged
        SetValue(IsValidProperty, PBKennwort.Password.Length > 0)
    End Sub
End Class
