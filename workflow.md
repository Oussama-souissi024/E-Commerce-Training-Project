# E-Commerce .NET Project Teaching Workflow

## Phase 1: Project Structure and Core Setup
### 1. Project Architecture (30 mins)
- Explain N-Tier Architecture
- Create solution with projects:
  - E-Commerce.Core
  - E-Commerce.Infrastructure
  - E-Commerce.Services
  - E-Commerce.Web

### 2. Core Project Setup (1 hour)
```markdown
1. Entity Framework Setup
   - Install packages:
     - Microsoft.AspNetCore.Identity.EntityFrameworkCore
     - Microsoft.EntityFrameworkCore.Tools

2. Create Entity Classes with Data Annotations
   - Product
   - Category
   - CartHeader
   - CartDetails
   - OrderHeader
   - OrderDetails
   - ApplicationUser (with ZipCode)
   - Coupon

3. Explain Important Concepts:
   - Data Annotations
   - Navigation Properties
   - Relationships (1:1, 1:N, N:N)
```

## Phase 2: Infrastructure Layer (1.5 hours)
```markdown
1. Database Configuration
   - Install packages:
     - Microsoft.EntityFrameworkCore.SqlServer
     - Microsoft.EntityFrameworkCore.Design
     - Microsoft.EntityFrameworkCore.Tools
     - Microsoft.AspNetCore.Identity.EntityFrameworkCore

2. DbContext Setup
   - Create ApplicationDbContext
   - Explain IdentityDbContext
   - Configure OnModelCreating
   - Explain Entity Relationships

3. Repository Pattern
   - Create IRepository interface
   - Implement Generic Repository
   - Explain Unit of Work pattern
   - Implement Repository in Persistence
   - Add InfrastructureRegistration
   - Explain Dependency Injection lifecycle (Singleton, Scoped, Transient)
```

## Phase 3: Web Project Initial Setup (1 hour)
```markdown
1. Configuration
   - Configure appsettings.json
   - Add ConnectionString
   - Configure Program.cs
   - Explain Middleware Pipeline
   - Configure ApplicationDbContext in program.cs

2. Database Setup
   - Add Initial Migration
   - Update Database
   - Explain Migration Commands

3. UI Framework Setup
   - Add Bootstrap theme from bootswatch.com
     * Download bootstrap.css
     * Copy to root/lib/bootstrap/dist/css/bootstrap.css
     * Modify layout
   - Add Dropdown list to navbar (Bootstrap 5)
   - Add Icons to footer
   - Add Toastr notifications
```

## Phase 4: Services Layer Implementation (2 hours)
```markdown
1. DTO Pattern Implementation
   - Create DTOs in Services layer
   - Explain DTO pattern benefits
   - Implement:
     * ProductDto
     * CategoryDto
     * CouponDto
     * CartDto
     * OrderDto

2. AutoMapper Configuration
   - Install AutoMapper
   - Create mapping profiles:
     * ProductMapping
     * CategoryMapping
     * CartMapping
     * OrderMapping

3. Service Implementation
   - Create service interfaces
   - Implement services:
     * ProductService
     * CategoryService
     * CouponService
     * CartService
     * OrderService
   - Add ServiceRegistration
```

## Phase 5: Product Management (2 hours)
```markdown
1. Product Controller
   - Implement CRUD operations
   - Create views:
     * Index
     * Create
     * Edit
     * Delete
     * Details
   - Implement file upload:
     * Create IFileHelper
     * Implement FileHelper
     * Handle image upload/delete

2. Category Integration
   - Create CategoryController
   - Implement views:
     * CategoryIndex
     * CreateCategory
     * EditCategory
     * DeleteCategory
   - Add category dropdown in product forms
   - Implement GetCategoryIdByCategoryName
```

## Phase 6: Authentication and Authorization (2 hours)
```markdown
1. Identity Setup
   - Create AuthenticationController
   - Implement IAuthService
   - Create DTOs:
     * RegistrationRequestDto
     * LoginRequestDto
     * LoginResponseDto

2. User Management
   - Implement Register/Login/Logout
   - Add session-based authentication
   - Configure cookie authentication
   - Add IdentityRole configuration
   - Implement role-based authorization

3. Logging Setup
   - Add ILogger
   - Configure console logging
   - Add builder.Services.AddLogging()
```

## Phase 7: Shopping Cart Implementation (2 hours)
```markdown
1. Cart Functionality
   - Create CartController
   - Implement ICartService
   - Create DTOs:
     * CartDto
     * CartHeaderDto
     * CartDetailsDto

2. Cart Operations
   - LoadCartDtoBasedOnLoggedInUser
   - UpsertCartAsync
   - RemoveFromCartAsync
   - ApplyCoupon
   - RemoveCoupon

3. Cart Views
   - CartIndex
   - Checkout
   - Confirmation
```

## Phase 8: Order Management (1.5 hours)
```markdown
1. Order Implementation
   - Create IOrder interface
   - Implement CreateOrder
   - Create DTOs:
     * OrderHeaderDto
     * OrderDetailsDto
   - Implement SD (Static Details)

2. Order Processing
   - Create order confirmation
   - Handle order status
   - Implement order history
```

## Phase 9: Stripe Integration (2 hours)
```markdown
1. Stripe Setup
   - Install Stripe.net package
   - Add Stripe configuration:
     * Add SecretKey to appsettings.json
     * Configure Stripe in Program.cs

2. Stripe Services
   - Create StripeRequestDto
   - Create IStripeSession interface
   - Implement StripeSessionService
   - Add dependency injection

3. Payment Integration
   - Implement CreateStripeSessionAsync
   - Add ValidateStripeSession
   - Integrate with cart checkout
   - Handle payment confirmation

4. Coupon Integration
   - Synchronize coupons with Stripe
   - Implement AddStripeCoupon
   - Implement DeleteStripeCoupon
   - Add discount handling in checkout
```

## Phase 10: Final Touches and Testing (1 hour)
```markdown
1. UI/UX Improvements
   - Responsive design
   - Loading states
   - Error handling
   - Toastr notifications

2. Testing
   - Unit tests
   - Integration tests
   - Payment flow testing
   - Security testing
```

## Teaching Tips
1. Start each session with a recap
2. Use practical examples
3. Encourage hands-on coding
4. Explain architectural decisions
5. Show real-world applications
6. Include error handling scenarios
7. Demonstrate debugging techniques

## Package Installation Commands
```powershell
# Core Project
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Tools

# Infrastructure Project
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# Web Project
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Stripe.net
```
