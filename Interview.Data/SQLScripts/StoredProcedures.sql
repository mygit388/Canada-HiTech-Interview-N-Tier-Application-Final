CREATE PROCEDURE sp_InsertProfile
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @DateOfBirth DATE,
    @PhoneNumber NVARCHAR(15),
    @EmailId NVARCHAR(100)
AS
BEGIN
    INSERT INTO Profiles (FirstName, LastName, DateOfBirth, PhoneNumber, EmailId)
    VALUES (@FirstName, @LastName, @DateOfBirth, @PhoneNumber, @EmailId);
END
GO

CREATE PROCEDURE sp_UpdateProfile
    @ProfileId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @DateOfBirth DATE,
    @PhoneNumber NVARCHAR(15),
    @EmailId NVARCHAR(100)
AS
BEGIN
    UPDATE Profiles
    SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth,
        PhoneNumber = @PhoneNumber, EmailId = @EmailId
    WHERE ProfileId = @ProfileId;
END
GO

CREATE PROCEDURE sp_DeleteProfile
    @ProfileId INT
AS
BEGIN
    DELETE FROM Profiles WHERE ProfileId = @ProfileId;
END
GO

CREATE PROCEDURE sp_GetProfilesByID
@ProfileId INT
AS
BEGIN
    SELECT * FROM Profiles Where ProfileId = @ProfileId;
END
GO
CREATE PROCEDURE sp_GetProfiles
AS
BEGIN
    SELECT * FROM Profiles;
END
GO

-- For Tasks
CREATE PROCEDURE sp_AddTask
    @ProfileId INT,
	@TaskName NVARCHAR(255),
    @TaskDescription NVARCHAR(255),
    @StartTime DATE,
	@Status INT
AS
BEGIN
    INSERT INTO Tasks (ProfileId,TaskName,TaskDescription, StartTime,Status)
    VALUES (@ProfileId,@TaskName,@TaskDescription, @StartTime,@Status);
END
GO

CREATE PROCEDURE sp_UpdateTask
      @Id INT,
	 @ProfileId INT,
	 @TaskName NVARCHAR(255),
    @TaskDescription NVARCHAR(255),
	@StartTime DATE,
    @Status INT
AS
BEGIN
    UPDATE Tasks
    SET TaskName=@TaskName,TaskDescription = @TaskDescription, StartTime = @StartTime,Status=@Status
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteTask
    @Id INT
AS
BEGIN
    DELETE FROM Tasks WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_GetTasks
    @ProfileId INT
AS
BEGIN
    SELECT * FROM Tasks WHERE ProfileId = @ProfileId;
END
GO
CREATE PROCEDURE sp_GetTaskByID
    @Id INT
AS
BEGIN
    SELECT * FROM Tasks WHERE Id = @Id;
END
GO
CREATE PROCEDURE sp_LoginCredentials
@userName nvarchar(50),@userPassword nvarchar(50)
AS
BEGIN
SET NOCOUNT ON;

SELECT 
        a.userName,
        a.userPassword,
        r.roleName
FROM 
        AccountTable a
    INNER JOIN 
        Roles r ON a.roleId = r.roleId
WHERE 
        a.userName = @userName
        AND a.userPassword = @userPassword;
END
GO