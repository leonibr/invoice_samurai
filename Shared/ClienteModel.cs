using System;

namespace NotaFiscalPoc.Shared
{
    public record ClienteModel
    {
        public string Nome { get; set; } = "Fulano de Tal";
        public string Endereco { get; set; } = "Rua da Lamentação, 256";
        public string Bairro { get; set; } = "Desespero";
        public string Cidade { get; set; } = "CaosCity";

        public DateTime DataNascimento { get; set; } = new DateTime(2020, 2, 3);
    }
}
