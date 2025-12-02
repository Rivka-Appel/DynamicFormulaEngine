-- Run SQL_SP method for all formulas in t_targil

DECLARE @targil_id INT;

DECLARE targil_cursor CURSOR FAST_FORWARD FOR
    SELECT targil_id
    FROM t_targil
    ORDER BY targil_id;

OPEN targil_cursor;

FETCH NEXT FROM targil_cursor INTO @targil_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT CONCAT('Running formula for targil_id = ', @targil_id);

    EXEC sp_run_formula_for_targil @targil_id = @targil_id;

    FETCH NEXT FROM targil_cursor INTO @targil_id;
END;

CLOSE targil_cursor;
DEALLOCATE targil_cursor;


