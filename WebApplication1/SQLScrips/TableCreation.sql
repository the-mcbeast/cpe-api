IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'CPEs')
BEGIN
	CREATE TABLE CPEs(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		[Name] NVARCHAR(100),
		[Title] NVARCHAR(100),
		Part NVARCHAR(100),
		Vendor NVARCHAR(100),
		Product NVARCHAR(100),
		[Version] NVARCHAR(100),
		[Update] NVARCHAR(100),
		Edition NVARCHAR(100),
		[Language] NVARCHAR(100),
		Sw_edition NVARCHAR(100),
		Target_sw NVARCHAR(100),
		Target_hw NVARCHAR(100),
		Other NVARCHAR(100),

	);
END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'TFProduct')
BEGIN
	CREATE TABLE TFProduct(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		Term NVARCHAR(100) NOT NULL,
		rawCount INT  NULL,
		TermFrequency REAL NULL,
		logNormalized REAL NULL,
		doubleNormalized REAL NULL,
		Idf REAL NULL,
		Idfsmooth REAL NULL,
		Idfmax REAL NULL,
		Idfprob REAL NULL,
		Occurence REAL NULL,
		tfIdfCount REAL NULL,
		tfIdf REAL NULL,
		tfIdfdoubleNorm REAL NULL,
		tfIdfLogNorm REAL NULL

	);
	

	
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Typ')
BEGIN
	CREATE TABLE Typ(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		[Name] NVARCHAR(100) NOT NULL
	);
	INSERT INTO Typ ([Name]) VALUES ('CONTROL'),('LEVEN'),('SET'),('MANUAL')
	END

	
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Anfragen')
BEGIN
	CREATE TABLE Anfragen(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		Part NVARCHAR(100)  NULL,
		Vendor NVARCHAR(100)  NULL,
		Product NVARCHAR(100) NOT NULL,
		[Version] NVARCHAR(100)  NULL,
		
		Created DATETIME 

	);
	END

	
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Antworten2')
BEGIN
	CREATE TABLE Antworten3(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		AnfrageId INT NOT NULL CONSTRAINT FK_Antworten_Anfrage FOREIGN KEY (AnfrageId) REFERENCES Anfragen(Id),
		CPEId INT NOT NULL CONSTRAINT FK_CPE_Anfrage FOREIGN KEY (CPEId) REFERENCES CPEs(Id),
		Typ INT NOT NULL CONSTRAINT FK_Anfragetyp_Typ FOREIGN KEY (Typ) REFERENCES Typ(Id),
		ResultNr INT NOT NULL,
		LocalScore REAL,
		GlobalScore REAL,
		Created DATETIME 
	);
	END
	
INSERT INTO TfProduct(Term)
 SELECT DISTINCT value
    FROM CPEs  
        CROSS APPLY STRING_SPLIT(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(
							Product, ' ', '_'), ',', '_'), '.', '_'),
									':', '_'), '\t', '_'), '\', '_'),
									'!', '_'), '"', '_'),'/', '_'),
									'#', '_'), '&', '_'),'%', '_'),
									'$', '_'), '?', '_'),'@', '_'),
									'(', '_'), ')', '_'),'[', '_'),
									']', '_'), '+', '_'),'|', '_'),
									'*', '_'), '<', '_'),'>', '_'),
									'=', '_'), '^', '_'),'-', '_'),
									'''','_'),
							'_')
    WHERE value != ''
	ORDER BY value ASC
	

END

UPDATE TFProduct
SET rawCount =
(   SELECT COUNT(1)
    FROM 
	(SELECT DISTINCT Product FROM CPEs)t
        CROSS APPLY STRING_SPLIT(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(
							t.Product, ' ', '_'), ',', '_'), '.', '_'),
									':', '_'), '\t', '_'), '\', '_'),
									'!', '_'), '"', '_'),'/', '_'),
									'#', '_'), '&', '_'),'%', '_'),
									'$', '_'), '?', '_'),'@', '_'),
									'(', '_'), ')', '_'),'[', '_'),
									']', '_'), '+', '_'),'|', '_'),
									'*', '_'), '<', '_'),'>', '_'),
									'=', '_'), '^', '_'),'-', '_'),
									'''','_'),
							'_')
 WHERE value = Term)
 UPDATE TFProduct
SET
TermFrequency = rawCount / @totalTermCount,
logNormalized = log(1+rawCount),
doubleNormalized = 0.5 + 0.5 * ( rawcount / (SELECT MAX(rawCount) FROM TFProduct)), 
 Idf = LOG(@totalTermCount / rawCount),
 Idfsmooth = LOG(@totalTermCount / (rawCount+1))+1,
 Idfmax = LOG(20616 / 1+ rawCount),
 Idfprob = LOG((@totalTermCount - rawCount)/rawCount)
UPDATE TFProduct
SET
 tfIdf = TermFrequency * Idf,
 tfIdfdoubleNorm = Idf *doubleNormalized,
 tfIdfLogNorm = logNormalized * Idf,
 tfIdfCount =rawCount*Idf



IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'TFVendor')
BEGIN
	CREATE TABLE TFVendor(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		Term NVARCHAR(100)  NULL,
		rawCount INT NOT NULL,
		TermFrequency REAL NULL,
		logNormalized REAL NULL,
		doubleNormalized REAL NULL,
		Idf REAL NULL,
		Idfsmooth REAL NULL,
		Idfmax REAL NULL,
		Idfprob REAL NULL,
		Occurence REAL NULL,
		tfIdfCount REAL NULL,
		tfIdf REAL NULL,
		tfIdfdoubleNorm REAL NULL,
		tfIdfLogNorm REAL NULL

	);
	
INSERT INTO TFVendor(Term)
 SELECT DISTINCT value
    FROM CPEs  
        CROSS APPLY STRING_SPLIT(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(
							Vendor, ' ', '_'), ',', '_'), '.', '_'),
									':', '_'), '\t', '_'), '\', '_'),
									'!', '_'), '"', '_'),'/', '_'),
									'#', '_'), '&', '_'),'%', '_'),
									'$', '_'), '?', '_'),'@', '_'),
									'(', '_'), ')', '_'),'[', '_'),
									']', '_'), '+', '_'),'|', '_'),
									'*', '_'), '<', '_'),'>', '_'),
									'=', '_'), '^', '_'),'-', '_'),
									'''','_'),
							'_')
    WHERE value != ''
	ORDER BY value ASC
	

END



IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'TFTitle')
BEGIN
	CREATE TABLE TFTitle(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		Term NVARCHAR(100) NOT NULL,
		rawCount INT  NULL,
		TermFrequency REAL NULL,
		logNormalized REAL NULL,
		doubleNormalized REAL NULL,
		Idf REAL NULL,
		Idfsmooth REAL NULL,
		Idfmax REAL NULL,
		Idfprob REAL NULL,
		Occurence REAL NULL,
		tfIdfCount REAL NULL,
		tfIdf REAL NULL,
		tfIdfdoubleNorm REAL NULL,
		tfIdfLogNorm REAL NULL
		
	);
	
INSERT INTO TfTitle(Term)
 SELECT DISTINCT value
    FROM CPEs  
        CROSS APPLY STRING_SPLIT(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(
							Title, ' ', '_'), ',', '_'), '.', '_'),
									':', '_'), '\t', '_'), '\', '_'),
									'!', '_'), '"', '_'),'/', '_'),
									'#', '_'), '&', '_'),'%', '_'),
									'$', '_'), '?', '_'),'@', '_'),
									'(', '_'), ')', '_'),'[', '_'),
									']', '_'), '+', '_'),'|', '_'),
									'*', '_'), '<', '_'),'>', '_'),
									'=', '_'), '^', '_'),'-', '_'),
									'''','_'),
							'_')
    WHERE value != ''
	ORDER BY value ASC
	

END


DECLARE @totalTermCount INT
SET @totalTermCount = (SELECT COUNT(value)
    FROM 
	(SELECT DISTINCT Product FROM CPEs)t
        CROSS APPLY STRING_SPLIT(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(REPLACE(REPLACE(
						REPLACE(
							t.Product, ' ', '_'), ',', '_'), '.', '_'),
									':', '_'), '\t', '_'), '\', '_'),
									'!', '_'), '"', '_'),'/', '_'),
									'#', '_'), '&', '_'),'%', '_'),
									'$', '_'), '?', '_'),'@', '_'),
									'(', '_'), ')', '_'),'[', '_'),
									']', '_'), '+', '_'),'|', '_'),
									'*', '_'), '<', '_'),'>', '_'),
									'=', '_'), '^', '_'),'-', '_'),
									'''','_'),
							'_') WHERE value IS NOT NULL AND value != '')
GO