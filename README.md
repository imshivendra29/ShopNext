# ShopNest API

A full-stack e-commerce REST API built with ASP.NET Core 8, following clean layered architecture and SOLID principles.

---

## Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server (MSSQL)
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt.Net-Next
- **API Docs:** Swagger / OpenAPI

---

## Architecture

```
Client (Postman / React)
        ↓
ErrorHandling Middleware   ← global exception catch
        ↓
JWT Auth Middleware        ← token validate
        ↓
Controllers               ← HTTP endpoints
        ↓
Services                  ← business logic
        ↓
Repositories              ← database queries
        ↓
SQL Server (ShopNextDb)
```

### Folder Structure

```
ShopNext/
├── Controllers/
│   ├── AuthController.cs       # Register, Login
│   └── UserController.cs       # Profile CRUD
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IUserService.cs
│   └── UserService.cs
├── Repositories/
│   ├── Interfaces/
│   │   └── IUserRepository.cs
│   └── Implementations/
│       └── UserRepository.cs
├── Models/
│   └── User.cs
├── DTOs/
│   ├── Auth/
│   │   ├── RegisterDto.cs
│   │   └── LoginDto.cs
│   └── User/
│       ├── UpdateProfileDto.cs
│       ├── ChangePasswordDto.cs
│       └── UserProfileDto.cs
├── Helpers/
│   └── JwtHelper.cs
├── Middleware/
│   └── ErrorHandlingMiddleware.cs
├── Exceptions/
│   └── AppException.cs
├── Data/
│   └── ShopNextDbContext.cs
└── Program.cs
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 / VS Code

### Setup

1. Clone the repo

```bash
git clone https://github.com/imshivendra29/ShopNext.git
cd ShopNext
```

2. Create `appsettings.json` from the example file

```bash
cp appsettings.example.json appsettings.json
```

3. Fill in your values in `appsettings.json`

```json
{
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "ShopNest",
    "Audience": "ShopNestUsers"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ShopNextDb;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

4. Run migrations

```bash
dotnet ef database update
```

5. Run the project

```bash
dotnet run
```

Swagger UI will be available at: `https://localhost:7089/swagger`

---

## API Reference

### Auth Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login and get JWT token | No |

#### Register

```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Shivendra",
  "email": "shivendra@gmail.com",
  "password": "yourpassword"
}
```

Response:
```json
{
  "token": "eyJhbGci..."
}
```

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "shivendra@gmail.com",
  "password": "yourpassword"
}
```

Response:
```json
{
  "token": "eyJhbGci..."
}
```

---

### User Endpoints

All user endpoints require `Authorization: Bearer <token>` header.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user/profile` | Get logged-in user profile |
| PUT | `/api/user/profile` | Update name and email |
| PATCH | `/api/user/change-password` | Change password |
| DELETE | `/api/user/delete` | Delete account |

#### Get Profile

```http
GET /api/user/profile
Authorization: Bearer eyJhbGci...
```

Response:
```json
{
  "name": "Shivendra",
  "email": "shivendra@gmail.com",
  "role": "User"
}
```

#### Update Profile

```http
PUT /api/user/profile
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{
  "name": "Shivendra Singh",
  "email": "new@gmail.com"
}
```

#### Change Password

```http
PATCH /api/user/change-password
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{
  "currentPassword": "oldpassword",
  "newPassword": "newpassword"
}
```

#### Delete Account

```http
DELETE /api/user/delete
Authorization: Bearer eyJhbGci...
```

---

## Error Handling

All errors return a consistent JSON response:

```json
{
  "message": "Error description here"
}
```

| Status Code | Meaning |
|-------------|---------|
| 400 | Bad request / validation error |
| 401 | Unauthorized / invalid credentials |
| 404 | Resource not found |
| 409 | Conflict (e.g. email already exists) |
| 500 | Internal server error |

---

## JWT Token

After login or register, you receive a JWT token. Use it in all protected requests:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Token contains:
- `uid` — user ID
- `name` — user name
- `email` — user email
- `role` — User / Admin
- `exp` — expiry timestamp

---
## Category Module

Built a full CRUD Category API with clean layered architecture.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/category` | Get all categories | No |
| GET | `/api/category/{id}` | Get category by ID | No |
| POST | `/api/category` | Create category | Admin only |
| PUT | `/api/category/{id}` | Update category | Admin only |
| DELETE | `/api/category/{id}` | Delete category | Admin only |

### Tech Decisions
- Only Admin can create, update, delete categories
- Public GET endpoints — no auth required
- Soft delete via `IsActive` flag supported
## Product Module

Built a full CRUD Product API with Admin-only write access.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/product` | Get all products | No |
| GET | `/api/product/{id}` | Get product by ID | No |
| POST | `/api/product` | Create product | Admin only |
| PUT | `/api/product/{id}` | Update product | Admin only |
| DELETE | `/api/product/{id}` | Delete product | Admin only |

### Tech Decisions
- Admin ID automatically extracted from JWT token on create
- Product response includes Category name via Include/Join
- Stock and IsActive managed separately

## Roadmap

- [x] User Auth (Register / Login)
- [x] JWT Authentication
- [x] Profile CRUD
- [x] Role-based Authorization
- [x] Category Module (CRUD)
- [*] Products Module
- [ ] Cart Module
- [ ] Orders Module
- [ ] PhonePe Payment Integration
- [ ] Cloudinary Image Upload
- [ ] Rate Limiting
- [ ] OTP Verification
---

## Author

**Shivendra Pratap Singh**
- GitHub: [@imshivendra29](https://github.com/imshivendra29)
- Portfolio: [shivendrasingh.vercel.app](https://shivendrasingh.vercel.app)