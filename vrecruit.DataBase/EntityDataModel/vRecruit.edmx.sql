
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/10/2024 12:34:34
-- Generated from EDMX file: F:\SynorisGitHub\vrecruit\vrecruit.DataBase\EntityDataModel\vRecruit.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [vRecruit];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__QuestionG__Compa__74AE54BC]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuestionGroup] DROP CONSTRAINT [FK__QuestionG__Compa__74AE54BC];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessPermission_Entity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccessPermission] DROP CONSTRAINT [FK_AccessPermission_Entity];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessPermission_Entity1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccessPermission] DROP CONSTRAINT [FK_AccessPermission_Entity1];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_Companies]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Companies];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_Job]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Job];
GO
IF OBJECT_ID(N'[dbo].[FK_Answer_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [FK_Answer_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetUsers_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_Candidate_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Candidate] DROP CONSTRAINT [FK_Candidate_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidateExam_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidateExam] DROP CONSTRAINT [FK_CandidateExam_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidateExam_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidateExam] DROP CONSTRAINT [FK_CandidateExam_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidateExam_Job]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidateExam] DROP CONSTRAINT [FK_CandidateExam_Job];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidatesResume_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidatesResume] DROP CONSTRAINT [FK_CandidatesResume_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_City_State]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[City] DROP CONSTRAINT [FK_City_State];
GO
IF OBJECT_ID(N'[dbo].[FK_Client_Companies]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Client] DROP CONSTRAINT [FK_Client_Companies];
GO
IF OBJECT_ID(N'[dbo].[FK_Client_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Client] DROP CONSTRAINT [FK_Client_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientCandidateMapping_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientCandidateMapping] DROP CONSTRAINT [FK_ClientCandidateMapping_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientCandidateMapping_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientCandidateMapping] DROP CONSTRAINT [FK_ClientCandidateMapping_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_Companies_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Companies] DROP CONSTRAINT [FK_Companies_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_Exam_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Exam] DROP CONSTRAINT [FK_Exam_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Interview_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Interview] DROP CONSTRAINT [FK_Interview_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_Interview_InterviewFeedback]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InterviewFeedback] DROP CONSTRAINT [FK_Interview_InterviewFeedback];
GO
IF OBJECT_ID(N'[dbo].[FK_InterviewEmotions_Interview]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InterviewEmotions] DROP CONSTRAINT [FK_InterviewEmotions_Interview];
GO
IF OBJECT_ID(N'[dbo].[FK_InterviewFeedback_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InterviewFeedback] DROP CONSTRAINT [FK_InterviewFeedback_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_InterviewQuestionsTime_Interview]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InterviewQuestionsTime] DROP CONSTRAINT [FK_InterviewQuestionsTime_Interview];
GO
IF OBJECT_ID(N'[dbo].[FK_InterviewQuestionsTime_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InterviewQuestionsTime] DROP CONSTRAINT [FK_InterviewQuestionsTime_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_Job_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Job] DROP CONSTRAINT [FK_Job_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_MCallerData_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MCallerData] DROP CONSTRAINT [FK_MCallerData_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_MCallerData_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MCallerData] DROP CONSTRAINT [FK_MCallerData_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_MCallerData_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MCallerData] DROP CONSTRAINT [FK_MCallerData_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_Question_Companies]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Question] DROP CONSTRAINT [FK_Question_Companies];
GO
IF OBJECT_ID(N'[dbo].[FK_Question_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Question] DROP CONSTRAINT [FK_Question_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Question_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Question] DROP CONSTRAINT [FK_Question_User];
GO
IF OBJECT_ID(N'[dbo].[FK_QuestionAnswerImages_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuestionAnswerImages] DROP CONSTRAINT [FK_QuestionAnswerImages_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_State_Country]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[State] DROP CONSTRAINT [FK_State_Country];
GO
IF OBJECT_ID(N'[dbo].[FK_Test_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Test] DROP CONSTRAINT [FK_Test_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_Test_QuestionGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Test] DROP CONSTRAINT [FK_Test_QuestionGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMapping_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMapping] DROP CONSTRAINT [FK_UserMapping_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMapping_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMapping] DROP CONSTRAINT [FK_UserMapping_Customer];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__MigrationHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__MigrationHistory];
GO
IF OBJECT_ID(N'[dbo].[AccessPermission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccessPermission];
GO
IF OBJECT_ID(N'[dbo].[Activity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Activity];
GO
IF OBJECT_ID(N'[dbo].[ActivityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityType];
GO
IF OBJECT_ID(N'[dbo].[Answer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Answer];
GO
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[Candidate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Candidate];
GO
IF OBJECT_ID(N'[dbo].[CandidateExam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateExam];
GO
IF OBJECT_ID(N'[dbo].[CandidatesResume]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidatesResume];
GO
IF OBJECT_ID(N'[dbo].[CandidateTestRecord]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateTestRecord];
GO
IF OBJECT_ID(N'[dbo].[CandidateTestResult]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateTestResult];
GO
IF OBJECT_ID(N'[dbo].[CandidateTestURL]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateTestURL];
GO
IF OBJECT_ID(N'[dbo].[CandidateUpload]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateUpload];
GO
IF OBJECT_ID(N'[dbo].[City]', 'U') IS NOT NULL
    DROP TABLE [dbo].[City];
GO
IF OBJECT_ID(N'[dbo].[Client]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Client];
GO
IF OBJECT_ID(N'[dbo].[ClientCandidateMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientCandidateMapping];
GO
IF OBJECT_ID(N'[dbo].[Companies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO
IF OBJECT_ID(N'[dbo].[Contact]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Contact];
GO
IF OBJECT_ID(N'[dbo].[Country]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Country];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[Entity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Entity];
GO
IF OBJECT_ID(N'[dbo].[Exam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Exam];
GO
IF OBJECT_ID(N'[dbo].[Feedback]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Feedback];
GO
IF OBJECT_ID(N'[dbo].[Interview]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Interview];
GO
IF OBJECT_ID(N'[dbo].[InterviewEmotions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InterviewEmotions];
GO
IF OBJECT_ID(N'[dbo].[InterviewFeedback]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InterviewFeedback];
GO
IF OBJECT_ID(N'[dbo].[InterviewQuestionsTime]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InterviewQuestionsTime];
GO
IF OBJECT_ID(N'[dbo].[Job]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Job];
GO
IF OBJECT_ID(N'[dbo].[MCallerData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MCallerData];
GO
IF OBJECT_ID(N'[dbo].[NextAction]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NextAction];
GO
IF OBJECT_ID(N'[dbo].[Question]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Question];
GO
IF OBJECT_ID(N'[dbo].[QuestionAnswerImages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionAnswerImages];
GO
IF OBJECT_ID(N'[dbo].[QuestionGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionGroup];
GO
IF OBJECT_ID(N'[dbo].[SendMesssage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SendMesssage];
GO
IF OBJECT_ID(N'[dbo].[SkillMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SkillMaster];
GO
IF OBJECT_ID(N'[dbo].[Skills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Skills];
GO
IF OBJECT_ID(N'[dbo].[State]', 'U') IS NOT NULL
    DROP TABLE [dbo].[State];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[Test]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Test];
GO
IF OBJECT_ID(N'[dbo].[UserMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMapping];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__MigrationHistory'
CREATE TABLE [dbo].[C__MigrationHistory] (
    [MigrationId] nvarchar(150)  NOT NULL,
    [ContextKey] nvarchar(300)  NOT NULL,
    [Model] varbinary(max)  NOT NULL,
    [ProductVersion] nvarchar(32)  NOT NULL
);
GO

-- Creating table 'Activities'
CREATE TABLE [dbo].[Activities] (
    [ActivityId] int IDENTITY(1,1) NOT NULL,
    [ActivityType] nvarchar(50)  NULL,
    [Activity1] nvarchar(max)  NULL,
    [ActivityDate] datetime  NULL,
    [ActivityRemark] nvarchar(50)  NULL,
    [ActivityComplete] bit  NULL,
    [CandidateId] int  NULL,
    [JobId] int  NULL,
    [UserId] uniqueidentifier  NULL,
    [CustomerId] int  NULL,
    [ClientId] int  NULL,
    [Companyid] int  NULL
);
GO

-- Creating table 'ActivityTypes'
CREATE TABLE [dbo].[ActivityTypes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [ActivityType1] nvarchar(100)  NULL
);
GO

-- Creating table 'Answers'
CREATE TABLE [dbo].[Answers] (
    [AnswerId] int IDENTITY(1,1) NOT NULL,
    [QuestiondId] int  NULL,
    [Answer1] nvarchar(max)  NULL,
    [AnswerCorrect] bit  NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL,
    [LastUpdatedBy] nvarchar(128)  NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'CandidateExams'
CREATE TABLE [dbo].[CandidateExams] (
    [CandidateExamId] int IDENTITY(1,1) NOT NULL,
    [ExamDate] datetime  NULL,
    [CandidateId] int  NULL,
    [JobId] int  NULL,
    [ClientId] int  NULL,
    [ExamAnswers] nvarchar(max)  NULL,
    [CandidateExamLinkId] uniqueidentifier  NULL,
    [CustomerId] int  NULL
);
GO

-- Creating table 'Cities'
CREATE TABLE [dbo].[Cities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [City1] nvarchar(100)  NULL,
    [StateId] int  NULL
);
GO

-- Creating table 'Clients'
CREATE TABLE [dbo].[Clients] (
    [ClientId] int IDENTITY(1,1) NOT NULL,
    [CompanyId] int  NULL,
    [ClientName] nvarchar(50)  NULL,
    [ClientAddress] nvarchar(max)  NULL,
    [ClientWWW] nvarchar(max)  NULL,
    [ClientGST] nvarchar(50)  NULL,
    [ClientPhone] nvarchar(50)  NULL,
    [ClientEmail] nvarchar(50)  NULL,
    [Password] nvarchar(50)  NULL,
    [ClientMobile] nvarchar(50)  NULL,
    [ClientStatus] bit  NULL,
    [CustomerId] int  NULL,
    [LastUpdateBy] uniqueidentifier  NULL,
    [LastUpdateDate] datetime  NULL,
    [Contact_Person] nvarchar(100)  NULL,
    [Client_PAN_No] nvarchar(100)  NULL,
    [IsActive] bit  NULL,
    [ClientCity] nvarchar(500)  NULL,
    [ClientCountryId] int  NULL
);
GO

-- Creating table 'Companies'
CREATE TABLE [dbo].[Companies] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CompanyName] varchar(255)  NOT NULL,
    [CompanyAddress] varchar(255)  NOT NULL,
    [City] varchar(30)  NOT NULL,
    [State] varchar(50)  NOT NULL,
    [Country] varchar(30)  NOT NULL,
    [CompanyEmail] varchar(50)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NOT NULL,
    [Pincode] nvarchar(max)  NULL,
    [UserID] nvarchar(128)  NULL,
    [LastUpdateDate] datetime  NULL,
    [LastUpdateBy] nvarchar(max)  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'Contacts'
CREATE TABLE [dbo].[Contacts] (
    [ContactId] int IDENTITY(1,1) NOT NULL,
    [ContactName] nvarchar(50)  NULL,
    [ContactPhone] nvarchar(50)  NULL,
    [ContactEmail] nvarchar(50)  NULL,
    [ContactPosition] nvarchar(50)  NULL,
    [CustomerId] int  NULL,
    [ClientId] int  NULL
);
GO

-- Creating table 'Countries'
CREATE TABLE [dbo].[Countries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Country1] nvarchar(100)  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [CustomerId] int IDENTITY(1,1) NOT NULL,
    [CustomerName] nvarchar(50)  NULL,
    [CustomerAddress] nvarchar(50)  NULL,
    [CustomerGST] nvarchar(50)  NULL,
    [CustomerPhone] nvarchar(50)  NULL,
    [CustomerEmail] nvarchar(50)  NULL,
    [CustomerWWW] nvarchar(50)  NULL,
    [CustomerRemarks] nvarchar(50)  NULL
);
GO

-- Creating table 'Exams'
CREATE TABLE [dbo].[Exams] (
    [ExamId] int IDENTITY(1,1) NOT NULL,
    [ExamName] nvarchar(50)  NULL,
    [ExamStatus] bit  NULL,
    [ExamSkill] nvarchar(50)  NULL,
    [ExamQuestions] nvarchar(max)  NULL,
    [CustomerId] int  NULL,
    [LastUpdateBy] uniqueidentifier  NULL
);
GO

-- Creating table 'Feedbacks'
CREATE TABLE [dbo].[Feedbacks] (
    [FeedbackId] int IDENTITY(1,1) NOT NULL,
    [CandidateId] int  NULL,
    [CustomerId] int  NULL,
    [ClientId] int  NULL,
    [FeedbackRemarks] nvarchar(max)  NULL
);
GO

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [QuestionId] int IDENTITY(1,1) NOT NULL,
    [Question1] nvarchar(max)  NULL,
    [QuestionStatus] bit  NULL,
    [QuestionType] nvarchar(50)  NULL,
    [QuestionSkill] nvarchar(50)  NULL,
    [CompanyId] int  NULL,
    [CustomerId] int  NULL,
    [QuestionGroupId] int  NULL,
    [LastUpdateBy] nvarchar(128)  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'QuestionAnswerImages'
CREATE TABLE [dbo].[QuestionAnswerImages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionId] int  NULL,
    [Images] nvarchar(max)  NULL
);
GO

-- Creating table 'QuestionGroups'
CREATE TABLE [dbo].[QuestionGroups] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [GroupName] varchar(50)  NOT NULL,
    [CompanyId] int  NULL,
    [LastUpdateBy] varchar(50)  NULL,
    [LastUpdateDate] datetime  NULL
);
GO

-- Creating table 'Skills'
CREATE TABLE [dbo].[Skills] (
    [Skills] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'States'
CREATE TABLE [dbo].[States] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [State1] nvarchar(100)  NULL,
    [CountryId] int  NULL
);
GO

-- Creating table 'UserMappings'
CREATE TABLE [dbo].[UserMappings] (
    [UserMappingId] int  NOT NULL,
    [UserId] uniqueidentifier  NULL,
    [ClientId] int  NULL,
    [CustomerId] int  NULL
);
GO

-- Creating table 'AccessPermissions'
CREATE TABLE [dbo].[AccessPermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] nvarchar(128)  NULL,
    [EntityId] int  NULL,
    [IsAdd] bit  NULL,
    [IsView] bit  NULL,
    [IsEdit] bit  NULL,
    [IsDelete] bit  NULL,
    [IsPrint] bit  NULL,
    [IsExport] bit  NULL
);
GO

-- Creating table 'Entities'
CREATE TABLE [dbo].[Entities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EntityName] nvarchar(max)  NULL
);
GO

-- Creating table 'CandidateTestResults'
CREATE TABLE [dbo].[CandidateTestResults] (
    [ID] int  NOT NULL,
    [QuestionId] int  NOT NULL,
    [CandidateAnswerId] int  NULL,
    [IsMulti] bit  NULL,
    [TestGroupId] int  NULL,
    [TestLink] varchar(50)  NULL,
    [SubmitDate] datetime  NULL,
    [CandidateId] int  NULL,
    [TestName] varchar(100)  NULL,
    [AssignBy] nvarchar(100)  NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] varchar(30)  NOT NULL,
    [CompanyId] int  NULL,
    [CreatedBy] nvarchar(128)  NULL,
    [ClientId] int  NULL,
    [IsActive] bit  NULL,
    [LastUpdateDate] datetime  NULL
);
GO

-- Creating table 'Jobs'
CREATE TABLE [dbo].[Jobs] (
    [JobId] int IDENTITY(1,1) NOT NULL,
    [JobName] nvarchar(max)  NULL,
    [JobDescription] nvarchar(max)  NULL,
    [JobStatus] bit  NULL,
    [JobCount] int  NULL,
    [JobTargetDate] datetime  NULL,
    [ClientId] int  NULL,
    [LastUpdatedBy] nvarchar(128)  NULL,
    [CompanyId] int  NULL,
    [Years_of_Exp] decimal(18,0)  NULL,
    [Location] nvarchar(100)  NULL,
    [Contact_Person] nvarchar(100)  NULL,
    [LastUpdateDate] datetime  NULL
);
GO

-- Creating table 'CandidatesResumes'
CREATE TABLE [dbo].[CandidatesResumes] (
    [Candidate_resume_Id] int IDENTITY(1,1) NOT NULL,
    [CandidateId] int  NULL,
    [CandidateResume] nvarchar(max)  NULL,
    [CreatedDate] datetime  NULL
);
GO

-- Creating table 'SkillMasters'
CREATE TABLE [dbo].[SkillMasters] (
    [SkillId] int IDENTITY(1,1) NOT NULL,
    [SkillName] nvarchar(200)  NULL,
    [Active] bit  NOT NULL,
    [LastUpdateDate] datetime  NULL
);
GO

-- Creating table 'CandidateTestURLs'
CREATE TABLE [dbo].[CandidateTestURLs] (
    [CandTestURLId] int IDENTITY(1,1) NOT NULL,
    [CandId] int  NOT NULL,
    [TestURLID] uniqueidentifier  NOT NULL,
    [Status] bit  NOT NULL
);
GO

-- Creating table 'CandidateUploads'
CREATE TABLE [dbo].[CandidateUploads] (
    [Id] int  NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Email] nvarchar(50)  NULL,
    [PhoneNumber] nvarchar(50)  NULL,
    [Age] nvarchar(50)  NULL,
    [Qualification] nvarchar(max)  NULL,
    [Location] nvarchar(max)  NULL,
    [Company] nvarchar(max)  NULL,
    [Project] nvarchar(max)  NULL,
    [Technology] nvarchar(max)  NULL,
    [Achievement] nvarchar(max)  NULL
);
GO

-- Creating table 'ClientCandidateMappings'
CREATE TABLE [dbo].[ClientCandidateMappings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ClientId] int  NULL,
    [CandidateId] int  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'InterviewEmotions'
CREATE TABLE [dbo].[InterviewEmotions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InterviewId] int  NULL,
    [EmotionTime] float  NULL,
    [EmotionName] nvarchar(50)  NULL,
    [EmotionScore] float  NULL
);
GO

-- Creating table 'NextActions'
CREATE TABLE [dbo].[NextActions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL
);
GO

-- Creating table 'SendMesssages'
CREATE TABLE [dbo].[SendMesssages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL
);
GO

-- Creating table 'MCallerDatas'
CREATE TABLE [dbo].[MCallerDatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CallDuration] nvarchar(10)  NULL,
    [DateTime] datetime  NULL,
    [CallType] nvarchar(10)  NULL,
    [Status] nvarchar(10)  NULL,
    [Remarks] nvarchar(max)  NULL,
    [UserId] nvarchar(128)  NULL,
    [CandidateId] int  NULL,
    [ClientId] int  NULL,
    [NextAction] nvarchar(max)  NULL,
    [SendSMSMessage] nvarchar(max)  NULL
);
GO

-- Creating table 'Tests'
CREATE TABLE [dbo].[Tests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TestName] varchar(max)  NULL,
    [GroupId] int  NULL,
    [QueId] int  NULL,
    [CompanyId] int  NULL,
    [Testtime] int  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'Interviews'
CREATE TABLE [dbo].[Interviews] (
    [InterviewId] int IDENTITY(1,1) NOT NULL,
    [CandidateId] int  NULL,
    [TestName] nvarchar(50)  NULL,
    [InterviewVideo] nvarchar(max)  NULL,
    [InterviewDate] datetime  NULL
);
GO

-- Creating table 'CandidateTestRecords'
CREATE TABLE [dbo].[CandidateTestRecords] (
    [CandidateScoreId] int IDENTITY(1,1) NOT NULL,
    [CandidateId] int  NULL,
    [TestName] nvarchar(100)  NULL,
    [TestLink] nvarchar(max)  NULL,
    [TestStatus] nvarchar(50)  NULL,
    [TestScore] nvarchar(50)  NULL
);
GO

-- Creating table 'InterviewQuestionsTimes'
CREATE TABLE [dbo].[InterviewQuestionsTimes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InterviewId] int  NULL,
    [QuestionId] int  NULL,
    [QuestionTime] float  NULL,
    [IsAnswerCorrect] bit  NULL,
    [UpdatedBy] nvarchar(128)  NULL,
    [UpdatedDate] datetime  NULL
);
GO

-- Creating table 'InterviewFeedbacks'
CREATE TABLE [dbo].[InterviewFeedbacks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InterviewId] int  NOT NULL,
    [Comment] nvarchar(max)  NULL,
    [UpdatedBy] nvarchar(128)  NULL,
    [CommentDate] datetime  NULL
);
GO

-- Creating table 'Candidates'
CREATE TABLE [dbo].[Candidates] (
    [CandidateId] int IDENTITY(1,1) NOT NULL,
    [CandidateName] nvarchar(50)  NULL,
    [CandidateDOB] datetime  NULL,
    [CandidateSkill] nvarchar(max)  NULL,
    [CandidateExperience] nvarchar(10)  NULL,
    [CandidateResume] nvarchar(max)  NULL,
    [CustomerId] int  NULL,
    [LastUpdateBy] uniqueidentifier  NULL,
    [CandidatePhone] nvarchar(50)  NULL,
    [CandidateEmail] nvarchar(50)  NULL,
    [CandidatePhone2] nvarchar(50)  NULL,
    [CandidateLocation] nvarchar(50)  NULL,
    [CandidateLastJobChange] nvarchar(50)  NULL,
    [SkypeId] nvarchar(100)  NULL,
    [CurrentCTC] decimal(18,2)  NULL,
    [ExpectedCTC] decimal(18,2)  NULL,
    [BuyoutOption] bit  NULL,
    [Education] varchar(100)  NULL,
    [OffersInHand] decimal(18,2)  NULL,
    [NoticePeriod] int  NULL,
    [ClientDesignation] nvarchar(50)  NULL,
    [Remarks] nvarchar(max)  NULL,
    [CompanyId] int  NULL,
    [Relevant_Exp] nvarchar(100)  NULL,
    [Preferred_Location] nvarchar(100)  NULL,
    [Last_Job_change_reason] nvarchar(max)  NULL,
    [IsActive] bit  NULL,
    [CurrentOrganisation] nvarchar(max)  NULL,
    [LastUpdateDate] datetime  NULL,
    [Created] datetime  NULL,
    [CreatedBy] uniqueidentifier  NULL,
    [CandidateVideo] nvarchar(max)  NULL,
    [PreviousDesignation] nvarchar(50)  NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [MigrationId], [ContextKey] in table 'C__MigrationHistory'
ALTER TABLE [dbo].[C__MigrationHistory]
ADD CONSTRAINT [PK_C__MigrationHistory]
    PRIMARY KEY CLUSTERED ([MigrationId], [ContextKey] ASC);
GO

-- Creating primary key on [ActivityId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [PK_Activities]
    PRIMARY KEY CLUSTERED ([ActivityId] ASC);
GO

-- Creating primary key on [id] in table 'ActivityTypes'
ALTER TABLE [dbo].[ActivityTypes]
ADD CONSTRAINT [PK_ActivityTypes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [AnswerId] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [PK_Answers]
    PRIMARY KEY CLUSTERED ([AnswerId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [CandidateExamId] in table 'CandidateExams'
ALTER TABLE [dbo].[CandidateExams]
ADD CONSTRAINT [PK_CandidateExams]
    PRIMARY KEY CLUSTERED ([CandidateExamId] ASC);
GO

-- Creating primary key on [Id] in table 'Cities'
ALTER TABLE [dbo].[Cities]
ADD CONSTRAINT [PK_Cities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ClientId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [PK_Clients]
    PRIMARY KEY CLUSTERED ([ClientId] ASC);
GO

-- Creating primary key on [ID] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [PK_Companies]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ContactId] in table 'Contacts'
ALTER TABLE [dbo].[Contacts]
ADD CONSTRAINT [PK_Contacts]
    PRIMARY KEY CLUSTERED ([ContactId] ASC);
GO

-- Creating primary key on [Id] in table 'Countries'
ALTER TABLE [dbo].[Countries]
ADD CONSTRAINT [PK_Countries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [CustomerId] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([CustomerId] ASC);
GO

-- Creating primary key on [ExamId] in table 'Exams'
ALTER TABLE [dbo].[Exams]
ADD CONSTRAINT [PK_Exams]
    PRIMARY KEY CLUSTERED ([ExamId] ASC);
GO

-- Creating primary key on [FeedbackId] in table 'Feedbacks'
ALTER TABLE [dbo].[Feedbacks]
ADD CONSTRAINT [PK_Feedbacks]
    PRIMARY KEY CLUSTERED ([FeedbackId] ASC);
GO

-- Creating primary key on [QuestionId] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([QuestionId] ASC);
GO

-- Creating primary key on [Id] in table 'QuestionAnswerImages'
ALTER TABLE [dbo].[QuestionAnswerImages]
ADD CONSTRAINT [PK_QuestionAnswerImages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'QuestionGroups'
ALTER TABLE [dbo].[QuestionGroups]
ADD CONSTRAINT [PK_QuestionGroups]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Skills] in table 'Skills'
ALTER TABLE [dbo].[Skills]
ADD CONSTRAINT [PK_Skills]
    PRIMARY KEY CLUSTERED ([Skills] ASC);
GO

-- Creating primary key on [Id] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [PK_States]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserMappingId] in table 'UserMappings'
ALTER TABLE [dbo].[UserMappings]
ADD CONSTRAINT [PK_UserMappings]
    PRIMARY KEY CLUSTERED ([UserMappingId] ASC);
GO

-- Creating primary key on [Id] in table 'AccessPermissions'
ALTER TABLE [dbo].[AccessPermissions]
ADD CONSTRAINT [PK_AccessPermissions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [PK_Entities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'CandidateTestResults'
ALTER TABLE [dbo].[CandidateTestResults]
ADD CONSTRAINT [PK_CandidateTestResults]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [JobId] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [PK_Jobs]
    PRIMARY KEY CLUSTERED ([JobId] ASC);
GO

-- Creating primary key on [Candidate_resume_Id] in table 'CandidatesResumes'
ALTER TABLE [dbo].[CandidatesResumes]
ADD CONSTRAINT [PK_CandidatesResumes]
    PRIMARY KEY CLUSTERED ([Candidate_resume_Id] ASC);
GO

-- Creating primary key on [SkillId] in table 'SkillMasters'
ALTER TABLE [dbo].[SkillMasters]
ADD CONSTRAINT [PK_SkillMasters]
    PRIMARY KEY CLUSTERED ([SkillId] ASC);
GO

-- Creating primary key on [CandTestURLId] in table 'CandidateTestURLs'
ALTER TABLE [dbo].[CandidateTestURLs]
ADD CONSTRAINT [PK_CandidateTestURLs]
    PRIMARY KEY CLUSTERED ([CandTestURLId] ASC);
GO

-- Creating primary key on [Id] in table 'CandidateUploads'
ALTER TABLE [dbo].[CandidateUploads]
ADD CONSTRAINT [PK_CandidateUploads]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClientCandidateMappings'
ALTER TABLE [dbo].[ClientCandidateMappings]
ADD CONSTRAINT [PK_ClientCandidateMappings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'InterviewEmotions'
ALTER TABLE [dbo].[InterviewEmotions]
ADD CONSTRAINT [PK_InterviewEmotions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NextActions'
ALTER TABLE [dbo].[NextActions]
ADD CONSTRAINT [PK_NextActions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SendMesssages'
ALTER TABLE [dbo].[SendMesssages]
ADD CONSTRAINT [PK_SendMesssages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MCallerDatas'
ALTER TABLE [dbo].[MCallerDatas]
ADD CONSTRAINT [PK_MCallerDatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [PK_Tests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [InterviewId] in table 'Interviews'
ALTER TABLE [dbo].[Interviews]
ADD CONSTRAINT [PK_Interviews]
    PRIMARY KEY CLUSTERED ([InterviewId] ASC);
GO

-- Creating primary key on [CandidateScoreId] in table 'CandidateTestRecords'
ALTER TABLE [dbo].[CandidateTestRecords]
ADD CONSTRAINT [PK_CandidateTestRecords]
    PRIMARY KEY CLUSTERED ([CandidateScoreId] ASC);
GO

-- Creating primary key on [Id] in table 'InterviewQuestionsTimes'
ALTER TABLE [dbo].[InterviewQuestionsTimes]
ADD CONSTRAINT [PK_InterviewQuestionsTimes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InterviewFeedbacks'
ALTER TABLE [dbo].[InterviewFeedbacks]
ADD CONSTRAINT [PK_InterviewFeedbacks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [CandidateId] in table 'Candidates'
ALTER TABLE [dbo].[Candidates]
ADD CONSTRAINT [PK_Candidates]
    PRIMARY KEY CLUSTERED ([CandidateId] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Companyid] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Companies]
    FOREIGN KEY ([Companyid])
    REFERENCES [dbo].[Companies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Companies'
CREATE INDEX [IX_FK_Activity_Companies]
ON [dbo].[Activities]
    ([Companyid]);
GO

-- Creating foreign key on [QuestiondId] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [FK_Answer_Question]
    FOREIGN KEY ([QuestiondId])
    REFERENCES [dbo].[Questions]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answer_Question'
CREATE INDEX [IX_FK_Answer_Question]
ON [dbo].[Answers]
    ([QuestiondId]);
GO

-- Creating foreign key on [CustomerId] in table 'CandidateExams'
ALTER TABLE [dbo].[CandidateExams]
ADD CONSTRAINT [FK_CandidateExam_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidateExam_Customer'
CREATE INDEX [IX_FK_CandidateExam_Customer]
ON [dbo].[CandidateExams]
    ([CustomerId]);
GO

-- Creating foreign key on [StateId] in table 'Cities'
ALTER TABLE [dbo].[Cities]
ADD CONSTRAINT [FK_City_State]
    FOREIGN KEY ([StateId])
    REFERENCES [dbo].[States]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_City_State'
CREATE INDEX [IX_FK_City_State]
ON [dbo].[Cities]
    ([StateId]);
GO

-- Creating foreign key on [CompanyId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_Client_Companies]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Companies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Client_Companies'
CREATE INDEX [IX_FK_Client_Companies]
ON [dbo].[Clients]
    ([CompanyId]);
GO

-- Creating foreign key on [CustomerId] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [FK_Client_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Client_Customer'
CREATE INDEX [IX_FK_Client_Customer]
ON [dbo].[Clients]
    ([CustomerId]);
GO

-- Creating foreign key on [ClientId] in table 'UserMappings'
ALTER TABLE [dbo].[UserMappings]
ADD CONSTRAINT [FK_UserMapping_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMapping_Client'
CREATE INDEX [IX_FK_UserMapping_Client]
ON [dbo].[UserMappings]
    ([ClientId]);
GO

-- Creating foreign key on [CompanyId] in table 'QuestionGroups'
ALTER TABLE [dbo].[QuestionGroups]
ADD CONSTRAINT [FK__QuestionG__Compa__74AE54BC]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Companies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__QuestionG__Compa__74AE54BC'
CREATE INDEX [IX_FK__QuestionG__Compa__74AE54BC]
ON [dbo].[QuestionGroups]
    ([CompanyId]);
GO

-- Creating foreign key on [CompanyId] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [FK_Question_Companies]
    FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Companies]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Question_Companies'
CREATE INDEX [IX_FK_Question_Companies]
ON [dbo].[Questions]
    ([CompanyId]);
GO

-- Creating foreign key on [CountryId] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [FK_State_Country]
    FOREIGN KEY ([CountryId])
    REFERENCES [dbo].[Countries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_State_Country'
CREATE INDEX [IX_FK_State_Country]
ON [dbo].[States]
    ([CountryId]);
GO

-- Creating foreign key on [CustomerId] in table 'Exams'
ALTER TABLE [dbo].[Exams]
ADD CONSTRAINT [FK_Exam_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Exam_Customer'
CREATE INDEX [IX_FK_Exam_Customer]
ON [dbo].[Exams]
    ([CustomerId]);
GO

-- Creating foreign key on [CustomerId] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [FK_Question_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Question_Customer'
CREATE INDEX [IX_FK_Question_Customer]
ON [dbo].[Questions]
    ([CustomerId]);
GO

-- Creating foreign key on [CustomerId] in table 'UserMappings'
ALTER TABLE [dbo].[UserMappings]
ADD CONSTRAINT [FK_UserMapping_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMapping_Customer'
CREATE INDEX [IX_FK_UserMapping_Customer]
ON [dbo].[UserMappings]
    ([CustomerId]);
GO

-- Creating foreign key on [QuestionId] in table 'QuestionAnswerImages'
ALTER TABLE [dbo].[QuestionAnswerImages]
ADD CONSTRAINT [FK_QuestionAnswerImages_Question]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_QuestionAnswerImages_Question'
CREATE INDEX [IX_FK_QuestionAnswerImages_Question]
ON [dbo].[QuestionAnswerImages]
    ([QuestionId]);
GO

-- Creating foreign key on [RoleId] in table 'AccessPermissions'
ALTER TABLE [dbo].[AccessPermissions]
ADD CONSTRAINT [FK_AccessPermission_Entity]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessPermission_Entity'
CREATE INDEX [IX_FK_AccessPermission_Entity]
ON [dbo].[AccessPermissions]
    ([RoleId]);
GO

-- Creating foreign key on [EntityId] in table 'AccessPermissions'
ALTER TABLE [dbo].[AccessPermissions]
ADD CONSTRAINT [FK_AccessPermission_Entity1]
    FOREIGN KEY ([EntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessPermission_Entity1'
CREATE INDEX [IX_FK_AccessPermission_Entity1]
ON [dbo].[AccessPermissions]
    ([EntityId]);
GO

-- Creating foreign key on [QuestionId] in table 'CandidateTestResults'
ALTER TABLE [dbo].[CandidateTestResults]
ADD CONSTRAINT [FK__Candidate__Quest__1CBC4616]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Candidate__Quest__1CBC4616'
CREATE INDEX [IX_FK__Candidate__Quest__1CBC4616]
ON [dbo].[CandidateTestResults]
    ([QuestionId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [ClientId] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [FK_AspNetUsers_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUsers_Client'
CREATE INDEX [IX_FK_AspNetUsers_Client]
ON [dbo].[AspNetUsers]
    ([ClientId]);
GO

-- Creating foreign key on [UserID] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [FK_Companies_AspNetUsers]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Companies_AspNetUsers'
CREATE INDEX [IX_FK_Companies_AspNetUsers]
ON [dbo].[Companies]
    ([UserID]);
GO

-- Creating foreign key on [LastUpdateBy] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [FK_Question_User]
    FOREIGN KEY ([LastUpdateBy])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Question_User'
CREATE INDEX [IX_FK_Question_User]
ON [dbo].[Questions]
    ([LastUpdateBy]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRole]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUser'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUser]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [JobId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Job]
    FOREIGN KEY ([JobId])
    REFERENCES [dbo].[Jobs]
        ([JobId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Job'
CREATE INDEX [IX_FK_Activity_Job]
ON [dbo].[Activities]
    ([JobId]);
GO

-- Creating foreign key on [JobId] in table 'CandidateExams'
ALTER TABLE [dbo].[CandidateExams]
ADD CONSTRAINT [FK_CandidateExam_Job]
    FOREIGN KEY ([JobId])
    REFERENCES [dbo].[Jobs]
        ([JobId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidateExam_Job'
CREATE INDEX [IX_FK_CandidateExam_Job]
ON [dbo].[CandidateExams]
    ([JobId]);
GO

-- Creating foreign key on [ClientId] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [FK_Job_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Job_Client'
CREATE INDEX [IX_FK_Job_Client]
ON [dbo].[Jobs]
    ([ClientId]);
GO

-- Creating foreign key on [ClientId] in table 'ClientCandidateMappings'
ALTER TABLE [dbo].[ClientCandidateMappings]
ADD CONSTRAINT [FK_ClientCandidateMapping_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientCandidateMapping_Client'
CREATE INDEX [IX_FK_ClientCandidateMapping_Client]
ON [dbo].[ClientCandidateMappings]
    ([ClientId]);
GO

-- Creating foreign key on [UserId] in table 'MCallerDatas'
ALTER TABLE [dbo].[MCallerDatas]
ADD CONSTRAINT [FK_MCallerData_AspNetUsers]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MCallerData_AspNetUsers'
CREATE INDEX [IX_FK_MCallerData_AspNetUsers]
ON [dbo].[MCallerDatas]
    ([UserId]);
GO

-- Creating foreign key on [ClientId] in table 'MCallerDatas'
ALTER TABLE [dbo].[MCallerDatas]
ADD CONSTRAINT [FK_MCallerData_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Clients]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MCallerData_Client'
CREATE INDEX [IX_FK_MCallerData_Client]
ON [dbo].[MCallerDatas]
    ([ClientId]);
GO

-- Creating foreign key on [QueId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [FK_Test_Question]
    FOREIGN KEY ([QueId])
    REFERENCES [dbo].[Questions]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Test_Question'
CREATE INDEX [IX_FK_Test_Question]
ON [dbo].[Tests]
    ([QueId]);
GO

-- Creating foreign key on [GroupId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [FK_Test_QuestionGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[QuestionGroups]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Test_QuestionGroup'
CREATE INDEX [IX_FK_Test_QuestionGroup]
ON [dbo].[Tests]
    ([GroupId]);
GO

-- Creating foreign key on [InterviewId] in table 'InterviewEmotions'
ALTER TABLE [dbo].[InterviewEmotions]
ADD CONSTRAINT [FK_InterviewEmotions_Interview]
    FOREIGN KEY ([InterviewId])
    REFERENCES [dbo].[Interviews]
        ([InterviewId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InterviewEmotions_Interview'
CREATE INDEX [IX_FK_InterviewEmotions_Interview]
ON [dbo].[InterviewEmotions]
    ([InterviewId]);
GO

-- Creating foreign key on [InterviewId] in table 'InterviewQuestionsTimes'
ALTER TABLE [dbo].[InterviewQuestionsTimes]
ADD CONSTRAINT [FK_InterviewQuestionsTime_Interview]
    FOREIGN KEY ([InterviewId])
    REFERENCES [dbo].[Interviews]
        ([InterviewId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InterviewQuestionsTime_Interview'
CREATE INDEX [IX_FK_InterviewQuestionsTime_Interview]
ON [dbo].[InterviewQuestionsTimes]
    ([InterviewId]);
GO

-- Creating foreign key on [QuestionId] in table 'InterviewQuestionsTimes'
ALTER TABLE [dbo].[InterviewQuestionsTimes]
ADD CONSTRAINT [FK_InterviewQuestionsTime_Question]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InterviewQuestionsTime_Question'
CREATE INDEX [IX_FK_InterviewQuestionsTime_Question]
ON [dbo].[InterviewQuestionsTimes]
    ([QuestionId]);
GO

-- Creating foreign key on [UpdatedBy] in table 'InterviewFeedbacks'
ALTER TABLE [dbo].[InterviewFeedbacks]
ADD CONSTRAINT [FK_InterviewFeedback_AspNetUsers]
    FOREIGN KEY ([UpdatedBy])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InterviewFeedback_AspNetUsers'
CREATE INDEX [IX_FK_InterviewFeedback_AspNetUsers]
ON [dbo].[InterviewFeedbacks]
    ([UpdatedBy]);
GO

-- Creating foreign key on [InterviewId] in table 'InterviewFeedbacks'
ALTER TABLE [dbo].[InterviewFeedbacks]
ADD CONSTRAINT [FK_Interview_InterviewFeedback]
    FOREIGN KEY ([InterviewId])
    REFERENCES [dbo].[Interviews]
        ([InterviewId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Interview_InterviewFeedback'
CREATE INDEX [IX_FK_Interview_InterviewFeedback]
ON [dbo].[InterviewFeedbacks]
    ([InterviewId]);
GO

-- Creating foreign key on [CandidateId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Candidate'
CREATE INDEX [IX_FK_Activity_Candidate]
ON [dbo].[Activities]
    ([CandidateId]);
GO

-- Creating foreign key on [CustomerId] in table 'Candidates'
ALTER TABLE [dbo].[Candidates]
ADD CONSTRAINT [FK_Candidate_Customer]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Candidate_Customer'
CREATE INDEX [IX_FK_Candidate_Customer]
ON [dbo].[Candidates]
    ([CustomerId]);
GO

-- Creating foreign key on [CandidateId] in table 'CandidateExams'
ALTER TABLE [dbo].[CandidateExams]
ADD CONSTRAINT [FK_CandidateExam_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidateExam_Candidate'
CREATE INDEX [IX_FK_CandidateExam_Candidate]
ON [dbo].[CandidateExams]
    ([CandidateId]);
GO

-- Creating foreign key on [CandidateId] in table 'CandidatesResumes'
ALTER TABLE [dbo].[CandidatesResumes]
ADD CONSTRAINT [FK_CandidatesResume_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidatesResume_Candidate'
CREATE INDEX [IX_FK_CandidatesResume_Candidate]
ON [dbo].[CandidatesResumes]
    ([CandidateId]);
GO

-- Creating foreign key on [CandidateId] in table 'ClientCandidateMappings'
ALTER TABLE [dbo].[ClientCandidateMappings]
ADD CONSTRAINT [FK_ClientCandidateMapping_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientCandidateMapping_Candidate'
CREATE INDEX [IX_FK_ClientCandidateMapping_Candidate]
ON [dbo].[ClientCandidateMappings]
    ([CandidateId]);
GO

-- Creating foreign key on [CandidateId] in table 'Interviews'
ALTER TABLE [dbo].[Interviews]
ADD CONSTRAINT [FK_Interview_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Interview_Candidate'
CREATE INDEX [IX_FK_Interview_Candidate]
ON [dbo].[Interviews]
    ([CandidateId]);
GO

-- Creating foreign key on [CandidateId] in table 'MCallerDatas'
ALTER TABLE [dbo].[MCallerDatas]
ADD CONSTRAINT [FK_MCallerData_Candidate]
    FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates]
        ([CandidateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MCallerData_Candidate'
CREATE INDEX [IX_FK_MCallerData_Candidate]
ON [dbo].[MCallerDatas]
    ([CandidateId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------