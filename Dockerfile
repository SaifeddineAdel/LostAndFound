FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["LostAndFound.Domain/LostAndFound.Domain.csproj", "LostAndFound.Domain/"]
COPY ["LostAndFound.Application/LostAndFound.Application.csproj", "LostAndFound.Application/"]
COPY ["LostAndFound.Infrastructure/LostAndFound.Infrastructure.csproj", "LostAndFound.Infrastructure/"]
COPY ["LostAndFound.Api/LostAndFound.Api.csproj", "LostAndFound.Api/"]

RUN dotnet restore "LostAndFound.Api/LostAndFound.Api.csproj"

COPY . .

WORKDIR /src/LostAndFound.Api

RUN dotnet publish "LostAndFound.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LostAndFound.Api.dll"]