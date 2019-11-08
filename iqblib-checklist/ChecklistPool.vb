Public MustInherit Class ChecklistPool
    Public MustOverride Property PoolLabel() As String
    Public MustOverride Property Pool() As Dictionary(Of String, XElement)
    Public MustOverride Property PlusPropLabel() As String
    Public MustOverride Property PlusPropValues() As Dictionary(Of String, String)
    Public MustOverride Sub AutoSave(sender As Object, e As XObjectChangeEventArgs)
    Public MustOverride Function AddChecklist(Name As String) As String
    Public MustOverride Function DeleteChecklist(Id As String) As Boolean

    Public Shared Function GetNewXPoint() As XElement
        Return <point input="False" final="False" plusprop="" prop=""><prompt/><label/><journaltext/><continue c="False" mc="False"/></point>
    End Function

    Public Function GetChecklistLabels() As List(Of String)
        Dim myReturn As New List(Of String)
        Dim myChecklists As Dictionary(Of String, XElement) = Me.Pool
        For Each cl As KeyValuePair(Of String, XElement) In myChecklists
            myReturn.Add(cl.Value.@lb)
        Next

        Return myReturn
    End Function
End Class
