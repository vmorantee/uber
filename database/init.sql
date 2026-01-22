CREATE DATABASE RideSharingDB;
GO

USE RideSharingDB;
GO

CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Passenger', 'Driver', 'Admin')),
    IsDriverApproved BIT DEFAULT 0,
    CurrentContext NVARCHAR(50) DEFAULT 'Passenger' CHECK (CurrentContext IN ('Passenger', 'Driver')),
    AvatarUrl NVARCHAR(500),
    PhoneNumber NVARCHAR(20),
    VehicleModel NVARCHAR(100),
    LicensePlate NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Wallets (
    ID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL UNIQUE,
    Balance DECIMAL(18,2) DEFAULT 0.00 CHECK (Balance >= 0),
    Currency NVARCHAR(3) DEFAULT 'USD',
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
);

CREATE TABLE WalletTransactions (
    ID INT PRIMARY KEY IDENTITY(1,1),
    WalletID INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('TopUp', 'RidePayment', 'Refund', 'Penalty', 'Earning')),
    TransactionDate DATETIME DEFAULT GETDATE(),
    ReferenceID INT NULL,
    Description NVARCHAR(500),
    FOREIGN KEY (WalletID) REFERENCES Wallets(ID)
);

CREATE TABLE Rides (
    ID INT PRIMARY KEY IDENTITY(1,1),
    PassengerID INT NOT NULL,
    DriverID INT NULL,
    StartLocationLat DECIMAL(10,8) NOT NULL,
    StartLocationLng DECIMAL(11,8) NOT NULL,
    EndLocationLat DECIMAL(10,8) NOT NULL,
    EndLocationLng DECIMAL(11,8) NOT NULL,
    StartAddress NVARCHAR(500),
    EndAddress NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Distance DECIMAL(10,2),
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('SEARCHING', 'ACCEPTED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED')),
    StartTime DATETIME DEFAULT GETDATE(),
    AcceptedTime DATETIME NULL,
    CompletedTime DATETIME NULL,
    CancelledTime DATETIME NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PassengerID) REFERENCES Users(ID),
    FOREIGN KEY (DriverID) REFERENCES Users(ID)
);

CREATE TABLE DriverLocations (
    ID INT PRIMARY KEY IDENTITY(1,1),
    DriverID INT NOT NULL,
    Latitude DECIMAL(10,8) NOT NULL,
    Longitude DECIMAL(11,8) NOT NULL,
    IsOnline BIT DEFAULT 0,
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DriverID) REFERENCES Users(ID)
);

CREATE TABLE Disputes (
    ID INT PRIMARY KEY IDENTITY(1,1),
    RideID INT NOT NULL,
    ReportingUserID INT NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('OPEN', 'RESOLVED')) DEFAULT 'OPEN',
    ResolutionDecision NVARCHAR(1000) NULL,
    RefundAmount DECIMAL(18,2) DEFAULT 0.00,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ResolvedAt DATETIME NULL,
    ResolvedByAdminID INT NULL,
    FOREIGN KEY (RideID) REFERENCES Rides(ID),
    FOREIGN KEY (ReportingUserID) REFERENCES Users(ID),
    FOREIGN KEY (ResolvedByAdminID) REFERENCES Users(ID)
);

CREATE TABLE Banners (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ImageUrl NVARCHAR(1000) NOT NULL,
    TargetUrl NVARCHAR(1000) NULL,
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE INDEX IDX_Users_Email ON Users(Email);
CREATE INDEX IDX_Users_Role ON Users(Role);
CREATE INDEX IDX_Rides_Status ON Rides(Status);
CREATE INDEX IDX_Rides_Passenger ON Rides(PassengerID);
CREATE INDEX IDX_Rides_Driver ON Rides(DriverID);
CREATE INDEX IDX_WalletTransactions_Wallet ON WalletTransactions(WalletID);
CREATE INDEX IDX_Disputes_Status ON Disputes(Status);
CREATE INDEX IDX_DriverLocations_Driver ON DriverLocations(DriverID);
