CREATE DATABASE RankingDemo;
GO

USE RankingDemo;

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Category VARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);

INSERT INTO Products (Name, Category, Price) VALUES
('iPhone 14', 'Electronics', 999.99),
('Samsung Galaxy S22', 'Electronics', 899.99),
('MacBook Pro', 'Electronics', 1999.99),
('Dell XPS 13', 'Electronics', 1499.99),
('Sony Headphones', 'Electronics', 199.99),
('Nike Shoes', 'Clothing', 150.00),
('Adidas Sneakers', 'Clothing', 120.00),
('Levi’s Jeans', 'Clothing', 80.00),
('Puma Jacket', 'Clothing', 150.00),
('Zara T-Shirt', 'Clothing', 50.00),
('Sofa Set', 'Furniture', 799.99),
('Dining Table', 'Furniture', 699.99),
('Bookshelf', 'Furniture', 249.99),
('Bed Frame', 'Furniture', 999.99),
('Office Chair', 'Furniture', 299.99);

SELECT *
FROM (
    SELECT 
        Id, Name, Category, Price,
        ROW_NUMBER() OVER (PARTITION BY Category ORDER BY Price DESC) AS RowNum
    FROM Products
) AS RankedProducts
WHERE RowNum <= 3;

SELECT *
FROM (
    SELECT 
        Id, Name, Category, Price,
        RANK() OVER (PARTITION BY Category ORDER BY Price DESC) AS RankNum
    FROM Products
) AS RankedProducts
WHERE RankNum <= 3;

SELECT *
FROM (
    SELECT 
        Id, Name, Category, Price,
        DENSE_RANK() OVER (PARTITION BY Category ORDER BY Price DESC) AS DenseRankNum
    FROM Products
) AS RankedProducts
WHERE DenseRankNum <= 3;
