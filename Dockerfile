# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore packages
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code và publish
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/out .

# Mở cổng 80
EXPOSE 80

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Presentation.dll"]
