using System;

namespace InvoiceSamurai.Shared
{
    public record InvoiceItem
    {
        private decimal _unitPrice;
        private int _quantity;
        private string _description;
        private string _sku;
        private int _itemOrder;

        public event EventHandler<bool> OnShouldDelete;
        public event EventHandler<bool> HasChanged;


        public int ItemOrdem { get => _itemOrder; set => _itemOrder = value; }



        public string Sku
        {
            get => _sku; set
            {
                _sku = value;
                HasHangedField();
            }
        }


        public string Description
        {
            get => _description; set
            {
                _description = value;
                HasHangedField();
            }
        }

        public int Quantity
        {
            get => _quantity; set
            {
                _quantity = value;
                HasHangedField();
            }
        }
        public decimal UnitPrice
        {
            get => _unitPrice; set
            {
                _unitPrice = value;
                HasHangedField();
            }
        }

        public decimal Total => Quantity > 0 && UnitPrice > 0 ? Quantity * UnitPrice : 0;
        public string FormattedTotal => $"$ {Total:n2}";


        private void HasHangedField() => HasChanged?.Invoke(this, true);
        public void ShouldDelete() =>OnShouldDelete?.Invoke(this, true);


    }
}
