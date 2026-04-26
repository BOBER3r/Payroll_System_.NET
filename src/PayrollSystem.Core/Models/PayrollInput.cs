namespace PayrollSystem.Core.Models
{
    /// <summary>
    /// Data transfer object carrying all inputs needed to compute a payroll run for one employee.
    /// </summary>
    public class PayrollInput
    {
        public int EmployeeId { get; set; }

        /// <summary>Human-readable period label, e.g. "2026-04".</summary>
        public string PeriodLabel { get; set; }

        /// <summary>Regular hours worked during the period (decimal to allow half-hours).</summary>
        public decimal RegularHours { get; set; }

        /// <summary>Overtime hours worked during the period.</summary>
        public decimal OvertimeHours { get; set; }

        /// <summary>Snapshot of the employee's hourly rate at calculation time.</summary>
        public decimal BaseRate { get; set; }

        /// <summary>Flat deduction amount (e.g. garnishments, voluntary deductions). Defaults to 0.</summary>
        public decimal OtherDeductions { get; set; } = 0m;
    }
}
