namespace PharmacyApp.Models
{
    /// <summary>
    /// The three lifecycle events a List ViewModel needs to react to when an
    /// Editor ViewModel elsewhere in the app inserts, updates, or soft-deletes an entity.
    /// </summary>
    public enum EntityMessageType
    {
        Added,
        Changed,
        Deleted
    }

    /// <summary>
    /// Generic messaging envelope sent via GalaSoft.MvvmLight.Messaging.Messenger.Default
    /// (per rulebook Rule 6.5). Editor ViewModels send these after a successful DAL
    /// Insert/Update/Delete; List ViewModels register for EntityMessage&lt;T&gt; of the
    /// entity type they display and update their ObservableCollection in OnMessage(...).
    /// </summary>
    public class EntityMessage<T>
    {
        public T Entity { get; }
        public EntityMessageType MessageType { get; }

        public EntityMessage(T entity, EntityMessageType messageType)
        {
            Entity = entity;
            MessageType = messageType;
        }
    }
}
