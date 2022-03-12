
# Invoice Samurai
### Sample showcase Blazor dynamic update features and dynamic pdf generation on webassembly

## See it [online](https://invoice-samurai.marques.top)

![Invoice Samurai](invoicesamurai.gif)

## Objective
- Showcase dynamic form update with calculations
- Dynamic Pdf generation on Browser (actually is generated on server and streamed to the browser throught http 😁) but prevents the user to leave the page to see the result, this way users end up having a faster loop interaction while building a report/an invoice.

## It uses
- Wasm Invoice (with aot support in docker 🐋) - if you type fast enought it brakes the canvas view since it is generating a new one before ending the exbition part of the previous one! 🚀🚀
- Blazor (.Net 6), Bootstrap 5 (Bootswatch awesome theme)
- Reactive.NET extensions 

## License
- MIT

## Changes
    - 2022-03-12 wasm generation (NO SERVER INVOLVED 🐱‍👤) invoice also added with embedded fonts
    - 2022-03-11 move renderer logic from script tag to script file
    - upgrade to .net 6 (no need to cast string to Uint8Array - better performance)
    - Added support to PDF.js instead of rendering on IFrame. see demo.
    

### Roadmap
    - send the binary data throught websockts (Signalr) instead http response.

