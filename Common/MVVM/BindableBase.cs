using System.ComponentModel;

namespace Common.MVVM
{
    /// <summary>
    /// Base class providing INotifyPropertyChanged for both Models and ViewModels.
    /// All property setters across the project raise change notification through
    /// this class's OnPropertyChanged(string) method using explicit backing fields.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises PropertyChanged for the given property name.
        /// Called explicitly from every property setter (no [CallerMemberName] convenience,
        /// per rulebook Rule 6.2 — explicit string property names are required).
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
