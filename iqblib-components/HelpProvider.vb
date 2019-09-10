Public Class HelpProvider
    Public Const HelpTopicIdAboutWindow = "ABOUT"

    Public Shared ReadOnly HelpTopicIdProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("HelpTopicId", GetType(String), GetType(HelpProvider),
                                            New FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits))

    Private Shared HelpFileName As String = Nothing

    Shared Sub New()
        CommandManager.RegisterClassCommandBinding(GetType(FrameworkElement), New CommandBinding(ApplicationCommands.Help,
                New ExecutedRoutedEventHandler(AddressOf HandleHelpExecuted)))
    End Sub

    Private Shared Sub HandleHelpExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim fe As FrameworkElement = e.OriginalSource
        If fe Is Nothing Then fe = sender
        Do While fe IsNot Nothing AndAlso Not TypeOf (fe) Is Window
            fe = VisualTreeHelper.GetParent(fe)
        Loop
        If fe Is Nothing Then fe = DialogFactory.MainWindow
        If fe IsNot Nothing Then
            Dim myWindow As Window = fe

            Dim myHelpTopicId As String = Nothing
            If e.Parameter IsNot Nothing AndAlso TypeOf (e.Parameter) Is String And Not String.IsNullOrEmpty(e.Parameter) Then
                myHelpTopicId = e.Parameter
            ElseIf e.Source IsNot Nothing Then
                myHelpTopicId = HelpProvider.GetHelpTopicId(e.Source)
            End If

            If String.IsNullOrEmpty(myHelpTopicId) OrElse myHelpTopicId.ToUpper = HelpTopicIdAboutWindow Then
                Dim myAboutDlg As New AppAboutDialog With {.Owner = myWindow}
                myAboutDlg.ShowDialog()
            Else
                If String.IsNullOrEmpty(HelpFileName) Then
                    If (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) Then
                        Dim helpsource As String = Nothing
                        Try
                            helpsource = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri
                        Catch ex As Exception
                            helpsource = Nothing
                        End Try
                        If helpsource IsNot Nothing Then
                            If helpsource.IndexOf("file://") = 0 Then
                                Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                                Dim myHelpFileName As String = IO.Path.GetDirectoryName(System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.LocalPath) +
                                   IO.Path.DirectorySeparatorChar + myMainAssembly.GetName.Name + ".chm"
                                Dim fi As New IO.FileInfo(myHelpFileName)
                                If fi.Exists Then
                                    Try
                                        HelpFileName = IO.Path.GetTempPath + IO.Path.DirectorySeparatorChar + System.Guid.NewGuid.ToString + ".chm"
                                        IO.File.Copy(fi.FullName, HelpFileName, True)
                                    Catch ex As Exception
                                        Dim msg As String = ex.Message
                                        If ex.InnerException IsNot Nothing Then msg += vbNewLine + ex.InnerException.Message
                                        Debug.Print("HelpProvider: Konnte HelpFile nicht öffnen (" + msg + ").")
                                        HelpFileName = Nothing
                                    End Try
                                End If
                            ElseIf helpsource.IndexOf("http://") = 0 Then
                                Dim myHelpFilePath As String = helpsource.Substring(0, helpsource.LastIndexOf(".")) + ".chm"
                                Try
                                    Using web As New Net.WebClient
                                        Dim b As Byte() = web.DownloadData(myHelpFilePath)
                                        HelpFileName = IO.Path.GetTempPath + IO.Path.DirectorySeparatorChar + System.Guid.NewGuid.ToString + ".chm"
                                        IO.File.WriteAllBytes(HelpFileName, b)
                                    End Using
                                Catch ex As Exception
                                    Dim msg As String = ex.Message
                                    If ex.InnerException IsNot Nothing Then msg += vbNewLine + ex.InnerException.Message
                                    Debug.Print("HelpProvider: Konnte HelpFile nicht öffnen (" + msg + ").")
                                    HelpFileName = Nothing
                                End Try
                            End If
                        End If
                    Else
                        Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                        HelpFileName = IO.Path.GetDirectoryName(myMainAssembly.Location) + IO.Path.DirectorySeparatorChar + IO.Path.GetFileNameWithoutExtension(myMainAssembly.Location) + ".chm"
                    End If
                End If

                If String.IsNullOrEmpty(HelpFileName) Then
                    Dim myAboutDlg As New AppAboutDialog With {.Owner = myWindow}
                    myAboutDlg.ShowDialog()
                Else
                    Try
                        System.Windows.Forms.Help.ShowHelp(Nothing, HelpFileName, System.Windows.Forms.HelpNavigator.TopicId, myHelpTopicId)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                End If
            End If
        Else
            Debug.Print("HelpProvider: ParentWindow nicht gefunden.")
        End If
    End Sub

    Public Shared Function GetHelpTopicId(obj As DependencyObject) As String
        Return obj.GetValue(HelpTopicIdProperty)
    End Function

    Public Shared Sub SetHelpTopicId(obj As DependencyObject, value As String)
        obj.SetValue(HelpTopicIdProperty, value)
    End Sub

End Class
