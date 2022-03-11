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
using InvoiceSamurai.Shared;
using System.Net;

namespace InvoiceSamurai.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class PdfinvoiceController : ControllerBase
{
    private readonly static string _lopsem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas dictum felis ut turpis viverra, a ultrices nisi tempor. Aliquam suscipit dui sit amet facilisis aliquam. In scelerisque sem ut elit molestie tempor. In finibus sagittis nulla, vitae vestibulum ante tristique sit amet. Phasellus facilisis rhoncus nunc id scelerisque. Praesent cursus erat nec turpis interdum condimentum. Aenean ut facilisis eros. Nam semper tincidunt libero in porttitor. Praesent nec dui vitae leo vulputate varius ut non risus. Quisque imperdiet euismod ipsum facilisis finibus. Duis ac felis eget leo malesuada gravida id at felis. Cras posuere, tortor sit amet bibendum tincidunt, augue lectus pulvinar nisl, ac blandit velit arcu sed nulla. Mauris id venenatis turpis, ut fringilla nunc. Aenean commodo fermentum nulla, non porta sapien viverra sed. Sed sed risus interdum, maximus sapien ac, bibendum diam.";



    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWebHostEnvironment env;

    public PdfinvoiceController(ILogger<WeatherForecastController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        this.env = env;
    }


    [HttpPost("")]
    public FileResult GeneratePdf(
        [FromBody] GeneratePdfCommand command,
        CancellationToken cancellationToken)
    {
        InvoiceModel invoice = command.Invoice;
        CustomerModel customer = command.Customer;
        var fontPath = Path.Combine(env.ContentRootPath, "Fonts", "LibreBarcode39Extended-Regular.ttf");
        BaseFont barcodeSource = BaseFont.CreateFont(fontPath, BaseFont.CP1252, BaseFont.EMBEDDED);

        var memoryStream = new MemoryStream();

        // in centimeters
        float leftMargin = 1.0f;
        float rightMargin = 1.0f;
        float topMargin = 1.0f;
        float bottomMargin = 1.5f;

        Document pdf = new Document(
                                PageSize.A4,
                                leftMargin.ToDpi(),
                                rightMargin.ToDpi(),
                                topMargin.ToDpi(),
                                bottomMargin.ToDpi()
                               );

        pdf.AddTitle("Invoice Samurai - Blazor");
        pdf.AddAuthor("Ashley Marques");
        pdf.AddCreationDate();
        pdf.AddKeywords("blazor");
        pdf.AddKeywords("invoice");
        pdf.AddSubject("Sample dynamic invoice generation based on form update");

        PdfWriter writer = PdfWriter.GetInstance(pdf, memoryStream);


        var fontStyle = FontFactory.GetFont("Arial", 12, BaseColor.White);
        var labelHeader = new Chunk($"Invoice Number {invoice.Number}", fontStyle);
        HeaderFooter header = new HeaderFooter(new Phrase(labelHeader), false)
        {
            BackgroundColor = new BaseColor(48, 17, 94),
            Alignment = Element.ALIGN_CENTER,
            Border = Rectangle.NO_BORDER
        };
        //header.Border = Rectangle.NO_BORDER;
        pdf.Header = header;


        var labelFooter = new Chunk("Page", fontStyle);
        HeaderFooter footer = new HeaderFooter(new Phrase(labelFooter), true)
        {
            Border = Rectangle.NO_BORDER,
            Alignment = Element.ALIGN_RIGHT
        };
        pdf.Footer = footer;

        pdf.Open();
        var company = new Paragraph(invoice.CompanyName, new Font(Font.HELVETICA, 22, Font.BOLD));
        pdf.Add(company);
        var businessNumber = new Paragraph($"Business Registration Number: {invoice.BusinessRegistrationNumber}", new Font(Font.HELVETICA, 11, Font.BOLD));
        pdf.Add(businessNumber);
        var title = new Paragraph($"Customer: {customer.Name} \nDate of birth: {customer.Dob:dd/MM/yyyy}",
            new Font(Font.HELVETICA, 12, Font.BOLD, BaseColor.Gray));
        // title.SpacingAfter = 1f;

        pdf.Add(title);

        Font _fontStyle = FontFactory.GetFont("Tahoma", 12f, Font.ITALIC);

        var address = new Phrase($"{customer.Address}, ", _fontStyle);
        pdf.Add(address);
        address = new Phrase($"{customer.Neigborhood} - {customer.City}-{customer.State}", _fontStyle);
        pdf.Add(address);

        var table = new Paragraph("List of Itens:", new Font(Font.HELVETICA, 10, Font.BOLD));
        pdf.Add(table);

        Table datatable = new Table(6);

        datatable.Width = 100;
        datatable.Padding = 2;
        datatable.Spacing = 0;
        datatable.BorderColor = new BaseColor(48, 17, 94);


        float[] headerwidths = { 5, 5, 32, 6, 6, 7 };
        var fontHeader = new Font(Font.HELVETICA, 11, Font.BOLD);
        var fontBody = new Font(Font.HELVETICA, 9);
        datatable.Widths = headerwidths;
        datatable.DefaultHorizontalAlignment = Element.ALIGN_LEFT;
        datatable.AddCell(new Phrase("#", fontHeader));
        datatable.AddCell(new Phrase("Sku", fontHeader));
        datatable.AddCell(new Phrase("Description", fontHeader));
        datatable.AddCell(new Phrase("Qty.", fontHeader));
        datatable.AddCell(new Phrase("U.Price", fontHeader));
        datatable.AddCell(new Phrase("Total", fontHeader));

        foreach (var row in invoice.Itens)
        {
            datatable.AddCell(new Phrase(row.ItemOrdem.ToString(), fontBody));
            datatable.AddCell(new Phrase(row.Sku?.ToString(), fontBody));
            datatable.AddCell(new Phrase(row.Description?.ToString(), fontBody));
            var qtd = new Cell(new Phrase(row.Quantity.ToString(), fontBody));
            qtd.SetHorizontalAlignment("CENTER");
            datatable.AddCell(qtd);
            var vu = new Cell(new Phrase(row.UnitPrice.ToString(), fontBody));
            vu.SetHorizontalAlignment("CENTER");
            datatable.AddCell(vu);
            datatable.AddCell(new Phrase(row.FormattedTotal?.ToString(), fontBody));
        }

        datatable.AddCell("");
        datatable.AddCell("");
        datatable.AddCell("");
        datatable.AddCell("");
        datatable.AddCell("");
        datatable.AddCell(invoice.FormattedPrice);

        pdf.Add(datatable);

        // Create and add a Paragraph
        var p = new Paragraph($"{customer.City}, {DateTime.Now:F}", _fontStyle);
        p.SpacingBefore = 20f;
        p.SetAlignment("RIGHT");



        Font font = new Font(barcodeSource, 42);
        StringBuilder sb = new StringBuilder();
        var rnd = (new Random());
        for (int i = 0; i < 16; i++)
        {
            sb.Append(rnd.Next(0, 9));
        }
        string s = sb.ToString();
        pdf.Add(new Paragraph($"Voucher Number {s}", _fontStyle));
        pdf.Add(new Paragraph(s, font));

        pdf.Add(p);
        title = new Paragraph("Small Contract letters", new Font(Font.HELVETICA, 20, Font.BOLD));
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
        Response.StatusCode = (int)HttpStatusCode.Created;
        return File(memoryStream.ToArray(), "application/pdf");
    }

}

