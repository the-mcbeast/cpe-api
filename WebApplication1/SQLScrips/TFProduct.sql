
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'TFProduct')
BEGIN
	CREATE TABLE TFProduct(
		Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
		Term NVARCHAR(100) NOT NULL,
		rawCount INT  NULL,
		TermFrequency REAL NULL,
		logNormalized REAL NULL,
	);
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
logNormalized = log(1+rawCount)



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