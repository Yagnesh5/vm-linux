
-- 12/Jun/2019 - changes by Vipul requested By Sandeepji
-- ASPNETUSERS - ADD COLUMN LastUpdateDate
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') 
         AND name = 'LastUpdateDate'
)
BEGIN
ALTER TABLE AspNetUsers add LastUpdateDate date
END
GO

-- Candidate - ADD COLUMN LastUpdateDate
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Candidate]') 
         AND name = 'LastUpdateDate'
)
BEGIN
ALTER TABLE Candidate add LastUpdateDate date
END
GO

-- Candidate - ADD COLUMN Job
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Job]') 
         AND name = 'LastUpdateDate'
)
BEGIN
ALTER TABLE Job add LastUpdateDate date
END
GO

--======================================================
IF NOT EXISTS(
SELECT * FROM INFORMATION_SCHEMA.TABLES
           WHERE TABLE_NAME = N'CandidatesResume')

BEGIN
CREATE TABLE [dbo].[CandidatesResume](
	[Candidate_resume_Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NULL,
	[CandidateResume] [nvarchar](max) NULL,
	[CreatedDate] [date] NULL DEFAULT (getdate()),
 CONSTRAINT [PK_CandidatesResume] PRIMARY KEY CLUSTERED 
(
	[Candidate_resume_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[CandidatesResume]  WITH CHECK ADD  CONSTRAINT [FK_CandidatesResume_Candidate] FOREIGN KEY([CandidateId])
REFERENCES [dbo].[Candidate] ([CandidateId])


ALTER TABLE [dbo].[CandidatesResume] CHECK CONSTRAINT [FK_CandidatesResume_Candidate]


END 
GO

--==================================================================
IF NOT EXISTS(
SELECT * FROM INFORMATION_SCHEMA.TABLES
           WHERE TABLE_NAME = N'SkillMaster')

BEGIN
CREATE TABLE [dbo].[SkillMaster](
	[SkillId] [int] IDENTITY(1,1) NOT NULL,
	[SkillName] [nvarchar](200) NULL,
	[Active] [bit] NOT NULL,
	[LastUpdateDate] [date] NULL,
 CONSTRAINT [PK_SkillMaster] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

--====================================================================
IF NOT EXISTS(SELECT * FROM Entity WHERE EntityName='SkillMaster')
BEGIN
INSERT INTO Entity VALUES('SkillMaster')
END
GO


--============================================================
IF NOT EXISTS(
SELECT * FROM INFORMATION_SCHEMA.TABLES
           WHERE TABLE_NAME = N'CandidateTestURL')

BEGIN

CREATE TABLE [dbo].[CandidateTestURL](
	[CandTestURLId] [int] IDENTITY(1,1) NOT NULL,
	[CandId] [int] NOT NULL,
	[TestURLID] [uniqueidentifier] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_CandidateTestURL] PRIMARY KEY CLUSTERED 
(
	[CandTestURLId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO
