FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
#Define o timezone do container como America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive
ENV	TZ=America/Sao_Paulo
RUN apt-get update && \
	apt-get -y install tzdata && \
	dpkg-reconfigure --frontend noninteractive tzdata && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime
WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Alteração no modo como é feito o restore com o objetivo de otimizar o cache dos builds do Docker
COPY ./ComicShop.sln ./ComicShop.sln
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore
COPY . .

WORKDIR /ComicShop.WebApi

FROM build AS publish
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

CMD ["dotnet", "ComicShop.WebApi.dll"]
