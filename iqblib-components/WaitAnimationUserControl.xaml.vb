Imports System.Windows.Media.Animation

Public Class WaitAnimationUserControl
    'Dynamisch, damit die aktuelle Breite des Controls berücksichtigt werden kann

    Private Sub StartHelper(sb As Storyboard, XValue As Integer, StartPause As Double, BackTime As Double)
        Dim DAni As DoubleAnimationUsingKeyFrames = sb.Children.First
        If StartPause > 0 Then DAni.KeyFrames.Add(New EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(New TimeSpan(0, 0, 0, Math.Truncate(StartPause), (StartPause - Math.Truncate(StartPause)) * 1000))))
        DAni.KeyFrames.Add(New EasingDoubleKeyFrame(XValue, KeyTime.FromTimeSpan(New TimeSpan(0, 0, 2.5)), New CircleEase With {.EasingMode = EasingMode.EaseOut}))
        DAni.KeyFrames.Add(New EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(New TimeSpan(0, 0, 0, Math.Truncate(BackTime), (BackTime - Math.Truncate(BackTime)) * 1000)), New CircleEase With {.EasingMode = EasingMode.EaseInOut}))
        sb.Begin()
    End Sub

    Public Sub StartAnimation()
        Dim NewXValue As Integer = Me.ActualWidth - 60
        StartHelper(Me.FindResource("Storyboard1"), NewXValue, 0, 5.2)
        StartHelper(Me.FindResource("Storyboard2"), NewXValue, 0.3, 4.9)
        StartHelper(Me.FindResource("Storyboard3"), NewXValue, 0.6, 4.6)
        StartHelper(Me.FindResource("Storyboard4"), NewXValue, 0.9, 4.3)
        StartHelper(Me.FindResource("Storyboard5"), NewXValue, 1.2, 4)
    End Sub

    Private Sub StopHelper(sb As Storyboard)
        Dim DAni As DoubleAnimationUsingKeyFrames = sb.Children.First
        sb.Stop()
        DAni.KeyFrames.Clear()
    End Sub

    Public Sub StopAnimation()
        StopHelper(Me.FindResource("Storyboard1"))
        StopHelper(Me.FindResource("Storyboard2"))
        StopHelper(Me.FindResource("Storyboard3"))
        StopHelper(Me.FindResource("Storyboard4"))
        StopHelper(Me.FindResource("Storyboard5"))
    End Sub
End Class
