# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["PagamentoPedidoNotificationConsumer/PagamentoPedidoNotificationConsumer.csproj", "PagamentoPedidoNotificationConsumer/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infra/Infra.csproj", "Infra/"]

RUN dotnet restore "PagamentoPedidoNotificationConsumer/PagamentoPedidoNotificationConsumer.csproj"
COPY . .

WORKDIR "/src/PagamentoPedidoNotificationConsumer"
RUN dotnet build "PagamentoPedidoNotificationConsumer.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PagamentoPedidoNotificationConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Command to run the application
ENTRYPOINT ["dotnet", "PagamentoPedidoNotificationConsumer.dll"]