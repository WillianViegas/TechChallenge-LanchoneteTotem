# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["TechChallenge-LanchoneteTotem/TechChallenge-LanchoneteTotem.csproj", "TechChallenge-LanchoneteTotem/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infra/Infra.csproj", "Infra/"]

RUN dotnet restore "TechChallenge-LanchoneteTotem/TechChallenge-LanchoneteTotem.csproj"
COPY . .

WORKDIR "/src/TechChallenge-LanchoneteTotem"
RUN dotnet build "TechChallenge-LanchoneteTotem.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TechChallenge-LanchoneteTotem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Command to run the application
ENTRYPOINT ["dotnet", "TechChallenge-LanchoneteTotem.dll"]