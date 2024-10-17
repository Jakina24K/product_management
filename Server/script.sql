USE product_management;

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'products')
    BEGIN 
        CREATE TABLE products (
            id INT IDENTITY(1,1) PRIMARY KEY,
            product_name NVARCHAR(255) NOT NULL,
            short_name NVARCHAR(50) NOT NULL,
            sku NVARCHAR(50) NOT NULL,
            category NVARCHAR(50) NOT NULL,
            price INT NOT NULL,
            delivery_timespan NVARCHAR(50) NOT NULL,
            thumbnail_imageurl NVARCHAR(255) NOT NULL
        )
    END