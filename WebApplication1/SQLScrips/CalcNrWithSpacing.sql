SELECT *, (seperatedVendors*1.0/allVendors)*100 AS percentileVendors,
(seperatedProducts*1.0/allProducts)*100 AS percentileProducts FROM
(
SELECT
(SELECT COUNT(*) FROM (SELECT DISTINCT Vendor FROM CPEs WHERE Vendor  LIKE '%[_]%')b)AS seperatedVendors,
(SELECT COUNT(*) FROM (SELECT DISTINCT Vendor FROM CPEs)c) As allVendors,

(SELECT COUNT(*) FROM (SELECT DISTINCT Product FROM CPEs WHERE Product  LIKE '%[_]%')b) AS seperatedProducts,
(SELECT COUNT(*) FROM (SELECT DISTINCT Product FROM CPEs)c) As allProducts )b 


seperatedVendors	allVendors	seperatedProducts	allProducts	percentileVendors	percentileProducts
5379	15422	58156	85169	34.878744650400	68.283060738000