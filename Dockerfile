# Imagem base para runtime .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

ARG COMIC_SHOP_API_PORT=80
EXPOSE ${COMIC_SHOP_API_PORT}

# Imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copia os arquivos do projeto primeiro para otimizar cache do restore
COPY ComicShop.sln ./
COPY */*.csproj ./

# Recria estrutura de diretórios para os csproj
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# Restaurar pacotes antes de copiar todo o código-fonte para melhor aproveitamento do cache
RUN dotnet restore

# Copia o código-fonte restante
COPY . .

WORKDIR /src/ComicShop.WebApi

# Build e publicação
RUN dotnet publish "ComicShop.WebApi.csproj" -c Release -o /app/publish --no-restore

# Imagem final otimizada
FROM base AS final

WORKDIR /app

# Variável de ambiente opcional
ARG BaseUrl
ENV BaseUrl=${BaseUrl}

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Define o comando de entrada
CMD ["dotnet", "ComicShop.WebApi.dll"]
