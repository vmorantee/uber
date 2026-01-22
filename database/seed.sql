USE RideSharingDB;
GO

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsDriverApproved, CurrentContext, PhoneNumber, VehicleModel, LicensePlate)
VALUES 
('john.doe@example.com', 'HASHED_PASSWORD_123', 'John', 'Doe', 'Passenger', 0, 'Passenger', '+1234567890', NULL, NULL),
('jane.smith@example.com', 'HASHED_PASSWORD_456', 'Jane', 'Smith', 'Passenger', 0, 'Passenger', '+1234567891', NULL, NULL),
('mike.driver@example.com', 'HASHED_PASSWORD_789', 'Mike', 'Driver', 'Driver', 1, 'Driver', '+1234567892', 'Toyota Camry 2022', 'ABC-1234'),
('sarah.driver@example.com', 'HASHED_PASSWORD_012', 'Sarah', 'Driver', 'Driver', 1, 'Driver', '+1234567893', 'Honda Accord 2023', 'XYZ-5678'),
('pending.driver@example.com', 'HASHED_PASSWORD_345', 'Tom', 'Pending', 'Driver', 0, 'Passenger', '+1234567894', 'Ford Focus 2021', 'PEN-9999'),
('admin@ridesharing.com', 'HASHED_PASSWORD_ADMIN', 'Admin', 'User', 'Admin', 0, 'Passenger', '+1234567895', NULL, NULL);

INSERT INTO Wallets (UserID, Balance, Currency)
VALUES 
(1, 150.00, 'USD'),
(2, 200.00, 'USD'),
(3, 450.00, 'USD'),
(4, 320.00, 'USD'),
(5, 50.00, 'USD'),
(6, 0.00, 'USD');

INSERT INTO WalletTransactions (WalletID, Amount, Type, TransactionDate, ReferenceID, Description)
VALUES 
(1, 200.00, 'TopUp', DATEADD(DAY, -10, GETDATE()), NULL, 'Initial top-up'),
(1, -50.00, 'RidePayment', DATEADD(DAY, -8, GETDATE()), 1, 'Payment for ride #1'),
(3, 40.00, 'Earning', DATEADD(DAY, -8, GETDATE()), 1, 'Earning from ride #1'),
(2, 250.00, 'TopUp', DATEADD(DAY, -5, GETDATE()), NULL, 'Wallet reload'),
(2, -50.00, 'RidePayment', DATEADD(DAY, -3, GETDATE()), 2, 'Payment for ride #2'),
(4, 40.00, 'Earning', DATEADD(DAY, -3, GETDATE()), 2, 'Earning from ride #2'),
(5, 50.00, 'TopUp', DATEADD(DAY, -1, GETDATE()), NULL, 'First top-up');

INSERT INTO Rides (PassengerID, DriverID, StartLocationLat, StartLocationLng, EndLocationLat, EndLocationLng, StartAddress, EndAddress, Price, Distance, Status, StartTime, AcceptedTime, CompletedTime)
VALUES 
(1, 3, 40.748817, -73.985428, 40.758896, -73.985130, '350 5th Ave, New York, NY', '1 E 42nd St, New York, NY', 50.00, 1.5, 'COMPLETED', DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -8, DATEADD(MINUTE, 2, GETDATE())), DATEADD(DAY, -8, DATEADD(MINUTE, 15, GETDATE()))),
(2, 4, 40.741895, -73.989308, 40.712776, -74.005974, '23 W 23rd St, New York, NY', '285 Fulton St, New York, NY', 50.00, 3.2, 'COMPLETED', DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -3, DATEADD(MINUTE, 1, GETDATE())), DATEADD(DAY, -3, DATEADD(MINUTE, 20, GETDATE()))),
(1, 3, 40.730610, -73.935242, 40.689247, -73.979681, 'Brooklyn, NY', 'Downtown Brooklyn, NY', 35.00, 4.5, 'COMPLETED', DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, DATEADD(MINUTE, 3, GETDATE())), DATEADD(DAY, -1, DATEADD(MINUTE, 25, GETDATE()))),
(2, 4, 40.758896, -73.985130, 40.706086, -74.008827, 'Times Square, New York, NY', 'Brooklyn Bridge Park, Brooklyn, NY', 45.00, 5.1, 'COMPLETED', DATEADD(HOUR, -2, GETDATE()), DATEADD(HOUR, -2, DATEADD(MINUTE, 2, GETDATE())), DATEADD(HOUR, -2, DATEADD(MINUTE, 28, GETDATE())));

INSERT INTO Disputes (RideID, ReportingUserID, Reason, Status, ResolutionDecision, RefundAmount)
VALUES 
(4, 2, 'Driver took a longer route than necessary. The ride was supposed to be 3 miles but ended up being 5.1 miles.', 'OPEN', NULL, 0.00);

INSERT INTO DriverLocations (DriverID, Latitude, Longitude, IsOnline, UpdatedAt)
VALUES 
(3, 40.748817, -73.985428, 1, GETDATE()),
(4, 40.741895, -73.989308, 1, GETDATE());

INSERT INTO Banners (ImageUrl, TargetUrl, IsActive, DisplayOrder)
VALUES 
('https://ridesharing.blob.core.windows.net/banners/promo-winter-2026.jpg', 'https://ridesharing.com/promotions/winter', 1, 1),
('https://ridesharing.blob.core.windows.net/banners/driver-bonus.jpg', 'https://ridesharing.com/driver-signup', 1, 2);
