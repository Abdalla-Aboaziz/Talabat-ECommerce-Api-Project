# Talabat ECommerce — Full Stack .NET Solution

A production-grade solution built with **ASP.NET Core 8**, implementing **Clean Architecture (Onion Architecture)** end-to-end. The solution includes a full-featured **RESTful API** covering the entire e-commerce domain, plus a complete **Admin MVC Dashboard** — both living in the same solution and sharing the same Core and Infrastructure layers.

---

## Table of Contents

- [Why This Project](#why-this-project)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Part 1 — ECommerce REST API](#part-1--ecommerce-rest-api)
  - [Key Topics Covered](#key-topics-covered)
  - [Features](#features)
- [Part 2 — Admin Dashboard MVC](#part-2--admin-dashboard-mvc)
  - [Admin Features](#admin-features)
  - [Admin Project Structure](#admin-project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Database Setup](#database-setup)
- [Error Handling](#error-handling)
- [Author](#author)

---

## Why This Project

Most tutorial projects stop at CRUD. This one doesn't.

- **Zero exceptions for control flow** — custom `Result<T>` / `Error` pattern used throughout the service layer
- **Specification pattern** for composable, reusable, testable queries — no raw LINQ leaking into controllers
- **Generic Repository + Unit of Work** cleanly abstracting all data access
- **Redis caching** implemented as a reusable `ActionFilter` attribute, not hardcoded per-endpoint
- **Stripe PaymentIntent lifecycle** handled correctly, including the edge case where a PaymentIntent is already succeeded
- **Two isolated databases** — store data and identity data never share a context
- **Auto-migration and seeding on startup** — the app is always in a valid state from first run
- **Admin Dashboard** built as a separate MVC project in the same solution — shares Domain, Service, and Persistence layers with zero code duplication

---

## Architecture

The project enforces **Onion Architecture** — dependencies only point inward. The Domain layer has zero external dependencies.

```
ECommerce Solution  (8 Projects)
│
├── ApplicationCoreLayer
│   ├── ECommerce.Domain               ← Entities, Contracts, Interfaces  (innermost)
│   ├── ECommerce.Service              ← Business Logic, Mapping, Specifications
│   └── ECommerce.ServiceAbstraction   ← Service Interfaces (dependency inversion)
│
├── InfrastructureLayer
│   └── ECommerce.Persistence          ← EF Core, Repositories, Migrations, Seeding
│
├── PresentationLayer
│   └── ECommerce.Presentation         ← API Controllers, Action Filters
│
├── UILayer
│   └── Admin.Dashboard                ← MVC Admin Dashboard
│
├── ECommerceWeb                       ← API Entry point: Program.cs, Middleware, appsettings
│
└── ECommerce.Shared                   ← DTOs, Result Pattern, Query Params
```

### Design Patterns

| Pattern | Where Applied |
|---|---|
| Onion Architecture | Full solution structure |
| Generic Repository | `GenericRepository<TEntity, TKey>` |
| Unit of Work | `UnitOfWork` coordinating all repositories |
| Specification Pattern | All product and order queries |
| Result Pattern | Service layer error handling without exceptions |
| Action Filter | Redis response caching (`RedisCasheAttribute`) |
| MVC Pattern | Admin Dashboard — Controllers / Views / ViewModels |

---

## Technology Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API + MVC |
| ORM | Entity Framework Core — Code First |
| Database | SQL Server |
| Cache / Basket | Redis via StackExchange.Redis |
| Authentication (API) | ASP.NET Core Identity + JWT Bearer |
| Authentication (Dashboard) | ASP.NET Core Identity + Cookie Auth |
| Payment | Stripe.net SDK |
| Object Mapping | AutoMapper with custom `IValueResolver` |
| UI Framework | Bootstrap 5 + Chart.js |
| Documentation | Swagger / OpenAPI |

---

## Project Structure

```
ECommerce.Domain                          [ApplicationCoreLayer]
├── Contracts
│   ├── IBasketRepository.cs
│   ├── ICachRepository.cs
│   ├── IDataInitializer.cs
│   ├── IGenericRepository.cs
│   ├── ISpecification.cs
│   └── IUnitOfWork.cs
├── Entities
│   ├── BasketModules
│   │   ├── BasketItem.cs
│   │   └── CustomerBasket.cs
│   ├── IdentityModule
│   │   ├── Address.cs
│   │   └── ApplicationUser.cs
│   ├── OrderModules
│   │   ├── DeliveryMethod.cs
│   │   ├── Order.cs
│   │   ├── OrderAddress.cs
│   │   ├── OrderItems.cs
│   │   ├── OrderStatus.cs
│   │   └── ProductItemOrder.cs
│   └── ProductModules
│       ├── Product.cs
│       ├── ProductBrand.cs
│       └── ProductType.cs
├── BaseEntity.cs
└── GlobalUsings.cs

ECommerce.Service                         [ApplicationCoreLayer]
├── Exceptions
│   └── NotFoundException.cs
├── MappingProfiles
│   ├── AuthProfile.cs
│   ├── BasketProfile.cs
│   ├── OrderItemPictureUrlResolver.cs
│   ├── OrderProfile.cs
│   ├── ProductPictureResolver.cs
│   └── ProductProfile.cs
├── Specifications
│   ├── BaseSpecification.cs
│   ├── OrderSpecifications.cs
│   ├── OrderWithPaymentIntentSpecification.cs
│   ├── ProductCountSpecifications.cs
│   └── ProductWithBrandsAndTypeSpecification.cs
├── AuthenticationSerivce.cs
├── BasketServices.cs
├── CashService.cs
├── OrderService.cs
├── PaymentService.cs
├── ProductService.cs
└── ServiceAssemplyRefrence.cs

ECommerce.ServiceAbstraction              [ApplicationCoreLayer]
├── IAuthenticationSerivce.cs
├── IBasketServices.cs
├── ICashService.cs
├── IOrderService.cs
├── IPaymentService.cs
└── IProductServices.cs

ECommerce.Persistence                     [InfrastructureLayer]
├── Data
│   ├── Configurations
│   │   ├── DeliveryMethodConfigration.cs
│   │   ├── OrderConfigration.cs
│   │   ├── OrderItemConfigration.cs
│   │   └── ProductConfigration.cs
│   ├── DataSeeding
│   │   ├── JSONFiles
│   │   │   ├── brands.json
│   │   │   ├── delivery.json
│   │   │   ├── products.json
│   │   │   └── types.json
│   │   └── DataIntializer.cs
│   ├── DBContexts
│   │   └── StoreDbContext.cs
│   └── Migrations
│       ├── 20260119210137_ProductModuleTables.cs
│       ├── 20260217101440_OrderModule.cs
│       ├── 20260306201928_AddOrderAddressColumns.cs
│       └── StoreDbContextModelSnapshot.cs
├── IdentityData
│   ├── DataSeed
│   │   └── IdentityDataInitalizer.cs
│   ├── DbContext
│   │   └── StoreIdentityDbContext.cs
│   └── Migrations
│       ├── 20260205150500_Identity Migration.cs
│       └── StoreIdentityDbContextModelSnapshot.cs
├── Repository
│   ├── BasketRepository.cs
│   ├── CachRepository.cs
│   ├── GenericRepository.cs
│   └── UnitOfWork.cs
└── SpecificationEvaluator.cs

ECommerce.Presentation                    [PresentationLayer]
├── Attributes
│   └── RedisCasheAttribute.cs
└── Controllers
    ├── ApiBaseController.cs
    ├── accountsController.cs
    ├── BasketController.cs
    ├── ordersController.cs
    ├── PaymentsController.cs
    └── ProductsController.cs

Admin.Dashboard                           [UILayer]
├── Controllers
│   ├── AdminController.cs          ← Login / Logout (Cookie Auth + Role Guard)
│   ├── HomeController.cs           ← Dashboard stats (ProductCount, UserCount)
│   ├── ProductsController.cs       ← Full CRUD + image upload/delete
│   ├── ProductBrandsController.cs  ← Create / Delete brands
│   ├── ProductTypesController.cs   ← Create / Delete types
│   ├── RolesController.cs          ← Create / Edit / Delete roles
│   └── UsersController.cs          ← List users + assign/remove roles
├── Helpers
│   └── PictureSettings.cs          ← UploadFile / DeleteFile helper
├── Models
│   ├── Products
│   │   └── ProductViewModel.cs
│   ├── Roles
│   │   ├── RoleViewModel.cs
│   │   └── UpdateRoleViewModel.cs
│   ├── Users
│   │   ├── UserViewModel.cs
│   │   └── UserRoleViewModel.cs
│   └── ErrorViewModel.cs
├── Views
│   ├── Admin
│   │   └── Login.cshtml
│   ├── Home
│   │   ├── Index.cshtml            ← Dashboard with stat cards + charts
│   │   └── Privacy.cshtml
│   ├── ProductBrands
│   │   ├── CreateBrandPartialView.cshtml
│   │   └── Index.cshtml
│   ├── Products
│   │   ├── Create.cshtml
│   │   ├── CreateEditProductPartialView.cshtml
│   │   ├── Delete.cshtml
│   │   ├── Edit.cshtml
│   │   └── Index.cshtml
│   ├── ProductTypes
│   │   ├── CreateTypePartialView.cshtml
│   │   └── Index.cshtml
│   ├── Roles
│   │   ├── CreateRolePartialView.cshtml
│   │   ├── Edit.cshtml
│   │   └── Index.cshtml
│   ├── Shared
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   └── Users
│       ├── Edit.cshtml
│       └── Index.cshtml
├── wwwroot
│   ├── css / js / images / lib
│   └── favicon.ico
├── appsettings.json
└── Program.cs

ECommerceWeb                              [API Entry Point]
├── CustomeMiddleWare
│   └── ExceptionHandlerMiddleWare.cs
├── Extentions
│   └── WebApplicationRegistration.cs
├── Factory
│   └── ApiResponceFactory.cs
├── GlobalUsings.cs
├── Program.cs
├── appsettings.json
└── appsettings.Development.json

ECommerce.Shared                          [Cross-cutting]
├── BasketDtos
│   ├── BasketDtos.cs
│   └── BasketItemDtos.cs
├── CommonResult
│   ├── Error.cs
│   ├── ErrorType.cs
│   └── Result.cs
├── IdentityDtos
│   ├── IdentityAddressDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── UserDto.cs
├── OrderDtos
│   ├── AddressDto.cs
│   ├── DeliveryMethodDto.cs
│   ├── OrderDto.cs
│   ├── OrderItemDto.cs
│   └── OrderToReturnDto.cs
├── ProductDtos
│   ├── PaginatedResult.cs
│   ├── ProductBrandDTO.cs
│   ├── ProductDTO.cs
│   └── ProductTypeDTO.cs
├── ProductQueryParams.cs
└── ProductSortingOptions.cs
```

---

## Part 1 — ECommerce REST API

### Key Topics Covered

| # | Topic | Implementation |
|---|---|---|
| 1 | ASP.NET Web APIs Overview | RESTful API design, HTTP verbs, status codes |
| 2 | Postman & Swagger Documentation | Full OpenAPI docs with JWT authorization support |
| 3 | RESTful APIs | Resource-based routing, stateless design |
| 4 | Onion Architecture | 8-project solution with strict inward dependency rule |
| 5 | Generic Repository | `IGenericRepository<TEntity, TKey>` + `GenericRepository<T, TKey>` |
| 6 | Products Module | Full catalog with brand/type relations and image URL resolution |
| 7 | Specification Design Pattern | `BaseSpecification`, `ProductWithBrandsAndTypeSpecification`, `ProductCountSpecification`, `OrderSpecifications` |
| 8 | AutoMapper | Profile-based mapping with custom `IValueResolver` for dynamic image URLs |
| 9 | API Error Handling | Global `ExceptionHandlerMiddleWare` + `Result<T>` pattern + ProblemDetails |
| 10 | Paging, Filtering, Sorting & Searching | `ProductQueryParams` with server-side pagination (configurable max page size) |
| 11 | Redis | Basket persistence + response caching via `RedisCasheAttribute` action filter |
| 12 | JWT Token Creation | HS256-signed tokens with email, username, and role claims |
| 13 | Security — Authentication & Authorization | ASP.NET Core Identity, role seeding (Admin / SuperAdmin), `[Authorize]` |
| 14 | Unit of Work | `UnitOfWork` coordinating all repositories in a single transaction scope |
| 15 | Orders Module | Full order lifecycle: create, validate, retrieve, map to DTOs |
| 16 | Payment Module | Stripe PaymentIntent creation, update, succeeded-state edge case, webhook |
| 17 | Caching | Redis cache attribute applied declaratively at controller action level |

### Features

#### Product Catalog
- Paginated product listing with filtering by brand, type, and free-text search
- Server-side sorting: name ascending/descending, price ascending/descending
- Dynamic image URL resolution using AutoMapper `IValueResolver`
- Redis response caching via `[RedisCashe]` action filter attribute

#### Authentication & Authorization (API)
- User registration and login — both return a signed JWT token
- Role-based access: Admin, SuperAdmin (seeded automatically)
- Get and update authenticated user's address
- Email existence check before registration

#### Shopping Basket
- Redis-backed basket — fast reads, automatic expiry
- Stores items, selected delivery method, Stripe PaymentIntentId, and calculated shipping cost

#### Order Management
- Create order from basket with shipping address and delivery method
- Duplicate order prevention using PaymentIntentId uniqueness check
- Retrieve all orders or a specific order for the authenticated user
- List available delivery methods

#### Payment Processing
- Stripe PaymentIntent creation with calculated order total
- Updates existing PaymentIntent if basket changes
- Detects already-succeeded PaymentIntents and creates a fresh one instead of failing
- Stripe webhook endpoint ready for event handling

#### Error Handling
- `ExceptionHandlerMiddleWare` catches all unhandled exceptions globally
- `Result<T>` / `Error` pattern — no exceptions used for expected failures
- `ApiResponceFactory` for consistent ModelState validation responses
- All responses conform to RFC 7807 ProblemDetails

---

## Part 2 — Admin Dashboard MVC

A fully functional Admin Dashboard built with **ASP.NET Core MVC**, living in its own `UILayer` project inside the same solution. It directly references `ECommerce.Domain`, `ECommerce.Persistence`, and `ECommerce.Shared` — **zero code duplication** with the API.

### Admin Features

#### Authentication (Cookie-based)
- Separate login page with email/password
- Role guard: only `Admin` or `SuperAdmin` can access the dashboard
- Login checks role **before** validating password — unauthorized users are rejected early
- Logout with `SignOutAsync()`


#### Dashboard Home
- Live stat cards: Total Products, Total Users, Total Orders, Revenue
- Sales Overview — Chart.js line chart (Jan → Jun)
- Products by Category — Chart.js doughnut chart with 5 categories
- Stats loaded via `IUnitOfWork` and `UserManager` directly in `HomeController`


#### Product Management (Full CRUD)
- Paginated product listing with Brand and Type info
- **Create**: upload product image → saved to API's `wwwroot/images/products` via `PictureSettings.UploadFile()`
- **Edit**: if a new image is uploaded, the old one is deleted from the server first, then the new one is saved
- **Delete**: removes the product image from the server before deleting the DB record
- Uses `IUnitOfWork` + `ProductWithBrandsAndTypeSpecification` (shared Specification with the API)


#### Product Brands & Types Management
- List all brands / types
- Create new brand or type (inline partial view form)
- Delete brand or type by ID

#### Role Management
- List all roles
- Create new role with duplicate name check
- Edit role name — checks for duplicate before saving
- Delete role by ID


#### User Management
- List all users with their assigned roles
- Edit user: assign or remove roles via checkboxes
- Computes `rolesToAdd` and `rolesToRemove` by diffing current roles vs submitted form


#### PictureSettings Helper

```csharp
public static class PictureSettings
{
    // Saves IFormFile to wwwroot/images/{folder}/
    public static string UploadFile(IFormFile file, string folder, string wwwRootPath) { ... }

    // Deletes file from wwwroot/images/{folder}/
    public static void DeleteFile(string folder, string fileName, string wwwRootPath) { ... }
}
```

### Admin Dashboard Flow

```
Login (Cookie Auth + Role Guard)
    ↓
Home — Dashboard (ProductCount, UserCount, Charts)
    ↓
├── Products      → Index / Create / Edit / Delete  (+ image upload/delete)
├── ProductBrands → Index / Create / Delete
├── ProductTypes  → Index / Create / Delete
├── Roles         → Index / Create / Edit / Delete
└── Users         → Index / Edit (role assignment)
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or Express)
- Redis Server
- Stripe account (test keys sufficient)

### Setup

```bash
# 1. Clone
git clone https://github.com/Abdalla-Aboaziz/Talabat-ECommerce-Api-Project.git
cd Talabat-ECommerce-Api-Project

# 2. Start Redis (Docker)
docker run -d -p 6379:6379 redis

# 3. Update appsettings.json for both projects (see Configuration below)

# 4. Run the API — migrations and seeding execute automatically on first startup
cd ECommerceWeb
dotnet run

# 5. Run the Admin Dashboard
cd Admin.Dashboard
dotnet run
```

- Swagger UI: `https://localhost:7097/swagger`
- Admin Dashboard: `https://localhost:7196`

---

## Configuration

### ECommerceWeb/appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerce;Trusted_Connection=true;TrustServerCertificate=true",
    "RedisConnection": "localhost",
    "IdentityConnection": "Server=.;Database=ECommerceIdentity;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "URLs": {
    "BaseURL": "https://localhost:7097"
  },
  "JwtOptions": {
    "secretKey": "your_secret_key_minimum_32_characters",
    "issuer": "https://localhost:7097",
    "audience": "https://localhost:7097"
  },
  "StripeOption": {
    "SecretKey": "sk_test_your_stripe_secret_key"
  }
}
```

### Admin.Dashboard/appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerce;Trusted_Connection=true;TrustServerCertificate=true",
    "IdentityConnection": "Server=.;Database=ECommerceIdentity;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "URLs": {
    "BaseURL": "https://localhost:7097",
    "ApiWwwRoot": ""
  }
}
```

> `URLs:BaseURL` is used by AutoMapper resolvers (`ProductPictureResolver`, `OrderItemPictureUrlResolver`) to build absolute image URLs dynamically.

> `URLs:ApiWwwRoot` is used by `PictureSettings` in the Admin Dashboard to save and delete product images inside the API's `wwwroot/images/products` folder.

---

## API Endpoints

### Products

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/products` | No | Paginated products with filters |
| GET | `/api/products/{id}` | No | Single product by ID |
| GET | `/api/products/brands` | No | All product brands |
| GET | `/api/products/types` | No | All product types |

**Query parameters for `GET /api/products`:**

| Param | Type | Description |
|---|---|---|
| brandId | int? | Filter by brand |
| typeId | int? | Filter by type |
| search | string? | Name contains search |
| sortingOptions | enum | 1=NameAsc, 2=NameDesc, 3=PriceAsc, 4=PriceDesc |
| pageIndex | int | Default: 1 |
| pageSize | int | Default: 5, Max: 10 |

### Accounts

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/accounts/login` | No | Login — returns JWT token |
| POST | `/api/accounts/register` | No | Register — returns JWT token |
| GET | `/api/accounts/emailexist?email=` | No | Check email availability |
| GET | `/api/accounts/currentuser` | Yes | Get authenticated user info |
| GET | `/api/accounts/address` | Yes | Get user's saved address |
| PUT | `/api/accounts/address` | Yes | Update user's address |

### Basket

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/basket?id={basketId}` | No | Get basket by ID |
| POST | `/api/basket` | No | Create or update basket |
| DELETE | `/api/basket/{id}` | No | Delete basket |

### Orders

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/orders` | Yes | Create order from basket |
| GET | `/api/orders` | Yes | Get all orders for current user |
| GET | `/api/orders/{id}` | Yes | Get specific order by GUID |
| GET | `/api/orders/deliverymethods` | No | List available delivery methods |

### Payments

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/payments/{basketId}` | No | Create or update Stripe PaymentIntent |
| POST | `/api/payments/webhook` | No | Stripe webhook event handler |

---

## Authentication

### API — JWT Bearer (HS256)

```
POST /api/accounts/login
→ { "token": "eyJhbGci..." }
```

Include in all protected requests:

```
Authorization: Bearer eyJhbGci...
```

Token claims: `email`, `username`, `roles`. Expires in 1 hour.

### Admin Dashboard — Cookie Authentication

```
POST /Admin/Login
→ Sets authentication cookie → redirects to Home/Index
```

- Only users with `Admin` or `SuperAdmin` roles can log in
- Role is checked **before** password validation
- Uses `SignInManager.PasswordSignInAsync()` with cookie persistence
- Logout calls `SignOutAsync()` and redirects to Login

---

## Database Setup

Two isolated SQL Server databases:

| Database | Context | Contains |
|---|---|---|
| ECommerce | `StoreDbContext` | Products, Brands, Types, Orders, OrderItems, DeliveryMethods |
| ECommerceIdentity | `StoreIdentityDbContext` | Users, Roles, UserRoles, Addresses |

Both databases are migrated and seeded automatically on API startup via `WebApplicationRegistration` extensions called from `Program.cs`.

Seed data loaded from JSON files in `ECommerce.Persistence/Data/DataSeeding/JSONFiles/`:
`brands.json`, `types.json`, `products.json`, `delivery.json`

### Default Seeded Users

| Email | Password | Role |
|---|---|---|
| abdallaaboaziz@gmail.com | Admin@123 | SuperAdmin |
| AhmedAli@gmail.com | Admin@123 | Admin |

### Add a New Migration

```bash
dotnet ef migrations add MigrationName \
  --project ECommerce.Persistence \
  --startup-project ECommerceWeb
```

---

## Error Handling

All API responses follow **RFC 7807 ProblemDetails**:

```json
{
  "title": "General.NotFound",
  "status": 404,
  "detail": "Resource was not found.",
  "instance": "/api/orders/some-id"
}
```

Validation errors:

```json
{
  "title": "Validation Errors",
  "status": 400,
  "detail": "one or more validation error",
  "Error": {
    "Email": ["The Email field is required."]
  }
}
```

The `Result<T>` / `Error` pattern is used in every service method. No exceptions are thrown for expected failures — making the codebase predictable, easy to trace, and straightforward to test.

### ErrorType HTTP Mappings

| ErrorType | HTTP Status |
|---|---|
| NotFound | 404 |
| Validation | 400 |
| InvalidCredentials | 401 |
| Unauthorized | 401 |
| Forbidden | 403 |
| Failure | 500 |

---

## Author

**Abdalla Aboaziz**

- GitHub: [github.com/Abdalla-Aboaziz]([https://github.com/Abdalla-Aboaziz](https://github.com/Abdalla-Aboaziz))
- LinkedIn: [linkedin.com/in/abdalla-aboaziz]([https://linkedin.com/in/abdalla-aboaziz](https://www.linkedin.com/in/abdalla-aboaziz-13a513331))
- 📧 abdallaaboaziz@gmail.com
