using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class SaleCartViewModel : BindableViewModel
    {
        private ObservableCollection<ProductVariant> _allVariants;

        public SaleCartViewModel()
        {
            CartItems = new ObservableCollection<SaleItem>();
            CartItems.CollectionChanged += (s, e) => {
                OnPropertyChanged("Subtotal");
                OnPropertyChanged("Total");
                OnPropertyChanged("RequiresPrescription");
            };
            LoadLookups();
        }

        private async void LoadLookups()
        {
            var variants = await ProductVariantDAL.GetAllProductVariants();
            _allVariants = variants.ToObservableCollection();
            FilterSearchResults();
        }

        private string _SearchText;
        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    OnPropertyChanged("SearchText");
                    FilterSearchResults();
                }
            }
        }

        private void FilterSearchResults()
        {
            if (_allVariants == null) return;
            
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults = _allVariants;
            }
            else
            {
                var lowerQuery = SearchText.ToLower();
                var filtered = _allVariants.Where(v => 
                    (v.Medicine != null && v.Medicine.Name.ToLower().Contains(lowerQuery)) ||
                    (v.Medicine != null && !string.IsNullOrEmpty(v.Medicine.GenericName) && v.Medicine.GenericName.ToLower().Contains(lowerQuery)) ||
                    (!string.IsNullOrEmpty(v.Barcode) && v.Barcode.ToLower().Contains(lowerQuery))
                );
                SearchResults = new ObservableCollection<ProductVariant>(filtered);
            }
        }

        private ObservableCollection<ProductVariant> _SearchResults;
        public ObservableCollection<ProductVariant> SearchResults
        {
            get => _SearchResults;
            set { if (_SearchResults != value) { _SearchResults = value; OnPropertyChanged("SearchResults"); } }
        }

        private ObservableCollection<SaleItem> _CartItems;
        public ObservableCollection<SaleItem> CartItems
        {
            get => _CartItems;
            set 
            { 
                if (_CartItems != value) 
                { 
                    _CartItems = value; 
                    OnPropertyChanged("CartItems");
                    OnPropertyChanged("Subtotal");
                    OnPropertyChanged("Total");
                    OnPropertyChanged("RequiresPrescription");
                } 
            }
        }

        public decimal Subtotal => CartItems == null ? 0 : CartItems.Sum(x => x.LineTotal);
        
        public decimal Total => Subtotal - DiscountAmount;

        public bool RequiresPrescription => CartItems != null && CartItems.Any(x => x.ProductVariant != null && x.ProductVariant.Medicine != null && x.ProductVariant.Medicine.RequiresPrescription);

        private decimal _DiscountAmount;
        public decimal DiscountAmount
        {
            get => _DiscountAmount;
            set 
            { 
                if (_DiscountAmount != value) 
                { 
                    _DiscountAmount = value; 
                    OnPropertyChanged("DiscountAmount"); 
                    OnPropertyChanged("Total");
                } 
            }
        }

        private string _PrescriptionReference;
        public string PrescriptionReference
        {
            get => _PrescriptionReference;
            set { if (_PrescriptionReference != value) { _PrescriptionReference = value; OnPropertyChanged("PrescriptionReference"); } }
        }

        private PaymentMethodEnum _SelectedPaymentMethod;
        public PaymentMethodEnum SelectedPaymentMethod
        {
            get => _SelectedPaymentMethod;
            set { if (_SelectedPaymentMethod != value) { _SelectedPaymentMethod = value; OnPropertyChanged("SelectedPaymentMethod"); } }
        }

        private decimal _AmountPaid;
        public decimal AmountPaid
        {
            get => _AmountPaid;
            set { if (_AmountPaid != value) { _AmountPaid = value; OnPropertyChanged("AmountPaid"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        RelayCommand _AddToCartCommand;
        public ICommand AddToCartCommand
        {
            get
            {
                if (_AddToCartCommand == null)
                    _AddToCartCommand = new RelayCommand(async p => await ExecuteAddToCartCommand(p));
                return _AddToCartCommand;
            }
        }

        RelayCommand _RemoveFromCartCommand;
        public ICommand RemoveFromCartCommand
        {
            get
            {
                if (_RemoveFromCartCommand == null)
                    _RemoveFromCartCommand = new RelayCommand(p => ExecuteRemoveFromCartCommand(p));
                return _RemoveFromCartCommand;
            }
        }

        RelayCommand _CheckoutCommand;
        public ICommand CheckoutCommand
        {
            get
            {
                if (_CheckoutCommand == null)
                    _CheckoutCommand = new RelayCommand(async p => await ExecuteCheckoutCommand(p));
                return _CheckoutCommand;
            }
        }

        public void RefreshTotals()
        {
            OnPropertyChanged("Subtotal");
            OnPropertyChanged("Total");
        }

        private async Task ExecuteAddToCartCommand(object parameter)
        {
            if (parameter is ProductVariant variant)
            {
                int qty = 1; // Default to 1
                int availableStock = await InventoryDAL.GetAvailableStock(variant.Id);
                
                var existingItem = CartItems.FirstOrDefault(x => x.ProductVariantId == variant.Id);
                int projectedTotal = (existingItem?.Quantity ?? 0) + qty;

                if (projectedTotal > availableStock)
                {
                    CustomMessageBoxView.Show("Insufficient Stock", $"Cannot add {variant.DisplayName}. Only {availableStock} in stock.", MessageBoxIconType.Warning, MessageBoxButtonsType.Ok);
                    return;
                }

                if (existingItem != null)
                {
                    existingItem.Quantity = projectedTotal;
                }
                else
                {
                    var newItem = new SaleItem
                    {
                        ProductVariantId = variant.Id,
                        ProductVariant = variant,
                        Quantity = qty,
                        UnitPriceAtSale = variant.SellingPrice
                    };
                    // Listen to quantity changes to update Subtotal
                    newItem.PropertyChanged += (s, e) => {
                        if (e.PropertyName == "Quantity" || e.PropertyName == "LineTotal")
                            RefreshTotals();
                    };
                    CartItems.Add(newItem);
                }
                ValidationMessage = "";
            }
        }

        private void ExecuteRemoveFromCartCommand(object parameter)
        {
            if (parameter is SaleItem item)
            {
                CartItems.Remove(item);
                RefreshTotals();
            }
        }

        private async Task ExecuteCheckoutCommand(object parameter)
        {
            if (CartItems.Count == 0)
            {
                ValidationMessage = "Cart is empty.";
                return;
            }

            if (RequiresPrescription && string.IsNullOrWhiteSpace(PrescriptionReference))
            {
                ValidationMessage = "A prescription reference is required for one or more items in the cart.";
                return;
            }

            if (AmountPaid < Total)
            {
                ValidationMessage = "Amount paid cannot be less than the total.";
                return;
            }

            // Double check stock right before inserting to be absolutely safe
            foreach (var item in CartItems)
            {
                int available = await InventoryDAL.GetAvailableStock(item.ProductVariantId);
                if (item.Quantity > available)
                {
                    ValidationMessage = $"Insufficient stock for {item.ProductVariant.DisplayName}. Available: {available}";
                    return;
                }
            }

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                DiscountAmount = DiscountAmount,
                PrescriptionReference = PrescriptionReference,
                IsDeleted = false
            };

            var insertedSale = await SaleDAL.Insert(sale);
            if (insertedSale == null)
            {
                ValidationMessage = "Failed to create sale header.";
                return;
            }

            foreach (var item in CartItems)
            {
                item.SaleId = insertedSale.Id;
                item.ProductVariant = null; // Detach before saving to avoid duplicate keys in EF
                await SaleItemDAL.Insert(item);
            }

            bool finalized = await SalesTransactionDAL.FinalizeSale(insertedSale.Id, SelectedPaymentMethod, AmountPaid);

            if (finalized)
            {
                var completeSale = await SaleDAL.GetSaleById(insertedSale.Id);
                Messenger.Default.Send(new EntityMessage<Sale>(completeSale, EntityMessageType.Added));
                
                CustomMessageBoxView.Show("Sale Complete", $"Sale #{insertedSale.Id} completed successfully.", MessageBoxIconType.Success, MessageBoxButtonsType.Ok);
                
                // Clear cart for next sale
                CartItems.Clear();
                DiscountAmount = 0;
                AmountPaid = 0;
                PrescriptionReference = string.Empty;
                ValidationMessage = string.Empty;
                SearchText = string.Empty;
            }
            else
            {
                ValidationMessage = "Sale Finalization failed due to insufficient stock or concurrent update.";
            }
        }
    }
}
