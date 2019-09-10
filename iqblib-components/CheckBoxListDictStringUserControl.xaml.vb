Public Class CheckBoxListDictStringUserControl
    Public Shared ReadOnly PropValueProperty As DependencyProperty =
        DependencyProperty.Register("PropValue", GetType(String), GetType(CheckBoxListDictStringUserControl),
                                    New FrameworkPropertyMetadata(Nothing,
                                                       FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                      Nothing))

    Public Property PropValue As String
        Get
            Return GetValue(PropValueProperty)
        End Get
        Set(ByVal value As String)
            SetValue(PropValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LabelProperty As DependencyProperty =
        DependencyProperty.Register("Label", GetType(String), GetType(CheckBoxListDictStringUserControl))
    Public Property Label As String
        Get
            Return GetValue(LabelProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    Public Shared ReadOnly SelectionListProperty As DependencyProperty =
        DependencyProperty.Register("SelectionList", GetType(Dictionary(Of String, String)), GetType(CheckBoxListDictStringUserControl))
    Public Property SelectionList As Dictionary(Of String, String)
        Get
            Return GetValue(SelectionListProperty)
        End Get
        Set(ByVal value As Dictionary(Of String, String))
            SetValue(SelectionListProperty, value)
        End Set
    End Property

End Class
