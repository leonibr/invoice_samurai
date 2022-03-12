using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using QuestPDF.Drawing;
using InvoiceSamurai.Client.Documents;

namespace InvoiceSamurai.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            using (Stream streamBarcode = Assembly
                                    .GetExecutingAssembly()
                                    .GetManifestResourceStream(AppFonts.LibreBarcode39Resourcename))
            using (Stream streamRoboto = Assembly
                                    .GetExecutingAssembly()
                                    .GetManifestResourceStream(AppFonts.RobotoResourcename))
            {
                FontManager.RegisterFontType(AppFonts.LibreBarcode39, streamBarcode);
                FontManager.RegisterFontType(AppFonts.Roboto, streamRoboto);
            }

            await builder.Build().RunAsync();
        }
    }
}
