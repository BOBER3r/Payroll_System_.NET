namespace PayrollSystem.Core.Models
{
    public class TaxBracket
    {
        public int Id { get; set; }

        /// <summary>Inclusive lower bound of the bracket (in currency units).</summary>
        public decimal LowerBound { get; set; }

        /// <summary>Exclusive upper bound of the bracket. Null means +infinity (top bracket).</summary>
        public decimal? UpperBound { get; set; }

        /// <summary>Tax rate as a fraction, e.g. 0.10m for 10%.</summary>
        public decimal Rate { get; set; }
    }
}
