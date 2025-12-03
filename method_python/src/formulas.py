import time
import math

METHOD_NAME = "PYTHON"

def IF(cond, x, y):
    """
    פונקציית תנאי לשימוש בתוך נוסחאות:
    IF(a > 5, b * 2, b / 2)
    """
    return x if cond else y


def get_formulas(cursor):
    """
    מחזירה את כל הנוסחאות מטבלת t_targil.
    העמודות לפי ה-DDL שלך:
    targil_id, targil, tnai, targil_false
    """
    cursor.execute(
        """
        SELECT targil_id,
               targil,
               tnai,
               targil_false AS false_targil
        FROM t_targil
        """
    )
    return cursor.fetchall()


def process_formula(cursor, targil_row):
    """
    מריץ נוסחה אחת (targil_row) על כל השורות ב-t_data,
    שומר תוצאות ב-t_results, ומוסיף שורה אחת ל-t_log.
    """

    targil_id = targil_row.targil_id
    targil = targil_row.targil
    tnai = targil_row.tnai
    false_targil = targil_row.false_targil

    print(f"Running {METHOD_NAME} for targil_id={targil_id}, formula={targil}")

    # מוחקים תוצאות קודמות לשיטה הזו (לנוסחה הזו)
    cursor.execute(
        """
        DELETE FROM t_results
        WHERE targil_id = ? AND method = ?
        """,
        targil_id, METHOD_NAME
    )

    # context בסיסי, נשתמש בו מחדש לכל שורה
    ctx = {
        "a": 0.0,
        "b": 0.0,
        "c": 0.0,
        "d": 0.0,
        # פונקציות מתמטיות
        "sqrt": math.sqrt,  # אם תכתבי sqrt(...) בנוסחה
        "SQRT": math.sqrt,  # אם תשתמשי באותיות גדולות
        "log": math.log,
        "LOG": math.log,
        "abs": abs,
        "ABS": abs,
        "POWER": math.pow,  # בשביל POWER(a, 2)
        # תנאי
        "IF": IF,
    }

    # מקמפלים את הנוסחאות פעם אחת
    compiled_targil = compile(targil, "<targil>", "eval")

    compiled_tnai = None
    compiled_false_targil = None

    if tnai is not None:
        compiled_tnai = compile(tnai, "<tnai>", "eval")
        if false_targil is not None:
            compiled_false_targil = compile(false_targil, "<false_targil>", "eval")

    start_time = time.time()

    # שולפים את כל הנתונים מתוך t_data
    cursor.execute("SELECT data_id, a, b, c, d FROM t_data")
    rows = cursor.fetchall()

    # במקום מיליון INSERT-ים – נצבור רשימה ונשתמש ב-executemany
    results_to_insert = []

    for data_id, a, b, c, d in rows:
        # מעדכנים את הערכים ב-context
        ctx["a"] = a
        ctx["b"] = b
        ctx["c"] = c
        ctx["d"] = d

        # בוחרים איזו נוסחה להריץ
        if compiled_tnai is None:
            formula_code = compiled_targil
        else:
            condition_result = eval(compiled_tnai, {}, ctx)
            if condition_result:
                formula_code = compiled_targil
            else:
                if compiled_false_targil is not None:
                    formula_code = compiled_false_targil
                else:
                    # 0 כקוד קבוע
                    formula_code = compile("0", "<default_false>", "eval")

        # מחשבים את התוצאה הסופית, עם טיפול בשגיאות מתמטיות
        try:
            result = eval(formula_code, {}, ctx)
        except ZeroDivisionError:
            # למשל: חלוקה ב-0 → נשמור NULL ב-DB
            result = None
        except ValueError:
            # למשל: sqrt שלילי, log לערך לא חוקי וכו'
            result = None

        # מוסיפים לרשימת התוצאות לצבירה
        results_to_insert.append(
            (data_id, targil_id, METHOD_NAME, result)
        )


    # עכשיו מבצעים INSERT מרוכז
    cursor.fast_executemany = True
    cursor.executemany(
        """
        INSERT INTO t_results (data_id, targil_id, method, result)
        VALUES (?, ?, ?, ?)
        """,
        results_to_insert
    )

    end_time = time.time()
    duration_seconds = end_time - start_time

    # מוסיפים שורה ללוג בטבלת t_log (שדה run_time)
    cursor.execute(
        """
        INSERT INTO t_log (targil_id, method, run_time)
        VALUES (?, ?, ?)
        """,
        targil_id, METHOD_NAME, duration_seconds
    )

    print(f"targil_id={targil_id} finished in {duration_seconds:.3f} seconds")
