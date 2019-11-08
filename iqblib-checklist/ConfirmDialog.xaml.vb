Public Class ConfirmDialog
    Private _XEntry As XElement
    Private _CLPool As ChecklistPool
    Private _JournalTitle As String

    Public ReadOnly Property JournalTitle() As String
        Get
            Return _JournalTitle
        End Get
    End Property
    Private _JournalText As String
    Public ReadOnly Property JournalText() As String
        Get
            Return _JournalText
        End Get
    End Property
    Private _PlusPropValue As String
    Public ReadOnly Property PlusPropValue() As String
        Get
            Return _PlusPropValue
        End Get
    End Property
    Private _CloseChecklist As Boolean
    Public ReadOnly Property CloseChecklist() As Boolean
        Get
            Return _CloseChecklist
        End Get
    End Property
    Private _XCLPointToAdd As XElement
    Public ReadOnly Property XCLPointToAdd() As XElement
        Get
            Return _XCLPointToAdd
        End Get
    End Property
    Private _XNewProps As List(Of XElement)
    Public ReadOnly Property XNewProps() As List(Of XElement)
        Get
            Return _XNewProps
        End Get
    End Property
    Private _CLsToLoad As List(Of String)
    Public ReadOnly Property CLsToLoad() As List(Of String)
        Get
            Return _CLsToLoad
        End Get
    End Property



    Public Sub New(XEntry As XElement, CLPool As ChecklistPool)
        InitializeComponent()
        _XEntry = XEntry
        _CLPool = CLPool
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        If _XEntry Is Nothing OrElse _XEntry.Elements.Count = 0 Then Throw New ArgumentException("ConfirmDialog _XEntry")

        Me.Title = "Aktion bestätigen: " + _XEntry.<label>.Value
        If String.IsNullOrEmpty(_XEntry.<journaltext>.Value) Then
            TBlLabel.Visibility = Windows.Visibility.Collapsed
        Else
            TBlLabel.Text = _XEntry.<journaltext>.Value
        End If

        If _XEntry.@input = "True" Then
            If String.IsNullOrEmpty(_XEntry.<prompt>.Value) Then
                TBlComment.Text = "Kommentar"
            Else
                TBlComment.Text = _XEntry.<prompt>.Value
            End If
        Else
            DPComment.Visibility = Windows.Visibility.Collapsed
        End If
        If _XEntry.@final = "False" Then
            TBlFinal.Visibility = Windows.Visibility.Collapsed
            BorderContinue.Visibility = Windows.Visibility.Collapsed
        Else
            Dim XContinue As XElement = _XEntry.<continue>.FirstOrDefault
            If XContinue IsNot Nothing Then
                BorderContinue.DataContext = XContinue
                Dim CheckListIDList As List(Of String) = (From xe As XElement In XContinue.Elements Select xe.@id).ToList
                Dim XCheckListList As New List(Of XElement)
                For Each CL As KeyValuePair(Of String, XElement) In _CLPool.Pool
                    If CheckListIDList.Contains(CL.Key) Then _
                        XCheckListList.Add(<cl id=<%= CL.Key %> IsChecked="False"><%= CL.Value.@lb %></cl>)
                Next

                ICContinue.ItemsSource = XCheckListList
            End If
            If XContinue Is Nothing OrElse XContinue.@c = "False" OrElse XContinue.Elements.Count < 2 Then BorderContinue.Visibility = Windows.Visibility.Collapsed
        End If

        Dim PlusPropKey As String = _XEntry.@plusprop
        If String.IsNullOrEmpty(PlusPropKey) Then PlusPropKey = _XEntry.@status
        If String.IsNullOrEmpty(PlusPropKey) OrElse _CLPool.PlusPropValues Is Nothing OrElse Not _CLPool.PlusPropValues.ContainsKey(PlusPropKey) Then
            TBPlusProp.Visibility = Windows.Visibility.Collapsed
        Else
            TBPlusProp.Text = "Achtung: Diese Bestätigung setzt das Merkmal '" + _CLPool.PlusPropLabel + "' auf den Wert '" + _CLPool.PlusPropValues.Item(PlusPropKey) + "'."
        End If

        If String.IsNullOrEmpty(_XEntry.@prop) Then
            MDLC.Visibility = Windows.Visibility.Collapsed
        Else
            MDLC.XMDList = <MDL/>
            Dim XDefaultMDList As XElement = <XDefaultMDList/>
            For Each mddef As String In _XEntry.@prop.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                Dim mddefsplits As String() = mddef.Split({"##"}, StringSplitOptions.RemoveEmptyEntries)
                If mddefsplits.Count = 2 Then
                    XDefaultMDList.Add(<MD cat=<%= mddefsplits(0) %> def=<%= mddefsplits(1) %>/>)
                End If
            Next
            MDLC.XDefaultMDList = XDefaultMDList
        End If
    End Sub

    Private Sub BtnCancel_Click() Handles BtnCancel.Click
        DialogResult = False
    End Sub

    Private Sub BtnConfirm_Click() Handles BtnConfirm.Click
        _JournalTitle = _XEntry.<label>.Value
        _JournalText = _XEntry.<journaltext>.Value
        If _XEntry.@input = "True" Then _JournalText += vbNewLine + TBComment.Text

        _PlusPropValue = _XEntry.@plusprop
        If String.IsNullOrEmpty(_PlusPropValue) Then _PlusPropValue = _XEntry.@status
        If Not String.IsNullOrEmpty(_PlusPropValue) AndAlso _CLPool.PlusPropValues IsNot Nothing AndAlso _CLPool.PlusPropValues.ContainsKey(_PlusPropValue) Then
            _JournalText += vbNewLine + _CLPool.PlusPropLabel + " auf '" + _CLPool.PlusPropValues.Item(_PlusPropValue) + "' gesetzt."
        End If

        _CloseChecklist = _XEntry.@final = "True"
        _XCLPointToAdd = <p id=<%= _XEntry.@id %> date=<%= Date.Now.ToShortDateString %>/>

        If Not String.IsNullOrEmpty(_XEntry.@prop) Then
            _XNewProps = MDLC.XMDList.Elements.ToList
        Else
            _XNewProps = Nothing
        End If

        Dim XContinue As XElement = BorderContinue.DataContext
        _CLsToLoad = New List(Of String)
        If XContinue IsNot Nothing Then
            Dim NoCheck As Boolean = XContinue.Elements.Count < 2 OrElse XContinue.@c = "False"
            For Each xe As XElement In ICContinue.Items
                If NoCheck OrElse xe.@IsChecked = "True" Then _CLsToLoad.Add(xe.@id)
            Next
        End If

        DialogResult = True
    End Sub
End Class
