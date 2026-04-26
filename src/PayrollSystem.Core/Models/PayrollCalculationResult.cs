namespace PayrollSystem.Core.Models
{
    /// <summary>
    /// Holds the fully-computed breakdown of a single payroll calculation.
    /// All monetary values are in the same currency unit as the inputs (decimal precision).
    /// </summary>
    public class PayrollCalculationResult
    {
        public decimal GrossPay { get; set; }
        public decimal SocialContribution { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal NetPay { get; set; }
    }
}
