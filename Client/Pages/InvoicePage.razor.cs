using Microsoft.AspNetCore.Components;
using InvoiceSamurai.Client.Config;
using InvoiceSamurai.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.Threading.Tasks;
using Microsoft.JSInterop;

namespace InvoiceSamurai.Client.Pages;

public partial class InvoicePage : ComponentBase
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
        Observable.FromEventPattern<int>(h => RaiseNewHashCode += h, h => RaiseNewHashCode -= h)
            .Throttle(TimeSpan.FromMilliseconds(600))
            .Select(async (c) => new { response = await httpClient.PostAsJsonAsync("/pdfinvoice", pdfCommand) })
                .Concat()
             .Where(c => c.response.StatusCode == System.Net.HttpStatusCode.Created)
             .Select(async (c) => new { content = await c.response.Content.ReadAsByteArrayAsync() })
             .Concat()
             .Select(c => c.content)
            .Subscribe(HandleIncomingPdf);


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
        _ = JSRuntime?.InvokeVoidAsync("PdfRenderer.clearCanvas");


    }



}
