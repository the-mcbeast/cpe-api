IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateResultsProduct')
BEGIN
	DROP PROCEDURE EvaluateResultsProduct
END

GO
CREATE PROCEDURE EvaluateResultsProduct(@typ INT )
AS


SELECT * ,
trueNeg / (trueNeg + falPos * 1.0) AS Spec,
truePos/(truePos + falNeg*1.0 )As Recall,
truePos/(truePos + falPos*1.0) As Precision,
(truePos + trueNeg)/(truePos + trueNeg + falNeg +falPos *1.0) As Accuracy ,
(2* 	((truePos/(truePos + falPos*1.0)	)*
	(	(truePos/ (truePos + falNeg*1.0 ))	)
	)/	((truePos/(truePos + falPos*1.0))
		+((truePos/(truePos + falNeg*1.0 )))	)) AS f1
FROM (

SELECT 
	(SELECT name FROM Typ WHERE Id = @typ) AS Name,
	SUM(CASE WHEN b.truePositivePerReq = 0 THEN 0 ELSE 1 END ) AS truePos,
	SUM(CASE WHEN b.trueNegativePerReq = 0 THEN 0 ELSE 1 END ) AS trueNeg,
	SUM(CASE WHEN b.falsePositivePerReq = 0 THEN 0 ELSE 1 END ) AS falPos,
	SUM(CASE WHEN b.falseNegativePerReq = 0 THEN 0 ELSE 1 END ) AS falNeg
	FROM (
SELECT TOP (100) *, (
	SELECT COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN Antworten3 TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE anfOut.Id = ExpRes.AnfrageId 
				AND ExpRes.Typ = @typ
				AND ExpRes.CPEId = TruRes.CPEId
				
		) As truePositivePerReq,

	(SELECT	COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN Antworten3 TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE TruRes.CPEId IS NULL 
				AND ExpRes.CPEId IS NULL 
				AND ExpRes.ResultNr = 0 
				AND	ExpRes.AnfrageId = anfOut.Id 
				AND	ExpRes.Typ = @typ) 
			AS trueNegativePerReq,
	(SELECT COUNT(1)
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
							) AS falsePositivePerReq,
 (
	SELECT 
			COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN Antworten3 TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE
				ExpRes.CPEId IS NULL AND ExpRes.ResultNr = 0 AND TruRes.CPEId IS NOT NULL
				AND ExpRes.Typ =@typ 
				AND ExpRes.AnfrageId = anfOut.Id
) As falseNegativePerReq
FROM Anfragen anfOut 
)b)v
GO

	

IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'EvaluateResultsVersion')
BEGIN
	DROP PROCEDURE EvaluateResultsVersion
END

GO
CREATE PROCEDURE EvaluateResultsVersion(@typ INT )
AS


SELECT * ,
trueNeg / (trueNeg + falPos * 1.0) AS Spec,
truePos/(truePos + falNeg*1.0 )As Recall,
truePos/(truePos + falPos*1.0) As Precision,
(truePos + trueNeg)/(truePos + trueNeg + falNeg +falPos *1.0) As Accuracy ,
(2* 	((truePos/(truePos + falPos*1.0)	)*
	(	(truePos/ (truePos + falNeg*1.0 ))	)
	)/	((truePos/(truePos + falPos*1.0))
		+((truePos/(truePos + falNeg*1.0 )))	)) AS f1
FROM (

SELECT 
	(SELECT name FROM Typ WHERE Id = @typ) + ' - V' AS Name,
	SUM(CASE WHEN b.truePositivePerReq = 0 THEN 0 ELSE 1 END ) AS truePos,
	SUM(CASE WHEN b.trueNegativePerReq = 0 THEN 0 ELSE 1 END ) AS trueNeg,
	SUM(CASE WHEN b.falsePositivePerReq = 0 THEN 0 ELSE 1 END ) AS falPos,
	SUM(CASE WHEN b.falseNegativePerReq = 0 THEN 0 ELSE 1 END ) AS falNeg
	FROM (
SELECT TOP (100) *, (
	SELECT COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN AntwortenV TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE anfOut.Id = ExpRes.AnfrageId 
				AND ExpRes.Typ = @typ
				AND ExpRes.CPEId = TruRes.CPEId
				
		) As truePositivePerReq,

	(SELECT	COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN AntwortenV TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE TruRes.CPEId IS NULL 
				AND ExpRes.CPEId IS NULL 
				AND ExpRes.ResultNr = 0 
				AND	ExpRes.AnfrageId = anfOut.Id 
				AND	ExpRes.Typ = @typ) 
			AS trueNegativePerReq,
	(SELECT COUNT(1)
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
							) AS falsePositivePerReq,
 (
	SELECT 
			COUNT(1)
				FROM  Antworten2 ExpRes 
				JOIN AntwortenV TruRes on ExpRes.AnfrageId  = TruRes.AnfrageId 
				WHERE
				ExpRes.CPEId IS NULL AND ExpRes.ResultNr = 0 AND TruRes.CPEId IS NOT NULL
				AND ExpRes.Typ =@typ 
				AND ExpRes.AnfrageId = anfOut.Id
) As falseNegativePerReq
FROM Anfragen anfOut 
)b)v
	GO