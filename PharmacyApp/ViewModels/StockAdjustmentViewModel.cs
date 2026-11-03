using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.MVVM;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class StockAdjustmentViewModel : BindableViewModel
    {
        public StockAdjustmentViewModel(Batch batch)
        {
            Batch = batch;
            AllowedTransactionTypes = new List<StockTransactionTypeEnum>
            {
                StockTransactionTypeEnum.Adjustment,
                StockTransactionTypeEnum.Damage,
                StockTransactionTypeEnum.Expiry,
                StockTransactionTypeEnum.Return
            };
            SelectedTransactionType = StockTransactionTypeEnum.Adjustment;
        }

        private Batch _Batch;
        public Batch Batch
        {
            get => _Batch;
            set { if (_Batch != value) { _Batch = value; OnPropertyChanged("Batch"); } }
        }

        private IEnumerable<StockTransactionTypeEnum> _AllowedTransactionTypes;
        public IEnumerable<StockTransactionTypeEnum> AllowedTransactionTypes
        {
            get => _AllowedTransactionTypes;
            set { if (_AllowedTransactionTypes != value) { _AllowedTransactionTypes = value; OnPropertyChanged("AllowedTransactionTypes"); } }
        }

        private StockTransactionTypeEnum _SelectedTransactionType;
        public StockTransactionTypeEnum SelectedTransactionType
        {
            get => _SelectedTransactionType;
            set { if (_SelectedTransactionType != value) { _SelectedTransactionType = value; OnPropertyChanged("SelectedTransactionType"); } }
        }

        private int _QuantityChange;
        public int QuantityChange
        {
            get => _QuantityChange;
            set 
            { 
                if (_QuantityChange != value) 
                { 
                    _QuantityChange = value; 
                    OnPropertyChanged("QuantityChange"); 
                    OnPropertyChanged("ResultingQuantity");
                } 
            }
        }

        public int ResultingQuantity => (Batch?.Quantity ?? 0) + QuantityChange;

        private string _Reason;
        public string Reason
        {
            get => _Reason;
            set { if (_Reason != value) { _Reason = value; OnPropertyChanged("Reason"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        RelayCommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new RelayCommand(async p => await ExecuteSaveCommand(p));
                return _SaveCommand;
            }
        }

        RelayCommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new RelayCommand(p => ExecuteCancelCommand(p));
                return _CancelCommand;
            }
        }

        private async Task ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (QuantityChange == 0)
                {
                    ValidationMessage = "Quantity change cannot be zero.";
                    return;
                }
                if (ResultingQuantity < 0)
                {
                    ValidationMessage = "Resulting quantity cannot be less than zero.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(Reason))
                {
                    ValidationMessage = "A reason must be provided.";
                    return;
                }

                bool result = await InventoryDAL.AdjustStock(
                    Batch.Id, 
                    QuantityChange, 
                    SelectedTransactionType, 
                    Reason
                );
                
                if (result)
                {
                    CloseDialog = true;
                }
                else
                {
                    ValidationMessage = "Failed to adjust stock. Check if the resulting quantity would be negative.";
                }
            }
            catch (Exception ex)
            {
                ValidationMessage = ex.Message;
            }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            CloseDialog = false;
        }
    }
}
