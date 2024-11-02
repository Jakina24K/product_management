using Microsoft.EntityFrameworkCore;
using Server.context;
using Server.model;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddJwtBearer()
    .AddJwtBearer("LocalAuthIssuer");

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_greetings", policy => 
        policy.RequireRole("admin")
        .RequireClaim("scope", "greetings_api"));
        
builder.Services.AddCors(options =>
{
    options.AddPolicy("product_Management", policy =>
    {
        policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod().
            SetIsOriginAllowed(origin => true);
    });
});

var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<ProductDb>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ProductAPI";
    config.Title = "ProductAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

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

app.UseAuthorization();
app.UseAuthentication();
app.UseCors("product_Management");

var productItems = app.MapGroup("/api/products");

productItems.MapGet("/", GetAllProducts);
productItems.MapGet("/{id}", GetSingleProduct);
productItems.MapPost("/", AddProducts);
productItems.MapPut("/{id}", UpdateProduct);
productItems.MapDelete("/{id}", DeleteProduct);

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

    Console.WriteLine(id);
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