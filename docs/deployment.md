# Deployment Guide

## Production URL

```txt
https://shopnext-bz8l.onrender.com
```

## Swagger URL

```txt
https://shopnext-bz8l.onrender.com/swagger
```

---

# Deployment Platform

Application is deployed using:

- Render
- Docker
- GitHub Auto Deploy
- Neon PostgreSQL

---

# Deployment Flow

```txt
GitHub Push
   ↓
Render Detects New Commit
   ↓
Docker Image Build
   ↓
Container Deployment
   ↓
Application Live
```

---

# Docker Configuration

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ShopNext.csproj", "."]
RUN dotnet restore "ShopNext.csproj"

COPY . .
RUN dotnet build "ShopNext.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShopNext.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "ShopNext.dll"]
```

---

# Important Notes

- Production uses dynamic Render port binding.
- HTTPS redirection enabled only for local development.
- Swagger enabled in production.
- Environment variables managed using Render dashboard.
- PostgreSQL database hosted on Neon.

---

# Environment Variables

```env
ConnectionStrings__DefaultConnection=
Jwt__Key=
Jwt__Issuer=
Jwt__Audience=
Cloudinary__CloudName=
Cloudinary__ApiKey=
Cloudinary__ApiSecret=
Razorpay__Key=
Razorpay__Secret=
```
---
# Auto Deployment
Every push to the connected GitHub branch automatically triggers deployment on Render.