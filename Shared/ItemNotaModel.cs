using System;

namespace NotaFiscalPoc.Shared
{
    public record ItemNotaModel
    {
        private decimal valorUnidade;
        private int qtd;
        private string descricao;
        private string codigo;
        private int itemOrdem;

        public event EventHandler<bool> OnShouldDelete;
        public event EventHandler<bool> HasChanged;


        public int ItemOrdem { get => itemOrdem; set => itemOrdem = value; }



        public string Codigo
        {
            get => codigo; set
            {
                codigo = value;
                HasHangedField();
            }
        }


        public string Descricao
        {
            get => descricao; set
            {
                descricao = value;
                HasHangedField();
            }
        }

        public int Qtd
        {
            get => qtd; set
            {
                qtd = value;
                HasHangedField();
            }
        }
        public decimal ValorUnidade
        {
            get => valorUnidade; set
            {
                valorUnidade = value;
                HasHangedField();
            }
        }

        public decimal Total => Qtd > 0 && ValorUnidade > 0 ? Qtd * ValorUnidade : 0;
        public string TotalFormatado => $"R$ {Total:n2}";


        private void HasHangedField() => HasChanged?.Invoke(this, true);
        public void DeveExcluir() =>OnShouldDelete?.Invoke(this, true);


    }
}
