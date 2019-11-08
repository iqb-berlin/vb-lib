Imports iqb.md.xml

Public Class EditChecklistsPointDialog

    Private _ChecklistPool As ChecklistPool
    Private _XChecklist As XElement
    Private _XPoint As XElement
    Private _MDFilter As MDFilter
    Private _MDCatalogList As List(Of String)

    Private _CheckListLabels As List(Of XElement)
    Public ReadOnly Property CheckListLabels() As List(Of XElement)
        Get
            Return _CheckListLabels
        End Get
    End Property



    Public Sub New(ChecklistPool As ChecklistPool, XChecklist As XElement, ByRef XPointToChange As XElement,
                   MDCatalogList As List(Of String), MDFilter As MDFilter)
        InitializeComponent()
        _XChecklist = XChecklist
        _ChecklistPool = ChecklistPool
        _XPoint = XPointToChange
        _MDCatalogList = MDCatalogList
        _MDFilter = MDFilter

        If _XPoint IsNot Nothing AndAlso _XPoint.<continue>.FirstOrDefault Is Nothing Then _XPoint.Add(<continue c="False" mc="False"/>)

        _CheckListLabels = New List(Of XElement)
        For Each CL As KeyValuePair(Of String, XElement) In From CLL As KeyValuePair(Of String, XElement) In _ChecklistPool.Pool Order By CLL.Value.@lb
            _CheckListLabels.Add(<cl id=<%= CL.Key %>><%= CL.Value.@lb %></cl>)
        Next
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        If _XPoint Is Nothing Then
            Me.Title = _ChecklistPool.PoolLabel + "-Checkliste: Neuer Punkt"
        Else
            Me.Title = _ChecklistPool.PoolLabel + "-Checkliste: Punkt bearbeiten"
        End If
        If String.IsNullOrEmpty(_ChecklistPool.PlusPropLabel) OrElse _ChecklistPool.PlusPropValues Is Nothing OrElse _ChecklistPool.PlusPropValues.Count < 1 Then
            DPPlusProp.Visibility = Windows.Visibility.Collapsed
        Else
            LbPlusProp.Text = "Änderung '" + _ChecklistPool.PlusPropLabel + "' auf:"
            Dim CBPlusProp_ItemsSource As New List(Of XElement)
            CBPlusProp_ItemsSource.Add(<pp id="">-keine Änderung-</pp>)
            CBPlusProp_ItemsSource.AddRange(From pp As KeyValuePair(Of String, String) In _ChecklistPool.PlusPropValues Select <pp id=<%= pp.Key %>><%= pp.Value %></pp>)
            CBPlusProp.ItemsSource = CBPlusProp_ItemsSource
        End If

        If _MDCatalogList Is Nothing OrElse _MDCatalogList.Count = 0 Then
            DPProps.Visibility = Windows.Visibility.Collapsed
        Else
            TBPropCat.DataContext = _PropCatalog
        End If

        If _XPoint Is Nothing Then
            Dim XNewPoint As XElement = ChecklistPool.GetNewXPoint()
            Me.DataContext = XNewPoint
        Else
            Dim XNewPoint As New XElement(_XPoint)
            If Not String.IsNullOrEmpty(XNewPoint.@status) Then XNewPoint.@plusprop = XNewPoint.@status
            Me.DataContext = XNewPoint
        End If
    End Sub


    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs)

        Me.DialogResult = False
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs)
        Dim XNewPoint As XElement = Me.DataContext
        Dim XLabel As XElement = XNewPoint.<label>.FirstOrDefault
        If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), Me.Title, "Bitte zumindest die Bezeichnung (Titel) eintragen!")
        Else
            If _XPoint Is Nothing Then
                Dim IDint As Integer = Integer.Parse(_XChecklist.@nextid)
                XNewPoint.@id = IDint.ToString
                _XChecklist.Add(XNewPoint)
                _XChecklist.@nextid = (IDint + 1).ToString
            Else
                _XPoint.@input = XNewPoint.@input
                _XPoint.@final = XNewPoint.@final
                _XPoint.@plusprop = XNewPoint.@plusprop
                If Not String.IsNullOrEmpty(_XPoint.@status) Then _XPoint.Attribute("status").Remove()
                _XPoint.@prop = XNewPoint.@prop
                Dim xe As XElement = _XPoint.<label>.FirstOrDefault
                If xe Is Nothing Then
                    _XPoint.Add(XLabel)
                Else
                    xe.Value = XLabel.Value
                End If
                xe = _XPoint.<journaltext>.FirstOrDefault
                If xe Is Nothing Then
                    _XPoint.Add(XNewPoint.<journaltext>.First)
                Else
                    xe.Value = XNewPoint.<journaltext>.First.Value
                End If
                If _XPoint.<prompt>.FirstOrDefault IsNot Nothing Then _XPoint.<prompt>.First.Remove()
                _XPoint.Add(XNewPoint.<prompt>.First)
                If _XPoint.<continue>.FirstOrDefault IsNot Nothing Then _XPoint.<continue>.First.Remove()
                _XPoint.Add(XNewPoint.<continue>.First)
            End If
            Me.DialogResult = True
        End If
    End Sub

    '#####################
    'Props
    Private Sub BtnNewProp_Click(sender As Object, e As RoutedEventArgs)
        Dim XPoint As XElement = Me.DataContext

        If XPoint IsNot Nothing Then
            Dim PropList As List(Of String) = XPoint.@prop.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim PropsAvailable As List(Of XElement) = (From xe As XElement In _PropCatalog.GetSortedPropEnumeration(New MDR.PropertyFilter(_PropScopes, True, False))
                                                       Where Not PropList.Contains(xe.@key)).ToList
            If PropsAvailable.Count = 0 Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), _ChecklistPool.PoolLabel + "-Checkliste: Merkmal hinzufügen", "Es sind keine weiteren Merkmale verfügbar.")
            Else
                Dim propicker As New MDR.PickPropertyDialog With {.AvailableProperties = PropsAvailable, .Owner = DialogFactory.GetParentWindow(Me)}
                If propicker.ShowDialog Then
                    For Each s As String In propicker.SelectedProperties
                        PropList.Add(s)
                    Next
                    XPoint.@prop = String.Join(" ", PropList)

                    Dim be As BindingExpression = LBProps.GetBindingExpression(ListBox.ItemsSourceProperty)
                    If be IsNot Nothing Then be.UpdateTarget()
                End If
            End If
        End If
    End Sub

    Private Sub BtnDeleteProp_Click(sender As Object, e As RoutedEventArgs)
        If LBProps.SelectedItems.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), _ChecklistPool.PoolLabel + "-Checkliste: Löschen Merkmal(e)", "Bitte markieren Sie ein Merkmal!")
        Else
            Dim XPoint As XElement = Me.DataContext
            Dim OldProps As List(Of String) = XPoint.@prop.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim PropsToDelete As List(Of String) = (From xs As XElement In LBProps.SelectedItems Select xs.@key).ToList
            XPoint.@prop = String.Join(" ", From s As String In OldProps Where Not PropsToDelete.Contains(s))

            Dim be As BindingExpression = LBProps.GetBindingExpression(ListBox.ItemsSourceProperty)
            If be IsNot Nothing Then be.UpdateTarget()
        End If
    End Sub

    '#####################
    'Continue
    Private Sub BtnNewContinue_Click(sender As Object, e As RoutedEventArgs)
        Dim XSelection As New List(Of XElement)
        Dim XPoint As XElement = Me.DataContext

        For Each xcl As XElement In _CheckListLabels
            Debug.Print(xcl.@id + " - " + xcl.Value)
            If (From xxcl As XElement In XPoint.<continue>.First.Elements Where xxcl.@id = xcl.@id).FirstOrDefault Is Nothing Then XSelection.Add(xcl)
        Next
        Dim myDlg As New XSelectionDialog With {.Title = _ChecklistPool.PoolLabel + "-Checkliste zuweisen", .XSelectionList = XSelection, .KeyAttributePath = "id"}
        If myDlg.ShowDialog Then
            XPoint.<continue>.First.Add(<cl id=<%= myDlg.Selected %>/>)
        End If
    End Sub

    Private Sub BtnDeleteContinue_Click(sender As Object, e As RoutedEventArgs)
        If LBContinue.Items.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), _ChecklistPool.PoolLabel + "-Checkliste entfernen", "Dieser Punkt enthält keine Checklisten zur Fortführung.")
        Else
            If LBContinue.SelectedItems.Count = 0 Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), _ChecklistPool.PoolLabel + "-Checkliste entfernen", "Bitte markieren Sie eine Checkliste zur Fortführung!")
            Else
                Dim XPoint As XElement = Me.DataContext
                For Each xc As XElement In (From xxc As XElement In LBContinue.SelectedItems).ToList
                    Dim xeToDelete As XElement = (From xxc As XElement In XPoint.<continue>.First.Elements Where xxc.@id = xc.@id).FirstOrDefault
                    If xeToDelete IsNot Nothing Then xeToDelete.Remove()
                Next
            End If
        End If
    End Sub

End Class
