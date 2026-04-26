using System;
using PayrollSystem.Core.Data;

namespace PayrollSystem.Web.App_Start
{
    /// <summary>
    /// Static facade over the three Sprint-2 repositories. Initialise once from Application_Start;
    /// every code-behind reads the static properties.
    /// </summary>
    public static class RepositoryFactory
    {
        private static IConnectionFactory _connectionFactory;
        private static IPayrollRepository _payroll;
        private static IEmployeeRepository _employees;
        private static ITaxBracketRepository _taxBrackets;

        /// <summary>Called once from Global.Application_Start.</summary>
        public static void Initialize(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is required", nameof(connectionString));

            _connectionFactory = new SqliteConnectionFactory(connectionString);

            // ORDER MATTERS: PayrollRepository first (no deps),
            // then EmployeeRepository (needs IPayrollRepository for the delete guard),
            // then TaxBracketRepository (no deps).
            _payroll      = new PayrollRepository(_connectionFactory);
            _employees    = new EmployeeRepository(_connectionFactory, _payroll);
            _taxBrackets  = new TaxBracketRepository(_connectionFactory);
        }

        public static IEmployeeRepository   Employees   => _employees;
        public static IPayrollRepository    Payroll     => _payroll;
        public static ITaxBracketRepository TaxBrackets => _taxBrackets;
    }
}
