Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet
Imports DocumentFormat.OpenXml

Public Class ExcelFile
    Implements IDisposable

    Friend mySpreadsheetDoc As SpreadsheetDocument
    Public CellStyles As Dictionary(Of CellFormatting, UInt32Value)
    Private IsReadOnly As Boolean
    Private mySheets As Dictionary(Of String, ExcelWorkSheet)

    Public Sub New(SSD As SpreadsheetDocument)
        mySpreadsheetDoc = SSD
        Me.CellStyles = Nothing
        Me.IsReadOnly = True
        Me.mySheets = Nothing
    End Sub

    Public Shared Function OpenReadOnly(Filename As String) As ExcelFile
        Dim myReturn As New ExcelFile(SpreadsheetDocument.Open(New IO.MemoryStream(IO.File.ReadAllBytes(Filename)), False))
        myReturn.LoadSheets()

        Return myReturn
    End Function

    Public Shared Function CreateNew(Filename As String, Optional TemplateFilename As String = Nothing) As Byte()
        Dim myReturn As Byte() = Nothing
        If String.IsNullOrEmpty(TemplateFilename) Then
            Dim TmpZielXLS As SpreadsheetDocument = SpreadsheetDocument.Create(Filename, SpreadsheetDocumentType.Workbook)
            Dim myWorkbookPart As WorkbookPart = TmpZielXLS.AddWorkbookPart()
            myWorkbookPart.Workbook = New Workbook()
            myWorkbookPart.Workbook.AppendChild(Of Sheets)(New Sheets())
            TmpZielXLS.Close()

            myReturn = IO.File.ReadAllBytes(Filename)
        Else
            myReturn = IO.File.ReadAllBytes(TemplateFilename)
            Using MemStream As New IO.MemoryStream()
                MemStream.Write(myReturn, 0, myReturn.Length)
                Using ZielXLS As SpreadsheetDocument = SpreadsheetDocument.Open(MemStream, True)
                    Dim sheets As Sheets = ZielXLS.WorkbookPart.Workbook.GetFirstChild(Of Sheets)()
                    If sheets.Elements(Of Sheet).Count() = 0 Then myReturn = Nothing
                End Using
                If myReturn IsNot Nothing Then
                    Using fs As New IO.FileStream(Filename, IO.FileMode.Create)
                        MemStream.WriteTo(fs)
                    End Using
                End If
            End Using
            If myReturn IsNot Nothing Then myReturn = IO.File.ReadAllBytes(Filename)
        End If

        Return myReturn
    End Function

    Public Shared Function OpenNewRW(FileMemoryStream As IO.MemoryStream) As ExcelFile
        Dim myReturn As New ExcelFile(SpreadsheetDocument.Open(FileMemoryStream, True))
        myReturn.CellStyles = AddIQBStandardStyles(myReturn.mySpreadsheetDoc.WorkbookPart)
        myReturn.IsReadOnly = False
        myReturn.LoadSheets()

        Return myReturn
    End Function

    Public Sub LoadSheets()
        Me.mySheets = New Dictionary(Of String, ExcelWorkSheet)
        For Each Sh As Sheet In Me.mySpreadsheetDoc.WorkbookPart.Workbook.Descendants(Of Sheet)()
            Me.mySheets.Add(Sh.Name, New ExcelWorkSheet(Me, Sh))
        Next
    End Sub

    '######################################################################
    Private Shared Function AddIQBStandardStyles(WB As WorkbookPart) As Dictionary(Of CellFormatting, UInt32Value)
        If WB.WorkbookStylesPart Is Nothing Then WB.AddNewPart(Of WorkbookStylesPart)()
        If WB.WorkbookStylesPart.Stylesheet Is Nothing Then
            WB.WorkbookStylesPart.Stylesheet = New Stylesheet()
            WB.WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("x", "http://schemas.openxmlformats.org/spreadsheetml/2006/main")
        End If
        Dim mySSh As Stylesheet = WB.WorkbookStylesPart.Stylesheet

        'Nummernformate
        Dim NumberingFormatIDs As New List(Of Integer)
        Dim myNF As NumberingFormats = mySSh.GetFirstChild(Of NumberingFormats)()
        If myNF Is Nothing Then
            mySSh.InsertAt(New NumberingFormats(), 0)
            myNF = mySSh.GetFirstChild(Of NumberingFormats)()
        End If
        NumberingFormatIDs.Add(164UI)
        myNF.Append(New NumberingFormat() With {.FormatCode = "#.##0", .NumberFormatId = 164UI})
        NumberingFormatIDs.Add(165UI)
        myNF.Append(New NumberingFormat() With {.FormatCode = "#,##0.00", .NumberFormatId = 165UI})

        'Fonts
        Dim FontIDs As New List(Of Integer)
        Dim myF As Fonts = mySSh.GetFirstChild(Of Fonts)()
        If myF Is Nothing Then
            mySSh.InsertAfter(New Fonts(), myNF)
            myF = mySSh.GetFirstChild(Of Fonts)()
        End If
        FontIDs.Add(myF.ChildElements.Count)
        myF.Append(New Font(
                    New FontSize() With {.Val = 11},
                    New Color() With {.Rgb = New HexBinaryValue() With {.Value = "0000"}},
                    New FontName() With {.Val = "Calibri"}))
        FontIDs.Add(myF.ChildElements.Count)
        myF.Append(New Font(
                    New Underline(),
                    New FontSize() With {.Val = 11},
                    New Color() With {.Rgb = New HexBinaryValue() With {.Value = "0000"}},
                    New FontName() With {.Val = "Calibri"}))
        FontIDs.Add(myF.ChildElements.Count)
        myF.Append(New Font(
                    New FontSize() With {.Val = 12},
                    New Color() With {.Rgb = New HexBinaryValue() With {.Value = "0000"}},
                    New FontName() With {.Val = "Calibri"}))

        'Fills
        Dim FillIDs As New List(Of Integer)
        Dim myFill As Fills = mySSh.GetFirstChild(Of Fills)()
        If myFill Is Nothing Then
            mySSh.InsertAfter(New Fills(), myF)
            myFill = mySSh.GetFirstChild(Of Fills)()
        End If
        FillIDs.Add(myFill.ChildElements.Count)
        myFill.Append(New Fill(New PatternFill() With {.PatternType = PatternValues.None}))
        FillIDs.Add(myFill.ChildElements.Count)
        myFill.Append(New Fill(New PatternFill(New ForegroundColor With {.Theme = 3UI, .Tint = DoubleValue.FromDouble(0.59999389629810485D)},
                                         New BackgroundColor With {.Indexed = 64UI}) With {.PatternType = PatternValues.Solid}))
        FillIDs.Add(myFill.ChildElements.Count)
        myFill.Append(New Fill(New PatternFill(New ForegroundColor With {.Theme = 3UI, .Tint = DoubleValue.FromDouble(0.59999389629810485D)},
                                         New BackgroundColor With {.Indexed = 64UI}) With {.PatternType = PatternValues.Solid}))
        FillIDs.Add(myFill.ChildElements.Count)
        myFill.Append(New Fill(New PatternFill(New ForegroundColor With {.Theme = 8UI, .Tint = DoubleValue.FromDouble(0.39997558519241921D)},
                                         New BackgroundColor With {.Indexed = 64UI}) With {.PatternType = PatternValues.Solid}))
        FillIDs.Add(myFill.ChildElements.Count)
        myFill.Append(New Fill(New PatternFill(New ForegroundColor With {.Theme = 8UI, .Tint = DoubleValue.FromDouble(0.59999389629810485D)},
                                         New BackgroundColor With {.Indexed = 64UI}) With {.PatternType = PatternValues.Solid}))

        'Border
        Dim myBorders As Borders = mySSh.GetFirstChild(Of Borders)()
        If myBorders Is Nothing Then
            mySSh.InsertAfter(New Borders(), myFill)
            myBorders = mySSh.GetFirstChild(Of Borders)()
        End If
        Dim BorderID As Integer = myBorders.ChildElements.Count
        myBorders.Append(New Border)

        'Styles
        Dim myReturn As New Dictionary(Of CellFormatting, UInt32Value)
        Dim myCellFormats As CellFormats = mySSh.GetFirstChild(Of CellFormats)()
        If myCellFormats Is Nothing Then
            mySSh.Append(New CellFormats())
            myCellFormats = mySSh.GetFirstChild(Of CellFormats)()
        End If
        myReturn.Add(CellFormatting.Null, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(0), .FillId = FillIDs(0), .BorderId = BorderID})
        myReturn.Add(CellFormatting.ColHeader, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(2), .FillId = FillIDs(2), .BorderId = BorderID, .ApplyFont = True, .ApplyFill = True})
        myReturn.Add(CellFormatting.RowHeader1, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(2), .FillId = FillIDs(3), .BorderId = BorderID, .ApplyFont = True, .ApplyFill = True})
        myReturn.Add(CellFormatting.RowHeader2, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(2), .FillId = FillIDs(4), .BorderId = BorderID, .ApplyFont = True, .ApplyFill = True})
        myReturn.Add(CellFormatting.NumInt, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(0), .FillId = FillIDs(0), .BorderId = BorderID, .NumberFormatId = 3, .ApplyNumberFormat = True})
        myReturn.Add(CellFormatting.Num2Dec, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(0), .FillId = FillIDs(0), .BorderId = BorderID, .NumberFormatId = 4, .ApplyNumberFormat = True})
        myReturn.Add(CellFormatting.Hyperlink, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(1), .FillId = FillIDs(0), .BorderId = BorderID, .ApplyFont = True})
        myReturn.Add(CellFormatting.MultiLineText, myCellFormats.ChildElements.Count)
        myCellFormats.Append(New CellFormat() With {.FontId = FontIDs(0), .FillId = FillIDs(0), .BorderId = BorderID, .ApplyAlignment = True, .Alignment = New Alignment With {.WrapText = True}})

        mySSh.Save()

        Return myReturn
    End Function

    '#############
    Public Sub DefineName(NewName As String, NewValue As String)
        Dim WB As Workbook = Me.mySpreadsheetDoc.WorkbookPart.Workbook
        If WB.DefinedNames Is Nothing Then WB.DefinedNames = New DefinedNames
        For Each dn As DefinedName In WB.DefinedNames
            If dn.Name.Value = NewName Then
                dn.Text = NewValue
                WB.Save()
                Return
            End If
        Next
        WB.DefinedNames.Append(New DefinedName With {.Name = NewName, .Text = NewValue})
        WB.Save()
    End Sub

    '#############
    Public Function AddWorksheet(SheetName As String) As ExcelWorkSheet
        Dim myreturn As ExcelWorkSheet = New ExcelWorkSheet(Me, SheetName)
        LoadSheets()
        Return myreturn
    End Function

    '#############
    Public Function GetCellValue(ByVal sheetName As String, ByVal addressName As String) As String
        Dim mySheet As ExcelWorkSheet = mySheets(sheetName)
        Return mySheet.GetCellValue(addressName)
    End Function

    Public Function GetCellValue(CellRef As NamedRef) As String
        Dim mySheet As ExcelWorkSheet = mySheets(CellRef.WorksheetName)
        Return mySheet.GetCellValue(CellRef.Address)
    End Function

    '#############
    Public Function GetDefinedNameValue(NameName As String) As NamedRef
        Dim myreturn As NamedRef = Nothing
        Dim WB As Workbook = mySpreadsheetDoc.WorkbookPart.Workbook
        If WB.DefinedNames IsNot Nothing Then
            For Each dn As DefinedName In WB.DefinedNames
                If dn.Name.Value = NameName Then
                    myreturn = New NamedRef(dn.Text)
                    Exit For
                End If
            Next
        End If
        Return myreturn
    End Function

    '#############
    Public Sub SetCellValue(CellRef As NamedRef, CellContent As String, Optional CellFormat As CellFormatting = CellFormatting.Null)
        Dim mySheet As ExcelWorkSheet = mySheets(CellRef.WorksheetName)
        mySheet.SetCellValue(CellRef.Column, CellRef.Row, CellContent, CellTypes.str, CellFormat)
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                mySpreadsheetDoc.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
    '#######################################################
    ' PRIVATE
    '#######################################################
End Class

'#######################################################
'#######################################################
Public Class ExcelWorkSheet
    Private myWorksheet As WorksheetPart
    Private ParentExcelFile As ExcelFile
    Private myName As String

    Public Sub New(Parent As ExcelFile, SheetObject As Sheet)
        ParentExcelFile = Parent
        myWorksheet = CType(ParentExcelFile.mySpreadsheetDoc.WorkbookPart.GetPartById(SheetObject.Id), WorksheetPart)
        myName = SheetObject.Name
    End Sub

    Public Sub New(Parent As ExcelFile, SheetName As String)
        ParentExcelFile = Parent
        myWorksheet = ParentExcelFile.mySpreadsheetDoc.WorkbookPart.AddNewPart(Of WorksheetPart)()

        myWorksheet.Worksheet = New Worksheet(New SheetData)
        myWorksheet.Worksheet.Save()
        Dim sheets As Sheets = ParentExcelFile.mySpreadsheetDoc.WorkbookPart.Workbook.GetFirstChild(Of Sheets)()
        Dim relationshipId As String = ParentExcelFile.mySpreadsheetDoc.WorkbookPart.GetIdOfPart(myWorksheet)

        ' Get a unique ID for the new sheet.
        Dim sheetId As UInteger = 1
        If (sheets.Elements(Of Sheet).Count() > 0) Then sheetId = sheets.Elements(Of Sheet).Select(Function(s) s.SheetId.Value).Max() + 1

        If String.IsNullOrEmpty(SheetName) Then SheetName = "Tabelle" + sheetId.ToString()
        myName = SheetName

        ' Add the new worksheet and associate it with the workbook.
        Dim sheet As Sheet = New Sheet
        sheet.Id = relationshipId
        sheet.SheetId = sheetId
        sheet.Name = SheetName
        sheets.Append(sheet)
        ParentExcelFile.mySpreadsheetDoc.WorkbookPart.Workbook.Save()
    End Sub

    Public Sub Save()
        myWorksheet.Worksheet.Save()
    End Sub


    '#############
    Public Function GetCellValue(ByVal addressName As String) As String
        Dim myreturn As String = ""

        Dim theCell As Cell = myWorksheet.Worksheet.Descendants(Of Cell).
                Where(Function(c) c.CellReference = addressName).FirstOrDefault
        If theCell IsNot Nothing Then
            If theCell.CellValue Is Nothing Then
                myreturn = theCell.InnerText
            Else
                myreturn = theCell.CellValue.InnerText
            End If

            If theCell.DataType IsNot Nothing Then
                Select Case theCell.DataType.Value
                    Case CellValues.SharedString
                        Dim stringTable = Me.ParentExcelFile.mySpreadsheetDoc.GetPartsOfType(Of SharedStringTablePart).FirstOrDefault()
                        If stringTable IsNot Nothing Then myreturn = stringTable.SharedStringTable.ElementAt(Integer.Parse(myreturn)).InnerText

                    Case CellValues.Boolean
                        Select Case myreturn
                            Case "0"
                                myreturn = "FALSE"
                            Case Else
                                myreturn = "TRUE"
                        End Select
                End Select
            End If
        End If

        Return myreturn
    End Function


    '#########################################################################
    ''' <summary>
    ''' Wenn CllType hyperlink, dann enthält CllValue zuerst den Link, dann '###', dann den Content
    ''' </summary>
    Private Function GetCell(ByVal columnName As String, ByVal rowIndex As UInteger, CllValue As String, CllType As CellTypes, CllFormat As CellFormatting,
                                Optional OldCell As Cell = Nothing) As Cell
        Dim StyleDef As Dictionary(Of CellFormatting, UInt32Value) = ParentExcelFile.CellStyles
        Dim myCell As Cell
        If OldCell Is Nothing Then
            myCell = New Cell With {.CellReference = columnName + rowIndex.ToString().Trim}
        Else
            myCell = OldCell
            If myCell.CellValue IsNot Nothing Then myCell.CellValue.Remove()
        End If

        If CllType = CellTypes.int Then
            With myCell
                If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CellFormatting.NumInt)
                .CellValue = New CellValue(CllValue)
                .DataType = New EnumValue(Of CellValues)(CellValues.Number)
            End With

        ElseIf CllType = CellTypes.dec Then
            Dim myCellValue As Double = 0
            If Double.TryParse(CllValue, myCellValue) Then
                CllValue = myCellValue.ToString("##########0.0#####", System.Globalization.CultureInfo.InvariantCulture)
            Else
                CllValue = "0"
            End If
            With myCell
                If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CellFormatting.Num2Dec)
                .CellValue = New CellValue(CllValue)
                .DataType = New EnumValue(Of CellValues)(CellValues.Number)
            End With

        ElseIf CllType = CellTypes.hyperlink Then
            Dim ValueSPlits As String() = CllValue.Split({"###"}, StringSplitOptions.RemoveEmptyEntries)
            If ValueSPlits.Length = 2 AndAlso System.Uri.IsWellFormedUriString(ValueSPlits(0), UriKind.Relative) Then
                '#######
                Dim HLinks As Hyperlinks
                If myWorksheet.Worksheet.Descendants(Of Hyperlinks)().Count = 0 Then
                    HLinks = New Hyperlinks
                    If myWorksheet.Worksheet.Descendants(Of PageMargins)().Count = 0 Then
                        myWorksheet.Worksheet.InsertAfter(Of Hyperlinks)(HLinks, myWorksheet.Worksheet.Descendants(Of SheetData)().First())
                    Else
                        myWorksheet.Worksheet.InsertBefore(Of Hyperlinks)(HLinks, myWorksheet.Worksheet.Descendants(Of PageMargins)().First())
                    End If
                Else
                    HLinks = myWorksheet.Worksheet.Descendants(Of Hyperlinks)().First
                End If

                Dim HLinkId As String = "rId" + (HLinks.ChildElements.Count + 1).ToString
                HLinks.Append(New Hyperlink With {.Reference = columnName + rowIndex.ToString().Trim, .Id = HLinkId})

                myWorksheet.Worksheet.Save()
                myWorksheet.AddHyperlinkRelationship(New System.Uri(ValueSPlits(0), UriKind.Relative), True, HLinkId)

                With myCell
                    .StyleIndex = StyleDef(CellFormatting.Hyperlink)
                    .CellValue = New CellValue(ValueSPlits(1))
                    .DataType = New EnumValue(Of CellValues)(CellValues.String)
                End With
            Else
                With myCell
                    If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CllFormat)
                    .CellValue = New CellValue(CllValue)
                    .DataType = New EnumValue(Of CellValues)(CellValues.String)
                End With
            End If
            '#######
        ElseIf CllType = CellTypes.datetime Then
            With myCell
                .CellValue = New CellValue(CllValue)
                .DataType = New EnumValue(Of CellValues)(CellValues.String)
            End With

        ElseIf CllType = CellTypes.text Then
            With myCell
                If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CellFormatting.MultiLineText)
                .CellValue = New CellValue(CllValue)
                .DataType = New EnumValue(Of CellValues)(CellValues.String)
            End With

        ElseIf CllType = CellTypes.formula Then
            With myCell
                If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CllFormat)
                .CellValue = Nothing
                .CellFormula = New CellFormula() With {.FormulaType = CellFormulaValues.Normal, .Text = CllValue}
                .DataType = New EnumValue(Of CellValues)(CellValues.Number)
            End With

        Else
            With myCell
                If .StyleIndex Is Nothing Then .StyleIndex = StyleDef(CllFormat)
                If Not String.IsNullOrEmpty(CllValue) Then .CellValue = New CellValue(CllValue)
                .DataType = New EnumValue(Of CellValues)(CellValues.String)
            End With
        End If

        Return myCell
    End Function

    Public Sub SetCellValue(ByVal columnName As String, ByVal rowIndex As UInteger,
                                        CllValue As String, CllType As CellTypes, Optional CllFormat As CellFormatting = CellFormatting.Null)
        Dim StyleDef As Dictionary(Of CellFormatting, UInt32Value) = ParentExcelFile.CellStyles

        'Einfügestelle finden bzw. schaffen
        Dim worksheet As Worksheet = myWorksheet.Worksheet
        Dim sheetData As SheetData = worksheet.GetFirstChild(Of SheetData)()
        Dim cellReference As String = (columnName + rowIndex.ToString().Trim)
        Dim row As Row
        Dim rowQ = sheetData.Elements(Of Row).Where(Function(r) r.RowIndex.Value = rowIndex)
        If (rowQ.Count() <> 0) Then
            row = rowQ.First()
        Else
            Dim refRow As Row = Nothing
            For Each r As Row In sheetData.Elements(Of Row)()
                If r.RowIndex.Value > rowIndex Then
                    refRow = r
                    Exit For
                End If
            Next
            row = New Row()
            row.RowIndex = rowIndex
            If refRow Is Nothing Then
                sheetData.Append(row)
            Else
                sheetData.InsertBefore(row, refRow)
            End If
        End If

        'Zelle finden, wenn schon da
        Dim myCell As Cell = Nothing
        Dim colQ = row.Elements(Of Cell).Where(Function(c) c.CellReference.Value = cellReference)
        If colQ.Count() > 0 Then
            myCell = colQ.First()
            If CllType = CellTypes.hyperlink Then
                'wegen Quereffekte des Links Sonderbehandlung
                If String.IsNullOrEmpty(CllValue) Then
                    If myCell.CellValue IsNot Nothing Then myCell.CellValue.Remove()
                Else
                    myCell.CellValue = New CellValue(CllValue)
                    myCell.DataType = New EnumValue(Of CellValues)(CellValues.String)
                End If
            Else
                GetCell(columnName, rowIndex, CllValue, CllType, CllFormat, myCell)
            End If
        Else
            Dim refCell As Cell = Nothing
            For Each cell As Cell In row.Elements(Of Cell)()
                If ExistingColumnIsGreater(cell, columnName) Then
                    refCell = cell
                    Exit For
                End If
            Next

            row.InsertBefore(GetCell(columnName, rowIndex, CllValue, CllType, CllFormat), refCell)
        End If
        worksheet.Save()
    End Sub

    ''' <summary>
    ''' Riskant: keine Prüfungen, sondern gleich Anhängen; ACHTUNG: kein worksheetPart.Worksheet.Save() !!
    ''' </summary>
    Public Sub AppendRow(ByVal rowIndex As UInteger, ByRef RowDataList As List(Of RowData))
        Dim worksheet As Worksheet = myWorksheet.Worksheet
        Dim sheetData As SheetData = worksheet.GetFirstChild(Of SheetData)()
        Dim row As New Row With {.RowIndex = rowIndex}

        For Each c As RowData In (From rd As RowData In RowDataList Where Not String.IsNullOrEmpty(rd.Column.Trim)
                                  Let sortstr = rd.Column.PadLeft(6) Order By sortstr Select rd).ToList
            row.InsertBefore(GetCell(c.Column, rowIndex, c.Value, c.CellType, c.CellStyle), Nothing)
        Next

        sheetData.Append(row)
    End Sub

    '#########################################################################
    Public Sub SetColumnHeader(ByVal columnName As String, ByVal rowIndex As UInteger, HeaderTitle As String,
                                CellFormat As CellFormatting, Optional ColumnWidth As Double = 0.0#, Optional NameToDefine As String = Nothing)
        SetCellValue(columnName, rowIndex, HeaderTitle, CellTypes.str, CellFormat)
        If ColumnWidth > 0.0# Then
            Dim SpreadSheetColumns As Columns
            If myWorksheet.Worksheet.Descendants(Of Columns)().Count = 0 Then
                SpreadSheetColumns = New Columns
                myWorksheet.Worksheet.InsertBefore(Of Columns)(SpreadSheetColumns, myWorksheet.Worksheet.Descendants(Of SheetData)().First())
            Else
                SpreadSheetColumns = myWorksheet.Worksheet.Descendants(Of Columns)().First
            End If
            Dim colIndex As Integer = GetColumnIndexFromString(columnName)
            SpreadSheetColumns.Append(New Column With {.CustomWidth = True, .Min = colIndex, .Max = colIndex, .Width = ColumnWidth})
        End If

        If Not String.IsNullOrEmpty(NameToDefine) Then Me.ParentExcelFile.DefineName(NameToDefine, "'" + myName + "'!$" + columnName + "$" + rowIndex.ToString)
    End Sub

    '##### private #####



    '############
    Private Function ExistingColumnIsGreater(ExistingCell As Cell, column As String) As Boolean
        Dim mc As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(ExistingCell.CellReference.Value.ToUpper, "^[A-Z]+")
        Dim existingC As String = mc.Item(0).Value
        If existingC.Length = column.Length Then
            Return (String.Compare(existingC, column.ToUpper, True) > 0)
        Else
            Return existingC.Length > column.Length
        End If
    End Function

    '############
    Private Function GetColumnIndexFromString(ColStr As String) As Integer
        ColStr = ColStr.ToUpper
        Dim myreturn As Integer = Asc(ColStr.Substring(ColStr.Length - 1, 1)) - Asc("A") + 1
        If ColStr.Length > 1 Then
            Dim multiplier As Integer = Asc("Z") - Asc("A") + 1
            myreturn = myreturn + (Asc(ColStr.Substring(ColStr.Length - 2, 1)) - Asc("A") + 1) * multiplier
            If ColStr.Length > 2 Then
                myreturn = myreturn + (Asc(ColStr.Substring(ColStr.Length - 3, 1)) - Asc("A") + 1) * multiplier * multiplier
            End If
        End If

        Return myreturn
    End Function
End Class

'#######################################################
'#######################################################
Public Class NamedRef
    Private myref As String
    Public Sub New(RefString)
        If String.IsNullOrEmpty(RefString) Then
            Throw New ArgumentNullException("NamedRef: String darf nicht leer sein")
        End If
        myref = RefString
    End Sub
    Public ReadOnly Property Column() As String
        Get
            Dim myreturn As String = ""
            Dim pos As Integer = myref.IndexOf("!$")
            If pos > 0 Then
                myreturn = myref.Substring(pos + 2) '$-Zeichen
                myreturn = myreturn.Substring(0, myreturn.IndexOf("$"))
            End If
            Return myreturn
        End Get
    End Property
    Public ReadOnly Property WorksheetName() As String
        Get
            Dim myreturn As String = ""
            Dim pos As Integer = myref.IndexOf("!$")
            If pos > 0 Then
                myreturn = myref.Substring(0, pos)
                If myreturn.Substring(0, 1) = "'" Then myreturn = myreturn.Substring(1, myreturn.Length - 2)
            End If

            Return myreturn
        End Get
    End Property
    Public ReadOnly Property Row() As Integer
        Get
            Dim myreturn As Integer = 0
            Dim mc As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(myref, "!\$[A-Z]+\$[0-9]+")
            If mc.Count > 0 Then
                Dim tmpstr As String = mc.Item(0).Value
                tmpstr = tmpstr.Substring(tmpstr.LastIndexOf("$") + 1)
                Try
                    myreturn = Integer.Parse(tmpstr)
                Catch ex As Exception
                    myreturn = 0
                End Try
            End If

            Return myreturn
        End Get
    End Property
    Public ReadOnly Property Address() As String
        Get
            Dim mc As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(myref, "\$[A-Z]+\$[0-9]+$")
            Return mc.Item(0).Value
        End Get
    End Property

End Class
