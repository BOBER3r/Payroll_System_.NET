using System;
using System.Data;
using System.Data.SQLite;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Creates and opens a SQLite connection from a given connection string.
    /// Wired by Sprint 3's Global.asax using the connection string from web.config.
    /// </summary>
    public sealed class SqliteConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <param name="connectionString">e.g. "Data Source=C:\path\App_Data\payroll.db"</param>
        public SqliteConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is required", nameof(connectionString));
            _connectionString = connectionString;
        }

        /// <summary>Returns an OPEN SQLiteConnection. Caller owns disposal.</summary>
        public IDbConnection CreateOpenConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
