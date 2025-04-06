# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ thư mục chứa project
COPY Presentation/ ./Presentation/

# Làm việc trong thư mục project
WORKDIR /src/Presentation

# Restore package
RUN dotnet restore

# Build & publish app
RUN dotnet publish Presentation.csproj -c Release -o /app/out

# Giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Presentation.dll"]
