# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Notifier/Notifier.csproj", "Notifier/"]
RUN dotnet restore "Notifier/Notifier.csproj"
COPY . .

WORKDIR "/src/Notifier"
RUN dotnet build "Notifier.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Notifier.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Command to run the application
ENTRYPOINT ["dotnet", "Notifier.dll"]