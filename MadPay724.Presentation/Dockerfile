# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /Presentation

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /Presentation
EXPOSE 80
COPY --from=build-env /Presentation/out .
ENTRYPOINT ["dotnet", "MadPay724.Presentation.dll"]




