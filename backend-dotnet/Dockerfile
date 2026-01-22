FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RideSharingAPI.csproj", "./"]
RUN dotnet restore "RideSharingAPI.csproj"
COPY . .
RUN dotnet build "RideSharingAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RideSharingAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RideSharingAPI.dll"]
