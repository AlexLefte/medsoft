USE [master]
GO
/****** Object:  Database [MedSoft]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE DATABASE [MedSoft]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MedSoft', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\MedSoft.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MedSoft_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\MedSoft_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [MedSoft] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MedSoft].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MedSoft] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MedSoft] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MedSoft] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MedSoft] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MedSoft] SET ARITHABORT OFF 
GO
ALTER DATABASE [MedSoft] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MedSoft] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MedSoft] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MedSoft] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MedSoft] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MedSoft] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MedSoft] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MedSoft] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MedSoft] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MedSoft] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MedSoft] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MedSoft] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MedSoft] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MedSoft] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MedSoft] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MedSoft] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MedSoft] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MedSoft] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MedSoft] SET  MULTI_USER 
GO
ALTER DATABASE [MedSoft] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MedSoft] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MedSoft] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MedSoft] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MedSoft] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MedSoft] SET QUERY_STORE = OFF
GO
USE [MedSoft]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [MedSoft]
GO
/****** Object:  User [MedSoftUser]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE USER [MedSoftUser] FOR LOGIN [MedSoftLogin] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [MedSoftUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [MedSoftUser]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Administrator]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Administrator](
	[AdministratorID] [nvarchar](450) NOT NULL,
	[Nume] [nvarchar](50) NOT NULL,
	[Prenume] [nvarchar](50) NOT NULL,
	[CNP] [char](13) NOT NULL,
	[Adresa] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AdministratorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[Discriminator] [nvarchar](max) NOT NULL,
	[Nume] [nvarchar](max) NULL,
	[Prenume] [nvarchar](max) NULL,
	[Telefon] [nvarchar](max) NULL,
	[Adresa] [nvarchar](max) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Consultatie]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consultatie](
	[ConsultatieID] [bigint] IDENTITY(1,1) NOT NULL,
	[Data] [datetime] NOT NULL,
	[MedicID] [nvarchar](450) NOT NULL,
	[PacientID] [nvarchar](450) NOT NULL,
	[MedicamentID] [bigint] NULL,
	[Diagnostic] [nvarchar](50) NULL,
	[Doza] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ConsultatieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medic]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medic](
	[NumeMedic] [nvarchar](50) NOT NULL,
	[PrenumeMedic] [nvarchar](50) NOT NULL,
	[PretConsultatie] [decimal](19, 4) NOT NULL,
	[SpecializareID] [bigint] NOT NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[CNP] [char](13) NOT NULL,
	[Adresa] [nvarchar](50) NOT NULL,
	[MedicID] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MedicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CNP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medicamente]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medicamente](
	[MedicamentID] [bigint] IDENTITY(1,1) NOT NULL,
	[Denumire] [nvarchar](45) NULL,
	[ImageUrl] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[MedicamentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pacient]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pacient](
	[CNP] [char](13) NOT NULL,
	[NumePacient] [nvarchar](50) NOT NULL,
	[PrenumePacient] [nvarchar](50) NOT NULL,
	[Adresa] [nvarchar](50) NOT NULL,
	[Asigurare] [decimal](19, 4) NOT NULL,
	[PacientID] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PacientID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_CNP] UNIQUE NONCLUSTERED 
(
	[CNP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Specializare]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Specializare](
	[SpecializareID] [bigint] IDENTITY(1,1) NOT NULL,
	[Nume] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[SpecializareID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Specializare_Nume] UNIQUE NONCLUSTERED 
(
	[Nume] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 1/31/2024 3:43:04 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Consultatie] ADD  DEFAULT ('In Asteptare') FOR [Status]
GO
ALTER TABLE [dbo].[Pacient] ADD  CONSTRAINT [DF_Pacient_Asigurare]  DEFAULT ((0)) FOR [Asigurare]
GO
ALTER TABLE [dbo].[Administrator]  WITH CHECK ADD  CONSTRAINT [FK_Administrator_User] FOREIGN KEY([AdministratorID])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Administrator] CHECK CONSTRAINT [FK_Administrator_User]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Consultatie]  WITH CHECK ADD  CONSTRAINT [FK_Consultatie_Medic] FOREIGN KEY([MedicID])
REFERENCES [dbo].[Medic] ([MedicID])
GO
ALTER TABLE [dbo].[Consultatie] CHECK CONSTRAINT [FK_Consultatie_Medic]
GO
ALTER TABLE [dbo].[Consultatie]  WITH CHECK ADD  CONSTRAINT [FK_Consultatie_Medicamente] FOREIGN KEY([MedicamentID])
REFERENCES [dbo].[Medicamente] ([MedicamentID])
GO
ALTER TABLE [dbo].[Consultatie] CHECK CONSTRAINT [FK_Consultatie_Medicamente]
GO
ALTER TABLE [dbo].[Consultatie]  WITH CHECK ADD  CONSTRAINT [FK_Consultatie_Pacient] FOREIGN KEY([PacientID])
REFERENCES [dbo].[Pacient] ([PacientID])
GO
ALTER TABLE [dbo].[Consultatie] CHECK CONSTRAINT [FK_Consultatie_Pacient]
GO
ALTER TABLE [dbo].[Medic]  WITH CHECK ADD FOREIGN KEY([SpecializareID])
REFERENCES [dbo].[Specializare] ([SpecializareID])
GO
ALTER TABLE [dbo].[Medic]  WITH CHECK ADD  CONSTRAINT [FK_Medic_User] FOREIGN KEY([MedicID])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Medic] CHECK CONSTRAINT [FK_Medic_User]
GO
ALTER TABLE [dbo].[Pacient]  WITH CHECK ADD  CONSTRAINT [FK_Pacient_User] FOREIGN KEY([PacientID])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Pacient] CHECK CONSTRAINT [FK_Pacient_User]
GO
/****** Object:  StoredProcedure [dbo].[NumaraMediciDupaSpecializareID]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NumaraMediciDupaSpecializareID]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS NumarMedici
    FROM Medic
    WHERE Medic.SpecializareID = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[SP_Adauga_Consultatie]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Adauga_Consultatie]
    @Data DATETIME,
    @MedicID NVARCHAR(450),
	@PacientID NVARCHAR(450),
	@Diagnostic NVARCHAR(50),
	@MedicamentID BIGINT,
	@Doza NVARCHAR(50),
	@Status VARCHAR(45),
	@ErrorMessage NVARCHAR(MAX) OUTPUT,
    @ErrorFlag BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	-- Initializare variabile
    SET @ErrorMessage = NULL;
    SET @ErrorFlag = 0;

    -- Check if there is an existing appointment at the same date and hour for the same medic
    IF EXISTS (
        SELECT 1
        FROM dbo.Consultatie
        WHERE MedicID = @MedicID
          AND Data = @Data
    )
    BEGIN
        -- Data indisponibila, medicul are programata o alta consultatie
        SET @ErrorMessage = 'Eroare: medicul are o programare la acea data.';
        SET @ErrorFlag = 1;
        RETURN;
    END
	ELSE IF EXISTS(
        SELECT 1
        FROM dbo.Consultatie
        WHERE (MedicID = @MedicID OR PacientID = @PacientID)
          AND Data = @Data
    )
    BEGIN
        -- Pacientul are programata o alta consultatie
        SET @ErrorMessage = 'Eroare: pacientul are programata o alta consultatie.';
        SET @ErrorFlag = 1;
        RETURN;
    END

    -- Start transaction
    BEGIN TRY
        BEGIN TRAN

        -- Set isolation level to SERIALIZABLE
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

        -- Add new appointment
        INSERT INTO dbo.Consultatie(Data, MedicID, PacientID, MedicamentID, Diagnostic, Doza, Status)
        VALUES (@Data, @MedicID, @PacientID, @MedicamentID, @Diagnostic, @Doza, @Status)

        -- Commit transaction
        COMMIT
    END TRY
    BEGIN CATCH
        -- Rollback transaction on error
        ROLLBACK;
        SET @ErrorMessage = 'Eroare: tranzactia a fost abandonata.';
        SET @ErrorFlag = 1;
        RETURN;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[SP_Modifica_Consultatie]    Script Date: 1/31/2024 3:43:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Modifica_Consultatie]
    @ConsultatieID BIGINT,
    @Data DATETIME,
    @MedicID NVARCHAR(450),
    @PacientID NVARCHAR(450),
    @Diagnostic NVARCHAR(50),
    @MedicamentID BIGINT,
    @Doza NVARCHAR(50),
    @Status VARCHAR(45),
	@ErrorMessage NVARCHAR(MAX) OUTPUT,
    @ErrorFlag BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	-- Initializare variabile
    SET @ErrorMessage = NULL;
    SET @ErrorFlag = 0;

    -- Check if there is an existing appointment at the same date and hour for the same medic
    IF EXISTS (
        SELECT 1
        FROM dbo.Consultatie
        WHERE MedicID = @MedicID
          AND Data = @Data
		  AND PacientID != @PacientID
    )
    BEGIN
        -- Data indisponibila, medicul are programata o alta consultatie
		SET @ErrorMessage = 'Eroare: medicul are o programare la acea data.';
        SET @ErrorFlag = 1;
        RETURN;
    END
	ELSE IF EXISTS(
        SELECT 1
        FROM dbo.Consultatie
        WHERE (MedicID != @MedicID AND PacientID = @PacientID)
          AND Data = @Data
    )
    BEGIN
        -- Pacientul are programata o alta consultatie
		SET @ErrorMessage = 'Eroare: pacientul are programata o alta consultatie la aceasta data.';
        SET @ErrorFlag = 1;
        RETURN;
    END

    -- Start transaction
    BEGIN TRY
        BEGIN TRAN

        -- Set isolation level to SERIALIZABLE
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

        -- Update appointment
        UPDATE dbo.Consultatie
        SET
			Data = @Data,
            MedicID = @MedicID,
            PacientID = @PacientID,
            MedicamentID = @MedicamentID,
            Diagnostic = @Diagnostic,
            Doza = @Doza,
            Status = @Status
        WHERE ConsultatieID = @ConsultatieID;

        -- Commit transaction
        COMMIT
    END TRY
    BEGIN CATCH
        -- Rollback transaction on error
        ROLLBACK;
		SET @ErrorMessage = 'Eroare: tranzactia a fost abandonata.';
        SET @ErrorFlag = 1;
        RETURN;
    END CATCH
END;
GO
USE [master]
GO
ALTER DATABASE [MedSoft] SET  READ_WRITE 
GO
