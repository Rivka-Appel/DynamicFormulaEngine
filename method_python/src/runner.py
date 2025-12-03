"""
Entry point for the PYTHON method:
load formulas from t_targil and process each one on all rows in t_data.
"""

from .db import get_connection
from .formulas import get_formulas, process_formula


def main():
    conn = get_connection()
    cursor = conn.cursor()

    formulas = get_formulas(cursor)
    print(f"Found {len(formulas)} formulas in t_targil")

    for targil_row in formulas:
        process_formula(cursor, targil_row)
        conn.commit()

    cursor.close()
    conn.close()


if __name__ == "__main__":
    main()
