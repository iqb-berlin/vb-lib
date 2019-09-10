Imports System.Security.Cryptography
'adapted from http://msdn.microsoft.com/en-us/library/ms172831.aspx

Public Class CryptoHelper

    Public Shared Function EncryptString(ByVal plaintext As String, EncryptionKey As String) As String
        If Not String.IsNullOrEmpty(plaintext) AndAlso Not String.IsNullOrEmpty(EncryptionKey) Then
            Dim plaintextBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(plaintext)
            Dim ms As New System.IO.MemoryStream
            Dim TripleDes As TripleDESCryptoServiceProvider = CreateTripleDESCryptoServiceProvider(EncryptionKey)
            Dim encStream As New CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
            encStream.FlushFinalBlock()

            Return Convert.ToBase64String(ms.ToArray)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function DecryptString(ByVal encryptedtext As String, EncryptionKey As String) As String
        Try
            Dim encryptedBytes() As Byte = Convert.FromBase64String(encryptedtext)
            Dim ms As New System.IO.MemoryStream
            Dim TripleDes As TripleDESCryptoServiceProvider = CreateTripleDESCryptoServiceProvider(EncryptionKey)
            Dim decStream As New CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
            decStream.FlushFinalBlock()

            Return System.Text.Encoding.Unicode.GetString(ms.ToArray)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()
        Dim sha1 As New SHA1CryptoServiceProvider
        Dim keyBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(key)
        Dim hash() As Byte = sha1.ComputeHash(keyBytes)
        ReDim Preserve hash(length - 1)
        Return hash
    End Function

    Private Shared Function CreateTripleDESCryptoServiceProvider(ByVal key As String) As TripleDESCryptoServiceProvider
        Dim TripleDES As New TripleDESCryptoServiceProvider
        TripleDES.Key = TruncateHash(key, TripleDES.KeySize \ 8)
        TripleDES.IV = TruncateHash("", TripleDES.BlockSize \ 8)
        Return TripleDES
    End Function
End Class
