using System.Data;

namespace PayrollSystem.Core.Data
{
    /// <summary>
    /// Abstraction over connection creation so repositories can be unit-tested with fakes
    /// and the Web project can wire in a connection string from web.config in Sprint 3.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>Returns an OPEN connection. Caller owns disposal (use a <c>using</c> block).</summary>
        IDbConnection CreateOpenConnection();
    }
}
