using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Data access contract for PayrollRecord persistence.
    /// </summary>
    public interface IPayrollRepository
    {
        /// <summary>Persists a PayrollRecord and sets record.Id to the new row Id. Returns the new Id.</summary>
        int Insert(PayrollRecord record);

        /// <summary>Returns the PayrollRecord with the given Id, or null if not found.</summary>
        PayrollRecord GetById(int id);

        /// <summary>Returns all PayrollRecords for a given employee, ordered by CalculatedAt DESC.</summary>
        IList<PayrollRecord> GetByEmployee(int employeeId);

        /// <summary>
        /// Returns PayrollRecords matching the optional filters, ordered by CalculatedAt DESC.
        /// All filter parameters are optional — passing all nulls returns every record.
        /// </summary>
        IList<PayrollRecord> Search(int? employeeId, DateTime? fromDate, DateTime? toDate);
    }

    /// <summary>
    /// ADO.NET repository for PayrollRecords using Microsoft.Data.Sqlite.
    /// All SQL uses @-named parameters — no string concatenation of user values.
    /// </summary>
    public sealed class PayrollRepository : IPayrollRepository
    {
        private readonly IConnectionFactory _factory;

        public PayrollRepository(IConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Inserts all 12 data columns of the PayrollRecord into PayrollRecords table.
        /// Sets record.Id to the new AUTOINCREMENT row id and returns it.
        /// </summary>
        public int Insert(PayrollRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            const string sql = @"
                INSERT INTO PayrollRecords (
                    EmployeeId, PeriodLabel, RegularHours, OvertimeHours, BaseRate,
                    GrossPay, SocialContribution, TaxableIncome, IncomeTax,
                    OtherDeductions, NetPay, CalculatedAt
                ) VALUES (
                    @employeeId, @periodLabel, @regularHours, @overtimeHours, @baseRate,
                    @grossPay, @socialContribution, @taxableIncome, @incomeTax,
                    @otherDeductions, @netPay, @calculatedAt
                );
                SELECT last_insert_rowid();";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@employeeId",         record.EmployeeId);
                cmd.Parameters.AddWithValue("@periodLabel",        record.PeriodLabel ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@regularHours",       record.RegularHours);
                cmd.Parameters.AddWithValue("@overtimeHours",      record.OvertimeHours);
                cmd.Parameters.AddWithValue("@baseRate",           record.BaseRate);
                cmd.Parameters.AddWithValue("@grossPay",           record.GrossPay);
                cmd.Parameters.AddWithValue("@socialContribution", record.SocialContribution);
                cmd.Parameters.AddWithValue("@taxableIncome",      record.TaxableIncome);
                cmd.Parameters.AddWithValue("@incomeTax",          record.IncomeTax);
                cmd.Parameters.AddWithValue("@otherDeductions",    record.OtherDeductions);
                cmd.Parameters.AddWithValue("@netPay",             record.NetPay);
                cmd.Parameters.AddWithValue("@calculatedAt",
                    record.CalculatedAt.ToString("O", CultureInfo.InvariantCulture));

                long newId = (long)cmd.ExecuteScalar();
                record.Id = (int)newId;
                return record.Id;
            }
        }

        /// <summary>Returns the PayrollRecord matching <paramref name="id"/>, or null if not found.</summary>
        public PayrollRecord GetById(int id)
        {
            const string sql = @"
                SELECT Id, EmployeeId, PeriodLabel, RegularHours, OvertimeHours, BaseRate,
                       GrossPay, SocialContribution, TaxableIncome, IncomeTax,
                       OtherDeductions, NetPay, CalculatedAt
                FROM   PayrollRecords
                WHERE  Id = @id;";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRecord(reader);
                }
            }
            return null;
        }

        /// <summary>Returns all records for the given employee ordered by CalculatedAt DESC.</summary>
        public IList<PayrollRecord> GetByEmployee(int employeeId)
        {
            const string sql = @"
                SELECT Id, EmployeeId, PeriodLabel, RegularHours, OvertimeHours, BaseRate,
                       GrossPay, SocialContribution, TaxableIncome, IncomeTax,
                       OtherDeductions, NetPay, CalculatedAt
                FROM   PayrollRecords
                WHERE  EmployeeId = @employeeId
                ORDER  BY CalculatedAt DESC;";

            var results = new List<PayrollRecord>();
            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@employeeId", employeeId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        results.Add(MapRecord(reader));
                }
            }
            return results;
        }

        /// <summary>
        /// Returns records matching optional filters. All filter parameters are optional.
        /// WHERE clauses are built from column-name constants only — user values go into @parameters.
        /// </summary>
        public IList<PayrollRecord> Search(int? employeeId, DateTime? fromDate, DateTime? toDate)
        {
            var where = new System.Collections.Generic.List<string>();

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                if (employeeId.HasValue)
                {
                    where.Add("EmployeeId = @employeeId");
                    cmd.Parameters.AddWithValue("@employeeId", employeeId.Value);
                }
                if (fromDate.HasValue)
                {
                    where.Add("CalculatedAt >= @fromDate");
                    cmd.Parameters.AddWithValue("@fromDate",
                        fromDate.Value.ToString("O", CultureInfo.InvariantCulture));
                }
                if (toDate.HasValue)
                {
                    where.Add("CalculatedAt <= @toDate");
                    cmd.Parameters.AddWithValue("@toDate",
                        toDate.Value.ToString("O", CultureInfo.InvariantCulture));
                }

                string whereClause = where.Count > 0
                    ? "WHERE " + string.Join(" AND ", where)
                    : string.Empty;

                cmd.CommandText = @"
                    SELECT Id, EmployeeId, PeriodLabel, RegularHours, OvertimeHours, BaseRate,
                           GrossPay, SocialContribution, TaxableIncome, IncomeTax,
                           OtherDeductions, NetPay, CalculatedAt
                    FROM   PayrollRecords "
                    + whereClause
                    + " ORDER BY CalculatedAt DESC;";

                var results = new List<PayrollRecord>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        results.Add(MapRecord(reader));
                }
                return results;
            }
        }

        private static PayrollRecord MapRecord(SqliteDataReader reader)
        {
            return new PayrollRecord
            {
                Id                 = reader.GetInt32(0),
                EmployeeId         = reader.GetInt32(1),
                PeriodLabel        = reader.IsDBNull(2) ? null : reader.GetString(2),
                RegularHours       = reader.GetDecimal(3),
                OvertimeHours      = reader.GetDecimal(4),
                BaseRate           = reader.GetDecimal(5),
                GrossPay           = reader.GetDecimal(6),
                SocialContribution = reader.GetDecimal(7),
                TaxableIncome      = reader.GetDecimal(8),
                IncomeTax          = reader.GetDecimal(9),
                OtherDeductions    = reader.GetDecimal(10),
                NetPay             = reader.GetDecimal(11),
                CalculatedAt       = DateTime.Parse(
                    reader.GetString(12),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind)
            };
        }
    }
}
