Imports System.Collections.Specialized
Imports System.Xml

Public Class AppConfig
    Inherits Dictionary(Of String, Dictionary(Of String, String))

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(XDoc As XDocument, Optional DecryptKey As String = Nothing)
        MyBase.New()

        If XDoc IsNot Nothing Then
            For Each XSection As XElement In XDoc.Root.Elements
                Dim SectionKey As String = XSection.@key
                For Each XOption As XElement In XSection.Elements
                    Dim OptionKey As String = XOption.@key
                    Dim OptionValue As String = XOption.Value
                    If Not String.IsNullOrEmpty(DecryptKey) Then OptionValue = CryptoHelper.DecryptString(OptionValue, DecryptKey)
                    If Not Me.ContainsKey(SectionKey) Then Me.Add(SectionKey, New Dictionary(Of String, String))
                    Dim myOptionDict As Dictionary(Of String, String) = Me.Item(SectionKey)
                    If Not myOptionDict.ContainsKey(OptionKey) Then myOptionDict.Add(OptionKey, OptionValue)
                Next
            Next
        End If
    End Sub

    Public Sub New(ConfigfileName As String, Optional DecryptKey As String = Nothing)
        MyBase.New()

        If String.IsNullOrEmpty(ConfigfileName) OrElse Not IO.File.Exists(ConfigfileName) Then
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
                        ConfigfileName = IO.Path.GetDirectoryName(System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.LocalPath) +
                           IO.Path.DirectorySeparatorChar + myMainAssembly.GetName.Name + ".cfg"
                        If Not IO.File.Exists(ConfigfileName) Then
                            ConfigfileName = ""
                        End If
                    End If
                End If
            End If
            If String.IsNullOrEmpty(ConfigfileName) Then
                Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                ConfigfileName = IO.Path.GetDirectoryName(myMainAssembly.Location) + IO.Path.DirectorySeparatorChar + IO.Path.GetFileNameWithoutExtension(myMainAssembly.Location) + ".cfg"
                If Not IO.File.Exists(ConfigfileName) Then
                    ConfigfileName = ""
                End If
            End If
        End If

        'Throws Exeption!
        Dim FileData As Byte() = IO.File.ReadAllBytes(ConfigfileName)
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
                    If Not String.IsNullOrEmpty(DecryptKey) Then OptionValue = CryptoHelper.DecryptString(OptionValue, DecryptKey)
                    If Not Me.ContainsKey(SectionKey) Then Me.Add(SectionKey, New Dictionary(Of String, String))
                    Dim myOptionDict As Dictionary(Of String, String) = Me.Item(SectionKey)
                    If Not myOptionDict.ContainsKey(OptionKey) Then myOptionDict.Add(OptionKey, OptionValue)
                Next
            Next
        End If
    End Sub

    Public Function GetConfigValue(SectionKey As String, OptionKey As String) As String
        Dim myreturn As String = ""
        If Me.ContainsKey(SectionKey) Then
            Dim myOptionDict As Dictionary(Of String, String) = Me.Item(SectionKey)
            myreturn = myOptionDict.Item(OptionKey)
        End If
        Return myreturn
    End Function


    Public Function ToXDocument(Optional EncryptKey As String = Nothing) As XDocument
        Dim myreturn As XDocument = <?xml version="1.0" encoding="utf-8"?>
                                    <AppConfig/>

        For Each Section As KeyValuePair(Of String, Dictionary(Of String, String)) In Me
            Dim XSection As XElement = <Section key=<%= Section.Key %>/>
            For Each Opt As KeyValuePair(Of String, String) In Section.Value
                If String.IsNullOrEmpty(EncryptKey) Then
                    XSection.Add(<Option key=<%= Opt.Key %>><%= Opt.Value %></Option>)
                Else
                    XSection.Add(<Option key=<%= Opt.Key %>><%= CryptoHelper.EncryptString(Opt.Value, EncryptKey) %></Option>)
                End If
            Next
            myreturn.Root.Add(XSection)
        Next
        Return myreturn
    End Function

    Public Sub Save(ConfigfileName As String, Optional EncryptKey As String = Nothing)
        Dim xdoc As XDocument = Me.ToXDocument(EncryptKey)

        Using mstream As New IO.MemoryStream
            Using myXWriter As XmlWriter = XmlWriter.Create(mstream, New XmlWriterSettings With {.Indent = True})
                xdoc.Save(myXWriter)
            End Using
            Using mstream2 As New IO.MemoryStream
                Dim b() As Byte = Nothing
                Using zipstream As New IO.Compression.GZipStream(mstream2, IO.Compression.CompressionMode.Compress)
                    b = mstream.ToArray()
                    zipstream.Write(b, 0, b.Length)
                End Using
                b = mstream2.ToArray()
                IO.File.WriteAllBytes(ConfigfileName, b)
            End Using
        End Using
    End Sub

End Class
