# 🛍️ Catalogo_API

A robust and scalable RESTful API for product catalog management, built with C# using best practices in architecture and design patterns.

## ✨ Key Features

- **Complete CRUD**: Full operations for products and categories
- **Clean Architecture**: Repository pattern implementation with Generic and Specific repositories
- **Unit of Work**: Transactional control and data consistency
- **Dependency Injection**: Low coupling and high testability
- **Advanced Logging**: Comprehensive logging system for monitoring and debugging
- **MySQL Database**: Robust and performant relational database

## 🏗️ Architecture

```
Catalogo_API/
├── 📁 Controllers/          # API Controllers
├── 📁 Models/              # Data Models
├── 📁 Repository/          # Repository Pattern
│   ├── Generic/           # Generic Repository
│   └── Specific/          # Specific Repositories
├── 📁 Services/           # Service Layer
├── 📁 Data/               # Database Context
├── 📁 DTOs/               # Data Transfer Objects
└── 📁 Configurations/     # Application Configurations
```

## 🚀 Technologies Used

- **Framework**: .NET 6/7/8
- **Database**: MySQL
- **ORM**: Entity Framework Core
- **Logging**: Serilog / NLog
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Documentation**: Swagger/OpenAPI

## 📋 Features

### Products
- ✅ List all products
- ✅ Get product by ID
- ✅ Create new product
- ✅ Update existing product
- ✅ Delete product
- ✅ Filter products by category
- ✅ Paginated search

### Categories
- ✅ List all categories
- ✅ Get category by ID
- ✅ Create new category
- ✅ Update existing category
- ✅ Delete category
- ✅ List products in a category

## 🛠️ Installation and Setup

### Prerequisites
- .NET SDK 6.0 or higher
- MySQL Server 8.0 or higher
- Visual Studio 2022 or VS Code

### 1. Clone the repository
```bash
git clone https://github.com/yourusername/Catalogo_API.git
cd Catalogo_API
```

### 2. Configure connection string
Edit the `appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=yourpassword;"
  }
}
```

### 3. Run migrations
```bash
dotnet ef database update
```

### 4. Run the application
```bash
dotnet run
```

The API will be available at: `https://localhost:7000` (or configured port)

## 📖 API Documentation

After running the project, access the interactive Swagger documentation:
```
https://localhost:7000/swagger
```

### Endpoint Examples

#### Products
```http
GET    /api/produtos              # List products
GET    /api/produtos/{id}         # Get product by ID
POST   /api/produtos              # Create product
PUT    /api/produtos/{id}         # Update product
DELETE /api/produtos/{id}         # Delete product
```

#### Categories
```http
GET    /api/categorias            # List categories
GET    /api/categorias/{id}       # Get category by ID
POST   /api/categorias            # Create category
PUT    /api/categorias/{id}       # Update category
DELETE /api/categorias/{id}       # Delete category
```

## 🏛️ Implemented Patterns

### Repository Pattern
```csharp
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Unit of Work
```csharp
public interface IUnitOfWork
{
    IProdutoRepository Produtos { get; }
    ICategoriaRepository Categorias { get; }
    Task<int> SaveAsync();
    void Dispose();
}
```

## 🔧 Development Environment Setup

### MySQL Configuration
1. Install MySQL Server
2. Create a database named `CatalogoDB`
3. Configure credentials in `appsettings.json`

### Environment Variables (Optional)
```bash
export ASPNETCORE_ENVIRONMENT=Development
export CONNECTION_STRING="Server=localhost;Database=CatalogoDB;Uid=root;Pwd=password;"
```

## 📊 Logging

The system uses structured logging with different levels:
- **Information**: Normal operations
- **Warning**: Situations that deserve attention
- **Error**: Errors that need investigation
- **Debug**: Detailed information for development

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Your Name**
- GitHub: [@JoandessonDev](https://github.com/JoandessonDev)
- LinkedIn:  Joandesson Santos(https://www.linkedin.com/in/joandesson-santos-9418b421a/)

⭐ If this project helped you, consider giving it a star!
