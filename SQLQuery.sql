CREATE DATABASE MunicipalityTaxStore
 
GO 
 
USE  MunicipalityTaxStore
 
GO 


CREATE TABLE MunicipalityTaxDetail
  ( 
     Id   BIGINT IDENTITY(1,1)  NOT NULL, 
     Name NVARCHAR(50) NOT NULL, 
	 MunicipalityId BIGINT NOT NULL,
     Frequency NVARCHAR(50) NULL, 
     StartDate DATE NULL,
     EndDate DATE NULL,
     Tax FLOAT NULL
     PRIMARY KEY (Id) 
  ) 
 
GO 

INSERT INTO MunicipalityTaxDetail
VALUES
('Copenhagen',1,'yearly','1/1/2016','12/31/2016',0.2),
('Copenhagen',1,'monthly','5/1/2016','5/31/2016',0.4),
('Copenhagen',1,'daily','1/1/2016','1/1/2016',0.1),
('Copenhagen',1,'daily','12/31/2016','12/31/2016',0.1)