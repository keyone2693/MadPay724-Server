#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MadPay724.Presentation/MadPay724.Presentation.csproj", "MadPay724.Presentation/"]
COPY ["MadPay724.Services/MadPay724.Services.csproj", "MadPay724.Services/"]
COPY ["MadPay724.Repo/MadPay724.Repo.csproj", "MadPay724.Repo/"]
COPY ["MadPay724.Common/MadPay724.Common.csproj", "MadPay724.Common/"]
COPY ["MadPay724.Data/MadPay724.Data.csproj", "MadPay724.Data/"]
RUN dotnet restore "MadPay724.Presentation/MadPay724.Presentation.csproj"
COPY . .
WORKDIR "/src/MadPay724.Presentation"
RUN dotnet build "MadPay724.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MadPay724.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MadPay724.Presentation.dll"]