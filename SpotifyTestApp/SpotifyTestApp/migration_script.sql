-- Create the EF Migrations History table if it doesn't exist
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

-- Start transaction for database updates
BEGIN TRANSACTION;

-- Create the Users table if it doesn't exist
IF OBJECT_ID(N'[Users]') IS NULL
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(MAX) NOT NULL,
        [FirstName] nvarchar(50),
        [LastName] nvarchar(50),
        [CreatedAt] datetime NOT NULL DEFAULT GETDATE()
    );
END;

-- Create the StudySessions table if it doesn't exist
IF OBJECT_ID(N'[StudySessions]') IS NULL
BEGIN
    CREATE TABLE [StudySessions] (
        [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [UserId] nvarchar(MAX),
        [StudyDate] datetime,
        [SongAudioFeaturesJson] nvarchar(MAX),
        [MusicHistory] nvarchar(MAX),
        [Productivity] int,
        [Genre] nvarchar(MAX)
    );
END;

-- Insert migration history entries
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
    (N'20241207174329_InitialCreate', N'8.0.0'),
    (N'20241210025907_AddUserModel', N'8.0.0');

-- Commit the transaction
COMMIT;
GO
