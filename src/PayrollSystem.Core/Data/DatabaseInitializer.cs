using System;
using System.Globalization;
using System.IO;
using System.Data.SQLite;
using PayrollSystem.Core.Services;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Creates the SQLite database file (if missing), runs CREATE TABLE IF NOT EXISTS for
    /// Employees, PayrollRecords, and TaxBrackets, and seeds TaxBrackets with TaxBracketSeed.Default
    /// on first run. Safe to call repeatedly — DDL is idempotent and seed is guarded by SELECT COUNT.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Creates the database file at <paramref name="dbPath"/> if it does not exist,
        /// creates all three tables (idempotent), and seeds the TaxBrackets table on first run only.
        /// </summary>
        /// <param name="dbPath">Absolute path to the SQLite .db file, e.g. C:\App_Data\payroll.db</param>
        public static void EnsureCreated(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("dbPath is required", nameof(dbPath));

            // Make sure parent directory exists (App_Data may not exist on first start)
            string dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string connectionString = "Data Source=" + dbPath;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Enable foreign-key enforcement (SQLite default is OFF per connection)
                using (var pragma = conn.CreateCommand())
                {
                    pragma.CommandText = "PRAGMA foreign_keys = ON;";
                    pragma.ExecuteNonQuery();
                }

                CreateTables(conn);
                SeedTaxBracketsIfEmpty(conn);
            }
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            const string ddlEmployees = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName       TEXT    NOT NULL,
                    LastName        TEXT    NOT NULL,
                    Position        TEXT    NULL,
                    BaseHourlyRate  NUMERIC NOT NULL,
                    IsActive        INTEGER NOT NULL DEFAULT 1,
                    CreatedAt       TEXT    NOT NULL
                );";

            const string ddlPayrollRecords = @"
                CREATE TABLE IF NOT EXISTS PayrollRecords (
                    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeId          INTEGER NOT NULL,
                    PeriodLabel         TEXT    NOT NULL,
                    RegularHours        NUMERIC NOT NULL,
                    OvertimeHours       NUMERIC NOT NULL,
                    BaseRate            NUMERIC NOT NULL,
                    GrossPay            NUMERIC NOT NULL,
                    SocialContribution  NUMERIC NOT NULL,
                    TaxableIncome       NUMERIC NOT NULL,
                    IncomeTax           NUMERIC NOT NULL,
                    OtherDeductions     NUMERIC NOT NULL,
                    NetPay              NUMERIC NOT NULL,
                    CalculatedAt        TEXT    NOT NULL,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
                );";

            const string ddlTaxBrackets = @"
                CREATE TABLE IF NOT EXISTS TaxBrackets (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    LowerBound  NUMERIC NOT NULL,
                    UpperBound  NUMERIC NULL,
                    Rate        NUMERIC NOT NULL
                );";

            ExecuteNonQuery(conn, ddlEmployees);
            ExecuteNonQuery(conn, ddlPayrollRecords);
            ExecuteNonQuery(conn, ddlTaxBrackets);
        }

        private static void SeedTaxBracketsIfEmpty(SQLiteConnection conn)
        {
            // Idempotent guard — SC3 requires re-running EnsureCreated NOT to duplicate seed
            using (var count = conn.CreateCommand())
            {
                count.CommandText = "SELECT COUNT(*) FROM TaxBrackets;";
                long existing = (long)count.ExecuteScalar();
                if (existing > 0) return;
            }

            // Insert each row from TaxBracketSeed.Default with parameterised INSERTs
            const string sql = @"
                INSERT INTO TaxBrackets (LowerBound, UpperBound, Rate)
                VALUES (@lower, @upper, @rate);";

            foreach (var bracket in TaxBracketSeed.Default)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@lower", bracket.LowerBound);
                    cmd.Parameters.AddWithValue("@upper",
                        bracket.UpperBound.HasValue ? (object)bracket.UpperBound.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@rate", bracket.Rate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection conn, string sql)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
