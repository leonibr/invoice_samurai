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

namespace InvoiceSamurai.Client.Pages
{ 

    public partial class InvoicePage : ComponentBase
    {
        protected enum UpdateFieldOn
        {
            Keystroke,
            LostFocus
        }   
        protected event EventHandler<int> RaiseNewHashCode;

        protected UpdateFieldOn TextFieldInput { get; set; } = UpdateFieldOn.Keystroke; // defualt
        protected string UpdateOnText => TextFieldInput == UpdateFieldOn.Keystroke ? "oninput" : "onchange";

        protected void SetUpdateSettings(UpdateFieldOn newValue)
        {
            TextFieldInput = newValue;
        }

        [Inject]
        HttpClient httpClient { get; set; }

        protected override void OnInitialized()
        {
            Observable.FromEventPattern<int>(h => RaiseNewHashCode += h, h => RaiseNewHashCode -= h)
                .Throttle(TimeSpan.FromMilliseconds(600))
                .Select( async (c) => new { response = await httpClient.PostAsJsonAsync("/pdfinvoice", pdfCommand) } )
                    .Concat()
                 .Where(c => c.response.StatusCode == System.Net.HttpStatusCode.Created)
                 .Select(async (c) => new { content = await c.response.Content.ReadAsStringAsync() })
                 .Concat()
                 .Select(c => c.content)
                .Subscribe(HandleIncomingPdf);



        }

        private void HandleIncomingPdf(string pdfBody)
        {
            PdfBody = pdfBody;
            StateHasChanged();
        }

        string NotaHeader => Customer.IsNull() ? "Invoice" : $"Invoice for {Customer?.Name}";

        protected InvoiceModel Nota { get; set; } = new();

        protected CustomerModel Customer = new CustomerModel();

        protected string PdfBody = string.Empty;
        protected GeneratePdfCommand pdfCommand = new GeneratePdfCommand();

        int commandAnterior = -1;
        protected async Task RenderPdf()
        {



            pdfCommand = pdfCommand with
            {
                Invoice = Nota,
                Customer = Customer
            };
            if (pdfCommand.GetHashCode() == commandAnterior)
            {
                return;
            }
            commandAnterior = pdfCommand.GetHashCode();
            RaiseNewHashCode?.Invoke(this, commandAnterior);
            
            PdfBody = string.Empty;


        }

      

    }






}