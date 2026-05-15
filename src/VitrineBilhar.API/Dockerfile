FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/VitrineBilhar.API/VitrineBilhar.API.csproj", "src/VitrineBilhar.API/"]
COPY ["src/VitrineBilhar.Application/VitrineBilhar.Application.csproj", "src/VitrineBilhar.Application/"]
COPY ["src/VitrineBilhar.Infrastructure/VitrineBilhar.Infrastructure.csproj", "src/VitrineBilhar.Infrastructure/"]
COPY ["src/VitrineBilhar.Domain/VitrineBilhar.Domain.csproj", "src/VitrineBilhar.Domain/"]
RUN dotnet restore "src/VitrineBilhar.API/VitrineBilhar.API.csproj"

COPY . .
WORKDIR "/src/src/VitrineBilhar.API"
RUN dotnet publish "VitrineBilhar.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VitrineBilhar.API.dll"]
