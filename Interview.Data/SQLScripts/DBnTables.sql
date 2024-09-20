CREATE DATABASE [WorkData]
GO

USE [WorkData]
GO
CREATE TABLE Profiles (
    ProfileId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DateOfBirth DATE,
    PhoneNumber NVARCHAR(15),
    EmailId NVARCHAR(100)
);

CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProfileId INT,
	TaskName NVARCHAR(255),
    TaskDescription NVARCHAR(255),
    StartTime DATE,
	Status  INT

    CONSTRAINT FK_ProfileTask FOREIGN KEY (ProfileId)
        REFERENCES Profiles(ProfileId)
        ON DELETE CASCADE
);
