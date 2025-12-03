
-- Index added to speed up grouping
CREATE INDEX IX_t_results_targil_method_data
ON t_results (targil_id, method, data_id);


/*
  For each (targil_id, data_id):
    - Calculate MIN and MAX result across all methods.
    - Count how many methods returned a value.
  A row is a mismatch if:
    - MIN != MAX (beyond epsilon), or
    - Not all methods returned a result.
  Returns mismatch count per targil_id.
*/

CREATE OR ALTER PROCEDURE dbo.sp_compare_methods
    @ExpectedMethodsCount INT = 3 --(SQL_SP, C_SHARP, PYTHON)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        t.targil_id,
        COUNT(*) AS mismatches
    FROM (
        SELECT 
            targil_id,
            data_id,
            MIN(result) AS min_result,
            MAX(result) AS max_result,
            COUNT(DISTINCT method) AS methods_count
        FROM t_results
        GROUP BY targil_id, data_id
    ) AS t
    WHERE 
        ABS(t.max_result - t.min_result) > 0.000001
        OR t.methods_count < @ExpectedMethodsCount
    GROUP BY t.targil_id
    ORDER BY t.targil_id;
END;
GO


-- exec
EXEC dbo.sp_compare_methods @ExpectedMethodsCount = 3;
