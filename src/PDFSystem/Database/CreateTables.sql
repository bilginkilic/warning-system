-- PDF İmza Sirküler Sistemi Veritabanı Tabloları
-- SQL Server için hazırlanmıştır

-- Veritabanı oluşturma (gerekirse)
-- CREATE DATABASE PDFSignatureSystem;
-- USE PDFSignatureSystem;

-- 1. Customers Tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
BEGIN
    CREATE TABLE Customers (
        CustomerId INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(200) NOT NULL,
        CustomerCode NVARCHAR(50) NULL,
        Email NVARCHAR(100) NULL,
        Phone NVARCHAR(20) NULL,
        Address NTEXT NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        IsActive BIT NOT NULL DEFAULT 1
    );
    
    CREATE INDEX IX_Customers_CustomerName ON Customers(CustomerName);
    CREATE INDEX IX_Customers_CustomerCode ON Customers(CustomerCode);
    CREATE INDEX IX_Customers_Email ON Customers(Email);
END

-- 2. Contracts Tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Contracts' AND xtype='U')
BEGIN
    CREATE TABLE Contracts (
        ContractId INT IDENTITY(1,1) PRIMARY KEY,
        CustomerId INT NOT NULL,
        ContractNumber NVARCHAR(50) NOT NULL,
        ContractTitle NVARCHAR(300) NOT NULL,
        Description NTEXT NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        StartDate DATETIME NULL,
        EndDate DATETIME NULL,
        ContractAmount DECIMAL(18,2) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Active, Completed, Cancelled
        IsActive BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT FK_Contracts_CustomerId FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
    
    CREATE INDEX IX_Contracts_CustomerId ON Contracts(CustomerId);
    CREATE INDEX IX_Contracts_ContractNumber ON Contracts(ContractNumber);
    CREATE INDEX IX_Contracts_Status ON Contracts(Status);
    CREATE INDEX IX_Contracts_CreatedDate ON Contracts(CreatedDate);
END

-- 3. SignatureCirculars Tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SignatureCirculars' AND xtype='U')
BEGIN
    CREATE TABLE SignatureCirculars (
        SignatureCircularId INT IDENTITY(1,1) PRIMARY KEY,
        ContractId INT NOT NULL,
        CircularTitle NVARCHAR(300) NOT NULL,
        Description NTEXT NULL,
        PDFFilePath NVARCHAR(500) NULL,
        PDFFileName NVARCHAR(200) NULL,
        PDFContent VARBINARY(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        DueDate DATETIME NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed, Cancelled
        IsActive BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT FK_SignatureCirculars_ContractId FOREIGN KEY (ContractId) REFERENCES Contracts(ContractId)
    );
    
    CREATE INDEX IX_SignatureCirculars_ContractId ON SignatureCirculars(ContractId);
    CREATE INDEX IX_SignatureCirculars_Status ON SignatureCirculars(Status);
    CREATE INDEX IX_SignatureCirculars_CreatedDate ON SignatureCirculars(CreatedDate);
    CREATE INDEX IX_SignatureCirculars_DueDate ON SignatureCirculars(DueDate);
END

-- 4. SignatureAssignments Tablosu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SignatureAssignments' AND xtype='U')
BEGIN
    CREATE TABLE SignatureAssignments (
        SignatureAssignmentId INT IDENTITY(1,1) PRIMARY KEY,
        SignatureCircularId INT NOT NULL,
        AssignedPersonName NVARCHAR(200) NOT NULL,
        AssignedPersonEmail NVARCHAR(100) NULL,
        AssignedPersonTitle NVARCHAR(100) NULL,
        AssignedPersonDepartment NVARCHAR(100) NULL,
        
        -- PDF'deki imza konumu bilgileri
        PDFPageNumber INT NOT NULL DEFAULT 1,
        SignatureX FLOAT NOT NULL DEFAULT 0,
        SignatureY FLOAT NOT NULL DEFAULT 0,
        SignatureWidth FLOAT NOT NULL DEFAULT 0,
        SignatureHeight FLOAT NOT NULL DEFAULT 0,
        
        -- İmza imajı bilgileri
        SignatureImagePath NVARCHAR(500) NULL,
        SignatureImageData VARBINARY(MAX) NULL,
        SignatureImageFormat NVARCHAR(10) NULL, -- PNG, JPG, etc.
        
        AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),
        SignedDate DATETIME NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Signed, Rejected
        Notes NTEXT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT FK_SignatureAssignments_SignatureCircularId FOREIGN KEY (SignatureCircularId) REFERENCES SignatureCirculars(SignatureCircularId)
    );
    
    CREATE INDEX IX_SignatureAssignments_SignatureCircularId ON SignatureAssignments(SignatureCircularId);
    CREATE INDEX IX_SignatureAssignments_AssignedPersonName ON SignatureAssignments(AssignedPersonName);
    CREATE INDEX IX_SignatureAssignments_AssignedPersonEmail ON SignatureAssignments(AssignedPersonEmail);
    CREATE INDEX IX_SignatureAssignments_Status ON SignatureAssignments(Status);
    CREATE INDEX IX_SignatureAssignments_AssignedDate ON SignatureAssignments(AssignedDate);
    CREATE INDEX IX_SignatureAssignments_SignedDate ON SignatureAssignments(SignedDate);
END

-- 5. Test verileri ekleme (opsiyonel)
-- Müşteri ekleme
IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerName = 'Test Müşteri A.Ş.')
BEGIN
    INSERT INTO Customers (CustomerName, CustomerCode, Email, Phone, Address)
    VALUES ('Test Müşteri A.Ş.', 'TST001', 'info@testmusteri.com', '0212 555 0001', 'İstanbul, Türkiye');
END

IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerName = 'Örnek Firma Ltd.')
BEGIN
    INSERT INTO Customers (CustomerName, CustomerCode, Email, Phone, Address)
    VALUES ('Örnek Firma Ltd.', 'ORN001', 'contact@ornekfirma.com', '0212 555 0002', 'Ankara, Türkiye');
END

-- Kontrakt ekleme (müşteri ID'leri dinamik olarak alınacak)
DECLARE @CustomerId1 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE CustomerCode = 'TST001');
DECLARE @CustomerId2 INT = (SELECT TOP 1 CustomerId FROM Customers WHERE CustomerCode = 'ORN001');

IF @CustomerId1 IS NOT NULL AND NOT EXISTS (SELECT * FROM Contracts WHERE ContractNumber = 'CONT-202401-0001')
BEGIN
    INSERT INTO Contracts (CustomerId, ContractNumber, ContractTitle, Description, StartDate, EndDate, ContractAmount, Status)
    VALUES (@CustomerId1, 'CONT-202401-0001', 'Yazılım Geliştirme Sözleşmesi', 'Web uygulaması geliştirme projesi', '2024-01-01', '2024-12-31', 150000.00, 'Active');
END

IF @CustomerId2 IS NOT NULL AND NOT EXISTS (SELECT * FROM Contracts WHERE ContractNumber = 'CONT-202401-0002')
BEGIN
    INSERT INTO Contracts (CustomerId, ContractNumber, ContractTitle, Description, StartDate, EndDate, ContractAmount, Status)
    VALUES (@CustomerId2, 'CONT-202401-0002', 'Danışmanlık Hizmet Sözleşmesi', 'IT danışmanlık hizmetleri', '2024-02-01', '2024-11-30', 80000.00, 'Active');
END

PRINT 'PDF İmza Sirküler Sistemi veritabanı tabloları başarıyla oluşturuldu.';
PRINT 'Test verileri eklendi.'; 