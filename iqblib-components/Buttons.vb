Imports System.Windows.Controls.Primitives

Public Class ButtonNew
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonNew), New FrameworkPropertyMetadata(GetType(ButtonNew)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.[New]

    End Sub

End Class

Public Class ButtonNewSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonNewSmall), New FrameworkPropertyMetadata(GetType(ButtonNewSmall)))
    End Sub

End Class

Public Class ButtonNewDocument
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonNewDocument), New FrameworkPropertyMetadata(GetType(ButtonNewDocument)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.[New]

    End Sub
End Class

Public Class ButtonDownload
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonDownload), New FrameworkPropertyMetadata(GetType(ButtonDownload)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Download

    End Sub

End Class

Public Class ButtonNewNiceAttached
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonNewNiceAttached), New FrameworkPropertyMetadata(GetType(ButtonNewNiceAttached)))
    End Sub
End Class

Public Class ButtonDelete
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonDelete), New FrameworkPropertyMetadata(GetType(ButtonDelete)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Delete

    End Sub

End Class

Public Class ButtonDeleteSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonDeleteSmall), New FrameworkPropertyMetadata(GetType(ButtonDeleteSmall)))
    End Sub

End Class

Public Class ButtonCopySmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonCopySmall), New FrameworkPropertyMetadata(GetType(ButtonCopySmall)))
    End Sub

End Class

Public Class ButtonProperties
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonProperties), New FrameworkPropertyMetadata(GetType(ButtonProperties)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Properties

    End Sub

End Class

Public Class ButtonReport
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonReport), New FrameworkPropertyMetadata(GetType(ButtonReport)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Report

    End Sub

End Class

Public Class ButtonEdit
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonEdit), New FrameworkPropertyMetadata(GetType(ButtonEdit)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.EditObject

    End Sub

End Class

Public Class ButtonEditSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonEditSmall), New FrameworkPropertyMetadata(GetType(ButtonEditSmall)))
    End Sub

End Class

Public Class ButtonOpen
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonOpen), New FrameworkPropertyMetadata(GetType(ButtonOpen)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Open

    End Sub

End Class

Public Class ButtonSave
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonSave), New FrameworkPropertyMetadata(GetType(ButtonSave)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Save

    End Sub

End Class

Public Class ButtonSaveSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonSaveSmall), New FrameworkPropertyMetadata(GetType(ButtonSaveSmall)))
    End Sub

End Class

Public Class ButtonHelp
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonHelp), New FrameworkPropertyMetadata(GetType(ButtonHelp)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Help

    End Sub

End Class

Public Class ButtonHelpSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonHelpSmall), New FrameworkPropertyMetadata(GetType(ButtonHelpSmall)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Help

    End Sub

End Class

Public Class ButtonReload
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonReload), New FrameworkPropertyMetadata(GetType(ButtonReload)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.ReloadObject

    End Sub

End Class
Public Class ButtonOptions
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonOptions), New FrameworkPropertyMetadata(GetType(ButtonOptions)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Options

    End Sub

End Class
Public Class ButtonOptionsSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonOptionsSmall), New FrameworkPropertyMetadata(GetType(ButtonOptionsSmall)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Options

    End Sub

End Class
Public Class ButtonUnDo
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonUnDo), New FrameworkPropertyMetadata(GetType(ButtonUnDo)))
    End Sub

    Sub New()
        Me.Command = ApplicationCommands.Undo

    End Sub

End Class
Public Class ButtonFilter
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonFilter), New FrameworkPropertyMetadata(GetType(ButtonFilter)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Filter

    End Sub

End Class

Public Class ButtonFilterRemove
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonFilterRemove), New FrameworkPropertyMetadata(GetType(ButtonFilterRemove)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.FilterRemove

    End Sub

End Class

Public Class ButtonTable
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonTable), New FrameworkPropertyMetadata(GetType(ButtonTable)))
    End Sub

    Sub New()
        Me.Command = IQBCommands.Table

    End Sub

End Class

Public Class ButtonTableSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonTableSmall), New FrameworkPropertyMetadata(GetType(ButtonTableSmall)))
    End Sub

End Class

Public Class ButtonUpArrowSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonUpArrowSmall), New FrameworkPropertyMetadata(GetType(ButtonUpArrowSmall)))
    End Sub

End Class

Public Class ButtonDownArrowSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonDownArrowSmall), New FrameworkPropertyMetadata(GetType(ButtonDownArrowSmall)))
    End Sub

End Class

Public Class ButtonRightArrowSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonRightArrowSmall), New FrameworkPropertyMetadata(GetType(ButtonRightArrowSmall)))
    End Sub

End Class

Public Class ButtonLeftArrowSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonLeftArrowSmall), New FrameworkPropertyMetadata(GetType(ButtonLeftArrowSmall)))
    End Sub

End Class

Public Class ButtonSearchOrZoomSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonSearchOrZoomSmall), New FrameworkPropertyMetadata(GetType(ButtonSearchOrZoomSmall)))
    End Sub

End Class

Public Class ButtonView
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonView), New FrameworkPropertyMetadata(GetType(ButtonView)))
    End Sub
End Class

Public Class ButtonViewSmall
    Inherits System.Windows.Controls.Button

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ButtonViewSmall), New FrameworkPropertyMetadata(GetType(ButtonViewSmall)))
    End Sub

End Class

