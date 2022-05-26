FROM mcr.microsoft.com/dotnet/sdk:3.1

WORKDIR /app
COPY . .

RUN dotnet restore

# run tests on docker run
ENTRYPOINT ["dotnet", "test"]