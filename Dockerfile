FROM mcr.microsoft.com/dotnet/core/sdk:2.2
COPY ./ .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2
COPY --from=0 ConsoleBot/bin/Release/netcoreapp2.1/publish/ app/
ENTRYPOINT ["dotnet", "app/ConsoleBot.dll"]