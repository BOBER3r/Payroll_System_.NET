using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PayrollSystem.Core.Models;

namespace PayrollSystem.Core.Services
{
    /// <summary>
    /// Pure CSV serialiser for PayrollRecord lists. UI-agnostic (returns a string).
    /// RFC 4180 escaping: any field containing comma, quote, CR, or LF is wrapped
    /// in double quotes with embedded quotes doubled. Lines joined with CRLF.
    /// </summary>
    public static class CsvExporter
    {
        private const string Header =
            "Id,EmployeeName,PeriodLabel,RegularHours,OvertimeHours,BaseRate,GrossPay,SocialContribution,TaxableIncome,IncomeTax,OtherDeductions,NetPay,CalculatedAt";

        public static string Export(IEnumerable<PayrollRecord> records,
                                    IDictionary<int, string> employeeNames)
        {
            var sb = new StringBuilder();
            sb.Append(Header);

            foreach (var r in records)
            {
                string name;
                if (!employeeNames.TryGetValue(r.EmployeeId, out name))
                    name = string.Empty;

                sb.Append("\r\n");
                sb.Append(Escape(r.Id.ToString(CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(name));
                sb.Append(",");
                sb.Append(Escape(r.PeriodLabel ?? string.Empty));
                sb.Append(",");
                sb.Append(Escape(r.RegularHours.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.OvertimeHours.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.BaseRate.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.GrossPay.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.SocialContribution.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.TaxableIncome.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.IncomeTax.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.OtherDeductions.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.NetPay.ToString("F2", CultureInfo.InvariantCulture)));
                sb.Append(",");
                sb.Append(Escape(r.CalculatedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));
            }

            return sb.ToString();
        }

        private static string Escape(string value)
        {
            if (value == null) value = string.Empty;

            // RFC 4180: wrap in quotes if value contains comma, double-quote, CR, or LF
            if (value.IndexOf(',') >= 0 || value.IndexOf('"') >= 0
                || value.IndexOf('\r') >= 0 || value.IndexOf('\n') >= 0)
            {
                // Double any embedded double-quotes
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
