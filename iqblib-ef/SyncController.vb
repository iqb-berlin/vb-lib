Imports System.Data.Entity.Core.Objects.DataClasses
Imports System.Data.Entity.Core.Objects
Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports System.ComponentModel
Imports System.Windows

Public Class SyncProperty
    Property SynchronizableEntity As SynchronizableEntity
    Property PropertyName As String
    Property NewValue As Object
    Property OldValue As Object
    Property PropertyInfo As System.Reflection.PropertyInfo
End Class

Public Class SynchronizableEntity
    Event Synced(ByRef SyncEntity As SynchronizableEntity)
    Property SyncController As ISyncController
    Sub New(ByRef entity As EntityObject, ByRef dbentity As EntityObject, ByRef SyncController As ISyncController)
        _e = New WeakReference(entity)
        _dbe = dbentity
        Me.SyncController = SyncController

        For Each p In Me.SyncController.PropertiesToSync
            Dim localValue As Object = p.GetValue(entity)
            Dim dbValue As Object = p.GetValue(dbentity)
            If localValue.GetHashCode <> dbValue.GetHashCode Then _cp.Add(New SyncProperty With {.SynchronizableEntity = Me, .PropertyName = p.Name, .PropertyInfo = p, .OldValue = p.GetValue(entity), .NewValue = p.GetValue(dbentity)})
        Next


    End Sub
    Private _e As WeakReference
    ReadOnly Property LocalEntity As EntityObject
        Get
            Return _e.Target
        End Get
    End Property
    Private _dbe As EntityObject
    ReadOnly Property DBEntity As EntityObject
        Get
            Return _dbe
        End Get
    End Property

    Private _cp As New ObservableCollection(Of SyncProperty)
    ReadOnly Property PropertiesToSync As ObservableCollection(Of SyncProperty)
        Get
            Return _cp
        End Get
    End Property

    Sub IgnoreDBData(ByVal SyncProperty As SyncProperty)
        Try
            _cp.Remove(SyncProperty)

        Catch ex As Exception

        End Try
    End Sub

    Sub ApplyDBData(ByVal SyncProperty As SyncProperty)
        Try
            If LocalEntity IsNot Nothing Then
                SyncProperty.PropertyInfo.SetValue(LocalEntity, SyncProperty.NewValue)
                _cp.Remove(SyncProperty)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Sub ApplyDBData()
        Try
            If LocalEntity IsNot Nothing Then
                SyncController.SynchronizeEntities(LocalEntity, DBEntity, System.Data.Entity.EntityState.Modified)
                RaiseEvent Synced(Me)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


End Class



Public Class SyncController
    Shared Property SyncContext As ObjectContext

    Private Shared timer As System.Threading.Timer

    Shared ReadOnly Property SynchronizableEntities As ObservableCollection(Of SynchronizableEntity)
        Get
            Return _ce
        End Get
    End Property

    Shared Sub AddObservingEntity(ByVal EntityObject As EntityObject, ByRef ObjectContext As ObjectContext)
        Try

            Dim s As ISyncController = (From ts In _tdic Where ts.Key = EntityObject.GetType().ToString Select ts.Value).SingleOrDefault
            If s IsNot Nothing Then

                s.ObservingEntities.Add(New WeakReference(EntityObject), New WeakReference(ObjectContext))

                Console.WriteLine("add to synccontrol" & EntityObject.EntityKey.ToString)
            Else
                Console.WriteLine("no synccontroller found " & EntityObject.GetType.ToString)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Shared Sub RemoveObservingEntity(ByVal EntityObject As EntityObject)
        Try
            Dim s As ISyncController = (From ts In _tdic Where ts.Key = EntityObject.GetType().ToString Select ts.Value).SingleOrDefault
            If s IsNot Nothing Then
                For Each weakreference In (From entry In s.ObservingEntities Where entry.Key.Target Is EntityObject Select entry.Key).ToList
                    s.ObservingEntities.Remove(weakreference)
                    Console.WriteLine("Remove entity " & EntityObject.EntityKey.ToString)
                Next

            Else
                Console.WriteLine("no synccontroller found " & EntityObject.GetType.ToString)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Shared Sub StartObserving(ByVal Intervall As UInteger)
        'Console.WriteLine("start sync thread")
        If timer IsNot Nothing Then timer.Dispose()
        timer = New System.Threading.Timer(AddressOf StartSync) ', New autoevent(False), 0, 1000)
        timer.Change(0, Intervall)
    End Sub


    Private Shared _tdic As New Dictionary(Of String, ISyncController)
    Shared Function RegisterType(Of T As EntityObject)() As ISyncController
        Try
            Dim s As New SyncController(Of T)
            TypedSyncController.Add(s)
            _tdic.Add(GetType(T).ToString, s)
            Return s
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Private Shared _ce As New ObservableCollection(Of SynchronizableEntity)
    Private Shared _tsc As New ObservableCollection(Of ISyncController)
    Shared ReadOnly Property TypedSyncController As ObservableCollection(Of ISyncController)
        Get
            Return _tsc
        End Get
    End Property
    Private Shared inaction As Boolean = False
    Shared Sub StartSync()
        Try
            If Not inaction Then
                Console.WriteLine("start sync thread ")
                inaction = True
                For Each k In TypedSyncController
                    Try
                        If k.ObservingEntities.Count > 0 Then
                            Console.WriteLine("start sync thread " & k.Type.ToString & k.ObservingEntities.Count)
                            k.Synchronize()
                        End If

                    Catch ex As Exception
                    End Try
                Next
                inaction = False
            Else
                Console.WriteLine("sync in action -> skip this run")
            End If

        Catch ex As Exception
            inaction = False
        End Try

    End Sub


End Class



Public Interface ISyncController
    Sub Synchronize()
    Property ObservingCriterias As List(Of String)
    Property ObservingEntities As Dictionary(Of WeakReference, WeakReference)

    ReadOnly Property Type As Type
    Property PropertiesToSync As List(Of System.Reflection.PropertyInfo)

    Sub SynchronizeEntities(ByRef local As EntityObject, ByRef db As EntityObject, ByVal newState As System.Data.Entity.EntityState)

End Interface
Public Class SyncController(Of T As EntityObject)
    Implements ISyncController

    Property PropertiesToSync As List(Of System.Reflection.PropertyInfo) = New List(Of System.Reflection.PropertyInfo) Implements ISyncController.PropertiesToSync
    Property ObservingCriterias As List(Of String) = New List(Of String) Implements ISyncController.ObservingCriterias
    Property ObservingEntities As Dictionary(Of WeakReference, WeakReference) = New Dictionary(Of WeakReference, WeakReference) Implements ISyncController.ObservingEntities

    Sub New()
        Console.WriteLine("new synccontroller for " & GetType(T).Name)
        Dim PropertiesOfType As System.Reflection.PropertyInfo() = GetType(T).GetProperties()
        For Each propertyInfo As System.Reflection.PropertyInfo In PropertiesOfType
            'just Scalar Properties -->EdmScalarPropertyAttribute
            If propertyInfo.IsDefined(GetType(EdmScalarPropertyAttribute), False) Then
                PropertiesToSync.Add(propertyInfo)
            End If
        Next

    End Sub

    Sub SynchronizeEntities(ByRef localentity As EntityObject, ByRef dbentity As EntityObject, ByVal newState As System.Data.Entity.EntityState) Implements ISyncController.SynchronizeEntities
        Try

            For Each prop In PropertiesToSync
                prop.SetValue(localentity, prop.GetValue(dbentity))
            Next

            'Set unchanged
            Dim vallocalentity = localentity
            Dim context = (From entry In ObservingEntities Where entry.Key.Target Is vallocalentity Select entry.Value.Target).SingleOrDefault
            If context IsNot Nothing Then
                context.ObjectStateManager.ChangeObjectState(localentity, newState)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Private Shared entitycomparer As New EntityObjectComparer
    Sub checkReferences()
        Dim c As UInteger = 0
        For Each k In (From entry In ObservingEntities Where entry.Key.IsAlive = False OrElse entry.Value.IsAlive = False Select entry.Key).ToList
            ObservingEntities.Remove(k)
            c += 1
        Next
        Console.WriteLine("removed " & c & " entries from synccontrol of " & Type.ToString)
    End Sub

    Private lastsynctimes As New Dictionary(Of String, Date)
    Sub Synchronize() Implements ISyncController.Synchronize
        Try

            '
            'Get entityobjects where referenced
            Dim currentObservingEntities = (From entry In ObservingEntities Where entry.Key.IsAlive AndAlso entry.Value.IsAlive Select CType(entry.Key.Target, EntityObject)).ToList
            '###########Console.WriteLine("sync " & Type.ToString & " " & currentObservingEntities.Count)
            '  Dim ContextType As Type = SyncController.SyncContext.GetType
            'USING entitycomparer to check equality, because different objectcontext
            For Each criteria In ObservingCriterias
                Dim LastSyncTime As Date
                If lastsynctimes.ContainsKey(criteria) Then
                    LastSyncTime = lastsynctimes(criteria)
                Else
                    LastSyncTime = Date.Now
                    lastsynctimes.Add(criteria, LastSyncTime)
                End If
                'get db-changed data maybe index for lastchange in db ??
                '###########Console.WriteLine("get sync for lastchange since : " & LastSyncTime)
                Dim start As Date = Date.Now
                Dim wcondition = "it." & criteria & " > @timestamp"
                Dim param As New ObjectParameter("timestamp", LastSyncTime)
                Dim db_entities = SyncController.SyncContext.CreateObjectSet(Of T).Where(wcondition, param).Execute(MergeOption.NoTracking).ToList

                '###########Console.WriteLine("find dbchanged:" & db_entities.Count & " in ms " & Date.Now.Subtract(start).TotalMilliseconds)

                '1.) entities where changed in db and not changed local 
                Dim localunchangedEntities = (From dbentity In db_entities Join localentity In currentObservingEntities.ToList On localentity.EntityKey.GetHashCode Equals dbentity.EntityKey.GetHashCode Where localentity.EntityState = System.Data.Entity.EntityState.Unchanged Select local = localentity, db = dbentity).ToList
                'simple overwrite local-data with db-data and set unchanged

                '###########Console.WriteLine("Update unchanged Entities : " & localunchangedEntities.Count)

                For Each EntityToSync In localunchangedEntities.AsParallel
                    '  Console.WriteLine("find unchanged entity for dbentity")
                    SynchronizeEntities(EntityToSync.local, EntityToSync.db, System.Data.Entity.EntityState.Unchanged)
                    'Report Update and DetectedDBChanges
                    If TypeOf (EntityToSync.local) Is ISyncEntity Then
                        Application.Current.Dispatcher.Invoke(New Action(Sub()
                                                                             CType(EntityToSync.local, ISyncEntity).ReportDetectDBChanges(criteria)
                                                                             CType(EntityToSync.local, ISyncEntity).ReportUpdated()
                                                                         End Sub))
                    End If
                Next

                '2.) entities where changed in db and local 
                Dim localchangedEntities = (From dbentity In db_entities Join localentity In currentObservingEntities.ToList On localentity.EntityKey.GetHashCode Equals dbentity.EntityKey.GetHashCode Where localentity.EntityState = System.Data.Entity.EntityState.Modified Select local = localentity, db = dbentity).ToList
                '###########Console.WriteLine("localchanged:" & localchangedEntities.Count)

                For Each entitysync In localchangedEntities
                    'Report Update and DetectedDBChanges
                    If TypeOf (entitysync.local) Is ISyncEntity Then
                        Application.Current.Dispatcher.Invoke(New Action(Sub()
                                                                             CType(entitysync.local, ISyncEntity).ReportDetectDBChanges(criteria)
                                                                         End Sub))
                    End If

                    Dim SynchronizableEntity As New SynchronizableEntity(entitysync.local, entitysync.db, Me)
                    'Remove existing SynchronizableEntities in AppThread
                    SyncController.SynchronizableEntities.ToList.Where(Function(k) k.LocalEntity.GetType Is GetType(T) AndAlso k.LocalEntity Is entitysync.local).ToList.ForEach(New Action(Of SynchronizableEntity)(Sub(element) Application.Current.Dispatcher.Invoke(New Action(Sub() SyncController.SynchronizableEntities.Remove(element)))))
                    'Add new SynchronizableEntitiy in AppThread
                    Application.Current.Dispatcher.Invoke(New Action(Sub() SyncController.SynchronizableEntities.Add(SynchronizableEntity)))

                    AddHandler SynchronizableEntity.Synced, Function(ByRef s As SynchronizableEntity) SyncController.SynchronizableEntities.Remove(s)
                Next
                Dim lastsyncProp As System.Reflection.PropertyInfo = GetType(T).GetProperty(criteria)
                If db_entities.Count > 0 Then
                    lastsynctimes(criteria) = db_entities.Max(Of Date)(Function(e) lastsyncProp.GetValue(e))
                End If
            Next
            checkReferences()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public ReadOnly Property Type As Type Implements ISyncController.Type
        Get
            Return GetType(T)
        End Get
    End Property
End Class

Public Class EntityObjectComparer
    Implements IEqualityComparer(Of EntityObject)


    Public Function Equals1(x As EntityObject, y As EntityObject) As Boolean Implements IEqualityComparer(Of EntityObject).Equals
        Return x.EntityKey.GetHashCode = y.EntityKey.GetHashCode
    End Function

    Public Function GetHashCode1(obj As EntityObject) As Integer Implements IEqualityComparer(Of EntityObject).GetHashCode
        Return obj.EntityKey.GetHashCode
    End Function
End Class
