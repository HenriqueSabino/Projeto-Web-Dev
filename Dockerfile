# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS Build
WORKDIR /source
COPY . .
RUN dotnet restore "./MyMovieList/MyMovieList.csproj"
RUN dotnet publish "./MyMovieList/MyMovieList.csproj" -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS Stage
WORKDIR /app
COPY --from=Build /app ./

ENV ASPNETCORE_URLS=http://+
ENV ASPNETCORE_ENVIRONMENT Docker

EXPOSE 80

ENTRYPOINT ["dotnet", "MyMovieList.dll"]