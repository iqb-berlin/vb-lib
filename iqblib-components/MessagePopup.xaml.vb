Imports System.Windows.Media.Animation

'Achtung: derartige Verrenkungen vor allem deshalb, weil Popup.IsOpen keine DependencyProperty ist!
Public Class MessagePopup
    Private StB As Storyboard = Nothing

    Public Shared ReadOnly IsOpenProperty As DependencyProperty =
        DependencyProperty.Register("IsOpen", GetType(Boolean), GetType(MessagePopup))
    Public Property IsOpen As Boolean
        Get
            Return GetValue(IsOpenProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsOpenProperty, value)
        End Set
    End Property


    Public Shared ReadOnly MaxPopupWidthProperty As DependencyProperty =
        DependencyProperty.Register("MaxPopupWidth", GetType(Double), GetType(MessagePopup))

    Public Property MaxPopupWidth As Double
        Get
            Return GetValue(MaxPopupWidthProperty)
        End Get
        Set(ByVal value As Double)
            SetValue(MaxPopupWidthProperty, value)
        End Set
    End Property


    Public Shared ReadOnly MessageTextProperty As DependencyProperty =
        DependencyProperty.Register("MessageText", GetType(String), GetType(MessagePopup), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.None,
            New PropertyChangedCallback(AddressOf MessageTextPropertyChanged)))
    Public Property MessageText As String
        Get
            Return GetValue(MessageTextProperty)
        End Get
        Set(ByVal value As String)
            SetValue(MessageTextProperty, value)
        End Set
    End Property


    Private Sub Me_Loaded() Handles Me.Loaded
        PppShortMessage.SetBinding(MessagePopup.MaxWidthProperty, New Binding With {.Source = Me, .Path = New PropertyPath("MaxPopupWidth")})
    End Sub

    Private Shared Sub MessageTextPropertyChanged(ByVal d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If Not String.IsNullOrEmpty(e.NewValue) AndAlso TypeOf (d) Is MessagePopup Then
            Dim myMessagePopup As MessagePopup = d
            If myMessagePopup.StB Is Nothing Then
                myMessagePopup.StB = New Storyboard
                Dim ba As New BooleanAnimationUsingKeyFrames
                Storyboard.SetTarget(ba, myMessagePopup)
                Storyboard.SetTargetProperty(ba, New PropertyPath(MessagePopup.IsOpenProperty))
                ba.KeyFrames.Add(New DiscreteBooleanKeyFrame(True, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))))
                ba.KeyFrames.Add(New DiscreteBooleanKeyFrame(False, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(4.0))))
                myMessagePopup.StB.Children.Add(ba)
                myMessagePopup.BeginStoryboard(myMessagePopup.StB)
            Else
                myMessagePopup.StB.Begin()
            End If
        End If
    End Sub
End Class
