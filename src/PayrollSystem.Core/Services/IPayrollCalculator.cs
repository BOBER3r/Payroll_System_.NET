using System.Collections.Generic;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Services
{
    public interface IPayrollCalculator
    {
        PayrollCalculationResult Calculate(PayrollInput input, IEnumerable<TaxBracket> brackets);
    }
}
