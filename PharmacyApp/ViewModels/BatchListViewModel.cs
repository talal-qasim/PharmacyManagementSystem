using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.Views;

namespace PharmacyApp.ViewModels
{
    public class BatchListViewModel : BindableViewModel
    {
        #region Constructor

        public BatchListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Batch>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<Batch> _Batches;
        public ObservableCollection<Batch> Batches
        {
            get => _Batches;
            set
            {
                if (_Batches != value)
                {
                    _Batches = value;
                    OnPropertyChanged("Batches");
                }
            }
        }

        private Batch _SelectedBatch;
        public Batch SelectedBatch
        {
            get => _SelectedBatch;
            set
            {
                if (_SelectedBatch != value)
                {
                    _SelectedBatch = value;
                    OnPropertyChanged("SelectedBatch");
                    IsRowSelected = _SelectedBatch != null;
                }
            }
        }

        private bool _IsRowSelected;
        public bool IsRowSelected
        {
            get => _IsRowSelected;
            set
            {
                if (_IsRowSelected != value)
                {
                    _IsRowSelected = value;
                    OnPropertyChanged("IsRowSelected");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadList()
        {
            try
            {
                var result = await BatchDAL.GetBatches();
                Batches = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        /// <summary>
        /// Re-fetches one batch and pushes it as a Changed message. Used after a
        /// successful stock adjustment, since InventoryDAL.AdjustStock doesn't
        /// return the updated entity itself (it returns bool by design).
        /// </summary>
        private async System.Threading.Tasks.Task RefreshBatch(int batchId)
        {
            var refreshed = await BatchDAL.GetBatchById(batchId);
            if (refreshed != null)
            {
                Messenger.Default.Send(new EntityMessage<Batch>(refreshed, EntityMessageType.Changed));
            }
        }

        #endregion

        #region Commands

        RelayCommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new RelayCommand(p => this.ExecuteAddCommand(p));
                }
                return _AddCommand;
            }
        }

        RelayCommand _UpdateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null)
                {
                    _UpdateCommand = new RelayCommand(async p => await ExecuteUpdateCommand(p), p => IsRowSelected);
                }
                return _UpdateCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(p), p => IsRowSelected);
                }
                return _DeleteCommand;
            }
        }

        RelayCommand _AdjustStockCommand;
        public ICommand AdjustStockCommand
        {
            get
            {
                if (_AdjustStockCommand == null)
                {
                    _AdjustStockCommand = new RelayCommand(async p => await ExecuteAdjustStockCommand(p), p => IsRowSelected);
                }
                return _AdjustStockCommand;
            }
        }

        #endregion

        #region Command Methods

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new BatchViewModel
                {
                    Batch = new Batch
                    {
                        ReceivedDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddYears(1)
                    }
                };

                var view = new BatchView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new BatchViewModel
                {
                    Batch = await BatchDAL.GetBatchById(SelectedBatch.Id)
                };

                var view = new BatchView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteDeleteCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Delete Batch",
                    $"Are you sure you want to delete batch '{SelectedBatch.BatchNumber}'? Its stock transaction history is preserved.",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await BatchDAL.Delete(SelectedBatch);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<Batch>(SelectedBatch, EntityMessageType.Deleted));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteAdjustStockCommand(object parameter)
        {
            try
            {
                var viewModel = new StockAdjustmentViewModel(SelectedBatch);

                var view = new StockAdjustmentView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();

                if (viewModel.CloseDialog == true)
                {
                    await RefreshBatch(SelectedBatch.Id);
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Messenger Handler

        void OnMessage(EntityMessage<Batch> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Batches.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Batches.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Batches.IndexOf(found);
                            Batches.Remove(found);
                            Batches.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Batches.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Batches.Remove(found);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}