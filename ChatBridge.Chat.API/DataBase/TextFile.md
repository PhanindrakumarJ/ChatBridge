# ChatBridge — Initial Database Schema

## Purpose

This document contains the initial SQL schema for ChatBridge, a one-to-one customer–vendor chat application.

**Database:** Azure SQL Database  
**Approach:** Database-first  
**Schema:** `dbo`

## Tables

| Table | Purpose |
|---|---|
| `Users` | Stores both vendors and customers, distinguished by `Role` |
| `Conversations` | Connects one vendor and one customer |
| `Messages` | Stores messages belonging to a conversation |

## SQL Script

Run the following script against the ChatBridge database using SQL Server Management Studio (SSMS).

```sql
CREATE TABLE dbo.Users
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Users PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL,

    OrganizationName NVARCHAR(200) NULL,
    OrganizationDescription NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(30) NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT UQ_Users_Email UNIQUE (Email),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Vendor', 'Customer'))
);

CREATE TABLE dbo.Conversations
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Conversations PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    VendorId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Conversations_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Conversations_Vendor
        FOREIGN KEY (VendorId)
        REFERENCES dbo.Users(Id),

    CONSTRAINT FK_Conversations_Customer
        FOREIGN KEY (CustomerId)
        REFERENCES dbo.Users(Id),

    CONSTRAINT CK_Conversations_DifferentUsers
        CHECK (VendorId <> CustomerId)
);

CREATE TABLE dbo.Messages
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Messages PRIMARY KEY
        DEFAULT NEWSEQUENTIALID(),

    ConversationId UNIQUEIDENTIFIER NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,

    Content NVARCHAR(4000) NOT NULL,

    SentAt DATETIME2 NOT NULL
        CONSTRAINT DF_Messages_SentAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Messages_Conversation
        FOREIGN KEY (ConversationId)
        REFERENCES dbo.Conversations(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_Messages_Sender
        FOREIGN KEY (SenderId)
        REFERENCES dbo.Users(Id),

    CONSTRAINT CK_Messages_Content
        CHECK (LEN(TRIM(Content)) > 0)
);

CREATE INDEX IX_Conversations_VendorId
    ON dbo.Conversations(VendorId);

CREATE INDEX IX_Conversations_CustomerId
    ON dbo.Conversations(CustomerId);

CREATE INDEX IX_Messages_ConversationId_SentAt
    ON dbo.Messages(ConversationId, SentAt);
```

## Important Notes

- `Users.Role` distinguishes vendors from customers.
- `VendorId` and `CustomerId` reference `Users.Id`.
- `Messages.SenderId` references the user who sent each message.
- The API must verify that the sender belongs to the conversation.
- The unique email constraint assumes each user has a distinct email address.
- This is the initial schema; vendor certificates may be introduced in a later script.

## Execution Status

- [ ] Connect to Azure SQL using SSMS.
- [ ] Verify the target database.
- [ ] Execute the SQL script.
- [ ] Verify all three tables and their constraints.
