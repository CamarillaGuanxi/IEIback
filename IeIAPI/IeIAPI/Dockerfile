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

# Etapa intermedia para instalar dependencias
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS install-dependencies
WORKDIR /app

# Instalar dependencias, por ejemplo, curl, gnupg, y libglib2.0-0
RUN apt-get update && apt-get install -y curl gnupg libglib2.0-0

# Descargar e instalar Google Chrome
RUN curl -sSL https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/google-chrome-archive-keyring.gpg
RUN echo "deb [signed-by=/usr/share/keyrings/google-chrome-archive-keyring.gpg arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" | tee /etc/apt/sources.list.d/google-chrome.list > /dev/null
RUN apt-get update && apt-get install -y google-chrome-stable

# Etapa de publicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la fase de publicación
COPY --from=publish /app/publish .

# Copiar los archivos de la etapa de instalación
COPY --from=install-dependencies /usr/bin/google-chrome-stable /usr/bin/

# Definir el punto de entrada para la aplicación
EXPOSE 5000
EXPOSE 56581
CMD ["dotnet", "IeIAPI.dll"]
