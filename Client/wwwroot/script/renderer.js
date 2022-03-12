console.log('loading pdrrenderer script.');
pdfjsLib.GlobalWorkerOptions.workerSrc =
    "https://cdn.jsdelivr.net/npm/pdfjs-dist@2.9.359/build/pdf.worker.min.js";

Timer = {
    start: () => new Date().getTime(),
    partial: (startTime) => `${new Date().getTime() - startTime}ms`,
}

PdfRenderer = {};

PdfRenderer.clearCanvas = () => {
    const canvas = document.getElementById("pdf-canvas");
    const context = canvas.getContext("2d");
    context.clearRect(0, 0, canvas.width, canvas.height)
}

PdfRenderer.renderPdf = function (pdfData) {
    const start = Timer.start();
    const binary = pdfData;
    var loadingTask = pdfjsLib.getDocument({ data: binary });
    loadingTask.promise.then(function (pdf) {
        console.log(`pdf loaded at ${Timer.partial(start)}`)
        pdf.getPage(1).then(function (page) {
            console.log(`page loaded at ${Timer.partial(start)}`)
            var scale = 0.95;
            var viewport = page.getViewport({ scale: scale });
            var outputScale = window.devicePixelRatio || 1;
            var canvas = document.getElementById("pdf-canvas");
            var context = canvas.getContext("2d");

            canvas.width = Math.floor(viewport.width * outputScale);
            canvas.height = Math.floor(viewport.height * outputScale);
            canvas.style.width = Math.floor(viewport.width) + "px";
            canvas.style.height = Math.floor(viewport.height) + "px";

            var transform =
                outputScale !== 1 ? [outputScale, 0, 0, outputScale, 0, 0] : null;

            var renderContext = {
                canvasContext: context,
                transform: transform,
                viewport: viewport,
            };
            var renderTask = page.render(renderContext);

            renderTask.promise.then(function () {
                console.log(`pdf page rendered in ${Timer.partial(start)}`);
            });
        });
    });
};