using System;
using System.Collections.Generic;
using System.Linq;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Services
{
    /// <summary>
    /// Computes payroll results from hours, rate, and a progressive tax bracket table.
    ///
    /// WORKED EXAMPLE (sanity check — keep in sync with the algorithm below):
    ///   Input:  RegularHours = 160, OvertimeHours = 10, BaseRate = 20.00, OtherDeductions = 50.00
    ///   Brackets (TaxBracketSeed.Default): [0,1000)@0%, [1000,3000)@10%, [3000,7000)@18%, [7000,null)@25%
    ///
    ///   Step 1 — GrossPay:
    ///     regular   = 160 * 20.00          = 3200.00
    ///     overtime  = 10  * 20.00 * 1.5    =  300.00
    ///     gross     = 3200.00 + 300.00     = 3500.00
    ///
    ///   Step 2 — SocialContribution (flat 13.78% of gross, rounded to 2 dp AwayFromZero):
    ///     social    = 3500.00 * 0.1378     =  482.30  (482.2999999... rounded to 482.30)
    ///
    ///   Step 3 — TaxableIncome:
    ///     taxable   = 3500.00 - 482.30     = 3017.70
    ///
    ///   Step 4 — IncomeTax (bracket-by-bracket on taxable = 3017.70):
    ///     [0,1000)    overlap = 1000 - 0       = 1000.00  x 0%   = 0.00
    ///     [1000,3000) overlap = 3000 - 1000    = 2000.00  x 10%  = 200.00
    ///     [3000,7000) overlap = 3017.70 - 3000 =   17.70  x 18%  = 3.186
    ///     [7000,null) taxable (3017.70) &lt;= lower (7000) -> skip  = 0.00
    ///     tax (rounded 2 dp AwayFromZero): 0.00 + 200.00 + 3.186 = 203.19
    ///
    ///   Step 5 — NetPay:
    ///     net = gross - social - tax - other
    ///         = 3500.00 - 482.30 - 203.19 - 50.00 = 2764.51
    ///
    ///   Expected result: Gross=3500.00, Social=482.30, Taxable=3017.70, Tax=203.19, Net=2764.51
    /// </summary>
    public sealed class PayrollCalculator : IPayrollCalculator
    {
        private const decimal OvertimeMultiplier = 1.5m;
        private const decimal SocialContributionRate = 0.1378m;

        /// <summary>
        /// Calculates a complete payroll breakdown for the given input and tax bracket table.
        /// </summary>
        /// <param name="input">Hours, rate, and deduction data for this pay period.</param>
        /// <param name="brackets">Progressive tax brackets; need not be pre-sorted.</param>
        /// <returns>A fully populated <see cref="PayrollCalculationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if input or brackets is null.</exception>
        public PayrollCalculationResult Calculate(PayrollInput input, IEnumerable<TaxBracket> brackets)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (brackets == null) throw new ArgumentNullException(nameof(brackets));

            // Step 1: Gross Pay = regular pay + overtime pay (OT at 1.5x rate)
            decimal gross = Round2(
                input.RegularHours * input.BaseRate
                + input.OvertimeHours * input.BaseRate * OvertimeMultiplier);

            // Step 2: Social Contribution = flat 13.78% of gross, rounded to 2 decimal places
            decimal social = Round2(gross * SocialContributionRate);

            // Step 3: Taxable Income = gross minus social contribution (floor at zero)
            decimal taxable = gross - social;
            if (taxable < 0m)
                taxable = 0m;

            // Step 4: Income Tax — iterate brackets in ascending LowerBound order
            // For each bracket where taxable > LowerBound, apply the rate to the overlap slice.
            decimal tax = 0m;
            foreach (TaxBracket bracket in brackets.OrderBy(b => b.LowerBound))
            {
                if (taxable <= bracket.LowerBound)
                    continue; // no portion of taxable income falls in this bracket

                decimal upper = bracket.UpperBound ?? decimal.MaxValue; // null = +infinity
                decimal cap   = Math.Min(taxable, upper);
                decimal slice = cap - bracket.LowerBound;

                if (slice > 0m)
                    tax += slice * bracket.Rate;
            }
            tax = Round2(tax);

            // Step 5: Net Pay
            decimal net = gross - social - tax - input.OtherDeductions;

            return new PayrollCalculationResult
            {
                GrossPay           = gross,
                SocialContribution = social,
                TaxableIncome      = taxable,
                IncomeTax          = tax,
                OtherDeductions    = input.OtherDeductions,
                NetPay             = net
            };
        }

        private static decimal Round2(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
