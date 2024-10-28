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
IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'users')
    BEGIN 
        CREATE TABLE users (
            id INT PRIMARY KEY,
            user_name NVARCHAR(50) NOT NULL,
        )

        INSERT INTO users(id, user_name) VALUES 
        (0, 'Mr Admin'),
        (1, 'Common User')
    END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name='roles')
    BEGIN
        CREATE TABLE roles (
            id INT PRIMARY KEY,
            title NVARCHAR(50) NOT NULL,
        )

        INSERT INTO roles (id, title) VALUES
        (0, 'admin'),
        (1, 'user')
    END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name='users_roles')
    BEGIN
        CREATE TABLE users_roles (
            id INT PRIMARY KEY,
            user_id INT NOT NULL,
            role_id INT NOT NULL,
            FOREIGN KEY (user_id) REFERENCES users(id),
            FOREIGN KEY (role_id) REFERENCES roles(id)
        )

        INSERT INTO users_roles(id, user_id, role_id) VALUES 
        (0,0,0),
        (1,1,1)
    END
