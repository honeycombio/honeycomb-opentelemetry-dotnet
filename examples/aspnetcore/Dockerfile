FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet publish -c release -f net6.0 -o /out

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=https://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "aspnetcore.dll"]