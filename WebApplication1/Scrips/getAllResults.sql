DECLARE @t TABLE (name VARCHAR(100),truePos INT, trueNeg INT, falPos INT, falNeg INT, Spec REAL, Recall REAL, Precision REAL, Accuracy REAL, f1 REAL)
INSERT INTO @t EXEC	 [dbo].[EvaluateResultsProduct] @typ = 1
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsProduct] @typ = 2
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsProduct] @typ =3
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 1
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 2
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 3

SELECT * FROM @t

DECLARE @g TABLE (name VARCHAR(100), avgTruPos REAL, maxTruePos REAL, minTruePos REAL, avgFalsePos REAL, maxFalsePos REAL, minFalsePos REAL)
INSERT INTO @g EXEC	 EvaluateGlobalScoreProduct @typ = 1
INSERT INTO @g 	EXEC EvaluateGlobalScoreProduct @typ = 2
INSERT INTO @g 	EXEC EvaluateGlobalScoreProduct @typ =3
INSERT INTO @g 	EXEC EvaluateGlobalScoreVersion @typ = 1
INSERT INTO @g 	EXEC EvaluateGlobalScoreVersion @typ = 2
INSERT INTO @g 	EXEC EvaluateGlobalScoreVersion @typ = 3

SELECT * FROM @g