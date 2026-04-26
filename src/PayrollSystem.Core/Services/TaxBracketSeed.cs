using System.Collections.Generic;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Services
{
    /// <summary>
    /// Canonical default progressive tax bracket table.
    /// Sprint 2 will seed these into the SQLite TaxBrackets table on first run.
    /// </summary>
    public static class TaxBracketSeed
    {
        public static IReadOnlyList<TaxBracket> Default => new List<TaxBracket>
        {
            new TaxBracket { LowerBound = 0m,    UpperBound = 1000m, Rate = 0.00m },
            new TaxBracket { LowerBound = 1000m, UpperBound = 3000m, Rate = 0.10m },
            new TaxBracket { LowerBound = 3000m, UpperBound = 7000m, Rate = 0.18m },
            new TaxBracket { LowerBound = 7000m, UpperBound = null,  Rate = 0.25m }
        };
    }
}
