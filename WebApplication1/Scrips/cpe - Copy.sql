SELECT COUNT(*) FROM (SELECT DISTINCT Vendor FROM CPEs WHERE Vendor  LIKE '%[_]%')b
SELECT COUNT(*) FROM (SELECT DISTINCT Vendor FROM CPEs)c


SELECT COUNT(*) FROM (SELECT DISTINCT Product FROM CPEs WHERE Product  LIKE '%[_]%')b
SELECT COUNT(*) FROM (SELECT DISTINCT Product FROM CPEs)c



IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateResultsProduct')
BEGIN
	DROP PROCEDURE EvaluateResultsProduct
END

GO
CREATE PROCEDURE EvaluateResultsProduct(@typ INT )
AS

SELECT v.truePositive, v.TrueNegative, v.falsePositive, v.falseNegative,

 COALESCE(v.truePositive,0) +COALESCE(v.TrueNegative,0) +COALESCE(v.falsePositive,0) +COALESCE(v.falseNegative,0) AS summe 
FROM 
(
SELECT COUNT(t.truePositive) AS truePositive
, COUNT(t.TrueNegative) 	 As TrueNegative
, COUNT(t.falsePositive)	  As falsePositive
, COUNT(t.falseNegatve) 	 AS falseNegative

FROM (
--true positive
		SELECT 
			b.AnfId, 
			b.truePerReq AS truePositive, 
			NULL AS TrueNegative,  
			NULL AS falsePositive,
			NULL AS falseNegatve
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN Antworten3 ant3 on anf1.Id = ant3.AnfrageId 
				WHERE ant3.CPEId = ant2.CPEId AND ant2.Typ = @typ
			GROUP BY anf1.Id)b
		
		UNION 
--true negative		
		SELECT b.AnfId,
		NULL,
		b.truePerReq as TrueNegative, NULL, NULL
		 
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN Antworten3 ant3 on anf1.Id = ant3.AnfrageId 		
				WHERE 
				ant3.CPEId IS  NULL AND  
				ant2.CPEId IS  NULL AND  
				ant2.ResultNr = 0 AND
				ant2.Typ = @typ
				GROUP BY anf1.Id)b
		UNION
--false positive
		SELECT b.AnfId,
				NULL,  NULL,
				b.truePerReq as falsepositive
				,NULL
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2a on anf1.Id = ant2a.AnfrageId 
					JOIN Antworten3 ant3a on anf1.Id = ant3a.AnfrageId 
		
				WHERE 
				(ant2a.CPEId IS NOT NULL 
					AND	NOT				
					 EXISTS(
					 SELECT * FROM (SELECT 
									COUNT(anf1.Id) AS truePerReq, 
									anf1.Id AS AnfId
										FROM Anfragen anf1  
											JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
											JOIN Antworten3 ant3 on anf1.Id = ant3.AnfrageId 
										WHERE ant3.CPEId = ant2.CPEId AND ant2.Typ = @typ
									GROUP BY anf1.Id)t
									WHERE t.AnfId = ant2a.AnfrageId
						
				))

					AND ant2a.Typ = @typ
				GROUP BY anf1.Id)b
		
		UNION
		 SELECT b.AnfId,
				NULL, NULL, NULL,
				b.truePerReq as falsenegative
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN Antworten3 ant3 on anf1.Id = ant3.AnfrageId 
		
				WHERE 
					 ant2.CPEId IS  NULL 
						AND ant3.CPEId IS NOT NULL
						AND ant2.ResultNr = 0
						AND  ant2.Typ = @typ
					
				GROUP BY anf1.Id)b
		)t
	)v


	

IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateResultsVersion')
BEGIN
	DROP PROCEDURE EvaluateResultsVersion
END

GO
CREATE PROCEDURE EvaluateResultsVersion(@typ INT )
AS

SELECT v.truePositive, v.TrueNegative, v.falsePositive, v.falseNegative,

 COALESCE(v.truePositive,0) +COALESCE(v.TrueNegative,0) +COALESCE(v.falsePositive,0) +COALESCE(v.falseNegative,0) AS summe 
FROM 
(
SELECT COUNT(t.truePositive) AS truePositive
, COUNT(t.TrueNegative) 	 As TrueNegative
, COUNT(t.falsePositive)	  As falsePositive
, COUNT(t.falseNegatve) 	 AS falseNegative

FROM (
--true positive
		SELECT 
			b.AnfId, 
			b.truePerReq AS truePositive, 
			NULL AS TrueNegative,  
			NULL AS falsePositive,
			NULL AS falseNegatve
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN AntwortenV ant3 on anf1.Id = ant3.AnfrageId 
				WHERE ant3.CPEId = ant2.CPEId AND ant2.Typ = @typ
			GROUP BY anf1.Id)b
		
		UNION 
--true negative		
		SELECT b.AnfId,
		NULL,
		b.truePerReq as TrueNegative, NULL, NULL
		 
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN AntwortenV ant3 on anf1.Id = ant3.AnfrageId 		
				WHERE 
				ant3.CPEId IS  NULL AND  
				ant2.CPEId IS  NULL AND  
				ant2.ResultNr = 0 AND
				ant2.Typ = @typ
				GROUP BY anf1.Id)b
		UNION
--false positive
		SELECT b.AnfId,
				NULL,  NULL,
				b.truePerReq as falsepositive
				,NULL
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2a on anf1.Id = ant2a.AnfrageId 
					JOIN AntwortenV ant3a on anf1.Id = ant3a.AnfrageId 
		
				WHERE 
				(ant2a.CPEId IS NOT NULL 
					AND	NOT				
					 EXISTS(
					 SELECT * FROM (SELECT 
									COUNT(anf1.Id) AS truePerReq, 
									anf1.Id AS AnfId
										FROM Anfragen anf1  
											JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
											JOIN AntwortenV ant3 on anf1.Id = ant3.AnfrageId 
										WHERE ant3.CPEId = ant2.CPEId AND ant2.Typ = @typ
									GROUP BY anf1.Id)t
									WHERE t.AnfId = ant2a.AnfrageId
						
				))

					AND ant2a.Typ = @typ
				GROUP BY anf1.Id)b
		
		UNION
		 SELECT b.AnfId,
				NULL, NULL, NULL,
				b.truePerReq as falsenegative
			FROM
			(SELECT 
			COUNT(anf1.Id) AS truePerReq, 
			anf1.Id AS AnfId
				FROM Anfragen anf1  
					JOIN Antworten2 ant2 on anf1.Id = ant2.AnfrageId 
					JOIN AntwortenV ant3 on anf1.Id = ant3.AnfrageId 
		
				WHERE 
					 ant2.CPEId IS  NULL 
						AND ant3.CPEId IS NOT NULL
						AND ant2.ResultNr = 0
						AND  ant2.Typ = @typ
					
				GROUP BY anf1.Id)b
		)t
	)v

	GO