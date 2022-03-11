
# Invoice Samurai
### Sample showcase Blazor dynamic update features and dynamic pdf generation on webassembly

## See it [online](https://invoice-samurai.marques.top)

![Invoice Samurai](invoicesamurai.gif)

## Objective
- Showcase dynamic form update with calculations
- Dynamic Pdf generation on Browser (actually is generated on server and streamed to the browser throught http 😁) but prevents the user to leave the page to see the result, this way users end up having a faster loop interaction while building a report/an invoice.

## It uses
- Blazor (.Net 6), Bootstrap 5 (Bootswatch awesome theme)
- Reactive.NET extensions 

## License
- MIT

## Changes
    - move renderer logic from script tag to script file
    - upgrade to .net 6 (no need to cast string to Uint8Array - better performance)
    - Added support to PDF.js instead of rendering on IFrame. see demo.
    

### Roadmap
    - send the binary data throught websockts (Signalr) instead http response.

