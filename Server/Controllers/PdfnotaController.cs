using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotaFiscalPoc.Shared;

namespace NotaFiscalPoc.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfnotaController : ControllerBase
    {
        private readonly static string _lopsem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas dictum felis ut turpis viverra, a ultrices nisi tempor. Aliquam suscipit dui sit amet facilisis aliquam. In scelerisque sem ut elit molestie tempor. In finibus sagittis nulla, vitae vestibulum ante tristique sit amet. Phasellus facilisis rhoncus nunc id scelerisque. Praesent cursus erat nec turpis interdum condimentum. Aenean ut facilisis eros. Nam semper tincidunt libero in porttitor. Praesent nec dui vitae leo vulputate varius ut non risus. Quisque imperdiet euismod ipsum facilisis finibus. Duis ac felis eget leo malesuada gravida id at felis. Cras posuere, tortor sit amet bibendum tincidunt, augue lectus pulvinar nisl, ac blandit velit arcu sed nulla. Mauris id venenatis turpis, ut fringilla nunc. Aenean commodo fermentum nulla, non porta sapien viverra sed. Sed sed risus interdum, maximus sapien ac, bibendum diam.";


  
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWebHostEnvironment env;

        public PdfnotaController(ILogger<WeatherForecastController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            this.env = env;
        }


        [HttpPost("")]
        public ActionResult<string> GeraPdf([FromBody] GeraPdfCommand command, 
        
                    CancellationToken cancellationToken)
        {
            NotaModel nota = command.Nota;
            ClienteModel cliente = command.Cliente ;
            var fontPath = Path.Combine(env.ContentRootPath, "Fonts", "LibreBarcode39Extended-Regular.ttf");
            BaseFont codigobarraFonte = BaseFont.CreateFont(fontPath, BaseFont.CP1252, BaseFont.EMBEDDED);

            var memoryStream = new MemoryStream();

            // em centímetros
            float margemEsquerda = 1.0f;
            float marqugemDireita = 1.0f;
            float margeTop = 1.0f;
            float margeBottom = 1.5f;

            Document pdf = new Document(
                                    PageSize.A4,
                                    margemEsquerda.ToDpi(),
                                    marqugemDireita.ToDpi(),
                                    margeTop.ToDpi(),
                                    margeBottom.ToDpi()
                                   );

            pdf.AddTitle("Nota Fiscal Blazor");
            pdf.AddAuthor("Ashley Marques");
            pdf.AddCreationDate();
            pdf.AddKeywords("blazor");
            pdf.AddKeywords("nota fiscal");
            pdf.AddSubject("Criação de exemplo de nota fiscal dinâmica");

            PdfWriter writer = PdfWriter.GetInstance(pdf, memoryStream);

      
            var fontStyle = FontFactory.GetFont("Arial", 12, BaseColor.White);
            var labelHeader = new Chunk($"Nota Número {nota.Numero}", fontStyle);
            HeaderFooter header = new HeaderFooter(new Phrase(labelHeader), false)
            {
                BackgroundColor = new BaseColor(33, 186, 199),
                Alignment = Element.ALIGN_CENTER,
                Border = Rectangle.NO_BORDER
            };
            //header.Border = Rectangle.NO_BORDER;
            pdf.Header = header;


            var labelFooter = new Chunk("Página", fontStyle);
            HeaderFooter footer = new HeaderFooter(new Phrase(labelFooter), true)
            {
                Border = Rectangle.NO_BORDER,
                Alignment = Element.ALIGN_RIGHT
            };
            pdf.Footer = footer;

            pdf.Open();
            var empresa = new Paragraph(nota.NomeEmpresa, new Font(Font.HELVETICA, 22, Font.BOLD));
            pdf.Add(empresa);
            var cnpj = new Paragraph($"CNPJ: {nota.Cnpj}", new Font(Font.HELVETICA, 11, Font.BOLD));
            pdf.Add(cnpj);
            var title = new Paragraph($"Cliente: {cliente.Nome} \nDt.Nasc.: {cliente.DataNascimento:dd/MM/yyyy}",
                new Font(Font.HELVETICA, 12, Font.BOLD, BaseColor.Gray));
            // title.SpacingAfter = 1f;

            pdf.Add(title);

            Font _fontStyle = FontFactory.GetFont("Tahoma", 12f, Font.ITALIC);

            var endereco = new Phrase($"{cliente.Endereco}, ", _fontStyle);
            pdf.Add(endereco);
            endereco = new Phrase($"{cliente.Bairro} - {cliente.Cidade}", _fontStyle);
            pdf.Add(endereco);

            var tabela = new Paragraph("Relação de Itens:", new Font(Font.HELVETICA, 10, Font.BOLD));
            pdf.Add(tabela);           

            Table datatable = new Table(6);
          
            datatable.Width = 100;
            datatable.Padding = 2;
            datatable.Spacing = 0;

            float[] headerwidths = { 5, 5, 32, 6, 6, 7};
            var fontHeader = new Font(Font.HELVETICA, 11, Font.BOLD);
            var fontBody = new Font(Font.HELVETICA, 9);
            datatable.Widths = headerwidths;
            datatable.DefaultHorizontalAlignment = Element.ALIGN_LEFT;
            datatable.AddCell(new Phrase("#", fontHeader));
            datatable.AddCell(new Phrase("Cod.", fontHeader));
            datatable.AddCell(new Phrase("Descrição", fontHeader));
            datatable.AddCell(new Phrase("Qtd.", fontHeader));
            datatable.AddCell(new Phrase("Vl.Uni.", fontHeader));
            datatable.AddCell(new Phrase("Total", fontHeader));

            foreach (var row in nota.ItensNota)
            {
                datatable.AddCell(new Phrase(row.ItemOrdem.ToString(), fontBody));
                datatable.AddCell(new Phrase(row.Codigo?.ToString(), fontBody));
                datatable.AddCell(new Phrase(row.Descricao?.ToString(), fontBody));
                var qtd = new Cell(new Phrase(row.Qtd.ToString(), fontBody));
                qtd.SetHorizontalAlignment("CENTER");
                datatable.AddCell(qtd);
                var vu = new Cell(new Phrase(row.ValorUnidade.ToString(), fontBody));
                vu.SetHorizontalAlignment("CENTER");
                datatable.AddCell(vu);
                datatable.AddCell(new Phrase(row.TotalFormatado?.ToString(), fontBody));
            }

            datatable.AddCell("");
            datatable.AddCell("");
            datatable.AddCell("");
            datatable.AddCell("");
            datatable.AddCell("");
            datatable.AddCell(nota.ValorNotaFormatado);

            pdf.Add(datatable);

            // Create and add a Paragraph
            var p = new Paragraph($"{cliente.Cidade}, {DateTime.Now:dddd, dd \\de MMMM \\de yyyy}", _fontStyle);
            p.SpacingBefore = 20f;
            p.SetAlignment("RIGHT");

         

            Font font = new Font(codigobarraFonte, 42);
            StringBuilder sb = new StringBuilder();
            var rnd = (new Random());
            for (int i = 0; i < 16; i++)
            {
                sb.Append(rnd.Next(0, 9));
            }
            string s = sb.ToString();
            pdf.Add(new Paragraph($"Suposta Linha digitável {s}", _fontStyle));
            pdf.Add(new Paragraph(s, font));

            pdf.Add(p);
            title = new Paragraph("Letras miúdas", new Font(Font.HELVETICA, 20, Font.BOLD));
            title.SpacingAfter = 13f;
            pdf.Add(title);

            float margeborder = 1.5f;
            float widhtColumn = 8.5f;
            float space = 1.0f;

            MultiColumnText columns = new MultiColumnText();
            columns.AddSimpleColumn(margeborder.ToDpi(),
                                    pdf.PageSize.Width - margeborder.ToDpi() - space.ToDpi() - widhtColumn.ToDpi());
            columns.AddSimpleColumn(margeborder.ToDpi() + widhtColumn.ToDpi() + space.ToDpi(),
                                    pdf.PageSize.Width - margeborder.ToDpi());

            Paragraph para = new Paragraph(_lopsem, new Font(Font.HELVETICA, 8f));
            para.SpacingAfter = 9f;
            para.Alignment = Element.ALIGN_JUSTIFIED;

            columns.AddElement(para);

            pdf.Add(columns);
            pdf.Close();

            return Created("", Convert.ToBase64String(memoryStream.ToArray()));
        }

    }
}
