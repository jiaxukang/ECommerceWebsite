# E-Commerce Platform

**Live Demo:** [https://skinet-felix.azurewebsites.net/](https://skinet-felix.azurewebsites.net/)

A modern, full-stack e-commerce application built with .NET 9.0 and Angular 20. Features include product catalog, shopping cart, secure payment processing with Stripe, real-time order notifications, and admin dashboard.

## Features

### Customer Features
- **Product Catalog** - Browse products with advanced filtering, sorting, and search
- **Shopping Cart** - Add/remove items with real-time total calculations
- **Secure Checkout** - Multi-step checkout with Stripe payment integration
- **Order Management** - View order history and track order status
- **Real-time Notifications** - Instant order confirmation via SignalR
- **User Accounts** - Registration, login, and profile management
- **Discount Codes** - Apply coupon codes for special offers

### Admin Features
- **Product Management** - Create, update, and delete products
- **Order Administration** - View all orders with filtering and pagination
- **Refund Processing** - Issue refunds directly through the admin panel
- **Role-based Access** - Secure admin-only operations

### Technical Features
- **Clean Architecture** - Separation of concerns with Domain-Driven Design
- **Responsive Design** - Mobile-friendly UI with Tailwind CSS and Angular Material
- **Performance Optimization** - Redis caching for improved response times
- **Real-time Communication** - SignalR for instant order updates
- **Payment Security** - PCI-compliant payment processing via Stripe

## Technology Stack

### Backend
- **.NET 9.0** - Modern C# web framework
- **ASP.NET Core Web API** - RESTful API architecture
- **Entity Framework Core 9** - ORM for database operations
- **SQL Server 2022** - Relational database
- **Redis** - In-memory caching and session storage
- **Stripe API** - Payment processing
- **SignalR** - Real-time web functionality
- **ASP.NET Core Identity** - User authentication and authorization

### Frontend
- **Angular 20** - Latest Angular framework
- **TypeScript 5.8** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **Angular Material** - UI component library
- **Tailwind CSS** - Utility-first CSS framework
- **Stripe.js** - Frontend payment integration

### Infrastructure
- **Docker** - Containerization for SQL Server and Redis
- **Azure** - Cloud hosting platform

## Prerequisites

Before running this project locally, ensure you have:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- A code editor like [Visual Studio Code](https://code.visualstudio.com/)

## Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd e-commerce
```

### 2. Start Infrastructure Services

Start SQL Server and Redis using Docker:

```bash
docker-compose up -d
```

This will start:
- SQL Server 2022 on port 1433
- Redis on port 6379

### 3. Set Up Backend

```bash
cd API
dotnet restore
dotnet build
```

The database will be automatically created and seeded when you first run the application.

### 4. Set Up Frontend

```bash
cd client
npm install
ng build
ng serve
```

## Running the Application

### Start Backend API

From the `API` directory:

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Start Frontend

From the `client` directory:

```bash
npm start
```

The application will open automatically at `http://localhost:4200`

## Configuration

### Stripe Setup

The application comes with test API keys pre-configured in `API/appsettings.json`. For production:

1. Create a [Stripe account](https://stripe.com)
2. Replace test keys with your production keys
3. Set up webhook endpoint at `/api/payments/webhook`

### Database Connection

Default connection string in `API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ECommerceDb;User Id=SA;Password=Password@12;TrustServerCertificate=True"
  }
}
```

Modify as needed for your environment.

### Redis Configuration

Default Redis connection: `localhost:6379`

Update in `API/Program.cs` if using a different Redis instance.

## Testing

### Frontend Tests

```bash
cd client
npm test
```

### Test User Account

Default test account for development:
- **Email:** tom@test.com
- **Password:** Pa$w0rd

### Test Payment Cards (Stripe Test Mode)

- **Success:** 4242 4242 4242 4242
- **Requires Authentication:** 4000 0025 0000 3155
- **Declined:** 4000 0000 0000 9995

Use any future expiry date and any 3-digit CVC.

## Project Structure

```
e-commerce/
├── API/                      # ASP.NET Core Web API
│   ├── Controllers/          # API endpoints
│   ├── DTOs/                 # Data transfer objects
│   ├── Middleware/           # Custom middleware
│   ├── SignalR/              # Real-time hubs
│   └── Program.cs            # Application entry point
├── Core/                     # Domain layer
│   ├── Entities/             # Domain models
│   ├── Interfaces/           # Repository & service interfaces
│   └── Specification/        # Query specifications
├── Infrastructure/           # Data access & external services
│   ├── Data/                 # EF Core context & repositories
│   ├── Services/             # Business services
│   ├── Config/               # Entity configurations
│   └── Migrations/           # Database migrations
├── client/                   # Angular frontend
│   └── src/
│       └── app/
│           ├── core/         # Core services & guards
│           ├── features/     # Feature modules
│           └── shared/       # Shared components & models
└── docker-compose.yml        # Docker services configuration
```

## API Endpoints

### Products
- `GET /api/products` - Get paginated products with filters
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/brands` - Get all brands
- `GET /api/products/types` - Get all product types
- `POST /api/products` - Create product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Shopping Cart
- `GET /api/cart?id={cartId}` - Get cart
- `POST /api/cart` - Create/update cart
- `DELETE /api/cart?id={cartId}` - Delete cart

### Orders
- `POST /api/orders` - Create order
- `GET /api/orders` - Get user orders
- `GET /api/orders/{id}` - Get order by ID

### Payments
- `POST /api/payments/{cartId}` - Create payment intent
- `GET /api/payments/deliveryMethods` - Get delivery options
- `POST /api/payments/webhook` - Stripe webhook handler

### Account
- `POST /api/register` - Register new user
- `POST /api/login?useCookies=true` - Login
- `POST /api/logout` - Logout
- `GET /api/account/user-info` - Get current user
- `POST /api/account/address` - Update address

### Admin
- `GET /api/admin/orders` - Get all orders (Admin)
- `POST /api/admin/orders/{id}/refund` - Refund order (Admin)

## Architecture Highlights

### Clean Architecture
The project follows Clean Architecture principles with clear separation between:
- **API Layer** - Presentation and API endpoints
- **Core Layer** - Business logic and domain entities
- **Infrastructure Layer** - Data access and external services

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Specification Pattern** - Composable query logic
- **Dependency Injection** - Loose coupling
- **CQRS Lite** - Separation of read/write operations

### Key Features
- **Owned Types** - EF Core value objects for ShippingAddress and PaymentSummary
- **Response Caching** - Redis-backed HTTP response caching
- **Cart Persistence** - Stateless cart storage in Redis
- **Real-time Updates** - SignalR for order notifications
- **Signal-based State** - Angular signals for reactive UI

## Development Workflow

### Adding Entity Framework Migration

```bash
cd API
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Generating Angular Components

```bash
cd client
ng generate component features/feature-name/component-name
ng generate service core/services/service-name
```

### Building for Production

**Backend:**
```bash
dotnet publish -c Release
```

**Frontend:**
```bash
cd client
npm run build
```

## Troubleshooting

### Port Already in Use
If ports 5001, 4200, 1433, or 6379 are in use, either:
- Stop the conflicting service
- Update port configuration in respective config files

### Database Connection Failed
Ensure Docker containers are running:
```bash
docker ps
```

Restart if needed:
```bash
docker-compose restart
```

### Frontend Build Errors
Clear Angular cache:
```bash
cd client
rm -rf .angular
npm install
```

### Lightningcss Native Module Error
Rebuild native dependencies:
```bash
cd client
npm rebuild lightningcss
```

## Deployment

The application is configured for Azure deployment with:
- Forwarded headers for HTTPS
- Azure-compatible connection strings
- CORS configuration for production domain

Update `appsettings.json` with production values before deploying.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

## Acknowledgments

- [Angular](https://angular.io/) - Frontend framework
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) - Backend framework
- [Stripe](https://stripe.com/) - Payment processing
- [Tailwind CSS](https://tailwindcss.com/) - CSS framework
- [Angular Material](https://material.angular.io/) - UI components

## Contact & Support

For questions or support, please open an issue in the repository.

---

**Note:** This application uses Stripe test mode by default. Always use test card numbers during development.
