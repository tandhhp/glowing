CREATE TABLE [dbo].[Users] (
    [Id] NVARCHAR(64) NOT NULL PRIMARY KEY,
    [DisplayName] NVARCHAR(120) NULL,
    [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE [dbo].[UserProgress] (
    [UserId] NVARCHAR(64) NOT NULL PRIMARY KEY,
    [TotalXp] INT NOT NULL DEFAULT 0,
    [CurrentStreak] INT NOT NULL DEFAULT 0,
    [BestStreak] INT NOT NULL DEFAULT 0,
    [StreakFreezeCount] INT NOT NULL DEFAULT 1,
    [Gold] INT NOT NULL DEFAULT 10,
    [PerfectScoreStreak] INT NOT NULL DEFAULT 0,
    [LastStreakDate] DATE NULL,
    [LastFiveMinuteQuestDate] DATE NULL,
    [LastGoldenHourQuestDate] DATE NULL,
    CONSTRAINT [FK_UserProgress_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);

CREATE TABLE [dbo].[PracticeSessions] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(64) NOT NULL,
    [TopicSlug] NVARCHAR(100) NOT NULL,
    [Transcript] NVARCHAR(MAX) NOT NULL,
    [Score] INT NOT NULL,
    [XpEarned] INT NOT NULL,
    [PracticedAtUtc] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [FK_PracticeSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);

CREATE TABLE [dbo].[UserBadges] (
    [UserId] NVARCHAR(64) NOT NULL,
    [BadgeCode] NVARCHAR(100) NOT NULL,
    [EarnedAtUtc] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_UserBadges] PRIMARY KEY ([UserId], [BadgeCode]),
    CONSTRAINT [FK_UserBadges_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);
