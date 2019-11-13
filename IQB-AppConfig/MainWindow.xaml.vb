Imports iqb.lib.components

Class MainWindow
    Private AppConfigChanged As Boolean
    Private EncryptKey As String
    Private XAppConfigDoc As XDocument = Nothing

    Private Sub MainApplication_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf MyUnhandledExceptionEventHandler

        Me.Title = My.Application.Info.AssemblyName

        DialogFactory.MainWindow = Me

        Dim ContinueStart As Boolean = True
        Dim ErrMsg As String = "Es gibt ein Problem bei dem Versuch, die alten lokalen Programmeinstellungen zu laden. Bitte deinstallieren Sie die Anwendung über die Systemsteuerung und installieren Sie sie dann erneut!"
        Dim UserConfigFilename As String = ""
        Try
            'neue Programmversion -> alte Settings holen
            If Not My.Settings.updated Then
                My.Settings.Upgrade()
                My.Settings.updated = True
                My.Settings.Save()
            End If
        Catch ex As System.Configuration.ConfigurationException
            ContinueStart = False
            If ex.InnerException Is Nothing Then
                Debug.Print("Configuration.ConfigurationException ohne InnerException")
            Else
                ErrMsg += " Alternativ können Sie die unten genannte Datei löschen (Achtung: Apps ist ein verstecktes Verzeichnis)." + vbNewLine + vbNewLine + ex.InnerException.Message
                Debug.Print(ex.InnerException.Message)
                Dim pos As Integer = ex.InnerException.Message.IndexOf("(")
                If pos > 0 Then
                    UserConfigFilename = ex.InnerException.Message.Substring(pos + 1)
                    pos = UserConfigFilename.IndexOf("\user.config ")
                    If pos > 0 Then
                        UserConfigFilename = UserConfigFilename.Substring(0, pos) + "\user.config"
                        Debug.Print(">>" + UserConfigFilename + "<<")
                    Else
                        UserConfigFilename = ""
                    End If
                End If
            End If
        End Try

        If Not ContinueStart Then
            If Not String.IsNullOrEmpty(UserConfigFilename) AndAlso
                UserConfigFilename.IndexOfAny(IO.Path.GetInvalidFileNameChars()) < 0 AndAlso
                IO.File.Exists(UserConfigFilename) Then
                Try
                    IO.File.Delete(UserConfigFilename)
                    ErrMsg = "Die lokalen Programmeinstellungen mussten gelöscht werden. Bitte starten Sie die Anwendung erneut!"
                Catch ex As Exception
                    ErrMsg += vbNewLine + vbNewLine + "Löschen gescheitert: " + ex.Message
                End Try
            End If
            DialogFactory.MsgError(Me, Me.Title, ErrMsg)
            Me.Close()
        End If


        If Not ContinueStart Then Me.Close()

        CommandBindings.Add(New CommandBinding(ApplicationCommands.[New], AddressOf HandleNewSectionExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Open, AddressOf HandleOpenExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Delete, AddressOf HandleDeleteSectionExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Save, AddressOf HandleSaveExecuted, AddressOf HandleSaveCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.NewConfig, AddressOf HandleNewConfigExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.EncryptFile, AddressOf HandleEncryptFileExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.EncryptFileNo, AddressOf HandleEncryptFileNoExecuted, AddressOf HandleEncryptFileNoCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.NewOption, AddressOf HandleNewOptionExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.DeleteOption, AddressOf HandleDeleteOptionExecuted))

        AppCommands.NewConfig.Execute(Nothing, Nothing)
    End Sub

    '############################################
    Private Sub MyUnhandledExceptionEventHandler(sender As Object, e As UnhandledExceptionEventArgs)
        Dim MsgText As String = "??"
        If TypeOf (e.ExceptionObject) Is System.Exception Then
            Dim myException As System.Exception = e.ExceptionObject
            MsgText = myException.Message
            If myException.InnerException IsNot Nothing Then MsgText += "; " + myException.InnerException.Message
            If Not String.IsNullOrEmpty(myException.StackTrace) Then
                If myException.StackTrace.Length > 500 Then
                    MsgText += vbNewLine + myException.StackTrace.Substring(0, 500) + "..."
                Else
                    MsgText += vbNewLine + myException.StackTrace
                End If
            End If
        ElseIf TypeOf (e.ExceptionObject) Is Runtime.CompilerServices.RuntimeWrappedException Then
            Dim myException As Runtime.CompilerServices.RuntimeWrappedException = e.ExceptionObject
            If myException.InnerException IsNot Nothing Then MsgText += "; " + myException.InnerException.Message
            If Not String.IsNullOrEmpty(myException.StackTrace) Then
                If myException.StackTrace.Length > 500 Then
                    MsgText += vbNewLine + myException.StackTrace.Substring(0, 500) + "..."
                Else
                    MsgText += vbNewLine + myException.StackTrace
                End If
            End If
        End If

        DialogFactory.MsgError(Me, "Absturz " + My.Application.Info.AssemblyName, "Die Anwendung hat einen unerwarteten Abbruch erlitten. Folgende Informationen könnten bei der Fehlersuche helfen:" +
                               vbNewLine + vbNewLine + MsgText)

        Me.Close()
    End Sub

    Private Sub Me_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If AppConfigChanged AndAlso
            DialogFactory.YesNo(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?") = vbOK Then
            ApplicationCommands.Save.Execute(Nothing, Nothing)
        End If
    End Sub
    Private Sub HandleNewConfigExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim mbresult As MessageBoxResult = MessageBoxResult.OK
        If AppConfigChanged Then
            mbresult = DialogFactory.YesNoCancel(Me, My.Application.Info.AssemblyName, "Die Konfiguration wurde geändert. Soll sie vor dem Schließen gespeichert werden?")
            If mbresult = MessageBoxResult.OK Then
                ApplicationCommands.Save.Execute(Nothing, Nothing)
            End If
        End If
        If mbresult <> MessageBoxResult.Cancel Then
            If XAppConfigDoc IsNot Nothing Then
                RemoveHandler XAppConfigDoc.Root.Changed, AddressOf Notify_XAppConfigDocChanged
            End If
            Dim tmpAppConfig As New iqb.lib.config.AppConfig("")
            XAppConfigDoc = tmpAppConfig.ToXDocument()
            Me.LBSections.DataContext = XAppConfigDoc.Root
            AddHandler XAppConfigDoc.Root.Changed, AddressOf Notify_XAppConfigDocChanged
            AppConfigChanged = False
        End If
    End Sub

    Private Sub HandleOpenExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim mbresult As MessageBoxResult = MessageBoxResult.OK
        If AppConfigChanged Then
            mbresult = DialogFactory.YesNoCancel(Me, My.Application.Info.AssemblyName, "Die Konfiguration wurde geändert. Soll sie vor dem Schließen gespeichert werden?")
            If mbresult = MessageBoxResult.OK Then
                ApplicationCommands.Save.Execute(Nothing, Nothing)
            End If
        End If
        If mbresult <> MessageBoxResult.Cancel Then

            Dim fname As String = ""
            Dim fdir As String = ""
            If Not String.IsNullOrEmpty(My.Settings.lastfile_cfg) Then
                fname = IO.Path.GetFileName(My.Settings.lastfile_cfg)
                fdir = IO.Path.GetDirectoryName(My.Settings.lastfile_cfg)
            End If
            Dim filepicker As New Microsoft.Win32.OpenFileDialog With {.FileName = fname, .Filter = "CFG-Dateien|*.cfg", .InitialDirectory = fdir,
                                                                           .DefaultExt = "cfg", .Title = "Anwendungskonfiguration öffnen"}
            If filepicker.ShowDialog Then
                My.Settings.lastfile_cfg = filepicker.FileName
                My.Settings.Save()

                Try
                    If XAppConfigDoc IsNot Nothing Then
                        RemoveHandler XAppConfigDoc.Root.Changed, AddressOf Notify_XAppConfigDocChanged
                    End If
                    Dim tmpAppConfig As New iqb.lib.config.AppConfig(filepicker.FileName, EncryptKey)
                    XAppConfigDoc = tmpAppConfig.ToXDocument()
                    Me.LBSections.DataContext = XAppConfigDoc.Root
                    AddHandler XAppConfigDoc.Root.Changed, AddressOf Notify_XAppConfigDocChanged
                    AppConfigChanged = False

                    'UpdateControls()
                Catch ex As Exception
                    DialogFactory.MsgError(Me, "Anwendungskonfiguration öffnen", "Konnte Datei nicht öffnen:" + vbNewLine + ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub Notify_XAppConfigDocChanged(sender As Object, e As System.Xml.Linq.XObjectChangeEventArgs)
        AppConfigChanged = True
    End Sub


    Private Sub HandleSaveExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If XAppConfigDoc IsNot Nothing Then
            Dim fname As String = ""
            Dim fdir As String = ""
            If Not String.IsNullOrEmpty(My.Settings.lastfile_cfg) Then
                fname = IO.Path.GetFileName(My.Settings.lastfile_cfg)
                fdir = IO.Path.GetDirectoryName(My.Settings.lastfile_cfg)
            End If

            Dim filepicker As New Microsoft.Win32.SaveFileDialog With {.FileName = fname, .Filter = "Anwendungskonfiguration|*.cfg", .InitialDirectory = fdir,
                .DefaultExt = "cfg", .Title = "Anwendungskonfiguration speichern"}
            If filepicker.ShowDialog Then
                My.Settings.lastfile_cfg = filepicker.FileName
                My.Settings.Save()

                Try
                    Dim tmpAppConfig As New iqb.lib.config.AppConfig(XAppConfigDoc)
                    tmpAppConfig.Save(filepicker.FileName, EncryptKey)
                    AppConfigChanged = False
                Catch ex As Exception
                    DialogFactory.MsgError(Me, "Speichern Anwendungskonfiguration '" + IO.Path.GetFileName(filepicker.FileName) + "'", "Konnte nicht speichern:" + vbNewLine + ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function HandleEncryptFileNoCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = Not String.IsNullOrEmpty(EncryptKey)
        Return e.CanExecute
    End Function

    Private Function HandleSaveCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = AppConfigChanged
        Return e.CanExecute
    End Function

    '############################################
    Private Sub HandleNewSectionExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If XAppConfigDoc IsNot Nothing Then
            Dim sectionkey As String = DialogFactory.InputText(Me, "Neue Sektion", "Bitte Schlüssel eingeben", "", "")
            If Not String.IsNullOrEmpty(sectionkey) Then
                Dim XFound As XElement = (From xe As XElement In XAppConfigDoc.Root.Elements Where xe.@key.ToLower = sectionkey.ToLower).FirstOrDefault
                If XFound IsNot Nothing Then
                    DialogFactory.MsgError(Me, "Neue Sektion", "Schlüssel ist bereits vorhanden.")
                Else
                    XAppConfigDoc.Root.Add(<Section key=<%= sectionkey %>></Section>)
                    LBSections.SelectedValue = sectionkey
                End If
            End If
        End If
    End Sub

    Private Sub HandleDeleteSectionExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If XAppConfigDoc IsNot Nothing Then
            Dim XSelection As List(Of XElement) = (From xe As XElement In XAppConfigDoc.Root.Elements
                                                   Order By xe.@key
                                                   Let xenew As XElement = <c key=<%= xe.@key %>><%= xe.@key %></c>
                                                   Select xenew).ToList
            Dim myDlg As New XSelectionDialog() With {.Owner = Me, .XSelectionList = XSelection, .MultipleSelection = True, .Title = "Sektion löschen"}
            If myDlg.ShowDialog Then
                Dim selectedKeys As List(Of String) = myDlg.Selected.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
                For Each XtoDelete As XElement In (From xe As XElement In XAppConfigDoc.Root.Elements Where selectedKeys.Contains(xe.@key) Select xe).ToList
                    XtoDelete.Remove()
                Next
            End If
        End If
    End Sub

    Private Sub HandleNewOptionExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If LBSections.SelectedItems.Count > 0 Then
            Dim optionkey As String = DialogFactory.InputText(Me, "Neue Option", "Bitte Schlüssel eingeben", "", "")
            If Not String.IsNullOrEmpty(optionkey) Then
                Dim XSection As XElement = LBSections.SelectedItem
                Dim XFound As XElement = (From xe As XElement In XSection.Elements Where xe.@key.ToLower = optionkey.ToLower).FirstOrDefault
                If XFound IsNot Nothing Then
                    DialogFactory.MsgError(Me, "Neue Option", "Schlüssel ist bereits vorhanden.")
                Else
                    XSection.Add(<Option key=<%= optionkey %>></Option>)
                    Dim be As BindingExpression = ICOptions.GetBindingExpression(ItemsControl.ItemsSourceProperty)
                    If be IsNot Nothing Then be.UpdateTarget()
                End If
            End If
        End If
    End Sub

    Private Sub HandleDeleteOptionExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If LBSections.SelectedItems.Count > 0 Then
            Dim XSection As XElement = LBSections.SelectedItem
            Dim XSelection As List(Of XElement) = (From xe As XElement In XSection.Elements
                                                   Order By xe.@key
                                                   Let xenew As XElement = <c key=<%= xe.@key %>><%= xe.@key %></c>
                                                   Select xenew).ToList

            Dim myDlg As New XSelectionDialog() With {.Owner = Me, .XSelectionList = XSelection, .MultipleSelection = True, .Title = "Option löschen"}
            If myDlg.ShowDialog Then
                Dim selectedKeys As List(Of String) = myDlg.Selected.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
                For Each XtoDelete As XElement In (From xe As XElement In XSection.Elements Where selectedKeys.Contains(xe.@key) Select xe).ToList
                    XtoDelete.Remove()
                Next
                Dim be As BindingExpression = ICOptions.GetBindingExpression(ItemsControl.ItemsSourceProperty)
                If be IsNot Nothing Then be.UpdateTarget()
            End If
        End If
    End Sub

    Private Sub HandleEncryptFileExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim newEK As String = DialogFactory.InputText(Me, "Verschlüsselungs-Code", "Bitte Code eingeben", EncryptKey, "")
        If Not String.IsNullOrEmpty(newEK) Then
            EncryptKey = newEK
            TBEnc.Text = "Code aktiv"
            AppConfigChanged = True
        End If
    End Sub

    Private Sub HandleEncryptFileNoExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        EncryptKey = ""
        TBEnc.Text = "kein Code"
        AppConfigChanged = True
    End Sub
End Class
