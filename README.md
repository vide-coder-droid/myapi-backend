# 🔐 JWT Authentication Demo – ASP.NET Core API

Demo xây dựng **REST API bằng ASP.NET Core**, sử dụng **JWT (JSON Web Token)** để xác thực người dùng và bảo vệ endpoint.

Project minh họa cách triển khai **JWT Authentication trong .NET API** với:

- Login tạo JWT token
- Bảo vệ endpoint bằng `[Authorize]`
- Repository pattern
- Entity Framework Core + PostgreSQL
- Rate limiting và logging

---

## 🎯 Mục tiêu dự án

- Hiểu cách **JWT hoạt động trong ASP.NET Core API**
- Xây dựng API có **xác thực JWT**
- Triển khai **Repository Pattern**
- Sử dụng **Entity Framework Core + PostgreSQL**
- Tổ chức project theo **structure clean cho Web API**

---

## 🧱 Công nghệ sử dụng

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Serilog (Logging)
- Swagger / OpenAPI
- Docker

---

## 📁 Project Structure

```
MyAPI/
├── Controllers/
│   └── AuthController.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── Seed/
│       └── DbSeeder.cs
│
├── Extensions/
│   ├── CorsExtensions.cs
│   ├── DatabaseExtensions.cs
│   ├── JwtExtensions.cs
│   ├── LoggingExtensions.cs
│   ├── MigrationExtensions.cs
│   ├── PortExtensions.cs
│   ├── RateLimitExtensions.cs
│   ├── ServiceExtensions.cs
│   └── SwaggerExtensions.cs
│
├── Models/
│   ├── Entities/
│   │   └── User.cs
│   │
│   └── Requests/
│       └── LoginRequest.cs
│
├── Repositories/
│   ├── IUserRepository.cs
│   └── DbUserRepository.cs
│
├── Services/
│   └── JwtService.cs
│
├── Migrations/
│
├── Dockerfile
├── Program.cs
├── appsettings.json
└── MyAPI.csproj
```

---

# 📌 MyAPI – ASP.NET Core JWT Demo

Demo REST API sử dụng **ASP.NET Core + JWT Authentication** để xác thực người dùng và bảo vệ endpoint.

---

## Yêu cầu

- .NET SDK 8+
- PostgreSQL
- Docker (optional)

---

## Cài đặt

Clone project:

```
git clone https://github.com/vide-coder-droid/myapi-backend.git
cd myapi-backend
```

Restore packages:

```
dotnet restore
```

---

## Cấu hình Environment Variables

JWT Secret nên được cấu hình bằng **environment variable**.

### Windows (PowerShell)

```
setx JWT_SECRET_KEY "your-secret-key"
```

### Linux / macOS

```
export JWT_SECRET_KEY="your-secret-key"
```

---

## Cấu hình Database

Sửa `appsettings.json`:

```
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=myapi;Username=postgres;Password=123456"
}
```

---

## Chạy Migration

```
dotnet ef database update
```

---

## Chạy ứng dụng

```
dotnet run
```

Swagger UI:

```
http://localhost:5000/swagger
```

---

## API chính

### Login

```
POST /api/auth/login
```

Body:

```
{
  "username": "admin",
  "password": "123456"
}
```

Response:

```
{
  "token": "JWT_TOKEN"
}
```

---

### Protected Endpoint

```
GET /api/auth/me
```

Header:

```
Authorization: Bearer <JWT_TOKEN>
```

---

## Logging

Logs được ghi bằng **Serilog** vào thư mục:

```
logs/
```

---

## Docker

Build image:

```
docker build -t myapi .
```

Run container:

```
docker run -p 5000:5000 myapi
```

---

## Lưu ý

Project này được xây dựng **cho mục đích học tập và demo**, không dùng trực tiếp cho production.
