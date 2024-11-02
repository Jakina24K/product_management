using Microsoft.EntityFrameworkCore;
using Server.context;
using Server.model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("product_Management", policy => {
        policy.WithOrigins("http://localhost:4200")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod().
            SetIsOriginAllowed(origin => true);
    });
});

var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<ProductDb>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => {
    config.DocumentName = "ProductAPI";
    config.Title = "ProductAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi(config => {
        config.DocumentTitle = "ProductAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.UseCors("product_Management");

app.MapPost("/api/products/login", async (HttpContext httpContext, ProductDb db, UserInfo userInfo) => {
    var user = db.Users.FirstOrDefault((user) => user.Email == userInfo.Email);

    if (user == null)
        return TypedResults.Unauthorized();

    var session = new Session() {
        UserId = (int)user.Id!,
        Duration = 3600,
        CreatedAt = DateTime.Now,
    };

    httpContext.Response.Cookies.Append("jshL15Pa", "123", new CookieOptions {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        MaxAge = TimeSpan.FromSeconds(3600),
        Secure = false
    });


    db.UserSessions.Add(session);
    await db.SaveChangesAsync();

    return Results.Ok();
});

var productItems = app.MapGroup("/api/products");

productItems.MapGet("/", async (ProductDb db, HttpContext httpContext) => {
    var authCookie = httpContext.Request.Cookies.FirstOrDefault((cookie) => cookie.Key == "jshL15Pa");

    Console.WriteLine("mande rangah ty ah");

    Console.WriteLine(authCookie.Key);

    if (string.IsNullOrEmpty(authCookie.Key)) {
        return Results.StatusCode(403);
    }

    return Results.Ok(await db.Products.ToListAsync());
});

productItems.MapGet("/{id}", GetSingleProduct);
productItems.MapPost("/", AddProducts);
productItems.MapPut("/{id}", UpdateProduct);
productItems.MapDelete("/{id}", DeleteProduct);

// static async Task<IResult> GetAllProducts(ProductDb db, HttpContext httpContext) {
//     var authCookie = httpContext.Request.Cookies.FirstOrDefault((cookie) => cookie.Key == "jshL15Pa");

//     Console.WriteLine(authCookie.Value);

//     if(authCookie.Key == "") {
//         return TypedResults.Forbid();
//     }

//     return TypedResults.Ok(await db.Products.ToListAsync());
// }

static async Task<IResult> GetSingleProduct(int id, ProductDb db) {
    return await db.Products.FindAsync(id)
        is Product product
            ? TypedResults.Ok(product)
            : TypedResults.NotFound();

}

static async Task<IResult> AddProducts(Product product, ProductDb db) {
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/products/{product.Id}", product);
}

static async Task<IResult> UpdateProduct(int id, Product inputProduct, ProductDb db) {

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

static async Task<IResult> DeleteProduct(int id, ProductDb db) {
    if (await db.Products.FindAsync(id) is Product product) {
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

app.Run();