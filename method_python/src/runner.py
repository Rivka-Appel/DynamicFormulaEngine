
from .db import get_connection
from .formulas import get_formulas, process_formula

def main():
    conn = get_connection()
    cursor = conn.cursor()

    formulas = get_formulas(cursor)
    print(f"Found {len(formulas)} formulas in t_targil")

    # <<< מצב דיבוג: מריצים רק targil_id=14 >>>
    formulas = [f for f in formulas if f.targil_id == 14]

    for targil_row in formulas:
        process_formula(cursor, targil_row)
        conn.commit()

    cursor.close()
    conn.close()


if __name__ == "__main__":
    main()
