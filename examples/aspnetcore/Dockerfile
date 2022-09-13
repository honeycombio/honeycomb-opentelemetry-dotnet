# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY . ./
# RUN dotnet publish -c release -f net6.0 -o /out
RUN dotnet publish -c release -f net5.0 -o /out

# final stage/image
# FROM mcr.microsoft.com/dotnet/sdk:6.0
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT="development"
EXPOSE 5001
ENTRYPOINT ["dotnet", "aspnetcore.dll"]