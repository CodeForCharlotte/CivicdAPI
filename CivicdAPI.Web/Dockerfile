FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 62687
EXPOSE 44302

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["CivicdAPI.Web/CivicdAPI.Web.csproj", "CivicdAPI.Web/"]
RUN dotnet restore "CivicdAPI.Web/CivicdAPI.Web.csproj"
COPY . .
WORKDIR "/src/CivicdAPI.Web"
RUN dotnet build "CivicdAPI.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CivicdAPI.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CivicdAPI.Web.dll"]