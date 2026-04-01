FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["Directory.Packages.props", "."]
COPY ["TradingDashboard.sln", "."]
COPY ["src/", "src/"]

RUN dotnet restore "src/Features/Cot/TradingDashboard.Features.Cot.Presentation.Api/TradingDashboard.Features.Cot.Presentation.Api.csproj"
RUN dotnet publish "src/Features/Cot/TradingDashboard.Features.Cot.Presentation.Api/TradingDashboard.Features.Cot.Presentation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TradingDashboard.Features.Cot.Presentation.Api.dll"]

