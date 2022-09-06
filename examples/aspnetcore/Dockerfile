ARG PUBLISH_FRAMEWORK=net5.0

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
COPY . ./
RUN dotnet restore
RUN dotnet publish -c release -f net5.0 -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT="development"
EXPOSE 5001
ENTRYPOINT ["dotnet", "aspnetcore.dll"]