using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceSamurai.Server;
public static class PdfExtensions
{
    public static float ToDpi(this float centimeter)
    {
        var inch = centimeter / 2.54;
        return (float)(inch * 72);
    }

}

