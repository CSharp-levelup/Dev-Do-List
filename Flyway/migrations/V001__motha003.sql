USE DevDoListDB
GO

CREATE TABLE [User] IF NOT EXISTS (
  [UserID] integer IDENTITY(1, 1),
  [Username] varchar(50) NOT NULL,
  [UserPicUrl] varchar(100) NOT NULL,
  [RoleID] integer NOT NULL
  CONSTRAINT PK_User PRIMARY KEY (UserID),
  CONSTRAINT UQ_Username UNIQUE (Username)
)
GO

CREATE TABLE [Comment] IF NOT EXISTS (
  [CommentID] integer IDENTITY(1, 1),
  [TaskId] integer NOT NULL,
  [Comment] varchar(100) NOT NULL,
  [DateCommented] datetime2 NOT NULL,
  CONSTRAINT PK_Comment PRIMARY KEY (CommentID)
)
GO

CREATE TABLE [TaskType] IF NOT EXISTS (
  [TaskTypeID] integer IDENTITY(1, 1),
  [TaskTypeDescription] varchar(50) NOT NULL,
  CONSTRAINT PK_TaskType PRIMARY KEY (TaskTypeID)
)
GO

CREATE TABLE [Status] IF NOT EXISTS (
  [StatusID] integer IDENTITY(1, 1),
  [StatusType] varchar(50) NOT NULL,
  CONSTRAINT PK_Status PRIMARY KEY (StatusID)
)
GO

CREATE TABLE [Task] IF NOT EXISTS (
  [TaskID] integer IDENTITY(1, 1),
  [Title] varchar(50) NOT NULL,
  [Description] varchar(100),
  [DateCreated] datetime2 NOT NULL,
  [DueDate] datetime2,
  [UserID] integer NOT NULL,
  [StatusID] integer NOT NULL,
  [TaskTypeID] integer NOT NULL,
  CONSTRAINT PK_Task PRIMARY KEY (TaskID)
)
GO

CREATE TABLE [Role IF NOT EXISTS (
  [RoleID] integer IDENTITY(1, 1),
  [RoleType] varchar(50) NOT NULL
  CONSTRAINT PK_Role PRIMARY KEY (RoleID)
)
GO

ALTER TABLE [User] ADD CONSTRAINT FK_UserRole FOREIGN KEY ([RoleID]) REFERENCES [Role] ([RoleID])
GO

ALTER TABLE [Comment] ADD CONSTRAINT FK_TaskComment FOREIGN KEY ([TaskId]) REFERENCES [Task] ([TaskID])
GO

ALTER TABLE [Task] ADD CONSTRAINT FK_UserTask FOREIGN KEY ([UserID]) REFERENCES [User] ([UserID])
GO

ALTER TABLE [Task] ADD CONSTRAINT FK_StatusTask FOREIGN KEY ([StatusID]) REFERENCES [Status] ([StatusID])
GO

ALTER TABLE [Task] ADD CONSTRAINT FK_TypeTask FOREIGN KEY ([TaskTypeID]) REFERENCES [TaskType] ([TaskTypeID])
GO