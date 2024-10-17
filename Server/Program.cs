using Microsoft.EntityFrameworkCore;
using Server.context;
using Server.model;

var builder = WebApplication.CreateBuilder(args);

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
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/api/products", async (ProductDb db) =>
    await db.Products.ToListAsync()
);

app.MapGet("api/products/{id}", async (int id, ProductDb db) =>
    await db.Products.FindAsync(id)
        is Product product
            ? Results.Ok(product)
            : Results.NotFound());

app.MapPost("api/products", async (Product product, ProductDb db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (string id, Product inputProduct, ProductDb db) =>
{
    Console.WriteLine(id);
    var product = await db.Products.FindAsync(id);

    if (product is null) return Results.NotFound();

    product.ProductName = inputProduct.ProductName;
    product.Category = inputProduct.Category;
    product.DeliveryTimeSpan = inputProduct.DeliveryTimeSpan;
    product.Price = inputProduct.Price;
    product.ShortName = inputProduct.ShortName;
    product.ThumbnailImageUrl = inputProduct.ThumbnailImageUrl;
    product.Sku = inputProduct.Sku;
    
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/products/{id}", async (int id, ProductDb db) =>
{
    if (await db.Products.FindAsync(id) is Product product)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    return Results.NotFound();
});

app.UseCors("product_Management");

app.Run();
