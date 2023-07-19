DECLARE @t TABLE (name VARCHAR(100),truePos INT, trueNeg INT, falPos INT, falNeg INT, Spec REAL, Recall REAL, Precision REAL, Accuracy REAL, f1 REAL)
INSERT INTO @t EXEC	 [dbo].[EvaluateResultsProduct] @typ = 1
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsProduct] @typ = 2
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsProduct] @typ =3
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 1
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 2
INSERT INTO @t 	EXEC [dbo].[EvaluateResultsVersion] @typ = 3

SELECT * FROM @t