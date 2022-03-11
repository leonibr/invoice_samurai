FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /build-dir
ADD Client/InvoiceSamurai.Client.csproj Client/InvoiceSamurai.Client.csproj
ADD Server/InvoiceSamurai.Server.csproj Server/InvoiceSamurai.Server.csproj
ADD Shared/InvoiceSamurai.Shared.csproj Shared/InvoiceSamurai.Shared.csproj
ADD InvoiceSamurai.sln .
RUN dotnet restore
ADD . ./
WORKDIR /build-dir/Server
RUN dotnet build -c:Release



FROM build as publish
WORKDIR /build-dir
RUN dotnet publish -c:Release -f:net6.0 --no-build --no-restore Server/InvoiceSamurai.Server.csproj -o /build-dir/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
RUN apk add icu-libs libx11-dev
RUN apk add libgdiplus-dev \
  --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /dist
COPY --from=publish /build-dir/publish .



FROM runtime as invoice_samurai
WORKDIR /dist
ENTRYPOINT ["dotnet", "InvoiceSamurai.Server.dll"]