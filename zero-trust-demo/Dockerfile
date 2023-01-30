FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["zero-trust-demo/zero-trust-demo.csproj", "zero-trust-demo/"]
RUN dotnet restore "zero-trust-demo/zero-trust-demo.csproj"
COPY . .
WORKDIR "/src/zero-trust-demo"
RUN dotnet build "zero-trust-demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "zero-trust-demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "zero-trust-demo.dll"]
