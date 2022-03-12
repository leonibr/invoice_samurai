

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reflection;
using InvoiceSamurai.Client.Config;
using InvoiceSamurai.Client.Documents;
using InvoiceSamurai.Client.Shared;
using InvoiceSamurai.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
namespace InvoiceSamurai.Client.Pages.InvoiceWasm;

[Route("/invoice-wasm")]
[Layout(typeof(InvoiceLayout))]
public partial class Index : ComponentBase
{

    [Inject] HttpClient httpClient { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    protected enum UpdateFieldOn
    {
        Keystroke,
        LostFocus
    }
    protected event EventHandler<int> RaiseNewHashCode;
    protected InvoiceModel Invoice { get; set; } = new();
    protected byte[] PdfBody = Array.Empty<byte>();

    protected CustomerModel Customer = new CustomerModel();
    protected GeneratePdfCommand pdfCommand = new GeneratePdfCommand();
    protected UpdateFieldOn TextFieldInput { get; set; } = UpdateFieldOn.Keystroke; // defualt
    protected string UpdateOnText => TextFieldInput == UpdateFieldOn.Keystroke ? "oninput" : "onchange";
    string InvoiceHeader => Customer.IsNull() ? "Invoice" : $"Invoice for {Customer?.Name}";
    protected override void OnInitialized()
    {

    }
    protected void SetUpdateSettings(UpdateFieldOn newValue)
    {
        TextFieldInput = newValue;
    }


    private void HandleIncomingPdf(byte[] pdfBody)
    {
        PdfBody = pdfBody;
        JSRuntime.InvokeVoidAsync("PdfRenderer.renderPdf", pdfBody);
        StateHasChanged();
    }




    int previousCommand = -1;
    protected void RenderPdf()
    {
        pdfCommand = pdfCommand with
        {
            Invoice = Invoice,
            Customer = Customer
        };
        if (pdfCommand.GetHashCode() == previousCommand)
        {
            return;
        }
        previousCommand = pdfCommand.GetHashCode();
        RaiseNewHashCode?.Invoke(this, previousCommand);

        PdfBody = Array.Empty<byte>();
        // JSRuntime?.InvokeVoidAsync("PdfRenderer.clearCanvas");
        var document = new InvoiceDocument(pdfCommand);
        HandleIncomingPdf(document.GeneratePdf());


    }



}

