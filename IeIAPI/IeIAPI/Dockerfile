# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./IeIAPI.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

# Use ADD to copy CSV and XML files
COPY ./CV.csv .
COPY ./CAT.xml .
# Debugging commands
RUN ls -al /app

# Etapa de publicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la fase de publicación
COPY --from=publish /app/publish .

# Definir el punto de entrada para la aplicación
EXPOSE 5000
EXPOSE 56581
CMD ["dotnet", "IeIAPI.dll"]