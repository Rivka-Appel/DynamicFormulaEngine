CREATE OR ALTER PROCEDURE dbo.sp_run_formula_for_targil
    @targil_id INT
AS
BEGIN
	-- Avoid row count messages
    SET NOCOUNT ON; 

    DECLARE
        @targil       NVARCHAR(4000),
        @tnai         NVARCHAR(4000),
        @targil_false NVARCHAR(4000),
        @expr         NVARCHAR(MAX),
        @sql          NVARCHAR(MAX),
        @method       NVARCHAR(50),
        @startTime    DATETIME2(3),
        @endTime      DATETIME2(3),
        @seconds      FLOAT;

    -- Load the targil row for the given targil_id
    SELECT
        @targil       = targil,
        @tnai         = tnai,
        @targil_false = targil_false
    FROM t_targil
    WHERE targil_id = @targil_id;

    IF @targil IS NULL
    BEGIN
        RAISERROR('targil_id %d not found in t_targil', 16, 1, @targil_id);
        RETURN;
    END;

    -- Build expression 
    IF @tnai IS NULL OR LTRIM(RTRIM(@tnai)) = ''
    BEGIN
        SET @expr = @targil;
    END
    ELSE
    BEGIN
        SET @expr = N'CASE WHEN ' + @tnai + N' THEN ' + @targil + N' ELSE ' + @targil_false + N' END';
    END;

	-- Identify this calculation method
    SET @method = N'SQL_SP'; 

    -- Remove previous results for this targil and method
    DELETE FROM t_results
    WHERE targil_id = @targil_id
      AND method = @method;

    SET @startTime = SYSDATETIME(); -- Start timing

    -- Insert results for all rows in t_data
    SET @sql = N'
        INSERT INTO t_results (data_id, targil_id, method, result)
        SELECT 
            data_id,
            ' + CAST(@targil_id AS NVARCHAR(20)) + N',
            ''' + @method + N''',
            ' + @expr + N'
        FROM t_data;
    ';

    EXEC sp_executesql @sql;

    SET @endTime = SYSDATETIME(); -- End timing

    -- Compute duration in seconds
    SET @seconds = DATEDIFF(MILLISECOND, @startTime, @endTime) / 1000.0;

    -- Log execution time for this targil and method
    INSERT INTO t_log (targil_id, method, run_time)
    VALUES (@targil_id, @method, @seconds);
END;
GO

-- exec
EXEC dbo.sp_run_formula_for_targil @targil_id = 1;

select count(*) from t_results

