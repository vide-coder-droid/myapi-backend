# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MyAPI.sln .
COPY MyAPI/*.csproj ./MyAPI/
RUN dotnet restore

COPY MyAPI/. ./MyAPI/

WORKDIR /src/MyAPI
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MyAPI.dll"]
