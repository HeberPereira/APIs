#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Staging

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["hb29.WorkerService/hb29.WorkerService.csproj", "hb29.WorkerService/"]
RUN dotnet restore "hb29.WorkerService/hb29.WorkerService.csproj"
COPY . .
WORKDIR "/src/hb29.WorkerService"
RUN dotnet build "hb29.WorkerService.csproj" -c Staging -o /app/build

FROM build AS publish
RUN dotnet publish "hb29.WorkerService.csproj" -c Staging -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "hb29.WorkerService.dll"]