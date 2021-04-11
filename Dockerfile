FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
COPY ./ .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:5.0

COPY --from=build src/ConsoleBot/bin/Release/netcoreapp2.2/publish/ app/
ENTRYPOINT ["dotnet", "app/ConsoleBot.dll"]