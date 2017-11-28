USE [bk_staff]
GO
INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[ConcurrencyStamp]
           ,[Name]
           ,[NormalizedName])
     VALUES
           (1
           ,NEWID()
           ,'ADMIN'
           ,'ADMIN')
GO
INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[ConcurrencyStamp]
           ,[Name]
           ,[NormalizedName])
     VALUES
           (2
           ,NEWID()
           ,'STAFF'
           ,'STAFF')
GO
INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[ConcurrencyStamp]
           ,[Name]
           ,[NormalizedName])
     VALUES
           (3
           ,NEWID()
           ,'CUSTOMER'
           ,'CUSTOMER')
GO

insert into [bk_staff].[dbo].[AspNetUserRoles] ([UserId] ,[RoleId]) VALUES ('e7a2d618-d9c1-46ee-b149-3836e988a589', '2')
GO