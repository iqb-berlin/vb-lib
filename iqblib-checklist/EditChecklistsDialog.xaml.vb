Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports iqb.md.xml
Imports iqb.lib.components

Public Class EditChecklistsDialog
    Private _ChecklistPoolToEdit As ChecklistPool
    Private _MDFilter As MDFilter
    Private _MDCatalogList As List(Of String)
    Private _Username As String
    Private Shared LastDocxReportFilename As String = ""
    Private Shared TemplateInfo As Windows.Resources.StreamResourceInfo = Nothing

    Public Sub New(ChecklistPoolToEdit As ChecklistPool, MDCatalogList As List(Of String), MDFilter As MDFilter, Username As String)
        InitializeComponent()
        _ChecklistPoolToEdit = ChecklistPoolToEdit
        _MDCatalogList = MDCatalogList
        _MDFilter = MDFilter
        _Username = Username
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        Me.Title = _ChecklistPoolToEdit.PoolLabel + "-Checklisten bearbeiten"

        DPChecklistData.DataContext = Nothing
        CPEUC.MDCatalogList = _MDCatalogList
        CPEUC.MDFilter = _MDFilter

        CommandBindings.Add(New CommandBinding(IQBCommands.Report, AddressOf HandleReportExecuted, AddressOf HandleReportCanExecute))

        LoadChecklists()
    End Sub

    Private Sub HandleReportExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        If LBChecklists.Items.Count > 0 Then
            Dim filepicker As New Microsoft.Win32.SaveFileDialog With {.FileName = LastDocxReportFilename, .Filter = "MS-Word-Dateien|*.docx",
                                                   .DefaultExt = "Docx", .Title = "Checklisten als Docx-Datei ausgeben"}
            If filepicker.ShowDialog Then
                LastDocxReportFilename = filepicker.FileName

                If TemplateInfo Is Nothing Then TemplateInfo = Application.GetResourceStream(
                    New Uri("/" + System.Reflection.Assembly.GetAssembly(GetType(EditChecklistsDialog)).GetName.Name + ";component/Reportvorlage Checklists.docx", UriKind.Relative))

                If TemplateInfo IsNot Nothing AndAlso TemplateInfo.Stream IsNot Nothing AndAlso TemplateInfo.Stream.Length > 0 AndAlso
                    _ChecklistPoolToEdit IsNot Nothing AndAlso Not String.IsNullOrEmpty(LastDocxReportFilename) Then

                    Dim msgText As String = ""

                    Try
                        Dim fs As New IO.FileStream(LastDocxReportFilename, IO.FileMode.Create)
                        TemplateInfo.Stream.CopyTo(fs)
                        fs.Close()
                    Catch ex As Exception
                        DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, ex.ToString)
                        TemplateInfo = Nothing
                    End Try
                    If TemplateInfo IsNot Nothing Then
                        Using memorystream As IO.MemoryStream = New IO.MemoryStream
                            TemplateInfo.Stream.Seek(0, IO.SeekOrigin.Begin)
                            TemplateInfo.Stream.CopyTo(memorystream)
                            'memorystream.Write(myTemplate, 0, CInt(myTemplate.Length))
                            Using NewDoc As WordprocessingDocument = WordprocessingDocument.Open(memorystream, True)
                                Dim docPart = NewDoc.MainDocumentPart
                                Dim doc = docPart.Document

                                Dim myType As Type = GetType(EditChecklistsDialog)
                                iqb.lib.openxml.docxFactory.SetCustomProperty(NewDoc, "Log",
                                                                              "Erzeugt mit " + myType.AssemblyQualifiedName + "; " + _Username + "; " + DateTime.Now.ToShortDateString)

                                Dim ChecklistLabels As New Dictionary(Of String, String)
                                For Each Checklist As KeyValuePair(Of String, XElement) In _ChecklistPoolToEdit.Pool
                                    ChecklistLabels.Add(Checklist.Value.@id, Checklist.Value.@lb)
                                Next

                                '########
                                'Kopf
                                Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                                Dim AppData As Reflection.AssemblyName = myMainAssembly.GetName

                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Title"}),
                                                      New Run(New Text(AppData.Name + ": " + _ChecklistPoolToEdit.PoolLabel + " - Checklisten-Übersicht"))))
                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Standard"}),
                                                      New Run(New Text("Stand " + DateTime.Now.ToShortDateString))))

                                For Each Checklist As KeyValuePair(Of String, XElement) In From CL As KeyValuePair(Of String, XElement) In _ChecklistPoolToEdit.Pool Order By CL.Value.@lb Select CL
                                    doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Header1"}),
                                                          New Run(New Text(Checklist.Value.@lb))))

                                    For Each XPoint As XElement In Checklist.Value.Elements
                                        doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Header2"}),
                                                              New Run(New Text(XPoint.<label>.First.Value))))
                                        doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                              New Run(New Text("Im Journal wird folgender Text eingetragen: """ + XPoint.<journaltext>.First.Value + """."))))

                                        If XPoint.@input = "True" Then
                                            Dim adding As String = ""
                                            If XPoint.<prompt>.FirstOrDefault IsNot Nothing AndAlso Not String.IsNullOrEmpty(XPoint.<prompt>.First.Value) Then _
                                        adding = " Die Eingabeaufforderung wird mit """ + XPoint.<prompt>.First.Value + """ beschriftet."
                                            doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                                  New Run(New Text("Dieser Punkt fordert vom Nutzer eine zusätzliche Eingabe für den Journaleintrag." + adding))))
                                        End If

                                        If Not String.IsNullOrEmpty(XPoint.@plusprop) AndAlso XPoint.@plusprop <> "-" Then
                                            If String.IsNullOrEmpty(_ChecklistPoolToEdit.PlusPropLabel) OrElse _ChecklistPoolToEdit.PlusPropValues Is Nothing OrElse
                                                 Not _ChecklistPoolToEdit.PlusPropValues.ContainsKey(XPoint.@plusprop) Then
                                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry-ROT"}),
                                                                          New Run(New Text("Dieser Punkt hat eine Änderung der Zusatzproperty auf '" + XPoint.@plusprop + "' vorgesehen, diese kann aber nicht gefunden werden."))))
                                            Else
                                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                                          New Run(New Text("Dieser Punkt ändert " + _ChecklistPoolToEdit.PlusPropLabel + " auf """ + _ChecklistPoolToEdit.PlusPropValues.Item(XPoint.@plusprop) + """."))))
                                            End If
                                        End If

                                        If Not String.IsNullOrEmpty(XPoint.@prop) Then
                                            Dim PropLabels As New List(Of String)
                                            For Each mddef As String In XPoint.@prop.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                                                Dim mddefsplits As String() = mddef.Split({"##"}, StringSplitOptions.RemoveEmptyEntries)
                                                If mddefsplits.Count = 2 Then
                                                    PropLabels.Add(md.xml.MDCFactory.GetMDLabel(mddefsplits(0), mddefsplits(1)))
                                                End If
                                            Next
                                            If PropLabels.Count > 1 Then
                                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                                      New Run(New Text("Dieser Punkt enthält Eingabeaufforderungen zu den Merkmalen """ + String.Join(", ", PropLabels) + """."))))
                                            Else
                                                doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                                      New Run(New Text("Dieser Punkt enthält eine Eingabeaufforderung zum Merkmal """ + PropLabels.First + """."))))
                                            End If
                                        End If

                                        If XPoint.@final = "True" Then
                                            doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = "Checklistreport-Entry"}),
                                                                  New Run(New Text("Dieser Punkt beendet die Checkliste."))))
                                            If XPoint.<continue>.FirstOrDefault IsNot Nothing AndAlso XPoint.<continue>.First.HasElements Then
                                                Dim ChecklistList As New List(Of String)
                                                Dim ParaStyle As String = "Checklistreport-Entry"
                                                Dim XContinue As XElement = XPoint.<continue>.First
                                                For Each xc As XElement In XContinue.Elements
                                                    If ChecklistLabels.ContainsKey(xc.@id) Then
                                                        ChecklistList.Add(ChecklistLabels.Item(xc.@id))
                                                    Else
                                                        ChecklistList.Add("[ungültiger Verweis: " + xc.@id + "]")
                                                        ParaStyle = "Checklistreport-Entry-ROT"
                                                    End If
                                                Next
                                                If ChecklistList.Count > 1 Then
                                                    Dim OutputText As String = ""
                                                    If XContinue.@c = "True" Then
                                                        OutputText = "Danach werden die Checklisten """ + String.Join(", ", ChecklistList) + """ dem Nutzer angeboten."
                                                        If XContinue.@mc = "True" Then
                                                            OutputText += " Der Nutzer kann mehrere auswählen."
                                                        Else
                                                            OutputText += " Der Nutzer kann eine auswählen."
                                                        End If
                                                    Else
                                                        OutputText = "Danach werden die Checklisten """ + String.Join(", ", ChecklistList) + """ geladen."
                                                    End If
                                                    doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = ParaStyle}),
                                                                  New Run(New Text(OutputText))))
                                                Else
                                                    doc.Body.Append(New Paragraph(New ParagraphProperties(New ParagraphStyleId With {.Val = ParaStyle}),
                                                                  New Run(New Text("Danach wird die Checkliste """ + ChecklistList.First + """ geladen."))))
                                                End If
                                            End If
                                        End If
                                    Next
                                Next

                            End Using

                            Try
                                Using fileStream As IO.FileStream = New IO.FileStream(LastDocxReportFilename, IO.FileMode.Create)
                                    memorystream.WriteTo(fileStream)
                                End Using
                                DialogFactory.Msg(DialogFactory.GetParentWindow(Me), Me.Title, "Schreiben von " + IO.Path.GetFileName(LastDocxReportFilename) + " beendet.")
                            Catch ex As Exception
                                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, ex.ToString)
                            End Try
                        End Using
                    End If

                End If
            End If
        End If
    End Sub

    Private Function HandleReportCanExecute(ByVal sender As Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs) As Boolean
        e.CanExecute = LBChecklists.Items.Count > 0
        Return e.CanExecute
    End Function

    Private Sub LoadChecklists(Optional Preselected As String = "")
        LBChecklists.ItemsSource = (From cl As KeyValuePair(Of String, XElement) In _ChecklistPoolToEdit.Pool Order By cl.Value.@lb).ToDictionary(Function(cl) cl.Key, Function(cl) cl.Value)
        If Not String.IsNullOrEmpty(Preselected) Then LBChecklists.SelectedValue = Preselected
        CPEUC.MyChecklistPool = _ChecklistPoolToEdit
    End Sub

    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs)

        Me.DialogResult = False
    End Sub

    Private Sub LBChecklists_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles LBChecklists.SelectionChanged
        If DPChecklistData.DataContext IsNot Nothing Then
            Dim XChecklist As XElement = DPChecklistData.DataContext
            RemoveHandler XChecklist.Changed, AddressOf _ChecklistPoolToEdit.AutoSave
        End If
        If LBChecklists.SelectedItems.Count = 0 Then
            DPChecklistData.DataContext = Nothing
        Else
            Dim SelectedItem As KeyValuePair(Of String, XElement) = LBChecklists.SelectedItems(0)
            Dim XChecklist As XElement = SelectedItem.Value
            AddHandler XChecklist.Changed, AddressOf _ChecklistPoolToEdit.AutoSave
            DPChecklistData.DataContext = XChecklist
        End If
    End Sub

    Private Sub BtnNewChecklist_Click(sender As Object, e As RoutedEventArgs)
        Dim NewChecklistName As String = DialogFactory.InputText(DialogFactory.GetParentWindow(Me), "Neue Checkliste", "Bitte Namen der Liste eingeben:", "", "")
        If Not String.IsNullOrEmpty(NewChecklistName) Then
            Dim i As Integer = 0
            i = (From cl_Label As String In _ChecklistPoolToEdit.GetChecklistLabels Where cl_Label.ToUpper = NewChecklistName.ToUpper).Count
            If i > 0 Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neue Checkliste", "Dieser Name existiert bereits. Bitte erst umbenennen oder einen anderen Namen wählen!")
            Else
                Dim NewChecklistId As String = _ChecklistPoolToEdit.AddChecklist(NewChecklistName)
                If Not String.IsNullOrEmpty(NewChecklistId) Then LoadChecklists(NewChecklistId)
            End If
        End If
    End Sub

    Private Sub BtnDeleteChecklist_Click(sender As Object, e As RoutedEventArgs)
        If LBChecklists.SelectedItems.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Löschen Checkliste", "Bitte wählen Sie erst eine Checkliste aus!")
        Else
            Dim SelectedItem As KeyValuePair(Of String, XElement) = LBChecklists.SelectedItems(0)
            If DialogFactory.YesNoCancel(DialogFactory.GetParentWindow(Me), "Löschen Checkliste", "Soll Checkliste '" + SelectedItem.Value.@lb + "' gelöscht werden?") AndAlso _ChecklistPoolToEdit.DeleteChecklist(SelectedItem.Key) Then LoadChecklists()
        End If
    End Sub

    Private Sub ChecklistLabelChanged(sender As Object, e As TextChangedEventArgs)
        If LBChecklists.SelectedItems.Count > 0 Then
            _ChecklistPoolToEdit.AutoSave(Me, Nothing)
            LoadChecklists(LBChecklists.SelectedValue)
        Else
            LoadChecklists()
        End If
    End Sub
End Class
