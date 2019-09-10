Imports System.Windows.Media.Animation

Public Class AsynchProgressBarUserControl
    Private Delegate Sub UpdateProgressStateDelegate(NewState As Double)
    Private MyUpdateProgressStateDelegate As UpdateProgressStateDelegate = Nothing

    Public Sub UpdateProgressState(NewState As Double)
        If MyUpdateProgressStateDelegate Is Nothing Then
            InternalUpdateProgressState(NewState)
        Else
            Me.MyProB.Dispatcher.Invoke(MyUpdateProgressStateDelegate, NewState)
        End If
    End Sub

    Private Sub InternalUpdateProgressState(NewState As Double)
        MyProB.Value = NewState
    End Sub

    Public Property Value() As Double
        Get
            Return MyProB.Value
        End Get
        Set(ByVal value As Double)
            MyProB.Value = value
        End Set
    End Property

    Public Sub StartInfiniteModus()
        MyProB.IsIndeterminate = True
    End Sub

    Public Sub StopInfiniteModus()
        MyProB.IsIndeterminate = False
    End Sub
End Class
