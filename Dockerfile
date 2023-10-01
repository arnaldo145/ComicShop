FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

ARG COMIC_SHOP_API_PORT=80
EXPOSE ${COMIC_SHOP_API_PORT}

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
# Copia todos os projetos para dentro do workdir
COPY ./ComicShop.sln ./ComicShop.sln
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

RUN dotnet restore
COPY . .
WORKDIR /src/ComicShop.WebApi

FROM build AS publish
RUN dotnet publish "ComicShop.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
ARG BaseUrl
ENV BaseUrl=BaseUrl

WORKDIR /app

COPY --from=publish /app/publish .

CMD ["dotnet", "ComicShop.WebApi.dll"]