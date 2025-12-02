# SQL Method (SQL_SP)

## 📂 קבצים

### `sp_run_formula_for_targil.sql`
**Stored Procedure** שמחשב נוסחה לפי `targil_id` בצורה דינמית.
התהליך כולל:
*  **בניית ביטוי:** יצירת הביטוי לחישוב (עם או בלי תנאי).
*  **חישוב:** הרצת הנוסחה על כל הרשומות בטבלת `t_data`.
*  **שמירה:** כתיבת התוצאות לטבלת `t_results`.
*  **לוגים:** מדידת זמן הריצה והוספת רשומה לטבלת `t_log`.

### `run_all_formulas_sql_sp.sql`
סקריפט המריץ את הפרוצדורה על כל הנוסחאות הקיימות בטבלת `t_targil` ברצף.