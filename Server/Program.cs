using Microsoft.EntityFrameworkCore;
using Server.context;
using Server.model;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication setup
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "product-management",
            ValidAudience = "product-management",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("productManagement123"))
        };
    });

// Authorization policy for Admin only
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("product_Management", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true);
    });
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<ProductDb>(opt => opt.UseSqlServer(connectionString));

// Swagger setup
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ProductAPI";
    config.Title = "ProductAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "ProductAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Use the authentication and authorization middleware
app.UseAuthentication();  // Ensure this is called before UseAuthorization
app.UseAuthorization();
app.UseCors("product_Management");

// Group the product routes
var productItems = app.MapGroup("/api/products");

// Public routes - accessible to all authenticated users
productItems.MapGet("/", GetAllProducts);
productItems.MapGet("/{id}", GetSingleProduct);

// Protected routes - accessible to admins only
productItems.MapPost("/", AddProducts).RequireAuthorization("AdminOnly");
productItems.MapPut("/{id}", UpdateProduct).RequireAuthorization("AdminOnly");
productItems.MapDelete("/{id}", DeleteProduct).RequireAuthorization("AdminOnly");

// Product CRUD operations
static async Task<IResult> GetAllProducts(ProductDb db)
{
    return TypedResults.Ok(await db.Products.ToListAsync());
}

static async Task<IResult> GetSingleProduct(int id, ProductDb db)
{
    return await db.Products.FindAsync(id)
        is Product product
            ? TypedResults.Ok(product)
            : TypedResults.NotFound();
}

static async Task<IResult> AddProducts(Product product, ProductDb db)
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/products/{product.Id}", product);
}

static async Task<IResult> UpdateProduct(int id, Product inputProduct, ProductDb db)
{
    var product = await db.Products.FindAsync(id);

    if (product is null) return TypedResults.NotFound();

    product.ProductName = inputProduct.ProductName;
    product.Category = inputProduct.Category;
    product.DeliveryTimeSpan = inputProduct.DeliveryTimeSpan;
    product.Price = inputProduct.Price;
    product.ShortName = inputProduct.ShortName;
    product.ThumbnailImageUrl = inputProduct.ThumbnailImageUrl;
    product.Sku = inputProduct.Sku;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduct(int id, ProductDb db)
{
    if (await db.Products.FindAsync(id) is Product product)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

app.Run();
