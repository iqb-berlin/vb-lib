Imports System.Data.Entity.Core.Objects
Imports System.Data.Entity.Core.Objects.DataClasses

Public Interface ISyncEntity
    ''' <summary>
    ''' Fires if the Entity was updated 
    ''' </summary>
    ''' <remarks></remarks>
    Event Updated()
    ''' <summary>
    ''' Fires, if db-data differ from local data
    ''' </summary>
    ''' <param name="DBEntity"></param>
    ''' <remarks></remarks>
    Event ChangesAvailable(ByVal DBEntity As EntityObject)
    ''' <summary>
    ''' Fires, if db-time differs to last sync time
    ''' </summary>
    ''' <param name="Propertyname"></param>
    ''' <remarks></remarks>
    Event DetectDBChanges(ByVal Propertyname As String)
    Sub ReportChangesAvailable(ByVal DBEntity As EntityObject)
    Sub ReportUpdated()
    Sub ReportDetectDBChanges(ByVal Propertyname As String)

End Interface
