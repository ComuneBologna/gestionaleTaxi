#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update -y && apt-get install -y libgdiplus
RUN apt-get update -y && apt-get install -y libfontconfig
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Asf.Taxi/nuget.config", "."]
COPY ["Asf.Taxi/Asf.Taxi.API/Asf.Taxi.API.csproj", "Asf.Taxi.API/"]
COPY ["Asf.Taxi/Asf.Taxi.BusinessLogic/Asf.Taxi.BusinessLogic.csproj", "Asf.Taxi.BusinessLogic/"]
COPY ["Asf.Taxi/Asf.Taxi.DAL/Asf.Taxi.DAL.csproj", "Asf.Taxi.DAL/"]
RUN dotnet restore "Asf.Taxi.API/Asf.Taxi.API.csproj"
COPY . .
WORKDIR "/src/Asf.Taxi/Asf.Taxi.API"
RUN dotnet build "Asf.Taxi.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asf.Taxi.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asf.Taxi.API.dll"]