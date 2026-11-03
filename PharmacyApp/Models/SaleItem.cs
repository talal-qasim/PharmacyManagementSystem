using Common.MVVM;

namespace PharmacyApp.Models
{
    public class SaleItem : BindableModel
    {
        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private int _SaleId;
        public int SaleId
        {
            get => _SaleId;
            set { if (_SaleId != value) { _SaleId = value; OnPropertyChanged("SaleId"); } }
        }

        private int _ProductVariantId;
        public int ProductVariantId
        {
            get => _ProductVariantId;
            set { if (_ProductVariantId != value) { _ProductVariantId = value; OnPropertyChanged("ProductVariantId"); } }
        }

        private ProductVariant _ProductVariant;
        public ProductVariant ProductVariant
        {
            get => _ProductVariant;
            set 
            { 
                if (_ProductVariant != value) 
                { 
                    _ProductVariant = value; 
                    OnPropertyChanged("ProductVariant"); 
                } 
            }
        }

        private int _Quantity;
        public int Quantity
        {
            get => _Quantity;
            set 
            { 
                if (_Quantity != value) 
                { 
                    _Quantity = value; 
                    OnPropertyChanged("Quantity"); 
                    OnPropertyChanged("LineTotal");
                } 
            }
        }

        private decimal _UnitPriceAtSale;
        public decimal UnitPriceAtSale
        {
            get => _UnitPriceAtSale;
            set 
            { 
                if (_UnitPriceAtSale != value) 
                { 
                    _UnitPriceAtSale = value; 
                    OnPropertyChanged("UnitPriceAtSale"); 
                    OnPropertyChanged("LineTotal");
                } 
            }
        }

        public decimal LineTotal => Quantity * UnitPriceAtSale;
    }
}
