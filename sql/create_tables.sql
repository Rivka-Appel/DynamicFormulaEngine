-- Create data table
CREATE TABLE t_data (
    data_id INT IDENTITY(1,1) PRIMARY KEY,
    a FLOAT NOT NULL,
    b FLOAT NOT NULL,
    c FLOAT NOT NULL,
    d FLOAT NOT NULL
);

-- Create targil table
CREATE TABLE t_targil (
    targil_id INT IDENTITY(1,1) PRIMARY KEY,
    targil VARCHAR(255) NOT NULL,
    tnai VARCHAR(255) NULL,
    targil_false VARCHAR(255) NULL
);

-- Create results table
CREATE TABLE t_results (
    results_id INT IDENTITY(1,1) PRIMARY KEY,
    data_id INT NOT NULL,
    targil_id INT NOT NULL,
    method VARCHAR(50) NOT NULL,
    result FLOAT,
    FOREIGN KEY (data_id) REFERENCES t_data(data_id),
    FOREIGN KEY (targil_id) REFERENCES t_targil(targil_id)
);

-- Create log table
CREATE TABLE t_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    targil_id INT NOT NULL,
    method VARCHAR(50) NOT NULL,
    run_time FLOAT,
    FOREIGN KEY (targil_id) REFERENCES t_targil(targil_id)
);
