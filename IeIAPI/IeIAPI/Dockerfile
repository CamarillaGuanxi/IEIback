# Utiliza la imagen de SDK de .NET 6.0 como base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app

# Exponer los puertos que la aplicación utilizará
EXPOSE 80
EXPOSE 443

# Fase de construcción
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY IeIAPI.csproj .
RUN dotnet restore

# Copiar el resto de la aplicación y compilar
COPY . .
RUN dotnet build -c Release -o /app/build


# Copiar el resto de los archivos y compilar la aplicación
COPY . .
RUN dotnet build "IeIAPI.csproj" -c Release -o /app/build

# Fase de publicación
FROM build AS publish
RUN dotnet publish "IeIAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app

# Copiar los archivos publicados desde la fase de publicación
COPY --from=publish /app/publish .

# Definir el punto de entrada para la aplicación
EXPOSE 5000
CMD ["dotnet", "IeIAPI.dll"]

# Etapa final para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "IeIAPI.dll"]