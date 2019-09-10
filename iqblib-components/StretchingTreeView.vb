Public Class StretchingTreeView
    Inherits TreeView
    Protected Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New StretchingTreeViewItem()
    End Function

    Protected Overrides Function IsItemItsOwnContainerOverride(item As Object) As Boolean
        Return TypeOf (item) Is StretchingTreeViewItem
    End Function
End Class

Public Class StretchingTreeViewItem
    Inherits TreeViewItem

    Public Sub New()
        MyBase.New
        AddHandler Me.Loaded, AddressOf StretchingTreeViewItem_Loaded
    End Sub

    Private Sub StretchingTreeViewItem_Loaded(sender As Object, e As RoutedEventArgs)
        'The purpose of this code Is to stretch the Header Content all the way accross the TreeView. 
        If (Me.VisualChildrenCount > 0) Then
            Dim myGrid As Grid = CType(Me.GetVisualChild(0), Grid)
            If myGrid IsNot Nothing AndAlso myGrid.ColumnDefinitions.Count = 3 Then
                'Remove the middle column which Is set to Auto And let it get replaced with the 
                'last column that Is set to Star.
                myGrid.ColumnDefinitions.RemoveAt(1)
            End If
        End If
    End Sub

    Protected Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New StretchingTreeViewItem()
    End Function

    Protected Overrides Function IsItemItsOwnContainerOverride(item As Object) As Boolean
        Return TypeOf (item) Is StretchingTreeViewItem
    End Function
End Class
