using System;
using Common.MVVM;

namespace PharmacyApp.Models
{
    public class Payment : BindableModel
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

        private PaymentMethodEnum _Method;
        public PaymentMethodEnum Method
        {
            get => _Method;
            set { if (_Method != value) { _Method = value; OnPropertyChanged("Method"); } }
        }

        private decimal _AmountPaid;
        public decimal AmountPaid
        {
            get => _AmountPaid;
            set { if (_AmountPaid != value) { _AmountPaid = value; OnPropertyChanged("AmountPaid"); } }
        }

        private DateTime _PaymentDate;
        public DateTime PaymentDate
        {
            get => _PaymentDate;
            set { if (_PaymentDate != value) { _PaymentDate = value; OnPropertyChanged("PaymentDate"); } }
        }
    }
}
