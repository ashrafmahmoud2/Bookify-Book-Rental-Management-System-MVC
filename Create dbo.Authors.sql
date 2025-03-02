USE [Bookify];
GO

SET IDENTITY_INSERT [dbo].[Authors] ON;
GO

INSERT INTO [dbo].[Authors] ([Id], [Name], [IsDeleted], [CreatedOn], [LastUpdatedOn])
VALUES
    (1, N'J.K. Rowling', 0, GETDATE(), NULL),
    (2, N'George Orwell', 0, GETDATE(), NULL),
    (3, N'Jane Austen', 0, GETDATE(), NULL),
    (4, N'Ernest Hemingway', 0, GETDATE(), NULL),
    (5, N'Leo Tolstoy', 0, GETDATE(), NULL),
    (6, N'Agatha Christie', 0, GETDATE(), NULL),
    (7, N'F. Scott Fitzgerald', 0, GETDATE(), NULL),
    (8, N'William Shakespeare', 0, GETDATE(), NULL),
    (9, N'Mark Twain', 0, GETDATE(), NULL),
    (10, N'Charles Dickens', 0, GETDATE(), NULL),
    (11, N'Emily Brontë', 0, GETDATE(), NULL),
    (12, N'Arthur Conan Doyle', 0, GETDATE(), NULL),
    (13, N'J.R.R. Tolkien', 0, GETDATE(), NULL),
    (14, N'Haruki Murakami', 0, GETDATE(), NULL),
    (15, N'Gabriel García Márquez', 0, GETDATE(), NULL),
    (16, N'Franz Kafka', 0, GETDATE(), NULL),
    (17, N'Fyodor Dostoevsky', 0, GETDATE(), NULL),
    (18, N'Virginia Woolf', 0, GETDATE(), NULL),
    (19, N'Kazuo Ishiguro', 0, GETDATE(), NULL),
    (20, N'Margaret Atwood', 0, GETDATE(), NULL);
GO

SET IDENTITY_INSERT [dbo].[Authors] OFF;
GO
