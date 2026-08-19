CREATE TABLE IF NOT EXISTS `Categories` (
  `Id` CHAR(36) NOT NULL,
  `Name` VARCHAR(120) NOT NULL,
  `Slug` VARCHAR(140) NOT NULL,
  `IsActive` TINYINT(1) NOT NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  `UpdatedAt` DATETIME(6) NULL,
  CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`),
  CONSTRAINT `IX_Categories_Slug` UNIQUE (`Slug`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Brands` (
  `Id` CHAR(36) NOT NULL,
  `Name` VARCHAR(120) NOT NULL,
  `Slug` VARCHAR(140) NOT NULL,
  `IsActive` TINYINT(1) NOT NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  `UpdatedAt` DATETIME(6) NULL,
  CONSTRAINT `PK_Brands` PRIMARY KEY (`Id`),
  CONSTRAINT `IX_Brands_Slug` UNIQUE (`Slug`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Products` (
  `Id` CHAR(36) NOT NULL,
  `CategoryId` CHAR(36) NOT NULL,
  `BrandId` CHAR(36) NOT NULL,
  `Name` VARCHAR(180) NOT NULL,
  `Slug` VARCHAR(220) NOT NULL,
  `Description` VARCHAR(2000) NOT NULL,
  `Price` DECIMAL(18,2) NOT NULL,
  `Sku` VARCHAR(80) NOT NULL,
  `IsActive` TINYINT(1) NOT NULL,
  `CreatedAt` DATETIME(6) NOT NULL,
  `UpdatedAt` DATETIME(6) NULL,
  CONSTRAINT `PK_Products` PRIMARY KEY (`Id`),
  CONSTRAINT `IX_Products_Slug` UNIQUE (`Slug`),
  CONSTRAINT `IX_Products_Sku` UNIQUE (`Sku`),
  INDEX `IX_Products_CategoryId` (`CategoryId`),
  INDEX `IX_Products_BrandId` (`BrandId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `ProductImages` (
  `Id` CHAR(36) NOT NULL,
  `ProductId` CHAR(36) NOT NULL,
  `Url` VARCHAR(500) NOT NULL,
  `AltText` VARCHAR(180) NOT NULL,
  `SortOrder` INT NOT NULL,
  CONSTRAINT `PK_ProductImages` PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ProductImages_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
  INDEX `IX_ProductImages_ProductId` (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('202607210001_InitialCatalogSchema', '9.0.0');
