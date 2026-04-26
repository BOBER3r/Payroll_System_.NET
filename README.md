# Payroll Calculation System

HR clerk web app for managing employees and calculating progressive-tax payroll.
Built with C# / ASP.NET Web Forms (.NET Framework 4.8) and SQLite.

## Solution Layout

- `PayrollSystem.sln` — Visual Studio 2022 solution
- `src/PayrollSystem.Core/` — .NET Standard 2.0 class library
  (domain models, payroll calculator, tax brackets, repositories)
- `src/PayrollSystem.Web/` — ASP.NET Web Forms project (.NET Framework 4.8)
  (added in Sprint 3)

## Build & Run (Windows only)

The Web Forms tier requires the full .NET Framework, so this app must be
built and run on Windows in Visual Studio. macOS / Linux are not supported
for the web tier.

1. Install **Visual Studio 2019 or 2022** with the following workloads:
   - ASP.NET and web development
   - .NET desktop development
2. Install the **.NET Framework 4.8 Developer Pack / Targeting Pack**
   (Visual Studio Installer -> Individual components).
3. Clone the repository and open `PayrollSystem.sln` in Visual Studio.
4. Right-click the solution -> **Restore NuGet Packages**.
5. Right-click `PayrollSystem.Web` -> **Set as Startup Project**.
6. Press **F5** to launch under IIS Express.

The SQLite database file is created on first request under
`PayrollSystem.Web/App_Data/payroll.db` and seeded with the default
progressive tax brackets.

## Payroll Calculation Summary

The `PayrollSystem.Core` library implements a progressive-tax payroll engine:

- **Gross Pay** = `RegularHours * BaseRate + OvertimeHours * BaseRate * 1.5`
- **Social Contribution** = `GrossPay * 13.78%` (rounded to 2 decimal places)
- **Taxable Income** = `GrossPay - SocialContribution`
- **Income Tax** = bracket-based progressive tax (see `TaxBracketSeed.Default`)
- **Net Pay** = `GrossPay - SocialContribution - IncomeTax - OtherDeductions`

Default tax brackets:

| Range          | Rate |
|----------------|------|
| 0 – 1,000      | 0%   |
| 1,000 – 3,000  | 10%  |
| 3,000 – 7,000  | 18%  |
| 7,000+         | 25%  |

## Sprint Status

This README will grow as sprints land. See `.bober/progress.md` for the
detailed sprint log.

| Sprint | Status | Description |
|--------|--------|-------------|
| 1      | Done   | Solution scaffolding, domain models, payroll calculator |
| 2      | Pending | SQLite repositories |
| 3      | Pending | ASP.NET Web Forms project + pages |
| 4      | Pending | Payroll entry UI (Web Forms) |
| 5      | Pending | Tax bracket admin UI |
