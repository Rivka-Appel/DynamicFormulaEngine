import pyodbc

def get_connection():
    conn = pyodbc.connect(
        "DRIVER={ODBC Driver 17 for SQL Server};"
        "SERVER=DESKTOP-MG2PNTU\\SQLEXPRESS;"
        "DATABASE=DynamicFormulaDB;"
        "Trusted_Connection=yes;"
    )
    conn.autocommit = False
    return conn
