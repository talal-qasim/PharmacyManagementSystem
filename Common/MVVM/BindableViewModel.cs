namespace Common.MVVM
{
    /// <summary>
    /// Base class for every ViewModel (Editor ViewModels and List ViewModels).
    ///
    /// NOTE ON NAMESPACE: the original rulebook text referenced
    /// "SentinalPOS.MVVM.BindableViewModel" — that was the namespace of a prior
    /// project this rulebook was extracted from. For this project (PharmacyApp)
    /// the equivalent class lives here, in Common.MVVM, alongside BindableModel,
    /// so both Models and ViewModels share one base-class library as the rulebook's
    /// own Rule 6.1 intends ("built-in MVVM abstractions in Common.MVVM").
    /// </summary>
    public abstract class BindableViewModel : BindableBase
    {
        /// <summary>
        /// Bound to med:DialogCloser.DialogResult in modal Views.
        /// Setting this to true/false from a command (e.g. Save, Cancel)
        /// closes the modal Window automatically.
        /// </summary>
        private bool? _CloseDialog;
        public bool? CloseDialog
        {
            get => _CloseDialog;
            set
            {
                if (_CloseDialog != value)
                {
                    _CloseDialog = value;
                    OnPropertyChanged("CloseDialog");
                }
            }
        }
    }
}
