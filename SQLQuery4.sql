CREATE DATABASE Game
GO

USE Game
GO



----------------------------------
--Створення БД
----------------------------------


CREATE TABLE [TypeOfTank] (
	TypeId int NOT NULL,
	GunTypeName varchar(255) NOT NULL,
  CONSTRAINT [PK_TYPEOFTANK] PRIMARY KEY CLUSTERED
  (
  [TypeId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Guns] (
	GunId int NOT NULL,
	GunTypeId int NOT NULL,
	GunName varchar(255) NOT NULL UNIQUE,
	Caliber int NOT NULL,
	Damage int NOT NULL,
  CONSTRAINT [PK_GUN] PRIMARY KEY CLUSTERED
  (
  [GunId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO



CREATE TABLE [Armament] (
	TowerId int NOT NULL,
	Tower varchar(255) NOT NULL,
	CurrentGunId int NOT NULL,
  CONSTRAINT [PK_ARMAMENT] PRIMARY KEY CLUSTERED
  (
  [TowerId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO

--значення за замовчуванням
CREATE TABLE [Player] (
	PlayerId int NOT NULL,
	NickName varchar(255) NOT NULL UNIQUE,
	Email varchar(255) NOT NULL UNIQUE,
	Passwords varchar(255) NOT NULL,
	Expir int NOT NULL DEFAULT 0,
	CurrencyAmount int NOT NULL DEFAULT 0,
  CONSTRAINT [PK_PLAYER] PRIMARY KEY CLUSTERED
  (
  [PlayerId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO



CREATE TABLE [PlayerVehicle] (
	CurrentPlayerId int NOT NULL,
	CurrentVehicleId int NOT NULL,
  CONSTRAINT [PK_PLAYERVEHICLE] PRIMARY KEY CLUSTERED
  (
  [CurrentPlayerId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO



CREATE TABLE [CombatVehicle] (
	CombatVehicleId int NOT NULL,
	CurrentTankId int NOT NULL,
	Cost int NOT NULL,
	CurrentLevel int NOT NULL,
	CurrentArmamentId int NOT NULL,
	CurrentSuspensiontId int NOT NULL,
  CONSTRAINT [PK_COMBATVEHICLE] PRIMARY KEY CLUSTERED
  (
  [CombatVehicleId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
----------
CREATE TABLE [Info] (
	TankNumber  int NOT NULL ,
	Cost int NOT NULL,
	CurrentLevel int NOT NULL,
	TankName varchar(255) NOT NULL,
	TypeOfTank varchar(255) NOT NULL
  CONSTRAINT [PK_INFO] PRIMARY KEY CLUSTERED
  (
  [TankNumber] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
------

CREATE TABLE History 
(
    Id INT IDENTITY PRIMARY KEY,
    NickName nvarchar(255) NOT NULL,
    DeleteAt DATETIME NOT NULL DEFAULT GETDATE(),
);


CREATE TABLE [Tank] (
	TankName varchar(255) NOT NULL,
	TankId int NOT NULL,
	TypeOfTank varchar(255) NOT NULL,
	Nation varchar(255) NOT NULL,
	CreationDate date NOT NULL,
  CONSTRAINT [PK_TANK] PRIMARY KEY CLUSTERED
  (
  [TankName] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO


CREATE TABLE [Suspension] (
	SuspensionId int NOT NULL,
	TankId int NOT NULL,
	TypeOfSuspension varchar(255) NOT NULL,
	LiftingСapacity int NOT NULL,
  CONSTRAINT [PK_SUSPENSION] PRIMARY KEY CLUSTERED
  (
  [SuspensionId] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO

--показує башні встановлені на танки
--view , join
CREATE VIEW ArmamentMountedOnTanks AS
SELECT dbo.Tank.TankName AS TankName, 
        dbo.Armament.Tower AS Tower    
FROM dbo.Tank INNER JOIN dbo.CombatVehicle ON dbo.Tank.TankId = dbo.CombatVehicle.CurrentTankId
INNER JOIN dbo.Armament ON dbo.CombatVehicle.CurrentArmamentId = dbo.Armament.TowerId
GO
select * from Player
go

CREATE TRIGGER Player_Delete
ON Player
AFTER DELETE
AS
INSERT INTO History (NickName)
SELECT NickName
FROM DELETED
GO


--показує всі модулі ,які можна встановити на танки
--union
CREATE VIEW Moduls
AS
	SELECT DISTINCT dbo.Armament.Tower FROM dbo.Armament 
	UNION ALL 
	SELECT DISTINCT dbo.Guns.GunName  FROM dbo.Guns 
	UNION ALL
	SELECT DISTINCT dbo.Suspension.TypeOfSuspension FROM dbo.Suspension 
GO


--виводить дані про пушку,встановлену на танк
--підзапит
CREATE FUNCTION GunIformation
(	
	@TankIds int=1
	
)
RETURNS TABLE
AS
RETURN(
	

	SELECT * FROM dbo.Guns
	WHERE dbo.Guns.GunId IN(SELECT dbo.Armament.CurrentGunId 
	FROM dbo.Armament
	WHERE dbo.Armament.CurrentGunId IN(SELECT dbo.CombatVehicle.CurrentArmamentId FROM
	dbo.CombatVehicle WHERE dbo.CombatVehicle.CurrentTankId= @Tankids))
	
)
GO

--таблична функція
CREATE FUNCTION TanksInRange
(	
	@beg int,
	@ending int
)
RETURNS TABLE
AS
RETURN(
	
	SELECT * FROM dbo.Info
	WHERE dbo.Info.TankNumber BETWEEN @beg And @ending
	
)
GO
SELECT * FROM TanksInRange(5,9)
GO


--процедура
CREATE PROCEDURE Add_Player
	@Player int,
	@Nick nvarchar(255),
	@Email nvarchar(255),
	@Pass nvarchar(255)='12345'
	
	
AS
	INSERT INTO dbo.Player(PlayerId, NickName,Email, Passwords, Expir ,CurrencyAmount)
	VALUES(@Player,@Nick,@Email,@Pass,0,0)

GO







--скалярна функція,case
CREATE FUNCTION ExperienceForTankResearch
(
	@Tankid int
)
RETURNS int
AS
BEGIN
	DECLARE @exper int = 0;
	DECLARE @type nvarchar(255);
	DECLARE @pursh int;
	DECLARE @lvl int;
	SET @pursh=(SELECT Cost FROM Info WHERE TankNumber=@Tankid)
	SET @lvl=(SELECT CurrentLevel FROM Info WHERE TankNumber=@Tankid)
	SET @type=(SELECT TypeOfTank FROM Info WHERE TankNumber=@Tankid)

	SET @exper=	CASE @type
		WHEN 'MediumTank' THEN @pursh*@lvl*1
		WHEN 'HeavyTank' THEN @pursh*@lvl*2
		WHEN 'LightTank' THEN @pursh*@lvl*2
		WHEN 'SAU' THEN @pursh*@lvl*15
		
	END

	RETURN(@exper)
END
GO
SELECT   dbo.ExperienceForTankResearch(3) AS Experience
GO



ALTER TABLE [Guns] WITH CHECK ADD CONSTRAINT [Gun_fk0] FOREIGN KEY ([GunTypeId]) REFERENCES [TypeOfTank]([TypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Guns] CHECK CONSTRAINT [Gun_fk0]
GO

ALTER TABLE [Armament] WITH CHECK ADD CONSTRAINT [Armament_fk0] FOREIGN KEY ([CurrentGunId]) REFERENCES [Guns]([GunId])
ON UPDATE CASCADE
GO
ALTER TABLE [Armament] CHECK CONSTRAINT [Armament_fk0]
GO


ALTER TABLE [PlayerVehicle] WITH CHECK ADD CONSTRAINT [PlayerVehicle_fk0] FOREIGN KEY ([CurrentPlayerId]) REFERENCES [Player]([PlayerId])
ON UPDATE CASCADE
GO
ALTER TABLE [PlayerVehicle] CHECK CONSTRAINT [PlayerVehicle_fk0]
GO
ALTER TABLE [PlayerVehicle] WITH CHECK ADD CONSTRAINT [PlayerVehicle_fk1] FOREIGN KEY ([CurrentVehicleId]) REFERENCES [CombatVehicle]([CombatVehicleId])
ON UPDATE CASCADE
GO
ALTER TABLE [PlayerVehicle] CHECK CONSTRAINT [PlayerVehicle_fk1]
GO
-----
ALTER TABLE [CombatVehicle] WITH CHECK ADD CONSTRAINT [CombatVehicle_fk0] FOREIGN KEY ([CurrentTankId]) REFERENCES [Tank]([TankId])
ON UPDATE CASCADE
GO
ALTER TABLE [CombatVehicle] CHECK CONSTRAINT [CombatVehicle_fk0]
GO
-----
-----
-----

ALTER TABLE [CombatVehicle] WITH CHECK ADD CONSTRAINT [CombatVehicle_fk1] FOREIGN KEY ([CurrentArmamentId]) REFERENCES [Armament]([TowerId])
ON UPDATE CASCADE
GO
ALTER TABLE [CombatVehicle] CHECK CONSTRAINT [CombatVehicle_fk1]
GO
ALTER TABLE [CombatVehicle] WITH CHECK ADD CONSTRAINT [CombatVehicle_fk2] FOREIGN KEY ([CurrentSuspensiontId]) REFERENCES [Suspension]([SuspensionId])
ON UPDATE CASCADE
GO
ALTER TABLE [CombatVehicle] CHECK CONSTRAINT [CombatVehicle_fk2]
GO
----
----
----
ALTER TABLE [Tank] WITH CHECK ADD CONSTRAINT [Tank_fk0] FOREIGN KEY ([TankId]) REFERENCES [Suspension]([TankId])
ON UPDATE CASCADE
GO
ALTER TABLE [Tank] CHECK CONSTRAINT [Tank_fk0]
GO
----
----
----
ALTER TABLE [Suspension] WITH CHECK ADD CONSTRAINT [Suspension_fk0] FOREIGN KEY ([TankId]) REFERENCES [Tank]([TankId])
ON UPDATE CASCADE
GO
ALTER TABLE [Suspension] CHECK CONSTRAINT [Suspension_fk0]
GO
-----


---------------
---------------
--Наповнення Таблиць
---------------
---------------

INSERT dbo.TypeOfTank(TypeId,GunTypeName)  
    VALUES (1, 'common')  
GO
INSERT dbo.TypeOfTank(TypeId,GunTypeName)  
    VALUES (2, 'swinging'),
	(3, 'artsau'),
	(4, 'sau'),
	(5, 'double-barreled'),
	(6, 'semi-sau'),
	(7, 'semi-artsau'),
	(8, 'hydraulic'),
	(9, 'motionless'),
	(10, 'terrestrial')
GO

INSERT dbo.Guns(GunId,GunTypeId,GunName,Caliber,Damage)  
    VALUES (1,1,'BL-9',152,750)  
GO
INSERT dbo.Guns(GunId,GunTypeId,GunName,Caliber,Damage)  
    VALUES  
	(2,2,'BL-10',152,750),
	(3,3,'Zis-10',100,300),
	(5,5,'Zis-15',76,180),
	(6,1,'T4',57,75),
	(7,8,'Pzkfc-1',122,320),
	(8,2,'DGKN',88,240),
	(9,6,'Plot',75,100),
	(10,8,'Zeves',107,240),
	(11,10,'Link-1',15,50),
	(12,9,'BL-7',192,1090),
	(13,3,'Pzcfc-5',750,10000)
GO

INSERT dbo.Armament(TowerId,Tower,CurrentGunId)  
    VALUES 	(1,'T34',1)
	
GO
INSERT dbo.Armament(TowerId,Tower,CurrentGunId)  
    VALUES 
	(2,'T34',2),
	(3,'T34',3),
	(4,'T-34-85',2),
	(5,'T4',2),
	(6,'T5',5),
	(7,'T4',1),
	(8,'T5',6),
	(9,'Tiger',7),
	(10,'Maus',1),
	(11,'Kv-4',3),
	(12,'IS-7',2),
	(13,'T-54',8)
GO



INSERT dbo.Player(PlayerId,NickName,Email,Passwords,Expir,CurrencyAmount)  
    VALUES 	(1,'NP','NP@gmail.com','12356',123400,7000)
	
GO
INSERT dbo.Player(PlayerId,NickName,Email,Passwords,Expir,CurrencyAmount)  
    VALUES 	
	(2,'Jove','Jove@gmail.com','1212356',12300,600),
	(3,'MP','MP@gmail.com','1qwr356',12445,75000),
	(4,'Porh','Porh@gmail.com','121256',123124400,3000),
	(5,'Qwerty','Qwerty@gmail.com','12123356',120,7200),
	(6,'JamesBond','JamesBond@gmail.com','1qw2356',3400,1000),
	(7,'Kulia','Kulia@gmail.com','12123356',1279,70000),
	(8,'NazckiSubaru','NazckiSubaru@gmail.com','14q2356',6457,17000),
	(9,'KanekckiKen','KanekckiKen@gmail.com','12das356',23547,72000),
	(10,'Naruto','Naruto@gmail.com','1212as356',346347,70700),
	(11,'Sasuke','Sasuke@gmail.com','12asd356',34784,70001),
	(12,'Meliodas','Meliodas@gmail.com','1212a356',34637,71000),
	(13,'WinsonKley','WinsonKley@gmail.com','123asf56',347488,67000),
	(14,'Rouch','Rouch@gmail.com','1asd2356',232352,7000),
	(15,'GeraltIzRIVIII','GeraltIzRIVIII@gmail.com','12ad356',4567547,997000)
	
GO

INSERT dbo.Suspension(SuspensionId,TankId,TypeOfSuspension,LiftingСapacity)  
    VALUES 	(1,1,'ordinary',720)
	
GO
INSERT dbo.Suspension(SuspensionId,TankId,TypeOfSuspension,LiftingСapacity)  
    VALUES 	
	(2,1,'wheeled',1000),
	(3,2,'special',1240),
	(4,1,'ordinary',560),
	(5,3,'ordinary',10000),
	(6,4,'special',5400),
	(7,1,'wheeled',12415),
	(8,5,'ordinary',12414),
	(9,6,'special',12456),
	(10,7,'ordinary',12523),
	(11,8,'special',72)
	
GO


INSERT dbo.Tank(TankName,TankId,TypeOfTank,Nation,CreationDate)  
    VALUES 	('T-34-85',1,'MediumTank','USSR','1942-01-01')
	
GO

INSERT dbo.Tank(TankName,TankId,TypeOfTank,Nation,CreationDate)  
    VALUES 	
	('T-34',2,'MediumTank','USSR','1941-10-01'),
	('IS-7',3,'HeavyTank','USSR','1943-01-10'),
	('KV-4',4,'HeavyTank','USSR','1940-09-01'),
	('ST-II',5,'HeavyTank','USSR','1947-01-09'),
	('T57',6,'HeavyTank','USA','1960-11-01'),
	('Type5',7,'HeavyTank','Japan','1939-01-11'),
	('Leopard',8,'MediumTank','Germany','1970-08-01'),
	('T110E5',9,'HeavyTank','USA','1960-01-08'),
	('AMX13105',10,'LightTank','France','1960-07-01'),
	('Obj.777',11,'HeavyTank','USSR','1946-01-07'),
	('Obj.268',11,'SAU','USSR','1948-11-11'),
	('Obj.268/4',11,'SAU','USSR','1945-12-12'),
	('Obj.257F',11,'HeavyTank','USSR','1999-10-10')
	
GO


INSERT dbo.CombatVehicle(CombatVehicleId,CurrentTankId,Cost,CurrentLevel,CurrentArmamentId,CurrentSuspensiontId)  
    VALUES 	(1,1,100,1,1,1)
	
GO
INSERT dbo.CombatVehicle(CombatVehicleId,CurrentTankId,Cost,CurrentLevel,CurrentArmamentId,CurrentSuspensiontId)  
    VALUES 
	(2,3,12100,3,7,1),
	(3,4,33550,5,5,2),
	(4,7,6100000,10,3,3),
	(5,3,4890000,9,2,2),
	(6,5,123000,7,6,5),
	(7,6,33690,5,7,6),
	(8,4,909000,6,8,2),
	(9,6,70550,3,9,5),
	(10,10,36000,2,10,7),
	(11,3,0,1,2,5),
	(12,4,230000,8,5,4)

	
GO

INSERT dbo.PlayerVehicle(CurrentPlayerId,CurrentVehicleId)  
    VALUES 	(1,1)
	
GO
INSERT dbo.PlayerVehicle(CurrentPlayerId,CurrentVehicleId)  
    VALUES 	
	(2,2),
	(3,3),
	(4,5),
	(5,6),
	(6,7),
	(7,8),
	(8,8),
	(9,9),
	(10,4),
	(11,7),
	(12,5),
	(13,7)

	
GO

INSERT dbo.Info(TankNumber,Cost,CurrentLevel,TankName,TypeOfTank)  
    VALUES 	(10,36000,2,'AMX13105','LightTank'),
	(11,123500,6,'Obj.268','SAU'),
	(12,102350,6,'Obj.268/4','SAU'),
	(13,102670,7,'Obj.777','HeavyTank')
	
GO

INSERT dbo.Info(TankNumber,Cost,CurrentLevel,TankName,TypeOfTank)  
    VALUES 	
	(2,12100,3,'T-34','MediumTank'),
	(3,33550,5,'IS-7','HeavyTank'),
	(4,6100000,10,'KV-4','HeavyTank'),
	(5,4890000,9,'ST-II','HeavyTank'),
	(6,123000,7,'T57','HeavyTank'),
	(7,33690,5,'Type5','HeavyTank'),
	(8,909000,6,'Leopard','MediumTank'),
	(9,70550,3,'T110E5','HeavyTank'),
	(10,36000,2,'AMX13105','LightTank'),
	(11,123500,6,'Obj.268','SAU'),
	(12,102350,6,'Obj.268/4','SAU'),
	(13,102670,7,'Obj.777','HeavyTank')
	
GO
select * from dbo.Armament
select* from dbo.CombatVehicle


-- користувачі
CREATE ROLE [OnlyRead]
GRANT SELECT ON [dbo].[Info] TO [OnlyRead];
DENY DELETE ON [dbo].[Info] TO [OnlyRead];
DENY UPDATE ON [dbo].[Info] TO [OnlyRead];
DENY Insert ON [dbo].[Info] TO [OnlyRead];

CREATE LOGIN Employee1 WITH PASSWORD = 'qwerty123';
CREATE USER Employee1 FOR LOGIN Employee1;  
ALTER ROLE [OnlyRead] ADD MEMBER Employee1


CREATE ROLE [SystemAdmin]
GRANT SELECT ON [dbo].[Info] TO [SystemAdmin];
GRANT DELETE ON [dbo].[Info] TO [SystemAdmin];
GRANT UPDATE ON [dbo].[Info] TO [SystemAdmin];
GRANT INSERT ON [dbo].[Info] TO [SystemAdmin];

CREATE LOGIN Employee2 WITH PASSWORD = 'qwerty123';
CREATE USER Employee2 FOR LOGIN Employee2;  
ALTER ROLE [SystemAdmin] ADD MEMBER Employee2

SELECT @@version





