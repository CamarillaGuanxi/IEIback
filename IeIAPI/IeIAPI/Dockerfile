FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./IeIAPI.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build
# Instala librerías necesarias
RUN apt-get update && apt-get install -y libglib2.0-0
# Use ADD to copy CSV and XML files
COPY ./CV.csv .
COPY ./CAT.xml .
# Debugging commands
RUN ls -al /app
# Fase de publicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la fase de publicación
COPY --from=publish /app/publish .

# Definir el punto de entrada para la aplicación
EXPOSE 5000
EXPOSE 56581
CMD ["dotnet", "IeIAPI.dll"]