Imports System.Xml

Public Class AppConfig
    Private Const _StandardConfigFileNameExtension = ".cfg"
    Private Shared _configData As Dictionary(Of String, String) = Nothing
    Private Shared _ConfigFileNameExtension As String = _StandardConfigFileNameExtension
    Public Shared Property ConfigFileNameExtension() As String
        Get
            Return _ConfigFileNameExtension
        End Get
        Set(ByVal value As String)
            _ConfigFileNameExtension = value
        End Set
    End Property
    Private Shared Function GetConfigData(Optional decryptKey As String = "") As Dictionary(Of String, String)
        Dim myreturn As New Dictionary(Of String, String)
        If String.IsNullOrEmpty(_ConfigFileNameExtension) Then
            _ConfigFileNameExtension = _StandardConfigFileNameExtension
        ElseIf _ConfigFileNameExtension.Substring(0, 1) <> "." Then
            _ConfigFileNameExtension = "." + _ConfigFileNameExtension
        End If

        Dim configFileName As String = ""
        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            Dim cfnsource As String = Nothing
            Try
                cfnsource = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri
            Catch ex As Exception
                cfnsource = Nothing
            End Try
            If cfnsource IsNot Nothing Then
                If cfnsource.IndexOf("file://") = 0 Then
                    Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                    configFileName = IO.Path.GetDirectoryName(System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.LocalPath) +
                           IO.Path.DirectorySeparatorChar + myMainAssembly.GetName.Name + _ConfigFileNameExtension
                    If Not IO.File.Exists(configFileName) Then
                        configFileName = ""
                    End If
                End If
            End If
        End If
        If String.IsNullOrEmpty(configFileName) Then
            Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
            configFileName = IO.Path.GetDirectoryName(myMainAssembly.Location) + IO.Path.DirectorySeparatorChar + IO.Path.GetFileNameWithoutExtension(myMainAssembly.Location) + _ConfigFileNameExtension
            If Not IO.File.Exists(configFileName) Then
                configFileName = ""
            End If
        End If

        If Not String.IsNullOrEmpty(configFileName) Then
            Dim FileData As Byte() = IO.File.ReadAllBytes(configFileName)
            Dim myXDoc As XDocument = Nothing

            Using mstream As New IO.MemoryStream(FileData)
                Using zipstream As New IO.Compression.GZipStream(mstream, IO.Compression.CompressionMode.Decompress)
                    Using myXReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(zipstream,
                                            New XmlReaderSettings With {.IgnoreWhitespace = True})
                        myXDoc = XDocument.Load(myXReader, LoadOptions.None)
                    End Using
                End Using
            End Using

            If myXDoc IsNot Nothing Then
                For Each XSection As XElement In myXDoc.Root.Elements
                    Dim SectionKey As String = XSection.@key
                    For Each XOption As XElement In XSection.Elements
                        Dim OptionKey As String = XOption.@key
                        Dim OptionValue As String = XOption.Value
                        Dim encrAttrValue As String = XOption.@encrypt
                        If String.IsNullOrEmpty(encrAttrValue) Then encrAttrValue = XOption.@encrypted
                        If Not String.IsNullOrEmpty(encrAttrValue) AndAlso encrAttrValue.ToUpper() = "TRUE" Then
                            OptionValue = CryptoHelper.DecryptString(OptionValue, decryptKey)
                        End If
                        If Not myreturn.ContainsKey(SectionKey + "::" + OptionKey) Then myreturn.Add(SectionKey + "::" + OptionKey, OptionValue)
                    Next
                Next
            End If
        End If

        Return myreturn
    End Function

    Public Shared Function GetConfigValue(SectionKey As String, OptionKey As String, Optional decryptKey As String = "") As String
        Dim myreturn As String = ""
        If _configData Is Nothing Then _configData = GetConfigData(decryptKey)
        If _configData.ContainsKey(SectionKey + "::" + OptionKey) Then myreturn = _configData.Item(SectionKey + "::" + OptionKey)
        Return myreturn
    End Function

    Public Shared Sub ResetConfigData()
        _configData = Nothing
    End Sub

End Class
