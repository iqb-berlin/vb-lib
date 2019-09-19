Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.CustomProperties

Imports wp = DocumentFormat.OpenXml.Drawing.Wordprocessing
Imports a = DocumentFormat.OpenXml.Drawing
Imports pic = DocumentFormat.OpenXml.Drawing.Pictures
Imports M = DocumentFormat.OpenXml.Math
Imports System.Drawing
Imports System.IO
Imports DocumentFormat.OpenXml.VariantTypes

Public Class docxFactory

    Public Enum PictureInsertMode
        AsItIs
        Shrink
        Cut
        CutWithMarking
    End Enum

    Public Shared Sub SetCoreProperties(WDoc As WordprocessingDocument, AuthorName As String, Optional Title As String = Nothing)
        Dim props As IO.Packaging.PackageProperties = WDoc.PackageProperties

        props.Creator = AuthorName
        props.Created = DateTime.Now
        props.LastModifiedBy = AuthorName
        props.ContentType = "application/msword"
        If Not String.IsNullOrEmpty(Title) Then props.Title = Title
    End Sub

    Public Shared Function SetCustomProperty(WDoc As WordprocessingDocument, ByVal propertyName As String, ByVal propertyValue As String) As String
        Dim returnValue As String = Nothing

        Dim newProp As New CustomDocumentProperty
        Dim propSet As Boolean = False

        newProp.VTLPWSTR = New VTLPWSTR(propertyValue)
        propSet = True


        If Not propSet Then
            ' If the code could not convert the 
            ' property to a valid value, throw an exception:
            Throw New InvalidDataException("propertyValue")
        End If

        ' Now that you have handled the parameters,
        ' work on the document.
        newProp.FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}"
        newProp.Name = propertyName

        Dim customProps = WDoc.CustomFilePropertiesPart
        If customProps Is Nothing Then
            ' No custom properties? Add the part, and the
            ' collection of properties now.
            customProps = WDoc.AddCustomFilePropertiesPart
            customProps.Properties = New Properties
        End If

        Dim props = customProps.Properties
        If props IsNot Nothing Then
            Dim prop = props.
              Where(Function(p) CType(p, CustomDocumentProperty).
                      Name.Value = propertyName).FirstOrDefault()
            ' Does the property exist? If so, get the return value, 
            ' and then delete the property.
            If prop IsNot Nothing Then
                returnValue = prop.InnerText
                prop.Remove()
            End If

            ' Append the new property, and 
            ' fix up all the property ID values. 
            ' The PropertyId value must start at 2.
            props.AppendChild(newProp)
            Dim pid As Integer = 2
            For Each item As CustomDocumentProperty In props
                item.PropertyId = pid
                pid += 1
            Next
            props.Save()
        End If

        Return returnValue
    End Function


    'ungetestet
    Public Shared Sub AddDocComment(ByRef WDoc As WordprocessingDocument, AuthorName As String, Comment As String, ByRef TargetParagraph As Paragraph)
        Dim comments As Comments = Nothing
        Dim id As String = "0"

        ' Verify that the document contains a 
        ' WordProcessingCommentsPart part; if not, add a new one.
        If WDoc.MainDocumentPart.GetPartsOfType(Of WordprocessingCommentsPart).Count > 0 Then
            comments = WDoc.MainDocumentPart.WordprocessingCommentsPart.Comments
            If comments.HasChildren Then
                ' Obtain an unused ID.
                id = comments.Descendants(Of Comment)().[Select](Function(e) e.Id.Value).Max()
            End If
        Else
            ' No WordprocessingCommentsPart part exists, so add one to the package.
            Dim commentPart As WordprocessingCommentsPart = WDoc.MainDocumentPart.AddNewPart(Of WordprocessingCommentsPart)()
            commentPart.Comments = New Comments()
            comments = commentPart.Comments
        End If

        ' Compose a new Comment and add it to the Comments part.
        Dim p As New Paragraph(New Run(New Text(Comment)))
        Dim cmt As New Comment() With {.Id = id, .Author = AuthorName, .Initials = "IQB", .Date = DateTime.Now}
        cmt.AppendChild(p)
        comments.AppendChild(cmt)
        comments.Save()

        ' Specify the text range for the Comment. 
        ' Insert the new CommentRangeStart before the first run of paragraph.
        TargetParagraph.InsertBefore(New CommentRangeStart() With {.Id = id}, TargetParagraph.GetFirstChild(Of Run)())

        ' Insert the new CommentRangeEnd after last run of paragraph.
        Dim cmtEnd = TargetParagraph.InsertAfter(New CommentRangeEnd() With {.Id = id}, TargetParagraph.Elements(Of Run)().Last())

        ' Compose a run with CommentReference and insert it.
        TargetParagraph.InsertAfter(New Run(New CommentReference() With {.Id = id}), cmtEnd)
    End Sub


    '############################
    Public Shared Function GetCustomProperties(Template As Byte()) As Dictionary(Of String, String)
        Dim myreturn As New Dictionary(Of String, String)
        Try
            Using memorystream As IO.MemoryStream = New IO.MemoryStream
                memorystream.Write(Template, 0, CInt(Template.Length))
                Using NewDoc As WordprocessingDocument = WordprocessingDocument.Open(memorystream, True)
                    For Each p As CustomProperties.CustomDocumentProperty In NewDoc.CustomFilePropertiesPart.Properties
                        If Not myreturn.ContainsKey(p.Name.ToString) Then myreturn.Add(p.Name.ToString, p.InnerText)
                    Next
                End Using
            End Using
        Catch ex As Exception

        End Try

        Return myreturn
    End Function

    '############################
    Public Shared Function AddImagePart(ByRef parent As WordprocessingDocument, ByRef ImageBytes As Byte(),
                                        ImgPartType As DocumentFormat.OpenXml.Packaging.ImagePartType,
                                        maximumWidthCm As Double) As Run
        Dim imageWidthEMU As Long = 0
        Dim imageHeightEMU As Long = 0
        Dim mainPart As MainDocumentPart = parent.MainDocumentPart
        Dim refID As String
        Dim imagePart As ImagePart = mainPart.AddImagePart(ImgPartType)
        refID = mainPart.GetIdOfPart(imagePart)
        GenerateImagePart(imagePart, ImageBytes, imageWidthEMU, imageHeightEMU, maximumWidthCm)

        Return AddImageToBody(ImageBytes, imageWidthEMU, imageHeightEMU, refID)
    End Function

    Private Shared Function AddImageToBody(ByRef ImageBytes As Byte(), ByVal imageWidthEMU As Long, ByVal imageHeightEMU As Long, ByVal refid As String) As Run
        Dim r As New Run(
            New Drawing(
                New wp.Inline(
                    New wp.Extent() With {.Cx = imageWidthEMU, .Cy = imageHeightEMU},
                    New wp.EffectExtent() With {.LeftEdge = 19050L, .TopEdge = 0L, .RightEdge = 9525L, .BottomEdge = 0L},
                    New wp.DocProperties() With {.Id = 1UI, .Name = "Inline Text Wrapping Picture", .Description = ""},
                    New wp.NonVisualGraphicFrameDrawingProperties(
                        New a.GraphicFrameLocks() With {.NoChangeAspect = True}
                        ),
                    New a.Graphic(
                        New a.GraphicData(
                            New pic.Picture(
                                New pic.NonVisualPictureProperties(
                                    New pic.NonVisualDrawingProperties() With {.Id = 0UI, .Name = ""},
                                    New pic.NonVisualPictureDrawingProperties()
                                    ),
                                New pic.BlipFill(
                                    New a.Blip() With {.Embed = refid},
                                    New a.Stretch(
                                        New a.FillRectangle()
                                        )
                                    ),
                                New pic.ShapeProperties(
                                    New a.Transform2D(
                                        New a.Offset() With {.X = 0L, .Y = 0L},
                                        New a.Extents() With {.Cx = imageWidthEMU, .Cy = imageHeightEMU}
                                        ),
                                    New a.PresetGeometry(
                                        New a.AdjustValueList()
                                        ) With {.Preset = a.ShapeTypeValues.Rectangle}
                                    )
                                )
                            ) With {.Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"}
                        )
                    ) With {.DistanceFromTop = 0UI, .DistanceFromBottom = 0UI, .DistanceFromLeft = 0UI, .DistanceFromRight = 0UI}
                )
            )
        Return r
    End Function

    Private Shared Sub GenerateImagePart(ByVal part As OpenXmlPart, ByRef imageBytes As Byte(), ByRef imageWidthEMU As Long, ByRef imageHeightEMU As Long, Optional maximumWidthCm As Double = 0.0)
        Dim imageFile As Bitmap

        imageFile = New Bitmap(New MemoryStream(imageBytes), False)
        ' Get the dimensions of the image in English Metric Units (EMU)
        ' for use when adding the markup for the image to the document.
        imageWidthEMU = CLng((imageFile.Width / imageFile.HorizontalResolution) * 914400L)
        imageHeightEMU = CLng((imageFile.Height / imageFile.VerticalResolution) * 914400L)

        If maximumWidthCm > 0.0 Then
            Dim maximumWidthEMU As Long = maximumWidthCm * 360000L
            If imageWidthEMU > maximumWidthEMU Then
                Dim TransferRate As Double = maximumWidthEMU / imageWidthEMU
                imageWidthEMU = imageWidthEMU * TransferRate
                imageHeightEMU = imageHeightEMU * TransferRate
            End If
        End If

        ' Write the contents of the image to the ImagePart.
        Using writer As New BinaryWriter(part.GetStream())
            writer.Write(imageBytes)
            writer.Flush()
        End Using
    End Sub

    Public Shared Function GetImagePartTypeFromFilename(FName As String) As DocumentFormat.OpenXml.Packaging.ImagePartType
        Select Case IO.Path.GetExtension(FName).ToUpper
            Case ".BMP" : Return ImagePartType.Bmp
            Case ".EMF" : Return ImagePartType.Emf
            Case ".GIF" : Return ImagePartType.Gif
            Case ".ICO" : Return ImagePartType.Icon
            Case ".JPG", ".JPEG" : Return ImagePartType.Jpeg
            Case ".PCX" : Return ImagePartType.Pcx
            Case ".PNG" : Return ImagePartType.Png
            Case ".TIF" : Return ImagePartType.Tiff
            Case ".WMF" : Return ImagePartType.Wmf
            Case Else : Return Nothing
        End Select
    End Function

    'Private Structure ModifiedImagePartRecord
    '    Dim oldIP As ImagePart
    '    Dim oldID As String
    '    Dim newID As String
    '    Dim modified As Boolean
    'End Structure

    'Private Shared Sub ChangeImagePardtId(ByVal wd As WordprocessingDocument, ByVal ipr As ModifiedImagePartRecord)
    '    Dim em As New StringValue(ipr.newID)
    '    Dim blips As IEnumerable(Of DocumentFormat.OpenXml.Drawing.Blip) = _
    '        From blip In wd.MainDocumentPart.Document.Body.Descendants(Of DocumentFormat.OpenXml.Drawing.Blip)() _
    '        Where (blip.Embed.Value = ipr.oldID)
    '    'Don't use blips.First.Embed = em. Because it would cause blips to be NOTHING
    '    'Why? Just don't understand. Maybe it's relevant to Type.IsGenericType.
    '    Dim tempblip As DocumentFormat.OpenXml.Drawing.Blip = New DocumentFormat.OpenXml.Drawing.Blip()
    '    tempblip = blips.First
    '    tempblip.Embed = em
    'End Sub


    Public Shared Function OpenDocumentWithWordApplication(DocMemoStream As IO.MemoryStream, Optional WorkingDir As String = Nothing) As Boolean
        If String.IsNullOrEmpty(WorkingDir) OrElse Not IO.Directory.Exists(WorkingDir) Then WorkingDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        Try

            Dim TempFileName As String = IO.Path.GetTempFileName() + ".dotx"
            Using fileStream As IO.FileStream = New IO.FileStream(TempFileName, IO.FileMode.Create)
                DocMemoStream.WriteTo(fileStream)
            End Using

            Dim proc As New Process
            With proc.StartInfo
                .FileName = TempFileName
                .WindowStyle = ProcessWindowStyle.Normal
                .WorkingDirectory = WorkingDir
            End With
            proc.Start()
            Return True
        Catch ex As Exception
            Throw ex
        End Try
    End Function



    '########################################################################################
    ''' <summary>
    ''' Trennt Zeilen mit Break, erkennt LaTeX-Ausdrücke
    ''' </summary>
    Public Shared Function AddMultiLineTextToDocument(ByVal TextToTransform As String, Optional PProps As ParagraphProperties = Nothing) As Paragraph
        Dim myreturn As Paragraph
        If PProps Is Nothing Then
            myreturn = New Paragraph()
        Else
            myreturn = New Paragraph(PProps)
        End If

        TextToTransform = validXml.GetValidXmlcodedString(TextToTransform)
        Dim LinesToTransform As String() = {TextToTransform}
        Dim TempSplits1 As String() = TextToTransform.Split({vbNewLine}, StringSplitOptions.None)
        If TempSplits1.Count > 1 Then
            LinesToTransform = TempSplits1
        Else
            Dim TempSplits2 As String() = TextToTransform.Split({vbLf}, StringSplitOptions.None)
            If TempSplits2.Count > 1 Then
                LinesToTransform = TempSplits2
            Else
                Dim CodeValueSplits3 As String() = TextToTransform.Split({vbCr}, StringSplitOptions.None)
                If CodeValueSplits3.Count > 1 Then LinesToTransform = CodeValueSplits3
            End If
        End If

        Dim lastrun As Run = Nothing
        For Each TextLine As String In LinesToTransform
            TextLine = TextLine.Trim(vbLf)
            If lastrun IsNot Nothing Then lastrun.AppendChild(New Break)

            If Not String.IsNullOrEmpty(TextLine) Then

                '#############################################################################
                '#############################################################################
                Dim posLatexStart As Integer


                Do While TextLine.Length > 0
                    posLatexStart = TextLine.IndexOf("$")

                    If posLatexStart >= 0 Then
                        lastrun = New Run(New Text With {.Text = TextLine.Substring(0, posLatexStart), .Space = SpaceProcessingModeValues.Preserve})
                        myreturn.Append(lastrun)
                        If TextLine.Length > posLatexStart + 1 AndAlso TextLine.Substring(posLatexStart, 2) = "$$" Then 'LaTex-Schutzbereich
                            Dim posLatexEnde As Integer = TextLine.Substring(posLatexStart + 1).IndexOf("$$")
                            If posLatexEnde > 0 Then
                                lastrun = New Run(New RunProperties(New Highlight With {.Val = HighlightColorValues.Yellow}),
                                                        New Text With {.Text = TextLine.Substring(posLatexStart + 1, posLatexEnde + 1), .Space = SpaceProcessingModeValues.Preserve})
                                TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 3)
                            Else
                                lastrun = New Run(New Text("$$"))
                                TextLine = TextLine.Substring(posLatexStart + 2)
                            End If
                            myreturn.Append(lastrun)

                        Else 'LaTex-Parsen
                            Dim posLatexEnde As Integer = TextLine.Substring(posLatexStart + 1).IndexOf("$")
                            If posLatexEnde > 0 Then
                                Dim InnerLatexStr As String = TextLine.Substring(posLatexStart + 1, posLatexEnde)
                                If Not String.IsNullOrEmpty(InnerLatexStr) AndAlso InnerLatexStr.Length > 2 AndAlso InnerLatexStr.Substring(0, 1) = "\" Then InnerLatexStr = InnerLatexStr.Substring(1)

                                If LatexSymbols.ContainsKey(InnerLatexStr) Then
                                    lastrun = New Run(New SymbolChar With {.Char = HexBinaryValue.FromString(LatexSymbols.Item(InnerLatexStr).ToString("X")), .Font = "Symbol"})
                                    TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)
                                Else
                                    Dim cmd As String = GetFirstCommand(InnerLatexStr)
                                    Select Case cmd
                                        Case "euro"
                                            lastrun = New Run(New Text With {.Text = "€", .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)
                                        Case "grqq"
                                            lastrun = New Run(New Text With {.Text = Chr(147), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)
                                        Case "glqq"
                                            lastrun = New Run(New Text With {.Text = Chr(132), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)
                                        Case "frac"
                                            Dim numTxt As String = GetInnerSchweif(InnerLatexStr)
                                            If numTxt.IndexOf("}{") >= 0 Then
                                                Dim denomTxt As String = numTxt.Substring(numTxt.IndexOf("}{") + 2)
                                                numTxt = numTxt.Substring(0, numTxt.IndexOf("}{"))
                                                lastrun = New Run(New M.OfficeMath(New M.Fraction(New M.Numerator(New Run(New Text(numTxt))), New M.Denominator(New Run(New Text(denomTxt))))))
                                            Else
                                                lastrun = New Run(New RunProperties(New Highlight With {.Val = HighlightColorValues.Green}), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                            End If
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)

                                        Case "mathbf"
                                            lastrun = New Run(New RunProperties(New Bold), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)

                                        Case "mathit"
                                            lastrun = New Run(New RunProperties(New Italic), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)

                                        Case "^"
                                            lastrun = New Run(New RunProperties(New VerticalTextAlignment With {.Val = VerticalPositionValues.Superscript}), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)

                                        Case "_"
                                            lastrun = New Run(New RunProperties(New VerticalTextAlignment With {.Val = VerticalPositionValues.Subscript}), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                            TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)

                                        Case Else
                                            If TextLine.IndexOf("\") >= 0 Then
                                                lastrun = New Run(New RunProperties(New Highlight With {.Val = HighlightColorValues.Green}), New Text With {.Text = GetInnerSchweif(InnerLatexStr), .Space = SpaceProcessingModeValues.Preserve})
                                                TextLine = TextLine.Substring(posLatexStart + posLatexEnde + 2)
                                            Else
                                                lastrun = New Run(New Text("$"))
                                                TextLine = TextLine.Substring(posLatexStart + 1)
                                            End If
                                    End Select
                                End If
                            Else
                                lastrun = New Run(New Text("$"))
                                TextLine = TextLine.Substring(posLatexStart + 1)
                            End If
                            myreturn.Append(lastrun)
                        End If
                    Else
                        lastrun = New Run(New Text With {.Text = TextLine, .Space = SpaceProcessingModeValues.Preserve})
                        myreturn.Append(lastrun)
                        TextLine = ""
                    End If
                Loop

            End If
        Next

        Return myreturn
    End Function

    Friend Shared Function GetFirstCommand(ByVal s As String) As String
        Dim myreturn As String = ""
        If Len(s) > 0 Then
            If s(0) = "^" Then Return "^"
            If s(0) = "_" Then Return "_"

            For Each c As Char In s
                If c < "a" OrElse c > "z" Then
                    myreturn += c
                Else
                    Exit For
                End If
            Next
            s = s.Substring(myreturn.Length)
            myreturn = ""
            For Each c As Char In s
                If c >= "a" AndAlso c <= "z" Then
                    myreturn += c
                Else
                    Exit For
                End If
            Next
        End If

        Return myreturn
    End Function

    Friend Shared Function GetInnerSchweif(ByVal s As String) As String
        Dim myreturn As String = ""
        If Len(s) > 0 Then
            Dim posStart As Integer = s.IndexOf("{")
            Dim posEnde As Integer = s.LastIndexOf("}")
            If posStart >= 0 AndAlso posEnde > 0 Then myreturn = s.Substring(posStart + 1, posEnde - posStart - 1)
        End If

        Return myreturn
    End Function

    Friend Shared Property LatexSymbols As New Dictionary(Of String, Integer) From {{"alpha", 97},
                                                                                        {"Alpha", 65},
                                                                                        {"angle", 208},
                                                                                        {"approx", 187},
                                                                                        {"beta", 98},
                                                                                        {"Beta", 66},
                                                                                        {"cdot", 215},
                                                                                        {"chi", 99},
                                                                                        {"Chi", 67},
                                                                                        {"delta", 100},
                                                                                        {"Delta", 68},
                                                                                        {"epsilon", 101},
                                                                                        {"Epsilon", 69},
                                                                                        {"eta", 104},
                                                                                        {"Eta", 72},
                                                                                        {"gamma", 103},
                                                                                        {"Gamma", 71},
                                                                                        {"geq", 179},
                                                                                        {"iota", 105},
                                                                                        {"Iota", 73},
                                                                                        {"kappa", 107},
                                                                                        {"Kappa", 75},
                                                                                        {"lambda", 108},
                                                                                        {"Lambda", 76},
                                                                                        {"leq", 163},
                                                                                        {"mu", 109},
                                                                                        {"Mu", 77},
                                                                                        {"neq", 185},
                                                                                        {"nu", 110},
                                                                                        {"Nu", 78},
                                                                                        {"omega", 119},
                                                                                        {"Omega", 87},
                                                                                        {"omicron", 111},
                                                                                        {"Omicron", 79},
                                                                                        {"phi", 102},
                                                                                        {"Phi", 70},
                                                                                        {"pi", 112},
                                                                                        {"Pi", 80},
                                                                                        {"pm", 177},
                                                                                        {"psi", 121},
                                                                                        {"Psi", 89},
                                                                                        {"rho", 114},
                                                                                        {"Rho", 82},
                                                                                        {"rightarrow", 174},
                                                                                        {"sigma", 115},
                                                                                        {"Sigma", 83},
                                                                                        {"tau", 116},
                                                                                        {"Tau", 83},
                                                                                        {"theta", 113},
                                                                                        {"Theta", 81},
                                                                                        {"upsilon", 117},
                                                                                        {"Upsilon", 85},
                                                                                        {"varphi", 106},
                                                                                        {"vartheta", 74},
                                                                                        {"xi", 120},
                                                                                        {"zeta", 122}}


    Private Shared Property LatexExpr As New List(Of String) From {"mathbf", "mathit", "_", "^"}
End Class
