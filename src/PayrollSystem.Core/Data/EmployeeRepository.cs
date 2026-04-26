using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Data access contract for Employee persistence.
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>Returns all employees ordered by LastName, FirstName.</summary>
        IList<Employee> GetAll();

        /// <summary>Returns the employee with the given Id, or null if not found.</summary>
        Employee GetById(int id);

        /// <summary>Inserts an employee, sets employee.Id to the new row Id, and returns the new Id.</summary>
        int Insert(Employee employee);

        /// <summary>Updates all mutable fields of an existing employee row.</summary>
        void Update(Employee employee);

        /// <summary>
        /// Deletes the employee with the given Id.
        /// Throws <see cref="InvalidOperationException"/> with message
        /// "Cannot delete employee with existing payroll records." if any PayrollRecords exist for this employee.
        /// </summary>
        void Delete(int id);
    }

    /// <summary>
    /// ADO.NET repository for Employees using Microsoft.Data.Sqlite.
    /// All SQL uses @-named parameters — no string concatenation of user values.
    /// </summary>
    public sealed class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConnectionFactory _factory;
        private readonly IPayrollRepository _payrollRepository;

        public EmployeeRepository(IConnectionFactory factory, IPayrollRepository payrollRepository)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _payrollRepository = payrollRepository ?? throw new ArgumentNullException(nameof(payrollRepository));
        }

        /// <summary>Returns all employees ordered by LastName ASC, FirstName ASC.</summary>
        public IList<Employee> GetAll()
        {
            const string sql = @"
                SELECT Id, FirstName, LastName, Position, BaseHourlyRate, IsActive, CreatedAt
                FROM   Employees
                ORDER  BY LastName, FirstName;";

            var results = new List<Employee>();
            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        results.Add(MapEmployee(reader));
                }
            }
            return results;
        }

        /// <summary>Returns the employee matching <paramref name="id"/>, or null if not found.</summary>
        public Employee GetById(int id)
        {
            const string sql = @"
                SELECT Id, FirstName, LastName, Position, BaseHourlyRate, IsActive, CreatedAt
                FROM   Employees
                WHERE  Id = @id;";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapEmployee(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// Inserts a new employee row. Sets employee.Id to the new AUTOINCREMENT Id and returns it.
        /// CreatedAt is stored as ISO-8601 TEXT using the round-trip format specifier.
        /// IsActive is stored as INTEGER (1 = true, 0 = false).
        /// </summary>
        public int Insert(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            const string sql = @"
                INSERT INTO Employees (FirstName, LastName, Position, BaseHourlyRate, IsActive, CreatedAt)
                VALUES (@firstName, @lastName, @position, @rate, @active, @createdAt);
                SELECT last_insert_rowid();";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@firstName", employee.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lastName",  employee.LastName  ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@position",  employee.Position  ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@rate",      employee.BaseHourlyRate);
                cmd.Parameters.AddWithValue("@active",    employee.IsActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@createdAt",
                    employee.CreatedAt.ToString("O", CultureInfo.InvariantCulture));

                long newId = (long)cmd.ExecuteScalar();
                employee.Id = (int)newId;
                return employee.Id;
            }
        }

        /// <summary>
        /// Updates all mutable fields (FirstName, LastName, Position, BaseHourlyRate, IsActive) for an existing employee.
        /// CreatedAt is intentionally not updated — it is set at insert time only.
        /// </summary>
        public void Update(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            const string sql = @"
                UPDATE Employees
                SET    FirstName      = @firstName,
                       LastName       = @lastName,
                       Position       = @position,
                       BaseHourlyRate = @rate,
                       IsActive       = @active
                WHERE  Id = @id;";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@firstName", employee.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lastName",  employee.LastName  ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@position",  employee.Position  ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@rate",      employee.BaseHourlyRate);
                cmd.Parameters.AddWithValue("@active",    employee.IsActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@id",        employee.Id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the employee with the given <paramref name="id"/>.
        /// Throws <see cref="InvalidOperationException"/> if the employee has any payroll records.
        /// </summary>
        public void Delete(int id)
        {
            // Guard: block delete if employee has payroll records (Sprint 3 displays this message verbatim)
            var existingRecords = _payrollRepository.GetByEmployee(id);
            if (existingRecords != null && existingRecords.Count > 0)
                throw new InvalidOperationException("Cannot delete employee with existing payroll records.");

            const string sql = "DELETE FROM Employees WHERE Id = @id;";

            using (var conn = (SqliteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private static Employee MapEmployee(SqliteDataReader reader)
        {
            return new Employee
            {
                Id             = reader.GetInt32(0),
                FirstName      = reader.IsDBNull(1) ? null : reader.GetString(1),
                LastName       = reader.IsDBNull(2) ? null : reader.GetString(2),
                Position       = reader.IsDBNull(3) ? null : reader.GetString(3),
                BaseHourlyRate = reader.GetDecimal(4),
                IsActive       = reader.GetInt32(5) != 0,
                CreatedAt      = DateTime.Parse(
                    reader.GetString(6),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind)
            };
        }
    }
}
