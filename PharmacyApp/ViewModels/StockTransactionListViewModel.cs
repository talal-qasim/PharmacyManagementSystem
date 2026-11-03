using System;
using System.Collections.ObjectModel;
using Common.MVVM;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    /// <summary>
    /// Read-only — no Add/Edit/Delete commands, since StockTransaction rows are
    /// immutable audit records (Section 28) created only as a side effect of
    /// InventoryDAL operations. No Messenger registration either, for the same
    /// reason: nothing ever sends EntityMessage&lt;StockTransaction&gt;.
    /// </summary>
    public class StockTransactionListViewModel : BindableViewModel
    {
        #region Constructor

        public StockTransactionListViewModel()
        {
            try
            {
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<StockTransaction> _StockTransactions;
        public ObservableCollection<StockTransaction> StockTransactions
        {
            get => _StockTransactions;
            set
            {
                if (_StockTransactions != value)
                {
                    _StockTransactions = value;
                    OnPropertyChanged("StockTransactions");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadList()
        {
            try
            {
                var result = await StockTransactionDAL.GetStockTransactions();
                StockTransactions = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion
    }
}