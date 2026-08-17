USE AISupportHub;
GO

IF OBJECT_ID('dbo.Messages', 'U') IS NOT NULL
    DROP TABLE dbo.Messages;
GO

IF OBJECT_ID('dbo.Tickets', 'U') IS NOT NULL
    DROP TABLE dbo.Tickets;
GO

IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL
    DROP TABLE dbo.Documents;
GO

IF OBJECT_ID('dbo.Conversations', 'U') IS NOT NULL
    DROP TABLE dbo.Conversations;
GO


CREATE TABLE dbo.Conversations
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Title NVARCHAR(200) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Conversations_CreatedAt
        DEFAULT GETUTCDATE(),

    SupportStatus NVARCHAR(30) NULL,

    SupportName NVARCHAR(150) NULL,

    SupportEmail NVARCHAR(200) NULL,

    SupportDescription NVARCHAR(MAX) NULL
);
GO


CREATE TABLE dbo.Messages
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    ConversationId INT NOT NULL,

    Role NVARCHAR(20) NOT NULL,

    Content NVARCHAR(MAX) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Messages_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Messages_Conversations
        FOREIGN KEY (ConversationId)
        REFERENCES dbo.Conversations(Id)
        ON DELETE CASCADE
);
GO


CREATE TABLE dbo.Documents
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    FileName NVARCHAR(255) NOT NULL,

    Content NVARCHAR(MAX) NOT NULL,

    UploadedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Documents_UploadedAt
        DEFAULT GETUTCDATE()
);
GO


CREATE TABLE dbo.Tickets
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    ConversationId INT NOT NULL,

    TicketNumber NVARCHAR(50) NULL,

    Name NVARCHAR(150) NOT NULL,

    Email NVARCHAR(200) NOT NULL,

    Description NVARCHAR(MAX) NOT NULL,

    Status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Tickets_Status
        DEFAULT 'OPEN',

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Tickets_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Tickets_Conversations
        FOREIGN KEY (ConversationId)
        REFERENCES dbo.Conversations(Id)
        ON DELETE CASCADE
);
GO


CREATE INDEX IX_Messages_ConversationId
ON dbo.Messages(ConversationId);
GO

CREATE INDEX IX_Tickets_ConversationId
ON dbo.Tickets(ConversationId);
GO

CREATE INDEX IX_Tickets_Status
ON dbo.Tickets(Status);
GO

CREATE UNIQUE INDEX UX_Tickets_TicketNumber
ON dbo.Tickets(TicketNumber)
WHERE TicketNumber IS NOT NULL;
GO