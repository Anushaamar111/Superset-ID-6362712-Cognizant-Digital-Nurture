using Microsoft.EntityFrameworkCore;
using RetailInventory.Models;

using var context = new AppDbContext();

Console.WriteLine("🔍 Filter and sort products (price > 1000):");

// 1️⃣ Filter and Sort
var filtered = await context.Products
    .Where(p => p.Price > 1000)
    .OrderByDescending(p => p.Price)
    .Include(p => p.Category)
    .ToListAsync();

foreach (var product in filtered)
{
    Console.WriteLine($"{product.Name} - ₹{product.Price} - Category: {product.Category?.Name}");
}

Console.WriteLine("\n📦 Project into DTOs (Name + Price only):");

// 2️⃣ Project into DTO (Anonymous Object)
var productDTOs = await context.Products
    .Select(p => new { p.Name, p.Price })
    .ToListAsync();

foreach (var dto in productDTOs)
{
    Console.WriteLine($"{dto.Name} - ₹{dto.Price}");
}
