FROM mcr.microsoft.com/dotnet/sdk:3.1 AS base
WORKDIR /app
COPY . .
RUN ["dotnet", "restore"]
COPY . ./
RUN dotnet build "Diffing API.csproj" -c Release -o /app/build
RUN dotnet publish "Diffing API.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
COPY --from=base /app/publish .
ENTRYPOINT ["dotnet", "Diffing API.dll"]