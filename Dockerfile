# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copier tous les fichiers du projet
COPY . .

# Restaurer les packages NuGet
RUN dotnet restore

# Compiler et publier l'application en Release
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copier le build publié depuis l'étape précédente
COPY --from=build /app/publish .

# Indiquer le port que l'application doit écouter (Railway utilise 5000)
ENV ASPNETCORE_URLS=http://+:5000

# Lancer l'application
ENTRYPOINT ["dotnet", "Stone1234.dll"]
