using System;
using System.Collections.Generic;
using System.Linq;
using Common.MVVM;

namespace PharmacyApp.Models
{
    public class Sale : BindableModel
    {
        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private DateTime _SaleDate;
        public DateTime SaleDate
        {
            get => _SaleDate;
            set { if (_SaleDate != value) { _SaleDate = value; OnPropertyChanged("SaleDate"); } }
        }

        private int? _CustomerId;
        public int? CustomerId
        {
            get => _CustomerId;
            set { if (_CustomerId != value) { _CustomerId = value; OnPropertyChanged("CustomerId"); } }
        }

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
                    OnPropertyChanged("TotalAmount");
                } 
            }
        }

        private SaleStatusEnum _Status;
        public SaleStatusEnum Status
        {
            get => _Status;
            set { if (_Status != value) { _Status = value; OnPropertyChanged("Status"); } }
        }

        private string _PrescriptionReference;
        public string PrescriptionReference
        {
            get => _PrescriptionReference;
            set { if (_PrescriptionReference != value) { _PrescriptionReference = value; OnPropertyChanged("PrescriptionReference"); } }
        }

        private bool _IsDeleted;
        public bool IsDeleted
        {
            get => _IsDeleted;
            set { if (_IsDeleted != value) { _IsDeleted = value; OnPropertyChanged("IsDeleted"); } }
        }

        // Navigation property
        private List<SaleItem> _SaleItems = new List<SaleItem>();
        public List<SaleItem> SaleItems
        {
            get => _SaleItems;
            set
            {
                if (_SaleItems != value)
                {
                    _SaleItems = value;
                    OnPropertyChanged("SaleItems");
                    OnPropertyChanged("TotalAmount");
                }
            }
        }

        // Computed
        public decimal TotalAmount
        {
            get
            {
                if (SaleItems == null || !SaleItems.Any())
                    return 0 - DiscountAmount;
                return SaleItems.Sum(x => x.LineTotal) - DiscountAmount;
            }
        }
    }
}
