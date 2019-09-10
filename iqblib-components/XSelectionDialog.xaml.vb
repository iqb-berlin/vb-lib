Public Class XSelectionDialog
    Public XSelectionList As List(Of XElement) = Nothing
    Public Selected As String
    Public HelpTopic As Integer
    Public TipText As String
    Public Prompt As String
    Public TitleStr As String
    Public MultipleSelection As Boolean = False
    Public GroupAttributePath As String = Nothing
    Public KeyAttributePath As String = "key"
    Public GroupStyleTaskGroup As Boolean = False

    Private Sub Me_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If Not String.IsNullOrEmpty(TitleStr) Then Me.Title = TitleStr

        If HelpTopic > 0 Then
            Me.SetValue(HelpProvider.HelpTopicIdProperty, HelpTopic.ToString)
        Else
            BtnHelp.Visibility = Windows.Visibility.Collapsed
        End If

        If String.IsNullOrEmpty(TipText) Then
            LbTip.Visibility = Windows.Visibility.Collapsed
        Else
            LbTip.Content = TipText
        End If

        If String.IsNullOrEmpty(Prompt) Then
            TBPrompt.Visibility = Windows.Visibility.Collapsed
        Else
            TBPrompt.Text = Prompt
        End If

        If MultipleSelection Then LBSelection.SelectionMode = SelectionMode.Extended

        LBSelection.ItemsSource = XSelectionList

        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBSelection.ItemsSource)
        If cv IsNot Nothing Then
            If Not String.IsNullOrEmpty(GroupAttributePath) Then
                cv.GroupDescriptions.Add(New PropertyGroupDescription("Attribute[" + GroupAttributePath + "].Value"))
                cv.SortDescriptions.Add(New ComponentModel.SortDescription("Attribute[" + GroupAttributePath + "].Value", ComponentModel.ListSortDirection.Ascending))

                Dim gs As New GroupStyle()
                If GroupStyleTaskGroup Then
                    gs.ContainerStyle = Me.FindResource("gsGroupStyleTaskGroup")
                Else
                    gs.ContainerStyle = Me.FindResource("gsGroupStyleStandard")
                End If
                LBSelection.GroupStyle.Add(gs)

            End If
            cv.SortDescriptions.Add(New ComponentModel.SortDescription("Value", ComponentModel.ListSortDirection.Ascending))
        End If

        If LBSelection.ItemContainerGenerator.Status = Primitives.GeneratorStatus.ContainersGenerated Then
            Dispatcher.BeginInvoke(Windows.Threading.DispatcherPriority.Render, New Action(AddressOf DelayedFocusToItem))
        Else
            AddHandler LBSelection.ItemContainerGenerator.StatusChanged, AddressOf ItemContainerGenerator_StatusChanged
        End If
    End Sub

    Private Sub OK_Click() Handles BtnOK.Click, LBSelection.MouseDoubleClick
        Selected = String.Join(" ", From xe As XElement In LBSelection.SelectedItems Select xe.Attribute(KeyAttributePath).Value)
        If Not String.IsNullOrEmpty(Selected) Then Me.DialogResult = True
    End Sub

    Private Sub ItemContainerGenerator_StatusChanged(sender As Object, e As EventArgs)
        If Not LBSelection.ItemContainerGenerator.Status = Primitives.GeneratorStatus.ContainersGenerated Then Return
        RemoveHandler LBSelection.ItemContainerGenerator.StatusChanged, AddressOf ItemContainerGenerator_StatusChanged
        Dispatcher.BeginInvoke(Windows.Threading.DispatcherPriority.Render, New Action(AddressOf DelayedFocusToItem))
    End Sub

    Private Sub DelayedFocusToItem()
        If Not String.IsNullOrEmpty(Selected) AndAlso XSelectionList IsNot Nothing AndAlso XSelectionList.Count > 0 Then
            Dim SelectedList As List(Of String) = Selected.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            For Each xe As XElement In LBSelection.Items
                If SelectedList.Contains(xe.Attribute(KeyAttributePath).Value) Then
                    LBSelection.SelectedItem = xe
                    Dim item As ListBoxItem = LBSelection.ItemContainerGenerator.ContainerFromItem(LBSelection.SelectedItem)
                    If item IsNot Nothing Then item.BringIntoView()

                    Exit For
                End If
            Next
        End If
        LBSelection.Focus()
    End Sub

End Class
