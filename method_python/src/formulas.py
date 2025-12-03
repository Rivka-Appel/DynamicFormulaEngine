"""
Dynamic formula evaluation in Python.

Loads formulas from t_targil, evaluates them on all rows in t_data,
and writes results to t_results and t_log.
"""

import time
import math

METHOD_NAME = "PYTHON"


def IF(cond, x, y):
    """Simple conditional helper: IF(cond, x, y)."""
    return x if cond else y


def get_formulas(cursor):
    """Return all formulas from t_targil."""
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
    """Run a single formula on all rows in t_data and log results + duration."""
    targil_id = targil_row.targil_id
    targil = targil_row.targil
    tnai = targil_row.tnai
    false_targil = targil_row.false_targil

    print(f"Running {METHOD_NAME} for targil_id={targil_id}, formula={targil}")

    # Clear previous results for this formula and method
    cursor.execute(
        """
        DELETE FROM t_results
        WHERE targil_id = ? AND method = ?
        """,
        targil_id, METHOD_NAME
    )

    # Base context reused for each row
    ctx = {
        "a": 0.0,
        "b": 0.0,
        "c": 0.0,
        "d": 0.0,
        # math functions
        "sqrt": math.sqrt,
        "SQRT": math.sqrt,
        "log": math.log,
        "LOG": math.log,
        "abs": abs,
        "ABS": abs,
        "POWER": math.pow,
        # conditional
        "IF": IF,
    }

    # Compile formulas once
    compiled_targil = compile(targil, "<targil>", "eval")

    compiled_tnai = None
    compiled_false_targil = None

    if tnai is not None:
        compiled_tnai = compile(tnai, "<tnai>", "eval")
        if false_targil is not None:
            compiled_false_targil = compile(false_targil, "<false_targil>", "eval")

    start_time = time.time()

    # Load all data rows
    cursor.execute("SELECT data_id, a, b, c, d FROM t_data")
    rows = cursor.fetchall()

    # Collect results and insert in batch
    results_to_insert = []

    for data_id, a, b, c, d in rows:
        ctx["a"] = a
        ctx["b"] = b
        ctx["c"] = c
        ctx["d"] = d

        # Choose which formula to run
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
                    formula_code = compile("0", "<default_false>", "eval")

        # Evaluate with basic error handling
        try:
            result = eval(formula_code, {}, ctx)
        except ZeroDivisionError:
            result = None
        except ValueError:
            result = None

        results_to_insert.append(
            (data_id, targil_id, METHOD_NAME, result)
        )

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

    cursor.execute(
        """
        INSERT INTO t_log (targil_id, method, run_time)
        VALUES (?, ?, ?)
        """,
        targil_id, METHOD_NAME, duration_seconds
    )

    print(f"targil_id={targil_id} finished in {duration_seconds:.3f} seconds")
