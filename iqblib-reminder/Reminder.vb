Public Class Reminder
    Public Shared ReadOnly AddReminder As RoutedUICommand = New RoutedUICommand("Neue Erinnerung", "AddReminder", GetType(Reminder))
    Public Shared ReadOnly ChangeReminder As RoutedUICommand = New RoutedUICommand("Erinnerung ändern", "ChangeReminder", GetType(Reminder))
    Public Shared ReadOnly RemoveReminder As RoutedUICommand = New RoutedUICommand("Erinnerung entfernen", "RemoveReminder", GetType(Reminder))

    Public Shared Property XStandardReminder As XElement = <r id="??" date="??"/>

    Private _ObjectId As Integer
    Public Property ObjectId() As Integer
        Get
            Return _ObjectId
        End Get
        Set(ByVal value As Integer)
            _ObjectId = value
        End Set
    End Property

    Private _ObjectLabel As String
    Public Property ObjectLabel() As String
        Get
            Return _ObjectLabel
        End Get
        Set(ByVal value As String)
            _ObjectLabel = value
        End Set
    End Property

    Private _ReminderId As Integer
    Public Property ReminderId() As Integer
        Get
            Return _ReminderId
        End Get
        Set(ByVal value As Integer)
            _ReminderId = value
        End Set
    End Property

    Private _Maturity As Date
    Public Property Maturity() As Date
        Get
            Return _Maturity
        End Get
        Set(ByVal value As Date)
            _Maturity = value
        End Set
    End Property

    Private _Text As String
    Public Property Text() As String
        Get
            Return _Text
        End Get
        Set(ByVal value As String)
            _Text = value
        End Set
    End Property

    Public ReadOnly Property DaysToMaturity As Integer
        Get
            Return (Me._Maturity - DateTime.Now).TotalDays
        End Get
    End Property

    Public ReadOnly Property MaturityBrush As Brush
        Get
            Dim DTM As Integer = Me.DaysToMaturity
            If DTM < -5 Then
                Return Brushes.Red
            ElseIf DTM < 2 Then
                Return Brushes.Gold
            ElseIf DTM < 10 Then
                Return Brushes.Yellow
            ElseIf DTM < 30 Then
                Return Brushes.YellowGreen
            Else
                Return Brushes.LightGray
            End If
        End Get
    End Property

    Public Sub New(ObjectId As Integer, ObjectLabel As String, ReminderId As Integer)
        _Text = ""
        _ObjectId = ObjectId
        _ObjectLabel = ObjectLabel
        _ReminderId = ReminderId
        _Maturity = Date.Now
    End Sub

    Public Sub New(ObjectId As Integer, ObjectLabel As String, XReminder As XElement)
        _Text = XReminder.Value
        _Maturity = Date.Parse(XReminder.@date)
        _ReminderId = Integer.Parse(XReminder.@id)
        _ObjectId = ObjectId
        _ObjectLabel = ObjectLabel
    End Sub

    ''' <returns>Achtung: Ohne Objekt-Id und -Label</returns>
    Public Function ToXml() As XElement
        Dim newXR As New XElement(XStandardReminder)
        newXR.@id = Me._ReminderId.ToString
        newXR.@date = Me._Maturity.ToString
        newXR.Value = Me._Text
        Return newXR
    End Function
End Class
