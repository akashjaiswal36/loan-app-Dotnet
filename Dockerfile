FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY LoanCalculatorApp.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish "LoanCalculatorApp.csproj" -c Release -o /app


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./


EXPOSE 5000
ENTRYPOINT ["dotnet", "LoanCalculatorApp.dll"]
# This Dockerfile builds a .NET application and sets it up to run in a container.