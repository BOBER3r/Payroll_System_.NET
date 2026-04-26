using System;

namespace PayrollSystem.Core.Models
{
    /// <summary>
    /// Persisted snapshot of a single payroll calculation. One row per (employee, period) calculation.
    /// All money values are decimal; CalculatedAt is DateTime stored as ISO-8601 TEXT in SQLite.
    /// </summary>
    public class PayrollRecord
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string PeriodLabel { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal BaseRate { get; set; }
        public decimal GrossPay { get; set; }
        public decimal SocialContribution { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
