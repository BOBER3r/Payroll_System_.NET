using System;
using System.Collections.Generic;
using System.Data.SQLite;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Data access contract for TaxBracket persistence.
    /// </summary>
    public interface ITaxBracketRepository
    {
        /// <summary>Returns all tax brackets ordered by LowerBound ASC (ready for PayrollCalculator.Calculate).</summary>
        IList<TaxBracket> GetAll();

        /// <summary>Updates a tax bracket's LowerBound, UpperBound, and Rate by Id.</summary>
        void Update(TaxBracket bracket);
    }

    /// <summary>
    /// ADO.NET repository for TaxBrackets using System.Data.SQLite.
    /// All SQL uses @-named parameters — no string concatenation of user values.
    /// </summary>
    public sealed class TaxBracketRepository : ITaxBracketRepository
    {
        private readonly IConnectionFactory _factory;

        public TaxBracketRepository(IConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Returns all tax brackets ordered by LowerBound ASC.
        /// Compatible with IEnumerable&lt;TaxBracket&gt; accepted by PayrollCalculator.Calculate.
        /// UpperBound is read with IsDBNull guard since the top bracket has no upper limit.
        /// </summary>
        public IList<TaxBracket> GetAll()
        {
            const string sql = @"
                SELECT Id, LowerBound, UpperBound, Rate
                FROM   TaxBrackets
                ORDER  BY LowerBound ASC;";

            var results = new List<TaxBracket>();
            using (var conn = (SQLiteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new TaxBracket
                        {
                            Id         = reader.GetInt32(0),
                            LowerBound = reader.GetDecimal(1),
                            UpperBound = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                            Rate       = reader.GetDecimal(3)
                        });
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Updates LowerBound, UpperBound, and Rate for an existing tax bracket row.
        /// Null UpperBound is written as DBNull.Value (not C# null) to store SQL NULL.
        /// </summary>
        public void Update(TaxBracket bracket)
        {
            if (bracket == null) throw new ArgumentNullException(nameof(bracket));

            const string sql = @"
                UPDATE TaxBrackets
                SET    LowerBound = @lower,
                       UpperBound = @upper,
                       Rate       = @rate
                WHERE  Id = @id;";

            using (var conn = (SQLiteConnection)_factory.CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id",    bracket.Id);
                cmd.Parameters.AddWithValue("@lower", bracket.LowerBound);
                cmd.Parameters.AddWithValue("@upper",
                    bracket.UpperBound.HasValue ? (object)bracket.UpperBound.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@rate",  bracket.Rate);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
