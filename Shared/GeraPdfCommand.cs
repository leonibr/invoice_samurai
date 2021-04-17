using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaFiscalPoc.Shared
{
    public record GeraPdfCommand
    {
        public ClienteModel Cliente { get; set; }
        public NotaModel Nota { get; set; }
    }
}
