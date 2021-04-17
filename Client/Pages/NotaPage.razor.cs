using Microsoft.AspNetCore.Components;
using NotaFiscalPoc.Client.Config;
using NotaFiscalPoc.Shared;
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

namespace NotaFiscalPoc.Client.Pages
{
    public partial class NotaPage : ComponentBase
    {
        protected event EventHandler<int> RaiseNewHashCode;
        [Inject]
        HttpClient httpClient { get; set; }

        protected override void OnInitialized()
        {
            Observable.FromEventPattern<int>(h => RaiseNewHashCode += h, h => RaiseNewHashCode -= h)
                .Throttle(TimeSpan.FromMilliseconds(600))
                .Select( async (c) => new { response = await httpClient.PostAsJsonAsync("/pdfnota", pdfCommand) } )
                    .Concat()
                 .Where(c => c.response.StatusCode == System.Net.HttpStatusCode.Created)
                 .Select(async (c) => new { content = await c.response.Content.ReadAsStringAsync() })
                 .Concat()
                 .Select(c => c.content)
                .Subscribe(RecebePdf);



        }

        private void RecebePdf(string pdfBody)
        {
            PdfBody = pdfBody;
            StateHasChanged();
        }

        string NotaHeader => Cliente.IsNull() ? "Nota" : $"Nota de {Cliente?.Nome}";

        protected NotaModel Nota { get; set; } = new();

        protected ClienteModel Cliente = new ClienteModel();

        protected string PdfBody = string.Empty;
        protected GeraPdfCommand pdfCommand = new GeraPdfCommand();

        int commandAnterior = -1;
        protected async Task ProcessaNota()
        {



            pdfCommand = pdfCommand with
            {
                Nota = Nota,
                Cliente = Cliente
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