IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateGlobalScoreProduct')
BEGIN
	DROP PROCEDURE EvaluateGlobalScoreProduct
END

GO
CREATE PROCEDURE EvaluateGlobalScoreProduct(@typ INT )
AS

SELECT 
	(SELECT name FROM Typ WHERE Id = @typ) AS Name,	
	AVG(truePositive) as averageTruePos,
	MAX(truePositive) as maxTruePos,
	MIN(truePositive) as minTruePos,
	AVG(falsePositive) as avgFalsePos,
	MAX(falsePositive) as maxFalsePos,
	MIN(falsePositive) as minFalsePos
	
	FROM (

SELECT TOP (100)  Id,
(
	SELECT AVG(ExpRes.GlobalScore)
				FROM  Antworten2 ExpRes 
				JOIN Antworten3 TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE anfOut.Id = ExpRes.AnfrageId 
				AND ExpRes.Typ = @typ
				AND ExpRes.CPEId = TruRes.CPEId
				
		) As truePositive,
	(SELECT AVG(ExpRes.GlobalScore)
				FROM  Antworten2 ExpRes 
				JOIN Antworten3 TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE 
				ExpRes.AnfrageId = anfOut.Id 
				AND ExpRes.Typ = @typ AND
				(
					(
						TruRes.CPEId IS  NULL AND 
						ExpRes.CPEId IS NOT NULL)
					OR
					(
						TruRes.CPEId IS NOT NULL AND
						ExpRes.CPEId IS NOT NULL AND
						NOT EXISTS(SELECT *
									FROM  Antworten2 ExpResIn 
									JOIN Antworten3 TruResIn on ExpResIn.AnfrageId  = TruResIn.AnfrageId 
									WHERE anfOut.Id = ExpResIn.AnfrageId 
										AND ExpResIn.Typ = @typ
										AND ExpResIn.CPEId = TruResIn.CPEId)))
							) AS falsePositive
FROM Anfragen anfOut 
)b

IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateGlobalScoreVersion')
BEGIN
	DROP PROCEDURE EvaluateGlobalScoreVersion
END

GO
CREATE PROCEDURE EvaluateGlobalScoreVersion(@typ INT )
AS

SELECT 
	(SELECT name FROM Typ WHERE Id = @typ)+ ' - V'  AS Name,	
	AVG(truePositive) as averageTruePos,
	MAX(truePositive) as maxTruePos,
	MIN(truePositive) as minTruePos,
	AVG(falsePositive) as avgFalsePos,
	MAX(falsePositive) as maxFalsePos,
	MIN(falsePositive) as minFalsePos
	
	FROM (

SELECT TOP (100)  Id,
(
	SELECT AVG(ExpRes.GlobalScore)
				FROM  Antworten2 ExpRes 
				JOIN AntwortenV TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE anfOut.Id = ExpRes.AnfrageId 
				AND ExpRes.Typ = @typ
				AND ExpRes.CPEId = TruRes.CPEId
				
		) As truePositive,
	(SELECT AVG(ExpRes.GlobalScore)
				FROM  Antworten2 ExpRes 
				JOIN AntwortenV TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE 
				ExpRes.AnfrageId = anfOut.Id 
				AND ExpRes.Typ = @typ AND
				(
					(
						TruRes.CPEId IS  NULL AND 
						ExpRes.CPEId IS NOT NULL)
					OR
					(
						TruRes.CPEId IS NOT NULL AND
						ExpRes.CPEId IS NOT NULL AND
						NOT EXISTS(SELECT *
									FROM  Antworten2 ExpResIn 
									JOIN AntwortenV TruResIn on ExpResIn.AnfrageId  = TruResIn.AnfrageId 
									WHERE anfOut.Id = ExpResIn.AnfrageId 
										AND ExpResIn.Typ = @typ
										AND ExpResIn.CPEId = TruResIn.CPEId)))
							) AS falsePositive
FROM Anfragen anfOut 
)b