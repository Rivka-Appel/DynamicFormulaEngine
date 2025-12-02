from db import get_connection

def main():
    conn = get_connection()
    cursor = conn.cursor()

    cursor.execute("SELECT COUNT(*) FROM t_targil")
    count = cursor.fetchone()[0]
    print(f"Found {count} formulas in t_targil")

    cursor.close()
    conn.close()


if __name__ == "__main__":
    main()
