DECLARE @i INT = 0;

WHILE @i < 1000000
BEGIN
    INSERT INTO t_data (a, b, c, d)
    VALUES (RAND()*1000, RAND()*1000, RAND()*1000, RAND()*1000);

    SET @i = @i + 1;
END;

SELECT COUNT(*) FROM t_data;

