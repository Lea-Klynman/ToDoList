# Use the official .NET 8.0 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80

# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["ToDoApi.csproj", "./"]
RUN dotnet restore "ToDoApi.csproj"

# Copy the rest of the application
COPY . .
WORKDIR "/src/."
RUN dotnet build "ToDoApi.csproj" -c $configuration -o /app/build

# Publish the application
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ToDoApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Use the final image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApi.dll"]