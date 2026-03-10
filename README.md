
# Talabat-ECommerce-Api-Project

> A production-grade RESTful API built with **ASP.NET Core 8**, implementing **Clean Architecture (Onion Architecture)** end-to-end. Covers the full e-commerce domain: product catalog, JWT authentication, Redis basket, order lifecycle, Stripe payments, and Redis caching — all wired together with industry-standard design patterns.



---

## Table of Contents

- [Why This Project](#why-this-project)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Key Topics Covered](#key-topics-covered)
- [Features](#features)
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
- **Stripe PaymentIntent lifecycle** handled correctly, including the edge case where a PaymentIntent is already `succeeded`
- **Two isolated databases** — store data and identity data never share a context
- **Auto-migration and seeding on startup** — the app is always in a valid state from first run

---

## Architecture

The project enforces **Onion Architecture** — dependencies only point inward. The Domain layer has zero external dependencies.

```
ECommerce Solution  (7 Projects)
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
│   └── ECommerce.Presentation         ← Controllers, Action Filters
│
├── ECommerceWeb                       ← Entry point: Program.cs, Middleware, appsettings
│
└── ECommerce.Shared                   ← DTOs, Result Pattern, Query Params
```

---

## Technology Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core — Code First |
| Database | SQL Server |
| Cache / Basket | Redis via StackExchange.Redis |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Payment | Stripe.net SDK |
| Object Mapping | AutoMapper with custom `IValueResolver` |
| Documentation | Swagger / OpenAPI |

**Design Patterns:**

| Pattern | Where Applied |
|---|---|
| Onion Architecture | Full solution structure |
| Generic Repository | `GenericRepository<TEntity, TKey>` |
| Unit of Work | `UnitOfWork` coordinating all repositories |
| Specification Pattern | All product and order queries |
| Result Pattern | Service layer error handling without exceptions |
| Action Filter | Redis response caching (`RedisCasheAttribute`) |

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

ECommerceWeb                              [Entry Point]
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

## Key Topics Covered

| # | Topic | Implementation |
|---|---|---|
| 1 | ASP.NET Web APIs Overview | RESTful API design, HTTP verbs, status codes |
| 2 | Postman & Swagger Documentation | Full OpenAPI docs with JWT authorization support |
| 3 | RESTful APIs | Resource-based routing, stateless design |
| 4 | Onion Architecture | 7-project solution with strict inward dependency rule |
| 5 | Generic Repository | `IGenericRepository<TEntity, TKey>` + `GenericRepository<T, TKey>` |
| 6 | Products Module | Full catalog with brand/type relations and image URL resolution |
| 7 | Specification Design Pattern | `BaseSpecification`, `ProductWithBrandsAndTypeSpecification`, `ProductCountSpecification`, `OrderSpecifications` |
| 8 | AutoMapper | Profile-based mapping with custom `IValueResolver` for dynamic image URLs |
| 9 | API Error Handling | Global `ExceptionHandlerMiddleWare` + `Result<T>` pattern + `ProblemDetails` |
| 10 | Paging, Filtering, Sorting & Searching | `ProductQueryParams` with server-side pagination (configurable max page size) |
| 11 | Redis | Basket persistence + response caching via `RedisCasheAttribute` action filter |
| 12 | JWT Token Creation | HS256-signed tokens with email, username, and role claims |
| 13 | Security — Authentication & Authorization | ASP.NET Core Identity, role seeding (Admin / SuperAdmin), `[Authorize]` |
| 14 | Unit of Work | `UnitOfWork` coordinating all repositories in a single transaction scope |
| 15 | Orders Module | Full order lifecycle: create, validate, retrieve, map to DTOs |
| 16 | Payment Module | Stripe PaymentIntent creation, update, succeeded-state edge case, webhook |
| 17 | Caching | Redis cache attribute applied declaratively at controller action level |

---

## Features

### Product Catalog
- Paginated product listing with filtering by brand, type, and free-text search
- Server-side sorting: name ascending/descending, price ascending/descending
- Dynamic image URL resolution using AutoMapper `IValueResolver`
- Redis response caching via `[RedisCashe]` action filter attribute

### Authentication & Authorization
- User registration and login — both return a signed JWT token
- Role-based access: `Admin`, `SuperAdmin` (seeded automatically)
- Get and update authenticated user's address
- Email existence check before registration

### Shopping Basket
- Redis-backed basket — fast reads, automatic expiry
- Stores items, selected delivery method, Stripe PaymentIntentId, and calculated shipping cost

### Order Management
- Create order from basket with shipping address and delivery method
- Duplicate order prevention using PaymentIntentId uniqueness check
- Retrieve all orders or a specific order for the authenticated user
- List available delivery methods

### Payment Processing
- Stripe PaymentIntent creation with calculated order total
- Updates existing PaymentIntent if basket changes
- Detects already-`succeeded` PaymentIntents and creates a fresh one instead of failing
- Stripe webhook endpoint ready for event handling

### Error Handling
- `ExceptionHandlerMiddleWare` catches all unhandled exceptions globally
- `Result<T>` / `Error` pattern — no exceptions used for expected failures
- `ApiResponceFactory` for consistent ModelState validation responses
- All responses conform to RFC 7807 `ProblemDetails`

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

# 3. Update appsettings.json (see Configuration below)

# 4. Run — migrations and seeding execute automatically on first startup
cd ECommerceWeb
dotnet run
```

Open Swagger UI: `https://localhost:7097/swagger`

---

## Configuration

`ECommerceWeb/appsettings.json`:

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

> **Note:** The `URLs:BaseURL` value is used by AutoMapper resolvers (`ProductPictureResolver`, `OrderItemPictureUrlResolver`) to build absolute image URLs dynamically.

---

## API Endpoints

### Products

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/products` | No | Paginated products with filters |
| GET | `/api/products/{id}` | No | Single product by ID |
| GET | `/api/products/brands` | No | All product brands |
| GET | `/api/products/types` | No | All product types |

**Query parameters** for `GET /api/products`:

| Param | Type | Description |
|---|---|---|
| `brandId` | int? | Filter by brand |
| `typeId` | int? | Filter by type |
| `search` | string? | Name contains search |
| `sortingOptions` | enum | 1=NameAsc, 2=NameDesc, 3=PriceAsc, 4=PriceDesc |
| `pageIndex` | int | Default: 1 |
| `pageSize` | int | Default: 5, Max: 10 |

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

JWT Bearer — HS256 signed, expires in **1 hour**.

```
POST /api/accounts/login
→ { "token": "eyJhbGci..." }
```

Include in all protected requests:

```
Authorization: Bearer eyJhbGci...
```

Token claims: `email`, `username`, `roles`.

---

## Database Setup

Two isolated SQL Server databases:

| Database | Context | Contains |
|---|---|---|
| `ECommerce` | `StoreDbContext` | Products, Brands, Types, Orders, OrderItems, DeliveryMethods |
| `ECommerceIdentity` | `StoreIdentityDbContext` | Users, Roles, UserRoles, Addresses |

**Both databases are migrated and seeded automatically on startup** via `WebApplicationRegistration` extensions called from `Program.cs`.

Seed data loaded from JSON files in `ECommerce.Persistence/Data/DataSeeding/JSONFiles/`:
`brands.json`, `types.json`, `products.json`, `delivery.json`

Default seeded users:

| Email | Password | Role |
|---|---|---|
| abdallaaboaziz@gmail.com | Admin@123 | SuperAdmin |
| AhmedAli@gmail.com | Admin@123 | Admin |

Add a new migration:

```bash
dotnet ef migrations add MigrationName \
  --project ECommerce.Persistence \
  --startup-project ECommerceWeb
```

---

## Error Handling

All responses follow [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) `ProblemDetails`:

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

`ErrorType` values and their HTTP mappings:

| ErrorType | HTTP Status |
|---|---|
| `NotFound` | 404 |
| `Validation` | 400 |
| `InvalidCredentials` | 401 |
| `Unauthorized` | 401 |
| `Forbidden` | 403 |
| `Failure` | 500 |

---

## Author

**Abdalla Aboaziz**

GitHub: [github.com/Abdalla-Aboaziz](https://github.com/Abdalla-Aboaziz)

📧 abdallaaboaziz@gmail.com
