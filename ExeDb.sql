/*
 Navicat Premium Data Transfer

 Source Server         : SQL Server
 Source Server Type    : SQL Server
 Source Server Version : 16004210 (16.00.4210)
 Source Host           : localhost:1433
 Source Catalog        : project_Exe
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 16004210 (16.00.4210)
 File Encoding         : 65001

 Date: 25/09/2025 16:11:03
*/


-- ----------------------------
-- Table structure for __EFMigrationsHistory
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type IN ('U'))
	DROP TABLE [dbo].[__EFMigrationsHistory]
GO

CREATE TABLE [dbo].[__EFMigrationsHistory] (
  [MigrationId] nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ProductVersion] nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[__EFMigrationsHistory] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of __EFMigrationsHistory
-- ----------------------------
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250924144637_InitialCreate', N'9.0.9')
GO


-- ----------------------------
-- Table structure for Bookings
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND type IN ('U'))
	DROP TABLE [dbo].[Bookings]
GO

CREATE TABLE [dbo].[Bookings] (
  [BookingId] bigint  IDENTITY(1,1) NOT NULL,
  [CustomerId] int  NOT NULL,
  [MuaId] int  NOT NULL,
  [ScheduledStart] datetime2(3)  NULL,
  [AddressLine] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Notes] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Status] int  NOT NULL,
  [TimeM] time(7)  NULL,
  [ServiceId] bigint  NULL
)
GO

ALTER TABLE [dbo].[Bookings] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Bookings
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Bookings] ON
GO

SET IDENTITY_INSERT [dbo].[Bookings] OFF
GO


-- ----------------------------
-- Table structure for Categories
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type IN ('U'))
	DROP TABLE [dbo].[Categories]
GO

CREATE TABLE [dbo].[Categories] (
  [CategoryId] int  IDENTITY(1,1) NOT NULL,
  [CategoryName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Categories] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Categories
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Categories] ON
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (N'1', N'Trang điểm sự kiện – lễ nghi', N'Dịch vụ trang điểm cho các buổi lễ, cưới hỏi, kỷ yếu, lễ hội...')
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (N'2', N'Trang điểm tiệc & giải trí', N'Dịch vụ trang điểm đi tiệc, dạ hội, sinh nhật, gặp gỡ đối tác...')
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (N'3', N'Trang điểm chụp ảnh', N'Dịch vụ trang điểm cho chụp ảnh cưới, ảnh kỷ yếu, ảnh nghệ thuật...')
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (N'4', N'Trang điểm công việc & thường ngày', N'Dịch vụ trang điểm đi làm, đi họp, đi chơi, phong cách Hàn/Tây...')
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (N'5', N'Trang điểm đặc biệt', N'Dịch vụ trang điểm sân khấu, biểu diễn, hóa trang, thi hoa hậu...')
GO

SET IDENTITY_INSERT [dbo].[Categories] OFF
GO


-- ----------------------------
-- Table structure for CustomerProfiles
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerProfiles]') AND type IN ('U'))
	DROP TABLE [dbo].[CustomerProfiles]
GO

CREATE TABLE [dbo].[CustomerProfiles] (
  [CustomerId] int  NOT NULL,
  [DisplayName] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AvatarUrl] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PhoneNumber] varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[CustomerProfiles] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of CustomerProfiles
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityVerifications
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[IdentityVerifications]') AND type IN ('U'))
	DROP TABLE [dbo].[IdentityVerifications]
GO

CREATE TABLE [dbo].[IdentityVerifications] (
  [VerificationId] bigint  IDENTITY(1,1) NOT NULL,
  [UserId] int  NOT NULL,
  [FullName] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [IdNumber] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [DateOfBirth] datetime2(7)  NOT NULL,
  [Address] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [FrontIdImageUrl] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [BackIdImageUrl] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [SelfieImageUrl] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Status] tinyint  NOT NULL,
  [AdminNotes] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ProcessedByAdminId] int  NULL,
  [CreatedAt] datetime2(7) DEFAULT getutcdate() NOT NULL,
  [ProcessedAt] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[IdentityVerifications] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of IdentityVerifications
-- ----------------------------
SET IDENTITY_INSERT [dbo].[IdentityVerifications] ON
GO

SET IDENTITY_INSERT [dbo].[IdentityVerifications] OFF
GO


-- ----------------------------
-- Table structure for MUAProfiles
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[MUAProfiles]') AND type IN ('U'))
	DROP TABLE [dbo].[MUAProfiles]
GO

CREATE TABLE [dbo].[MUAProfiles] (
  [MUAId] int  NOT NULL,
  [DisplayName] nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Bio] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AddressLine] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ExperienceYears] int  NULL,
  [ProfilePublic] bit DEFAULT CONVERT([bit],(1)) NOT NULL
)
GO

ALTER TABLE [dbo].[MUAProfiles] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of MUAProfiles
-- ----------------------------
INSERT INTO [dbo].[MUAProfiles] ([MUAId], [DisplayName], [Bio], [AddressLine], [ExperienceYears], [ProfilePublic]) VALUES (N'1', N'testusername', N'âfaaa', N'dâdad', N'3', N'1')
GO


-- ----------------------------
-- Table structure for PortfolioItems
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PortfolioItems]') AND type IN ('U'))
	DROP TABLE [dbo].[PortfolioItems]
GO

CREATE TABLE [dbo].[PortfolioItems] (
  [ItemId] bigint  IDENTITY(1,1) NOT NULL,
  [MUAId] int  NOT NULL,
  [Title] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Description] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [MediaUrl] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CreatedAtUtc] datetime2(3) DEFAULT sysutcdatetime() NOT NULL
)
GO

ALTER TABLE [dbo].[PortfolioItems] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of PortfolioItems
-- ----------------------------
SET IDENTITY_INSERT [dbo].[PortfolioItems] ON
GO

SET IDENTITY_INSERT [dbo].[PortfolioItems] OFF
GO


-- ----------------------------
-- Table structure for Reviews
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type IN ('U'))
	DROP TABLE [dbo].[Reviews]
GO

CREATE TABLE [dbo].[Reviews] (
  [ReviewId] bigint  IDENTITY(1,1) NOT NULL,
  [BookingId] bigint  NOT NULL,
  [CustomerId] int  NOT NULL,
  [MuaId] int  NOT NULL,
  [Rating] tinyint  NOT NULL,
  [Comment] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CreatedAt] datetime2(3) DEFAULT sysdatetime() NOT NULL
)
GO

ALTER TABLE [dbo].[Reviews] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Reviews
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Reviews] ON
GO

SET IDENTITY_INSERT [dbo].[Reviews] OFF
GO


-- ----------------------------
-- Table structure for Roles
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type IN ('U'))
	DROP TABLE [dbo].[Roles]
GO

CREATE TABLE [dbo].[Roles] (
  [RoleId] tinyint  NOT NULL,
  [RoleName] varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL
)
GO

ALTER TABLE [dbo].[Roles] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Roles
-- ----------------------------
INSERT INTO [dbo].[Roles] ([RoleId], [RoleName]) VALUES (N'1', N'Customer')
GO

INSERT INTO [dbo].[Roles] ([RoleId], [RoleName]) VALUES (N'2', N'Mua')
GO


-- ----------------------------
-- Table structure for Services
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND type IN ('U'))
	DROP TABLE [dbo].[Services]
GO

CREATE TABLE [dbo].[Services] (
  [ServiceId] bigint  IDENTITY(1,1) NOT NULL,
  [MuaId] int  NOT NULL,
  [Name] nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BasePrice] decimal(18,2)  NOT NULL,
  [Currency] char(3) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT 'VND' NOT NULL,
  [DurationMin] int  NOT NULL,
  [Active] bit DEFAULT CONVERT([bit],(1)) NOT NULL,
  [CategoryId] int  NULL
)
GO

ALTER TABLE [dbo].[Services] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Services
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Services] ON
GO

SET IDENTITY_INSERT [dbo].[Services] OFF
GO


-- ----------------------------
-- Table structure for TimeOffs
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[TimeOffs]') AND type IN ('U'))
	DROP TABLE [dbo].[TimeOffs]
GO

CREATE TABLE [dbo].[TimeOffs] (
  [TimeOffId] bigint  IDENTITY(1,1) NOT NULL,
  [MuaId] int  NOT NULL,
  [StartUtc] datetime2(3)  NOT NULL,
  [EndUtc] datetime2(3)  NOT NULL,
  [Reason] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[TimeOffs] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of TimeOffs
-- ----------------------------
SET IDENTITY_INSERT [dbo].[TimeOffs] ON
GO

SET IDENTITY_INSERT [dbo].[TimeOffs] OFF
GO


-- ----------------------------
-- Table structure for Tokens
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Tokens]') AND type IN ('U'))
	DROP TABLE [dbo].[Tokens]
GO

CREATE TABLE [dbo].[Tokens] (
  [Email] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Token] char(36) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Expired] datetime  NOT NULL,
  [Status] bit  NOT NULL
)
GO

ALTER TABLE [dbo].[Tokens] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Tokens
-- ----------------------------

-- ----------------------------
-- Table structure for TwoFactorAuths
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[TwoFactorAuths]') AND type IN ('U'))
	DROP TABLE [dbo].[TwoFactorAuths]
GO

CREATE TABLE [dbo].[TwoFactorAuths] (
  [TwoFactorId] int  IDENTITY(1,1) NOT NULL,
  [UserId] int  NOT NULL,
  [IsEnabled] bit  NOT NULL,
  [SecretKey] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [BackupCodes] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [EnabledAt] datetime2(7)  NULL,
  [LastUsedAt] datetime2(7)  NULL,
  [FailedAttempts] int  NOT NULL,
  [LockedUntil] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[TwoFactorAuths] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of TwoFactorAuths
-- ----------------------------
SET IDENTITY_INSERT [dbo].[TwoFactorAuths] ON
GO

SET IDENTITY_INSERT [dbo].[TwoFactorAuths] OFF
GO


-- ----------------------------
-- Table structure for Users
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type IN ('U'))
	DROP TABLE [dbo].[Users]
GO

CREATE TABLE [dbo].[Users] (
  [UserId] int  IDENTITY(1,1) NOT NULL,
  [Username] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Email] varchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [PasswordHash] varbinary(256)  NOT NULL,
  [FullName] varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [PhoneNumber] varchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AvatarUrl] varchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsActive] bit DEFAULT CONVERT([bit],(1)) NOT NULL,
  [IsEmailVerified] bit  NOT NULL,
  [RoleId] tinyint DEFAULT CONVERT([tinyint],(1)) NOT NULL
)
GO

ALTER TABLE [dbo].[Users] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Users
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Users] ON
GO

INSERT INTO [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [PhoneNumber], [AvatarUrl], [IsActive], [IsEmailVerified], [RoleId]) VALUES (N'1', N'lam01', N'vietlam2k4@gmail.com', 0xD341DB7C246C68B31211241E9BA78A6069D0BACFF625EAAE4BF1DED8275363D2, N'testusername', N'0378790991', NULL, N'1', N'0', N'2')
GO
INSERT INTO [dbo].[Users] 
    ([Username], [Email], [PasswordHash], [FullName], [PhoneNumber], [AvatarUrl], [IsActive], [IsEmailVerified], [RoleId]) 
VALUES 
    ( N'vutran', N'vu0410@gmail.com', 
     0x8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92, 
     N'vutraan', N'0943313588', NULL, N'1', N'0', N'1')
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO


-- ----------------------------
-- Table structure for WorkingHours
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkingHours]') AND type IN ('U'))
	DROP TABLE [dbo].[WorkingHours]
GO

CREATE TABLE [dbo].[WorkingHours] (
  [WorkingHourId] bigint  IDENTITY(1,1) NOT NULL,
  [MuaId] int  NOT NULL,
  [DayOfWeek] tinyint  NOT NULL,
  [StartTime] time(7)  NOT NULL,
  [EndTime] time(7)  NOT NULL
)
GO

ALTER TABLE [dbo].[WorkingHours] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of WorkingHours
-- ----------------------------
SET IDENTITY_INSERT [dbo].[WorkingHours] ON
GO

SET IDENTITY_INSERT [dbo].[WorkingHours] OFF
GO


-- ----------------------------
-- Primary Key structure for table __EFMigrationsHistory
-- ----------------------------
ALTER TABLE [dbo].[__EFMigrationsHistory] ADD CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Bookings
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Bookings]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table Bookings
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Bookings_CustomerId]
ON [dbo].[Bookings] (
  [CustomerId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Bookings_MuaId]
ON [dbo].[Bookings] (
  [MuaId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Bookings
-- ----------------------------
ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED ([BookingId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Categories
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Categories]', RESEED, 5)
GO


-- ----------------------------
-- Indexes structure for table Categories
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Categories_CategoryName]
ON [dbo].[Categories] (
  [CategoryName] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Categories
-- ----------------------------
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([CategoryId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table CustomerProfiles
-- ----------------------------
ALTER TABLE [dbo].[CustomerProfiles] ADD CONSTRAINT [PK_CustomerProfiles] PRIMARY KEY CLUSTERED ([CustomerId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for IdentityVerifications
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[IdentityVerifications]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table IdentityVerifications
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_IdentityVerifications_ProcessedByAdminId]
ON [dbo].[IdentityVerifications] (
  [ProcessedByAdminId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_IdentityVerifications_UserId]
ON [dbo].[IdentityVerifications] (
  [UserId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table IdentityVerifications
-- ----------------------------
ALTER TABLE [dbo].[IdentityVerifications] ADD CONSTRAINT [PK_IdentityVerifications] PRIMARY KEY CLUSTERED ([VerificationId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table MUAProfiles
-- ----------------------------
ALTER TABLE [dbo].[MUAProfiles] ADD CONSTRAINT [PK_MUAProfiles] PRIMARY KEY CLUSTERED ([MUAId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for PortfolioItems
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[PortfolioItems]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table PortfolioItems
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_PortfolioItems_MUAId]
ON [dbo].[PortfolioItems] (
  [MUAId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table PortfolioItems
-- ----------------------------
ALTER TABLE [dbo].[PortfolioItems] ADD CONSTRAINT [PK_PortfolioItems] PRIMARY KEY CLUSTERED ([ItemId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Reviews
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Reviews]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table Reviews
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Reviews_CustomerId]
ON [dbo].[Reviews] (
  [CustomerId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Reviews_MuaId]
ON [dbo].[Reviews] (
  [MuaId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Review_Booking_Customer]
ON [dbo].[Reviews] (
  [BookingId] ASC,
  [CustomerId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Reviews
-- ----------------------------
ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED ([ReviewId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Indexes structure for table Roles
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Roles_RoleName]
ON [dbo].[Roles] (
  [RoleName] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Roles
-- ----------------------------
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Services
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Services]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table Services
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Services_CategoryId]
ON [dbo].[Services] (
  [CategoryId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Services_MuaId]
ON [dbo].[Services] (
  [MuaId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Services
-- ----------------------------
ALTER TABLE [dbo].[Services] ADD CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED ([ServiceId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for TimeOffs
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[TimeOffs]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table TimeOffs
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_TimeOffs_MuaId]
ON [dbo].[TimeOffs] (
  [MuaId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table TimeOffs
-- ----------------------------
ALTER TABLE [dbo].[TimeOffs] ADD CONSTRAINT [PK_TimeOffs] PRIMARY KEY CLUSTERED ([TimeOffId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Tokens
-- ----------------------------
ALTER TABLE [dbo].[Tokens] ADD CONSTRAINT [PK_Tokens] PRIMARY KEY CLUSTERED ([Email])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for TwoFactorAuths
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[TwoFactorAuths]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table TwoFactorAuths
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [IX_TwoFactorAuths_UserId]
ON [dbo].[TwoFactorAuths] (
  [UserId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table TwoFactorAuths
-- ----------------------------
ALTER TABLE [dbo].[TwoFactorAuths] ADD CONSTRAINT [PK_TwoFactorAuths] PRIMARY KEY CLUSTERED ([TwoFactorId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Users
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table Users
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Users_RoleId]
ON [dbo].[Users] (
  [RoleId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Users_Email]
ON [dbo].[Users] (
  [Email] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Users_Username]
ON [dbo].[Users] (
  [Username] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for WorkingHours
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[WorkingHours]', RESEED, 1)
GO


-- ----------------------------
-- Indexes structure for table WorkingHours
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_WorkingHours_MuaId]
ON [dbo].[WorkingHours] (
  [MuaId] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table WorkingHours
-- ----------------------------
ALTER TABLE [dbo].[WorkingHours] ADD CONSTRAINT [PK_WorkingHours] PRIMARY KEY CLUSTERED ([WorkingHourId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Foreign Keys structure for table Bookings
-- ----------------------------
ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerProfiles] ([CustomerId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_MUA] FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT [FK_Bookings_Service] FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[Services] ([ServiceId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table CustomerProfiles
-- ----------------------------
ALTER TABLE [dbo].[CustomerProfiles] ADD CONSTRAINT [FK_CustomerProfiles_User] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table IdentityVerifications
-- ----------------------------
ALTER TABLE [dbo].[IdentityVerifications] ADD CONSTRAINT [FK_IdentityVerifications_Users_ProcessedByAdminId] FOREIGN KEY ([ProcessedByAdminId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[IdentityVerifications] ADD CONSTRAINT [FK_IdentityVerifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table MUAProfiles
-- ----------------------------
ALTER TABLE [dbo].[MUAProfiles] ADD CONSTRAINT [FK_MUAProfiles_User] FOREIGN KEY ([MUAId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table PortfolioItems
-- ----------------------------
ALTER TABLE [dbo].[PortfolioItems] ADD CONSTRAINT [FK_PortfolioItems_MUA] FOREIGN KEY ([MUAId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Reviews
-- ----------------------------
ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Booking] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings] ([BookingId]) ON DELETE CASCADE ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerProfiles] ([CustomerId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Reviews] ADD CONSTRAINT [FK_Reviews_MUA] FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Services
-- ----------------------------
ALTER TABLE [dbo].[Services] ADD CONSTRAINT [FK_Services_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Services] ADD CONSTRAINT [FK_Services_MUA] FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table TimeOffs
-- ----------------------------
ALTER TABLE [dbo].[TimeOffs] ADD CONSTRAINT [FK_TimeOffs_MUA] FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table TwoFactorAuths
-- ----------------------------
ALTER TABLE [dbo].[TwoFactorAuths] ADD CONSTRAINT [FK_TwoFactorAuths_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table WorkingHours
-- ----------------------------
ALTER TABLE [dbo].[WorkingHours] ADD CONSTRAINT [FK_WorkingHours_MUA] FOREIGN KEY ([MuaId]) REFERENCES [dbo].[MUAProfiles] ([MUAId]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

-- CustomerProfiles
INSERT INTO [dbo].[CustomerProfiles] ([CustomerId], [DisplayName], [AvatarUrl], [PhoneNumber]) 
VALUES (1, N'Nguyễn Văn A', NULL, '0901234567'),
       (2, N'Trần Thị B', NULL, '0912345678');

-- Services
SET IDENTITY_INSERT [dbo].[Services] ON
INSERT INTO [dbo].[Services] ([ServiceId], [MuaId], [Name], [Description], [BasePrice], [Currency], [DurationMin], [Active], [CategoryId]) 
VALUES (1, 1, N'Trang điểm cô dâu', N'Trang điểm chuyên nghiệp cho ngày cưới', 1500000, 'VND', 120, 1, 1),
       (2, 1, N'Trang điểm dự tiệc', N'Phong cách nhẹ nhàng đi tiệc', 500000, 'VND', 60, 1, 2),
       (3, 1, N'Trang điểm chụp ảnh kỷ yếu', N'Trang điểm trẻ trung, tự nhiên', 400000, 'VND', 45, 1, 3);
SET IDENTITY_INSERT [dbo].[Services] OFF

-- Bookings
SET IDENTITY_INSERT [dbo].[Bookings] ON
INSERT INTO [dbo].[Bookings] ([BookingId], [CustomerId], [MuaId], [ScheduledStart], [AddressLine], [Notes], [Status], [TimeM], [ServiceId]) 
VALUES (1, 1, 1, '2025-09-30 09:00:00', N'123 Nguyễn Trãi, Hà Nội', N'Khách muốn trang điểm cô dâu', 1, '09:00:00', 1),
       (2, 2, 1, '2025-09-30 18:30:00', N'456 Lê Lợi, Hà Nội', N'Trang điểm dự tiệc sang trọng', 0, '18:30:00', 2);
SET IDENTITY_INSERT [dbo].[Bookings] OFF

-- Reviews
SET IDENTITY_INSERT [dbo].[Reviews] ON
INSERT INTO [dbo].[Reviews] ([ReviewId], [BookingId], [CustomerId], [MuaId], [Rating], [Comment]) 
VALUES (1, 1, 1, 1, 5, N'Dịch vụ rất tốt, MUA thân thiện.'),
       (2, 2, 2, 1, 4, N'Trang điểm đẹp nhưng đến hơi muộn.');
SET IDENTITY_INSERT [dbo].[Reviews] OFF

-- PortfolioItems
INSERT INTO [dbo].[PortfolioItems] ([MUAId], [Title], [Description], [MediaUrl]) 
VALUES (1, N'Trang điểm cô dâu nhẹ nhàng', N'Phong cách Hàn Quốc', 'uploads/portfolio1.jpg'),
       (1, N'Trang điểm dạ hội sang trọng', N'Makeup tone Tây', 'uploads/portfolio2.jpg');

-- TimeOffs
INSERT INTO [dbo].[TimeOffs] ([MuaId], [StartUtc], [EndUtc], [Reason]) 
VALUES (1, '2025-10-05 00:00:00', '2025-10-06 23:59:59', N'Đi công tác');

-- WorkingHours
INSERT INTO [dbo].[WorkingHours] ([MuaId], [DayOfWeek], [StartTime], [EndTime]) 
VALUES (1, 1, '08:00:00', '18:00:00'),
       (1, 2, '08:00:00', '18:00:00'),
       (1, 3, '08:00:00', '18:00:00'),
       (1, 4, '08:00:00', '18:00:00'),
       (1, 5, '08:00:00', '18:00:00');

-- Tokens (demo cho reset password)
INSERT INTO [dbo].[Tokens] ([Email], [Token], [Expired], [Status]) 
VALUES ('vietlam2k4@gmail.com', NEWID(), DATEADD(DAY, 1, GETDATE()), 0);

-- TwoFactorAuths
INSERT INTO [dbo].[TwoFactorAuths] ([UserId], [IsEnabled], [SecretKey], [BackupCodes], [EnabledAt], [FailedAttempts]) 
VALUES (1, 1, N'ABCDEFGH12345678', N'CODE1,CODE2,CODE3', GETDATE(), 0);

