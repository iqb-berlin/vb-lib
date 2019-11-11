Imports iqb.lib.components
Imports iqb.md.xml

Public Class ChecklistPointsEditUserControl
    Public Shared ReadOnly MyChecklistPoolProperty As DependencyProperty =
        DependencyProperty.Register("MyChecklistPool", GetType(ChecklistPool), GetType(ChecklistPointsEditUserControl))

    Public Property MyChecklistPool As ChecklistPool
        Get
            Return GetValue(MyChecklistPoolProperty)
        End Get
        Set(ByVal value As ChecklistPool)
            SetValue(MyChecklistPoolProperty, value)
        End Set
    End Property

    Public Shared ReadOnly XChecklistProperty As DependencyProperty =
        DependencyProperty.Register("XChecklist", GetType(XElement), GetType(ChecklistPointsEditUserControl))

    Public Property XChecklist As XElement
        Get
            Return GetValue(XChecklistProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XChecklistProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MDFilterProperty As DependencyProperty =
        DependencyProperty.Register("MDFilter", GetType(MDFilter), GetType(ChecklistPointsEditUserControl))

    Public Property MDFilter As MDFilter
        Get
            Return GetValue(MDFilterProperty)
        End Get
        Set(ByVal value As MDFilter)
            SetValue(MDFilterProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MDCatalogListProperty As DependencyProperty =
        DependencyProperty.Register("MDCatalogList", GetType(List(Of String)), GetType(ChecklistPointsEditUserControl))

    Public Property MDCatalogList As List(Of String)
        Get
            Return GetValue(MDCatalogListProperty)
        End Get
        Set(ByVal value As List(Of String))
            SetValue(MDCatalogListProperty, value)
        End Set
    End Property

    '_ As List(Of String)
    '################################################################################################
    Public Sub RefreshPointList()
        Dim be As BindingExpression = LBPoints.GetBindingExpression(ItemsControl.ItemsSourceProperty)
        If be IsNot Nothing Then be.UpdateTarget()
    End Sub

    Private Sub BtnNewPoint_Click(sender As Object, e As RoutedEventArgs)
        If XChecklist Is Nothing Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neuer Eintrag", "Bitte wählen Sie erst eine Checkliste aus!")
        Else
            Dim myDlg As New EditChecklistsPointDialog(MyChecklistPool, XChecklist, Nothing, Me.MDCatalogList, Me.MDFilter) With {.Owner = DialogFactory.GetParentWindow(Me)}
            If myDlg.ShowDialog Then RefreshPointList()
        End If
    End Sub

    Private Sub BtnDeletePoint_Click(sender As Object, e As RoutedEventArgs)
        If LBPoints.SelectedItems.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Löschen Punkt", "Bitte wählen Sie erst einen Punkt aus der aktuellen Checkliste aus!")
        Else
            Dim XPoint As XElement = LBPoints.SelectedItems(0)
            XPoint.Remove()
        End If
    End Sub

    Private Sub BtnEditPoint_Click(sender As Object, e As RoutedEventArgs)
        If LBPoints.SelectedItems.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Ändern Punkt", "Bitte wählen Sie erst einen Punkt aus der aktuellen Checkliste aus!")
        Else
            Dim XPoint As XElement = LBPoints.SelectedItems(0)

            Dim myDlg As New EditChecklistsPointDialog(MyChecklistPool, XChecklist, XPoint, Me.MDCatalogList, Me.MDFilter) With {.Owner = DialogFactory.GetParentWindow(Me)}
            myDlg.ShowDialog()
            RefreshPointList()
        End If
    End Sub


    '######################## Move-Buttons
    Private Sub BtnMoveUp_Click(sender As Object, e As RoutedEventArgs)
        If LBPoints.SelectedItems.Count > 0 Then
            Dim XPoint As XElement = LBPoints.SelectedItems(0)
            Dim PrevPoint As XElement = XPoint.PreviousNode
            If PrevPoint IsNot Nothing Then
                Dim newX As New XElement(XPoint)
                XPoint.Remove()
                PrevPoint.AddBeforeSelf(newX)
                LBPoints.SelectedItem = newX
            End If
        Else
            If DPChecklistData.DataContext Is Nothing Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Punkt nach oben verschieben", "Bitte markieren Sie eine Checkliste und einen Punkt!")
            Else
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Punkt nach oben verschieben", "Bitte markieren Sie einen Punkt!")
            End If
        End If
    End Sub

    Private Sub BtnMoveDown_Click(sender As Object, e As RoutedEventArgs)
        If LBPoints.SelectedItems.Count > 0 Then
            Dim XPoint As XElement = LBPoints.SelectedItems(0)
            Dim NextPoint As XElement = XPoint.NextNode
            If NextPoint IsNot Nothing Then
                Dim newX As New XElement(XPoint)
                XPoint.Remove()
                NextPoint.AddAfterSelf(newX)
                LBPoints.SelectedItem = newX
            End If
        Else
            If DPChecklistData.DataContext Is Nothing Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Punkt nach oben verschieben", "Bitte markieren Sie eine Checkliste und einen Punkt!")
            Else
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Punkt nach oben verschieben", "Bitte markieren Sie einen Punkt!")
            End If
        End If
    End Sub
End Class
