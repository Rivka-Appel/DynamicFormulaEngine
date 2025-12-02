INSERT INTO t_targil (targil) VALUES
('a + b'),
('a - c'),
('b * d'),
('c / 4');

INSERT INTO t_targil (targil) VALUES
('POWER(a, 2) + POWER(b, 2)'),
('POWER(d, 3) - POWER(a, 2)'),
('SQRT(POWER(a, 2) + POWER(b, 2))'),
('ABS(b - d)'),
('LOG(c + 1)');

INSERT INTO t_targil (targil) VALUES
('8 * (b + a)'),
('2 * (a - d + c)');

INSERT INTO t_targil (targil, tnai, targil_false) VALUES
('b * 2', 'a > 5', 'b / 2'),
('a + 1', 'b < 10', 'd - 1'),
('POWER(a,2)', 'c = d', '0');
