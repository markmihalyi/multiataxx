IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MultiAtaxx')
BEGIN
    CREATE DATABASE MultiAtaxx;
END
GO

-- Ellenőrizzük, hogy az adatbázis biztosan elérhető-e
WHILE NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MultiAtaxx')
BEGIN
    WAITFOR DELAY '00:00:01'
END
GO


USE [MultiAtaxx]
GO
/****** Object:  Table [dbo].[Boosters]    Script Date: 2025. 03. 04. 22:00:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Boosters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	[Price] [int] NOT NULL,
 CONSTRAINT [PK_Boosters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Matches]    Script Date: 2025. 03. 04. 22:00:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Matches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerOneId] [int] NOT NULL,
	[PlayerTwoId] [int] NOT NULL,
	[WinnerId] [int] NULL,
	[Steps] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Matches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OwnedBoosters]    Script Date: 2025. 03. 04. 22:00:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OwnedBoosters](
	[UserId] [int] NOT NULL,
	[BoosterId] [int] NOT NULL,
	[Amount] [int] NOT NULL,
 CONSTRAINT [PK_OwnedBoosters] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[BoosterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2025. 03. 04. 22:00:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](20) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[RefreshToken] [varchar](128) NULL,
	[RefreshTokenExpiryTime] [datetime2](7) NULL,
	[Balance] [int] NOT NULL,
 CONSTRAINT [PK_accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserStatistics]    Script Date: 2025. 03. 04. 22:00:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserStatistics](
	[UserId] [int] NOT NULL,
	[Wins] [int] NOT NULL,
	[Losses] [int] NOT NULL,
	[Draws] [int] NOT NULL,
	[TotalTimePlayed] [int] NOT NULL,
	[AverageGameDuration] [int] NOT NULL,
	[FastestWinTime] [int] NULL,
 CONSTRAINT [PK_statistics] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_1] FOREIGN KEY([PlayerOneId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_1]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_2] FOREIGN KEY([PlayerTwoId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_2]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_Winner] FOREIGN KEY([WinnerId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_Winner]
GO
ALTER TABLE [dbo].[OwnedBoosters]  WITH CHECK ADD  CONSTRAINT [FK_OwnedBoosters_Boosters] FOREIGN KEY([BoosterId])
REFERENCES [dbo].[Boosters] ([Id])
GO
ALTER TABLE [dbo].[OwnedBoosters] CHECK CONSTRAINT [FK_OwnedBoosters_Boosters]
GO
ALTER TABLE [dbo].[OwnedBoosters]  WITH CHECK ADD  CONSTRAINT [FK_OwnedBoosters_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[OwnedBoosters] CHECK CONSTRAINT [FK_OwnedBoosters_Users]
GO
ALTER TABLE [dbo].[UserStatistics]  WITH CHECK ADD  CONSTRAINT [FK_statistics_accounts1] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserStatistics] CHECK CONSTRAINT [FK_statistics_accounts1]
GO
