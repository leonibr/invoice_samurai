using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NotaFiscalPoc.Shared
{
    public record NotaModel
    {


        private int _collectionHaschanged = -1;
        public NotaModel()
        {
            ItensNota = new ObservableCollection<ItemNotaModel>();
            ItensNota.CollectionChanged += ItensNota_CollectionChanged;
            AddItemNota();
        }

        private void ItemNota_HasChanged(object sender, bool e)
        {
            AlteraHash();
        }

        
        private void ItensNota_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AlteraHash();
        }

        private void AlteraHash()
        {
            _collectionHaschanged = (new Random()).Next(int.MinValue, int.MaxValue);
        }
        private void ItemNota_OnShouldDelete(object sender, bool e)
        {
            var item = (ItemNotaModel)sender;
            if (ItensNota.Count <= 1)
            {
                return;
            }
            item.OnShouldDelete -= ItemNota_OnShouldDelete;
            item.HasChanged -= ItemNota_HasChanged;
            ItensNota.Remove(item);
            int i = 0;
            foreach (var itn in ItensNota)
            {
                itn.ItemOrdem = ++i;
            }
        }
        public void AddItemNota()
        {
            var item = new ItemNotaModel()
            {
                ItemOrdem = ItensNota.Count + 1
            };
            item.OnShouldDelete += ItemNota_OnShouldDelete;
            item.HasChanged += ItemNota_HasChanged;
            ItensNota.Add(item);

        }
        public ClienteModel Cliente { get; set; }
                       
        public int Numero { get; set; } = 58656;
        public string Cnpj { get; set; } = "25.888.666/0000-88";
        public string NomeEmpresa { get; set; } = "Industria e Comércio Ltda.";
        public int ItensNotaHasChanged => _collectionHaschanged ;

        public ObservableCollection<ItemNotaModel> ItensNota { get; set; }

      

        public string ValorNotaFormatado => $"R$ {ItensNota.Sum(c => c.Total):n2}";


    }
}
