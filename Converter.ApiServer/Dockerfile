#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Converter.ApiServer/Converter.ApiServer.csproj", "Converter.ApiServer/"]
RUN dotnet restore "Converter.ApiServer/Converter.ApiServer.csproj"
COPY . .
WORKDIR "/src/Converter.ApiServer"
RUN dotnet build "Converter.ApiServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Converter.ApiServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Converter.ApiServer.dll"]