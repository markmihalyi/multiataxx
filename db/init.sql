CREATE DATABASE MultiAtaxx
GO
USE [MultiAtaxx]
GO
/****** Object:  Table [dbo].[Boosters]    Script Date: 2025. 02. 27. 18:40:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Boosters](
	[id] [int] NOT NULL,
	[name] [nvarchar](30) NOT NULL,
	[price] [float] NOT NULL,
 CONSTRAINT [PK_Boosters] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Matches]    Script Date: 2025. 02. 27. 18:40:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Matches](
	[id] [int] NOT NULL,
	[player1_id] [int] NOT NULL,
	[player2_id] [int] NOT NULL,
	[winner_id] [int] NULL,
	[steps] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Matches] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OwnedBoosters]    Script Date: 2025. 02. 27. 18:40:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OwnedBoosters](
	[user_id] [int] NOT NULL,
	[booster_id] [int] NOT NULL,
	[amount] [int] NOT NULL,
 CONSTRAINT [PK_OwnedBoosters] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[booster_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2025. 02. 27. 18:40:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [int] NOT NULL,
	[username] [nvarchar](20) NOT NULL,
	[password] [nvarchar](100) NOT NULL,
	[balance] [float] NOT NULL,
 CONSTRAINT [PK_accounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserStatistics]    Script Date: 2025. 02. 27. 18:40:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserStatistics](
	[user_id] [int] NOT NULL,
	[wins] [int] NOT NULL,
	[losses] [int] NOT NULL,
	[draws] [int] NOT NULL,
	[total_time_played] [int] NOT NULL,
	[average_game_duration] [int] NOT NULL,
	[fastest_win_time] [int] NULL,
 CONSTRAINT [PK_statistics] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_1] FOREIGN KEY([player1_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_1]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_2] FOREIGN KEY([player2_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_2]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Users_Winner] FOREIGN KEY([winner_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Users_Winner]
GO
ALTER TABLE [dbo].[OwnedBoosters]  WITH CHECK ADD  CONSTRAINT [FK_OwnedBoosters_Boosters] FOREIGN KEY([booster_id])
REFERENCES [dbo].[Boosters] ([id])
GO
ALTER TABLE [dbo].[OwnedBoosters] CHECK CONSTRAINT [FK_OwnedBoosters_Boosters]
GO
ALTER TABLE [dbo].[OwnedBoosters]  WITH CHECK ADD  CONSTRAINT [FK_OwnedBoosters_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[OwnedBoosters] CHECK CONSTRAINT [FK_OwnedBoosters_Users]
GO
ALTER TABLE [dbo].[UserStatistics]  WITH CHECK ADD  CONSTRAINT [FK_Statistics_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[UserStatistics] CHECK CONSTRAINT [FK_Statistics_Users]
GO
