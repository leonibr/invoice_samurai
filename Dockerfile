FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as build
WORKDIR /build-dir
ADD Client/NotaFiscalPoc.Client.csproj Client/NotaFiscalPoc.Client.csproj
ADD Server/NotaFiscalPoc.Server.csproj Server/NotaFiscalPoc.Server.csproj
ADD Shared/NotaFiscalPoc.Shared.csproj Shared/NotaFiscalPoc.Shared.csproj
ADD NotaFiscalPoc.sln .
RUN dotnet restore
ADD . ./
WORKDIR /build-dir/Server
RUN dotnet build -c:Release



FROM build as publish
WORKDIR /build-dir
RUN dotnet publish -c:Release -f:net5.0 --no-build --no-restore Server/NotaFiscalPoc.Server.csproj -o /build-dir/publish


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
RUN apk add icu-libs libx11-dev
RUN apk add libgdiplus-dev \
  --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /dist
COPY --from=publish /build-dir/publish .



FROM runtime as nota_poc
WORKDIR /dist
ENTRYPOINT ["dotnet", "NotaFiscalPoc.Server.dll"]