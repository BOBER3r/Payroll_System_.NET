using System;
using System.Configuration;
using System.IO;
using System.Web;
using PayrollSystem.Core.Data;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Resolve App_Data to a real filesystem path (works under IIS + IIS Express)
            string appDataPath = Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            // Make the |DataDirectory| token resolve correctly for any provider that DOES expand it
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);

            // Microsoft.Data.Sqlite does NOT expand |DataDirectory| — build the real path ourselves
            string dbPath = Path.Combine(appDataPath, "payroll.db");
            string connectionString = "Data Source=" + dbPath;

            // Create tables + seed tax brackets on first run (idempotent)
            DatabaseInitializer.EnsureCreated(dbPath);

            // Wire up the static repository factory used by every code-behind
            RepositoryFactory.Initialize(connectionString);
        }
    }
}
