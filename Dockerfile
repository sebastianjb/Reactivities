FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

#copy .csproj and restore as distinct layers
COPY "Reactivities.sln" "Reactivities.sln"
COPY "API/API.csproj" "API/API.csproj"
COPY "Application/Application.csproj" "Application/Application.csproj"
COPY "Persistence/Persistence.csproj" "Persistence/Persistence.csproj"
COPY "Domain/Domain.csproj" "Domain/Domain.csproj"
COPY "Infrastructure/Infrastructure.csproj" "Infrastructure/Infrastructure.csproj"
RUN dotnet restore 

#copy everything else build
COPY . .
RUN dotnet publish -c Release --property:PublishDir=/out

#build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "API.dll" ]