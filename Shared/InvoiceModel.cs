using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InvoiceSamurai.Shared
{
    public record InvoiceModel
    {


        private int _collectionHaschanged = -1;
        public InvoiceModel()
        {
            Itens = new ObservableCollection<InvoiceItem>();
            Itens.CollectionChanged += InvoiceItens_CollectionChanged;
            AddInvoiceItem();
        }

        private void InvoiceItem_HasChanged(object sender, bool e)
        {
            UpdateHash();
        }

        
        private void InvoiceItens_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateHash();
        }

        private void UpdateHash()
        {
            _collectionHaschanged = (new Random()).Next(int.MinValue, int.MaxValue);
        }
        private void InvoiceItem_OnShouldDelete(object sender, bool e)
        {
            var item = (InvoiceItem)sender;
            if (Itens.Count <= 1)
            {
                return;
            }
            item.OnShouldDelete -= InvoiceItem_OnShouldDelete;
            item.HasChanged -= InvoiceItem_HasChanged;
            Itens.Remove(item);
            int i = 0;
            foreach (var itn in Itens)
            {
                itn.ItemOrdem = ++i;
            }
        }
        public void AddInvoiceItem()
        {
            var item = new InvoiceItem()
            {
                ItemOrdem = Itens.Count + 1
            };
            item.OnShouldDelete += InvoiceItem_OnShouldDelete;
            item.HasChanged += InvoiceItem_HasChanged;
            Itens.Add(item);

        }
        public CustomerModel Customer { get; set; }
                       
        public int Number { get; set; } = 58656;
        public string BusinessRegistrationNumber { get; set; } = "25.888.666/0000-88";
        public string CompanyName { get; set; } = "Stark & Duster llc";
        public int InvoiceItensHasChanged => _collectionHaschanged ;

        public ObservableCollection<InvoiceItem> Itens { get; set; }

      

        public string FormattedPrice => $"$ {Itens.Sum(c => c.Total):n2}";


    }
}
