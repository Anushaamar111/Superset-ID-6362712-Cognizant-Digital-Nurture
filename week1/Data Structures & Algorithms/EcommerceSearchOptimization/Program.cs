using System;
using System.Linq;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Category { get; set; }

    public Product(int id, string name, string category)
    {
        ProductId = id;
        ProductName = name;
        Category = category;
    }

    public override string ToString()
    {
        return $"[{ProductId}] {ProductName} - {Category}";
    }
}

public class SearchEngine
{
    // Linear Search
    public static Product LinearSearch(Product[] products, int id)
    {
        foreach (var product in products)
        {
            if (product.ProductId == id)
                return product;
        }
        return null;
    }

    // Binary Search
    public static Product BinarySearch(Product[] sortedProducts, int id)
    {
        int left = 0;
        int right = sortedProducts.Length - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (sortedProducts[mid].ProductId == id)
                return sortedProducts[mid];
            else if (sortedProducts[mid].ProductId < id)
                left = mid + 1;
            else
                right = mid - 1;
        }
        return null;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Product[] products = new Product[]
        {
            new Product(101, "Laptop", "Electronics"),
            new Product(205, "Shirt", "Clothing"),
            new Product(153, "Book", "Stationery"),
            new Product(330, "Phone", "Electronics"),
            new Product(412, "Blender", "Appliances")
        };

        Console.WriteLine("🔍 Performing Linear Search (unsorted list)...");
        Product linearResult = SearchEngine.LinearSearch(products, 153);
        Console.WriteLine("Linear Search Result: " + (linearResult != null ? linearResult.ToString() : "Not Found"));

        Console.WriteLine("\n📋 Sorting products by ProductId for Binary Search...");
        Product[] sortedProducts = products.OrderBy(p => p.ProductId).ToArray();

        Console.WriteLine("🔍 Performing Binary Search...");
        Product binaryResult = SearchEngine.BinarySearch(sortedProducts, 153);
        Console.WriteLine("Binary Search Result: " + (binaryResult != null ? binaryResult.ToString() : "Not Found"));
    }
}
