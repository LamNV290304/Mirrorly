/*
 Navicat Premium Data Transfer

 Source Server         : SQL Server
 Source Server Type    : SQL Server
 Source Server Version : 16004210 (16.00.4210)
 Source Host           : localhost:1433
 Source Catalog        : Test9
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 16004210 (16.00.4210)
 File Encoding         : 65001

 Date: 12/09/2025
*/

-- ==========================
-- TABLE STRUCTURE
-- ==========================

-- BookingItems
IF OBJECT_ID(N'[dbo].[BookingItems]', 'U') IS NOT NULL DROP TABLE [dbo].[BookingItems]
GO
CREATE TABLE [dbo].[BookingItems] (
  [BookingItemId] BIGINT IDENTITY(1,1) NOT NULL,
  [BookingId] BIGINT NOT NULL,
  [ServiceId] BIGINT NOT NULL,
  [Quantity] INT DEFAULT 1 NOT NULL,
  [UnitPrice] DECIMAL(18,2) NOT NULL,
  CONSTRAINT PK_BookingItems PRIMARY KEY CLUSTERED ([BookingItemId])
)
GO

-- Bookings
IF OBJECT_ID(N'[dbo].[Bookings]', 'U') IS NOT NULL DROP TABLE [dbo].[Bookings]
GO
CREATE TABLE [dbo].[Bookings] (
  [BookingId] BIGINT IDENTITY(1,1) NOT NULL,
  [CustomerId] INT NOT NULL,
  [MuaId] INT NOT NULL,
  [ScheduledStart] DATETIME2(3) NULL,
  [ScheduledEnd] DATETIME2(3) NULL,
  [AddressLine] NVARCHAR(255) NULL,
  [Currency] CHAR(3) DEFAULT 'VND' NOT NULL,
  [Notes] NVARCHAR(MAX) NULL,
  [Status] TINYINT NULL,
  CONSTRAINT PK_Bookings PRIMARY KEY CLUSTERED ([BookingId])
)
GO

-- Categories
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL DROP TABLE [dbo].[Categories]
GO
CREATE TABLE [dbo].[Categories] (
  [CategoryId] INT IDENTITY(1,1) NOT NULL,
  [CategoryName] NVARCHAR(100) NOT NULL,
  [Description] NVARCHAR(255) NULL,
  CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED ([CategoryId]),
  CONSTRAINT UQ_Categories_CategoryName UNIQUE ([CategoryName])
)
GO

-- CustomerProfiles
IF OBJECT_ID(N'[dbo].[CustomerProfiles]', 'U') IS NOT NULL DROP TABLE [dbo].[CustomerProfiles]
GO
CREATE TABLE [dbo].[CustomerProfiles] (
  [CustomerId] INT NOT NULL,
  [DisplayName] NVARCHAR(120) NULL,
  [AvatarUrl] NVARCHAR(500) NULL,
  [PhoneNumber] VARCHAR(30) NULL,
  CONSTRAINT PK_CustomerProfiles PRIMARY KEY CLUSTERED ([CustomerId])
)
GO

-- MUAProfiles
IF OBJECT_ID(N'[dbo].[MUAProfiles]', 'U') IS NOT NULL DROP TABLE [dbo].[MUAProfiles]
GO
CREATE TABLE [dbo].[MUAProfiles] (
  [MUAId] INT NOT NULL,
  [DisplayName] NVARCHAR(120) NOT NULL,
  [Bio] NVARCHAR(MAX) NULL,
  [AddressLine] NVARCHAR(255) NULL,
  [ExperienceYears] INT NULL,
  [ProfilePublic] BIT DEFAULT 1 NOT NULL,
  CONSTRAINT PK_MUAProfiles PRIMARY KEY CLUSTERED ([MUAId])
)
GO

-- Reviews
IF OBJECT_ID(N'[dbo].[Reviews]', 'U') IS NOT NULL DROP TABLE [dbo].[Reviews]
GO
CREATE TABLE [dbo].[Reviews] (
  [ReviewId] BIGINT IDENTITY(1,1) NOT NULL,
  [BookingId] BIGINT NOT NULL,
  [CustomerId] INT NOT NULL,
  [MuaId] INT NOT NULL,
  [Rating] TINYINT NOT NULL CHECK ([Rating] BETWEEN 1 AND 5),
  [Comment] NVARCHAR(MAX) NULL,
  [CreatedAt] DATETIME2(3) DEFAULT SYSDATETIME() NOT NULL,
  CONSTRAINT PK_Reviews PRIMARY KEY CLUSTERED ([ReviewId])
)
GO

-- PortfolioItems
IF OBJECT_ID(N'[dbo].[PortfolioItems]', 'U') IS NOT NULL DROP TABLE [dbo].[PortfolioItems]
GO
CREATE TABLE [dbo].[PortfolioItems] (
  [ItemId] BIGINT IDENTITY(1,1) NOT NULL,
  [MUAId] INT NOT NULL,
  [Title] NVARCHAR(200) NULL,
  [Description] NVARCHAR(MAX) NULL,
  [MediaUrl] NVARCHAR(500) NOT NULL,
  [CreatedAtUtc] DATETIME2(3) DEFAULT SYSUTCDATETIME() NOT NULL,
  CONSTRAINT PK_PortfolioItems PRIMARY KEY CLUSTERED ([ItemId])
)
GO

-- Roles
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL DROP TABLE [dbo].[Roles]
GO
CREATE TABLE [dbo].[Roles] (
  [RoleId] TINYINT NOT NULL,
  [RoleName] VARCHAR(30) NOT NULL,
  CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED ([RoleId]),
  CONSTRAINT UQ_Roles_RoleName UNIQUE ([RoleName])
)
GO

-- Services
IF OBJECT_ID(N'[dbo].[Services]', 'U') IS NOT NULL DROP TABLE [dbo].[Services]
GO
CREATE TABLE [dbo].[Services] (
  [ServiceId] BIGINT IDENTITY(1,1) NOT NULL,
  [MuaId] INT NOT NULL,
  [Name] NVARCHAR(160) NOT NULL,
  [Description] NVARCHAR(MAX) NULL,
  [BasePrice] DECIMAL(18,2) NOT NULL,
  [Currency] CHAR(3) DEFAULT 'VND' NOT NULL,
  [DurationMin] INT NOT NULL,
  [Active] BIT DEFAULT 1 NOT NULL,
  [CategoryId] INT NULL,
  CONSTRAINT PK_Services PRIMARY KEY CLUSTERED ([ServiceId])
)
GO

-- TimeOffs
IF OBJECT_ID(N'[dbo].[TimeOffs]', 'U') IS NOT NULL DROP TABLE [dbo].[TimeOffs]
GO
CREATE TABLE [dbo].[TimeOffs] (
  [TimeOffId] BIGINT IDENTITY(1,1) NOT NULL,
  [MuaId] INT NOT NULL,
  [StartUtc] DATETIME2(3) NOT NULL,
  [EndUtc] DATETIME2(3) NOT NULL,
  [Reason] NVARCHAR(200) NULL,
  CONSTRAINT PK_TimeOffs PRIMARY KEY CLUSTERED ([TimeOffId])
)
GO

-- Tokens
IF OBJECT_ID(N'[dbo].[Tokens]', 'U') IS NOT NULL DROP TABLE [dbo].[Tokens]
GO
CREATE TABLE [dbo].[Tokens] (
  [Email] VARCHAR(255) NOT NULL,
  [Token] CHAR(36) NOT NULL,
  [Expired] DATETIME NOT NULL,
  [Status] BIT DEFAULT 0 NOT NULL,
  CONSTRAINT PK_Tokens PRIMARY KEY CLUSTERED ([Email])
)
GO

-- Users
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL DROP TABLE [dbo].[Users]
GO
CREATE TABLE [dbo].[Users] (
  [UserId] INT IDENTITY(1,1) NOT NULL,
  [Username] VARCHAR(50) NOT NULL,
  [Email] VARCHAR(255) NOT NULL,
  [PasswordHash] VARBINARY(256) NOT NULL,
  [FullName] VARCHAR(100) NULL,
  [PhoneNumber] VARCHAR(30) NULL,
  [AvatarUrl] VARCHAR(500) NULL,
  [IsActive] BIT DEFAULT 1 NOT NULL,
  [IsEmailVerified] BIT DEFAULT 0 NOT NULL,
  [RoleId] TINYINT DEFAULT 1 NOT NULL,
  CONSTRAINT PK_Users PRIMARY KEY CLUSTERED ([UserId]),
  CONSTRAINT UQ_Users_Username UNIQUE ([Username]),
  CONSTRAINT UQ_Users_Email UNIQUE ([Email])
)
GO

-- WorkingHours
IF OBJECT_ID(N'[dbo].[WorkingHours]', 'U') IS NOT NULL DROP TABLE [dbo].[WorkingHours]
GO
CREATE TABLE [dbo].[WorkingHours] (
  [WorkingHourId] BIGINT IDENTITY(1,1) NOT NULL,
  [MuaId] INT NOT NULL,
  [DayOfWeek] TINYINT NOT NULL,
  [StartTime] TIME NOT NULL,
  [EndTime] TIME NOT NULL,
  CONSTRAINT PK_WorkingHours PRIMARY KEY CLUSTERED ([WorkingHourId])
)
GO

-- ==========================
-- FOREIGN KEYS
-- ==========================

-- BookingItems
ALTER TABLE [dbo].[BookingItems] 
ADD CONSTRAINT FK_BookingItems_Booking FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings] ([BookingId])
ON DELETE NO ACTION ON UPDATE NO ACTION

ALTER TABLE [dbo].[BookingItems] 
ADD CONSTRAINT FK_BookingItems_Service FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Services] ([ServiceId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- Bookings
ALTER TABLE [dbo].[Bookings] 
ADD CONSTRAINT FK_Bookings_Customer FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerProfiles] ([CustomerId])
ON DELETE NO ACTION ON UPDATE NO ACTION

ALTER TABLE [dbo].[Bookings] 
ADD CONSTRAINT FK_Bookings_MUA FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- CustomerProfiles
ALTER TABLE [dbo].[CustomerProfiles] 
ADD CONSTRAINT FK_CustomerProfiles_User FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users] ([UserId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- MUAProfiles
ALTER TABLE [dbo].[MUAProfiles] 
ADD CONSTRAINT FK_MUAProfiles_User FOREIGN KEY ([MUAId]) REFERENCES [dbo].[Users] ([UserId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- PortfolioItems
ALTER TABLE [dbo].[PortfolioItems] 
ADD CONSTRAINT FK_PortfolioItems_MUA FOREIGN KEY ([MUAId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- Services
ALTER TABLE [dbo].[Services] 
ADD CONSTRAINT FK_Services_MUA FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION

ALTER TABLE [dbo].[Services] 
ADD CONSTRAINT FK_Services_Category FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- TimeOffs
ALTER TABLE [dbo].[TimeOffs] 
ADD CONSTRAINT FK_TimeOffs_MUA FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- Users
ALTER TABLE [dbo].[Users] 
ADD CONSTRAINT FK_Users_Roles FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- WorkingHours
ALTER TABLE [dbo].[WorkingHours] 
ADD CONSTRAINT FK_WorkingHours_MUA FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- Reviews
ALTER TABLE [dbo].[Reviews] 
ADD CONSTRAINT FK_Reviews_Booking FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings] ([BookingId])
ON DELETE CASCADE ON UPDATE NO ACTION

ALTER TABLE [dbo].[Reviews] 
ADD CONSTRAINT FK_Reviews_Customer FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerProfiles] ([CustomerId])
ON DELETE NO ACTION ON UPDATE NO ACTION

ALTER TABLE [dbo].[Reviews] 
ADD CONSTRAINT FK_Reviews_MUA FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId])
ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- ==========================
-- UNIQUE CONSTRAINT FOR REVIEWS
-- ==========================
ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT UQ_Review_Booking_Customer UNIQUE ([BookingId], [CustomerId])
GO

-- ==========================
-- INDEXES FOR REVIEWS
-- ==========================
CREATE INDEX IX_Reviews_MuaId ON [dbo].[Reviews] ([MuaId])
CREATE INDEX IX_Reviews_CustomerId ON [dbo].[Reviews] ([CustomerId])
GO
