
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS copier
WORKDIR /src
COPY ["Api", "Api/"]
COPY ["Domain", "Domain/"]
COPY ["Data", "Data/"]
WORKDIR /src/Api
RUN dotnet publish "Api.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 as final
LABEL org.opencontainers.image.source https://github.com/11dot001001/TelegramBot
ARG TelegramTokenArg
ENV TelegramToken=$TelegramTokenArg
WORKDIR /app
COPY --from=copier /app .
ENTRYPOINT ["dotnet", "Api.dll"]