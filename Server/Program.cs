using Azure.Core;
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

    Console.WriteLine("ici la valeur de ID :" + user!.Id);

    if (user == null)
        return TypedResults.Unauthorized();

    var session = new Session() {
        UserId = (int)user.Id!,
        Duration = 3600,
        CreatedAt = DateTime.Now,
    };

    httpContext.Response.Cookies.Append("jshL15Pa", user.Id.ToString()!, new CookieOptions {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        MaxAge = TimeSpan.FromSeconds(3600),
        Secure = false
    });


    db.UserSessions.Add(session);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapDelete("/api/products/logout", async (HttpContext httpContext, ProductDb db) => {

    var userSessionId = httpContext.Request.Cookies["jshL15Pa"];
    Console.WriteLine("User Id must be there :" + userSessionId);
    if (userSessionId == null) {
        return Results.Unauthorized();
    }

    if (!int.TryParse(userSessionId, out int userId)) {
        return Results.BadRequest("Invalid session ID.");
    }
    var session = db.UserSessions.FirstOrDefault(s => s.Id == userId);

    if (session != null) {
        db.UserSessions.Remove(session);
        await db.SaveChangesAsync();
    }

    httpContext.Response.Cookies.Delete("jshL15Pa");

    return Results.Ok();
});

var productItems = app.MapGroup("/api/products");

productItems.MapGet("/", async (ProductDb db, HttpContext httpContext) => {
    var authCookie = httpContext.Request.Cookies.FirstOrDefault((cookie) => cookie.Key == "jshL15Pa");

    if (string.IsNullOrEmpty(authCookie.Key)) {
        return Results.StatusCode(403);
    }

    return Results.Ok(await db.Products.ToListAsync());
});

productItems.MapGet("/{id}", async (int id, ProductDb db) => {
    return await db.Products.FindAsync(id)
         is Product product
             ? Results.Ok(product)
             : Results.NotFound();
});

productItems.MapPost("/", async (Product product, ProductDb db) => {
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.Id}", product);
});

productItems.MapPut("/{id}", async (int id, Product inputProduct, ProductDb db) => {
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

    return Results.NoContent();
});

productItems.MapDelete("/{id}", async (int id, ProductDb db) => {
    if (await db.Products.FindAsync(id) is Product product) {
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();