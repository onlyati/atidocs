#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AtiDocs/AtiDocs.csproj", "AtiDocs/"]
RUN dotnet restore "AtiDocs/AtiDocs.csproj"
COPY . .
WORKDIR "/src/AtiDocs"
RUN dotnet build "AtiDocs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AtiDocs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AtiDocs.dll"]