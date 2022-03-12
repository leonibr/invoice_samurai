using System;
using System.Text;
using InvoiceSamurai.Shared;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceSamurai.Client.Documents;
public class InvoiceDocument : IDocument
{
    private readonly static string _loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas dictum felis ut turpis viverra, a ultrices nisi tempor. Aliquam suscipit dui sit amet facilisis aliquam. In scelerisque sem ut elit molestie tempor. In finibus sagittis nulla, vitae vestibulum ante tristique sit amet. Phasellus facilisis rhoncus nunc id scelerisque. Praesent cursus erat nec turpis interdum condimentum. Aenean ut facilisis eros. Nam semper tincidunt libero in porttitor. Praesent nec dui vitae leo vulputate varius ut non risus. Quisque imperdiet euismod ipsum facilisis finibus. Duis ac felis eget leo malesuada gravida id at felis. Cras posuere, tortor sit amet bibendum tincidunt, augue lectus pulvinar nisl, ac blandit velit arcu sed nulla. Mauris id venenatis turpis, ut fringilla nunc. Aenean commodo fermentum nulla, non porta sapien viverra sed. Sed sed risus interdum, maximus sapien ac, bibendum diam.";

    private string purpleTheme = "#30115e";
    private TextStyle baseStyle = TextStyle.Default.FontType(AppFonts.Roboto);
    public GeneratePdfCommand Model { get; }
    private InvoiceModel invoice => Model?.Invoice;
    private CustomerModel customer => Model?.Customer;
    public InvoiceDocument(GeneratePdfCommand model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata()
    {
        var result = DocumentMetadata.Default;
        result.Title = "Invoice Samurai - Blazor WASM";
        result.Author = "Ashley Marques";
        result.CreationDate = System.DateTime.Now;
        result.Keywords = "balzor, wasm, invoice";
        result.Subject = "Sample dynamic invoice generation based on form update";
        return result;
    }

    public void Compose(IDocumentContainer container)
    {

        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.MarginBottom(1.5f, Unit.Centimetre);
                page.Header().Element(ComposeHeader);
                page.Content()
                .Element(ComposeContent);

                page.Footer().Height(30)
                .Background(purpleTheme)
                .AlignCenter()
                .Text($"{customer.City}, {DateTime.Now:F}",
                baseStyle.Color(Colors.White)

                );
            });
    }

    void ComposeContent(IContainer container)
    {
        var companyStyle = baseStyle
    .Size(22)
    .Bold();
        var subtitleStyle = baseStyle
        .Size(11).Bold().Black();
        var customerStyle = baseStyle
        .Size(11).Bold()
        .BackgroundColor(purpleTheme)
        .Color(Colors.Grey.Lighten1);

        container.PaddingTop(20)
        .Column(c =>
        {
            c.Item().Row(row =>
            {
                row.RelativeItem()
                .Row(r =>
                {
                    r.RelativeItem()
                    .AlignLeft()
                    .Text(invoice.CompanyName, companyStyle);

                });



            });

            c.Item().Row(row =>
            {
                row.RelativeItem()
                    .Row(r =>
                    {
                        r.RelativeItem()
                        .AlignLeft()
                        .Text($"Business Registration Number: {invoice.BusinessRegistrationNumber}", subtitleStyle);

                    });
            });
            c.Item()
            .PaddingTop(5)
            .Row(row =>
            {
                row.RelativeItem()
                    .Row(r =>
                    {
                        r.RelativeItem()
                        .AlignLeft()
                        .Text($"Customer: {customer.Name}", customerStyle);

                    });

            });
            c.Item()
            .Row(row =>
            {

                row.RelativeItem()
                    .Row(r =>
                    {
                        r.RelativeItem()
                        .AlignLeft()
                        .Text($"Date of birth: {customer.Dob:dd/MM/yyyy}", customerStyle);

                    });
            });

            c.Item().Element(ComposeTable);
            c.Item().Element(ComposeBarCode);
            c.Item().Element(ComposeContractTerm);
        });


    }
    void ComposeContractTerm(IContainer container)
    {

        container
        .PaddingTop(5)
        .Column(col =>
        {
            col.Item()
            .Row(r =>
            {
                r.RelativeItem()
                              .Text("Small Contract letters", baseStyle.Size(14).ExtraBold());
            });

            col

            .Item()
            .Row(r =>
            {

                r.RelativeItem()
                .PaddingTop(5)
                .Text(_loremIpsum, baseStyle
                .Size(8).Italic());
                r.RelativeItem()
               .Text(string.Empty, baseStyle);
            });


        });
    }
    void ComposeBarCode(IContainer container)
    {
        StringBuilder sb = new StringBuilder();
        var rnd = (new Random());
        for (int i = 0; i < 16; i++)
        {
            sb.Append(rnd.Next(0, 9));
        }
        container
        .PaddingTop(20)
        .Column(col =>
        {
            col.Item()
            .Text($"Voucher number {sb.ToString()}",
            TextStyle.Default.Size(10).Italic());
            col.Item()
                .Text(sb.ToString(),
                 TextStyle.Default.FontType(AppFonts.LibreBarcode39).Size(42));
        });

    }
    void ComposeHeader(IContainer container)
    {

        var titleStyle = TextStyle.Default
            //  .FontType("Verdana")
            .Size(12)
            .Color(Colors.White);

        container
        .Height(18)
        .Background(purpleTheme) //same purple
        .Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                .AlignCenter()
                .Text($"Invoice Number {invoice.Number}", titleStyle)

                ;


            });



        });
    }

    void ComposeTable(IContainer container)
    {
        var headerStyle = TextStyle.Default.Size(10).SemiBold();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(10);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Text("#", headerStyle);
                header.Cell().Text("Sku", headerStyle);
                header.Cell().Text("Description", headerStyle);
                header.Cell().AlignRight().Text("Quantity", headerStyle);
                header.Cell().AlignRight().Text("Unit price", headerStyle);
                header.Cell().AlignRight().Text("Total", headerStyle);

                header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
            });

            static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
            foreach (var item in invoice.Itens)
            {
                table.Cell().Element(CellStyle).Text(item.ItemOrdem.ToString());
                table.Cell().Element(CellStyle).Text(item.Sku);
                table.Cell().Element(CellStyle).Text(item.Description);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice}");

                table.Cell().Element(CellStyle).AlignRight().Text($"{item.FormattedTotal}");

            }
            table.Cell().Element(CellStyle).Text("");
            table.Cell().Element(CellStyle).Text("");
            table.Cell().Element(CellStyle).Text("");
            table.Cell().Element(CellStyle).AlignRight().Text("");
            table.Cell().Element(CellStyle).AlignRight().Text("Total:");
            table.Cell().Element(CellStyle).AlignRight().Text($"{invoice.FormattedPrice}");

        });
    }
}