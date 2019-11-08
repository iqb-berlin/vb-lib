Public Class JournalFactory
    Public Shared Property JournalEntryCategories As New Dictionary(Of String, Brush) From {
        {"Information", Brushes.Transparent},
        {"FDZ intern", Brushes.Transparent},
        {"Anweisung", Brushes.Transparent},
        {"vor Ort", Brushes.Transparent},
        {"!Wichtig!", Brushes.OrangeRed},
        {"E-Mail-Ausgang", Brushes.Transparent},
        {"E-Mail-Eingang", Brushes.Transparent},
        {"Post-Ausgang", Brushes.Transparent},
        {"Post-Eingang", Brushes.Transparent},
        {"Anruf zu", Brushes.Transparent},
        {"Anruf von", Brushes.Transparent}
    }

    Public Shared Sub ClearJournalEntryCategories()
        JournalEntryCategories.Clear()
    End Sub

    Public Shared Sub AddJournalEntryCategory(Label As String, Brush As Brush)
        If Not String.IsNullOrEmpty(Label) Then
            If Not JournalEntryCategories.ContainsKey(Label) Then
                JournalEntryCategories.Add(Label, Brush)
            Else
                JournalEntryCategories.Item(Label) = Brush
            End If
        End If
    End Sub

    Public Shared Function NewLogEntry(UserName As String, Category As String, Label As String, RefStrings As List(Of String), EntryText As String, IsSystemEntry As Boolean, ReferenceDate As Date)
        Dim XNewLogEntry As XElement = <l date=<%= ReferenceDate.ToString("yyyy'-'MM'-'dd") %> sortstr=<%= ReferenceDate.ToString("s") %> lb=<%= Label %> user=<%= UserName %> category=<%= Category %> sys=<%= IsSystemEntry.ToString %>><text><%= EntryText %></text></l>
        XNewLogEntry.@lastedit = Date.Now.ToString("yyyy'-'MM'-'dd")

        If RefStrings IsNot Nothing Then
            For Each s As String In RefStrings
                XNewLogEntry.Add(<ref><%= s %></ref>)
            Next
        End If
        Return XNewLogEntry
    End Function

End Class
