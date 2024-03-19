using tds_ex_2_main;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductConnection")));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inicializa o banco de dados
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
    dbContext.Database.EnsureCreated(); // Cria o banco se não existir
}

// Rota GET para listar todos os produtos
app.MapGet("/products", async (ProductContext db) => await db.Products.ToListAsync());

// Rota GET para obter um produto pelo ID
app.MapGet("/products/{id}", async (ProductContext db, int id) =>
    await db.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound());

// Rota POST para adicionar um novo produto
app.MapPost("/products", async (ProductContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// Rota PUT para atualizar um produto
app.MapPut("/products/{id}", async (ProductContext db, int id, Product updatedProduct) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    product.Quantity = updatedProduct.Quantity;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Rota DELETE para excluir um produto
app.MapDelete("/products/{id}", async (ProductContext db, int id) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
