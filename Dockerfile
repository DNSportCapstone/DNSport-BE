# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore packages
COPY Presentation/Presentation.csproj ./Presentation/
WORKDIR /src/Presentation
RUN dotnet restore

# Copy toàn bộ source code và publish
COPY . .
RUN dotnet publish -c Release -o /app/out

# Giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Presentation.dll"]
