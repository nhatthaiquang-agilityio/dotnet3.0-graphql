FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY dotnet-graphql.csproj ./dotnet-graphql/
RUN dotnet restore ./dotnet-graphql/dotnet-graphql.csproj

# copy everything else and build app
COPY . ./dotnet-graphql/
WORKDIR /app/dotnet-graphql
RUN dotnet publish dotnet-graphql.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/dotnet-graphql/out ./
ENTRYPOINT ["dotnet", "dotnet-graphql.dll"]