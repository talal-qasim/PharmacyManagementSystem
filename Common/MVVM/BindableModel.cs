namespace Common.MVVM
{
    /// <summary>
    /// Base class for every domain entity model (Models/[Entity].cs).
    /// Entities inherit this instead of implementing INotifyPropertyChanged directly,
    /// so that entities bound to editor forms (e.g. a Medicine being edited in a modal)
    /// push live updates to the UI as fields change.
    /// </summary>
    public abstract class BindableModel : BindableBase
    {
    }
}
