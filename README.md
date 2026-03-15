# MyAPI -- ASP.NET Core JWT + Realtime Chat API

MyAPI is a demo REST API built with ASP.NET Core that demonstrates:

-   JWT authentication
-   Protected endpoints using `[Authorize]`
-   Realtime messaging with SignalR
-   PostgreSQL database using Entity Framework Core
-   Repository pattern
-   Structured logging
-   Rate limiting
-   Docker support

The goal of this project is to show a clean and modern ASP.NET Core Web
API structure suitable for learning and small backend services.

------------------------------------------------------------------------

## Tech Stack

-   .NET 9
-   ASP.NET Core Web API
-   Entity Framework Core
-   PostgreSQL
-   JWT Authentication
-   SignalR
-   Serilog
-   Swagger / OpenAPI
-   Docker

------------------------------------------------------------------------

## Project Structure

MyAPI/
│
├── Controllers/
│   └── AuthController.cs
│
├── Hubs/
│   └── ChatHub.cs
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
│   ├── SignalRExtensions.cs
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

------------------------------------------------------------------------

## Realtime Chat

Realtime messaging is implemented using SignalR.

Hub endpoint:

/chatHub

Example JavaScript client:

```{=html}
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js"></script>
```
```{=html}
<script>
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(user + ": " + message);
});

connection.start();

function send() {
    connection.invoke("SendMessage", "Tom", "Hello");
}
</script>
```

------------------------------------------------------------------------

## Requirements

-   .NET SDK 9+
-   PostgreSQL
-   Docker (optional)

------------------------------------------------------------------------

## Installation

Clone the repository

git clone https://github.com/vide-coder-droid/myapi-backend.git cd
myapi-backend

Restore dependencies

dotnet restore

------------------------------------------------------------------------

## Environment Variables

JWT secret should be configured using an environment variable.

Windows (PowerShell)

setx JWT_SECRET_KEY "your-secret-key"

Linux / macOS

export JWT_SECRET_KEY="your-secret-key"

------------------------------------------------------------------------

## Database Configuration

Update `appsettings.json`:

"ConnectionStrings": { "DefaultConnection":
"Host=localhost;Port=5432;Database=myapi;Username=postgres;Password=123456"
}

Run migrations:

dotnet ef database update

------------------------------------------------------------------------

## Run the Application

dotnet run

Swagger UI (development only):

http://localhost:5000/swagger

------------------------------------------------------------------------

## API Endpoints

Login

POST /api/auth/login

Request body

{ "username": "admin", "password": "123456" }

Response

{ "token": "JWT_TOKEN" }

Protected endpoint

GET /api/auth/me

Header

Authorization: Bearer `<JWT_TOKEN>`{=html}

------------------------------------------------------------------------

## Logging

Logs are written using Serilog to:

logs/

------------------------------------------------------------------------

## Docker

Build image

docker build -t myapi .

Run container

docker run -p 5000:5000 myapi

------------------------------------------------------------------------

## Notes

This project is intended for learning and demonstration purposes.
Additional security, validation, and architecture improvements should be
applied before using it in production.
