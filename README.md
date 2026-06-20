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
    "Key": "addkey-32-char-min",
    "Issuer": "ShopNest",
    "Audience": "ShopNestUsers"
  },
  "Fast2SMS": {
    "ApiKey": "key"
  },
  "Redis": {
    "ConnectionString": "connection string dalo yha pr ",
    "DefaultTtlMinutes": 5
  },
  "Razorpay": {
  "KeyId": "key add karo yha pr repo me mt push karna",
  "KeySecret": "ye wala to bilkul nhi kyu order verification esi key se hoga "
},
  "Cloudinary": {
    "CloudName": "addname",
    "ApiKey": "addkey",
    "ApiSecret": "addkey"
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
## Review Module

Users can post, update, and delete reviews on products.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/review/{productId}` | Get all reviews for a product | No |
| POST | `/api/review` | Add a review | User |
| PUT | `/api/review/{id}` | Update your review | User |
| DELETE | `/api/review/{id}` | Delete your review | User |

### Tech Decisions
- One review per user per product (duplicate check enforced)
- Only owner can update or delete their review
- Product AverageRating and ReviewCount auto-updated on review create
- User loaded via EF Core Include for response

## Cart Module

Users can manage their shopping cart with full CRUD operations.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/cart` | Get user's cart | User |
| POST | `/api/cart/add` | Add item to cart | User |
| PUT | `/api/cart/update` | Update item quantity | User |
| DELETE | `/api/cart/remove/{productId}` | Remove item from cart | User |
| DELETE | `/api/cart/clear` | Clear entire cart | User |

### Tech Decisions
- Every user gets their own cart auto-created on first add
- Grand total and per-item total calculated in service layer
- Duplicate product check — quantity increases if already in cart


## Order Module

Users can place orders directly from their cart with COD or Online payment support.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/order/checkout` | Place order from cart | User |
| GET | `/api/order/my-orders` | Get all my orders | User |
| GET | `/api/order/{id}` | Get order detail | User |
| PATCH | `/api/order/{id}/status` | Update order status | Admin only |

### Tech Decisions
- Order items and price snapshot taken from Cart at checkout time — price manipulation not possible from frontend
- Cart auto-cleared after successful order
- Payment record created automatically on checkout
- COD and Online payment methods supported
- Only Admin can update order status

## Cloudinary Image Upload

All images (Products and Categories) are uploaded to Cloudinary via the backend. Frontend never directly communicates with Cloudinary — API keys stay secure on the server.

### How it works
1. Admin sends image as `multipart/form-data` to the API
2. Backend uploads image to Cloudinary
3. Cloudinary returns a secure URL
4. URL is saved in the database

### Image Optimization
All uploaded images are automatically:
- Resized to max 800x800
- Quality optimized (`auto`)
- Format optimized (`auto` — WebP where supported)

### Folder Structure on Cloudinary
### Endpoints that accept images

| Method | Endpoint | Field Name |
|--------|----------|------------|
| POST | `/api/product` | `Image` |
| PUT | `/api/product/{id}` | `Image` |
| POST | `/api/category` | `Image` |
| PUT | `/api/category/{id}` | `Image` |

> All image endpoints use `multipart/form-data`, not `application/json`.


## User Profile Module / new change in user module-- read thiss

Users can update their profile including phone number, date of birth, and profile picture.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/user/profile` | Get profile | User |
| PUT | `/api/user/profile` | Update profile with image | User |
| PATCH | `/api/user/change-password` | Change password | User |
| DELETE | `/api/user/delete` | Delete account | User |

### Tech Decisions
- Profile image uploaded to Cloudinary — URL saved in Users table
- Phone stored but verified at checkout time
- IsPhoneVerified flag for future OTP implementation

---

## Address Module

Users can manage multiple saved addresses with default address support.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/address` | Get all my addresses | User |
| POST | `/api/address` | Add new address | User |
| PUT | `/api/address/{id}` | Update address | User |
| DELETE | `/api/address/{id}` | Delete address | User |
| PATCH | `/api/address/{id}/default` | Set default address | User |

### Tech Decisions
- Multiple addresses per user supported
- First address automatically set as default
- Default address shown first in list
- Only owner can modify their addresses

## Payment Module (Razorpay)

Secure payment integration using Razorpay. Payment amount is always taken from the database — never from frontend, preventing price manipulation.

### Flow
### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/payment/initiate/{orderId}` | Create Razorpay order | User |
| POST | `/api/payment/verify` | Verify payment signature | User |

### Tech Decisions
- Amount fetched from DB — frontend cannot manipulate price
- HMAC SHA256 signature verification — industry standard
- Payment record auto-updated on success
- COD orders blocked from payment initiation
- Already paid orders blocked from re-initiation

### Testing in Postman (without frontend)

**Step 1 — Register/Login**
```json
POST /api/auth/login
{
  "email": "admin@shopnext.com",
  "password": "Admin@123"
}
```
Copy the JWT token from response.

**Step 2 — Add to Cart**
```json
POST /api/cart/add
Authorization: Bearer <token>
{
  "productId": 1,
  "quantity": 2
}
```

**Step 3 — Place Order**
```json
POST /api/order/checkout
Authorization: Bearer <token>
{
  "shippingAddress": "123 Delhi, India",
  "paymentMethod": "Online"
}
```
Note the `id` from response.

**Step 4 — Initiate Payment**

Note the `razorpayOrderId` from response.

**Step 5 — Verify Payment**
```json
POST /api/payment/verify
Authorization: Bearer <token>
{
  "orderId": 4,
  "razorpayOrderId": "order_xxx",
  "razorpayPaymentId": "pay_xxx",
  "razorpaySignature": "invalid_for_test"
}
```
Expected: `"Invalid payment signature"` — correct behavior without real frontend.

## Search & Filter Module

Products can be searched and filtered with pagination support.

### Endpoint

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/product/search` | Search and filter products | No |

### Query Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| keyword | string | Search in name and description |
| categoryId | int | Filter by category |
| minPrice | decimal | Minimum price filter |
| maxPrice | decimal | Maximum price filter |
| sortBy | string | price_asc, price_desc, rating, reviews, newest |
| page | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 10, max: 50) |

### Tech Decisions
- Deferred execution — single optimized SQL query
- PageSize capped at 50 — prevents server overload
- Debounce recommended on frontend — 300ms delay

## OTP Verification Module

Phone number verification using OTP before order placement.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/otp/send` | Send OTP to registered phone | User |
| POST | `/api/otp/verify` | Verify OTP | User |

### Security
- 1 minute cooldown between OTP requests
- Max 5 requests per hour per phone
- 3 wrong attempts → 15 minute block
- OTP stored as BCrypt hash — never plain text
- OTP expires in 5 minutes
- One-time use only

### Tech Decisions
- Phone number taken from JWT — not from request body
- IsPhoneVerified flag updated on successful verification
- Phone change resets verification status
- SMS provider: Console mode (development) — MSG91 recommended for production

### Order Checkout Protection
- Phone number required before order
- Phone must be verified before order
- Unverified users get 400 error at checkout

## Banner Module

Homepage promotional banners for the storefront. Admin can upload banner images, and frontend displays them in an auto-sliding carousel.

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/banner` | Get all active banners | No |
| POST | `/api/banner` | Upload new banner image | Admin only |
| DELETE | `/api/banner/{id}` | Soft delete banner | Admin only |

### Tech Decisions

- Banner images uploaded to :contentReference[oaicite:0]{index=0} via backend
- Only active banners shown on frontend
- Soft delete using `IsActive = false`
- Used for homepage auto-slider

### Model

- Id
- ImageUrl
- IsActive


## Roadmap

- [x] User Auth (Register / Login)
- [x] JWT Authentication
- [x] User Profile (Update + Profile Image)
- [x] Phone Verification Flag
- [x] Address Module (CRUD + Default)
- [x] Role-based Authorization
- [x] Category Module (CRUD + Cloudinary)
- [x] Products Module (CRUD + Cloudinary)
- [x] Review Module (CRUD + Duplicate Protection)
- [x] Cart Module (Add, Update, Remove, Clear)
- [x] Order Module (Checkout, History, Status)
- [x] Cloudinary Image Upload (Products + Categories + Profiles)
- [x] Razorpay Payment Integration (Initiate + Verify + Signature Validation)
- [x] Search & Filter (Keyword, Category, Price, Sort, Pagination)
- [x] OTP Verification (Rate Limited + BCrypt hashed)
- [x] Phone Verification at Checkout
- [ ] Rate Limiting ------ > Implement global rate limiting to prevent abuse (e.g. 100 requests per minute per IP)
- [ ] Admin Dashboard (ASP.NET Core MVC or Blazor) for managing products, categories, orders/ 
- [ ] React Frontend--- 

## Author

**Shivendra Pratap Singh**
- GitHub: [@imshivendra29](https://github.com/imshivendra29)
- Portfolio: [shivendrasingh.vercel.app](https://shivendrasingh.vercel.app)