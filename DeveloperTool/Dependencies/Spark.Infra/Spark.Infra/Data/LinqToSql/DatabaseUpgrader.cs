using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spark.Infra.Data.LinqToSql
{
    
    public abstract class DatabaseUpgrader<TDataContext, TUpgradeScriptTable>
        where TDataContext : IDbDataContext
        where TUpgradeScriptTable : class, IUpgradeScriptTableRecord, new()
    {


        protected abstract string GetUpgradeScriptsResourcePath();
        protected abstract Table<TUpgradeScriptTable> GetUpgradeScriptTable(TDataContext dataContext);


        public void UpgradeWithRetry(TDataContext dataContext, TimeSpan retryInterval, Func<bool> cancelCallback, Action<Exception> logException)
        {
            bool exceptionLogged = false;
            while (!cancelCallback())
            {
                try
                {
                    Upgrade(dataContext);
                    return;
                }
                catch (Exception ex)
                {
                    if (!exceptionLogged)
                    {
                        logException(ex);
                        exceptionLogged = true;
                    }
                    Thread.Sleep(retryInterval);
                }
            }
        }

        public void Upgrade(TDataContext dataContext)
        {
            bool shouldCloseConnection = false;
            if (dataContext.ConnectionState != System.Data.ConnectionState.Open)
            {
                dataContext.OpenConnection();
                shouldCloseConnection = true;
            }

            try
            {
                OnBeforeUpgrade(dataContext);

                using (var tx = dataContext.BeginTransaction())
                {
                    
                    CreateUpgradeScriptsTableIfNotExists(dataContext);

                    var assembly = typeof(TDataContext).Assembly;
                    var resourceNames = assembly.GetManifestResourceNames()
                                                .Where(n => n.StartsWith(UpgradeScriptsResourcePath))
                                                .ToArray();

                    if (resourceNames.Length > 0)
                    {

                        var alreadyRunnedScript = GetAlreadyRunnedScripts(dataContext, resourceNames);


                        foreach (var scriptToRun in resourceNames.Where(s => !alreadyRunnedScript.Contains(s)).OrderBy(n => n))
                        {
                            using (var stream = assembly.GetManifestResourceStream(scriptToRun))
                            using (var streamReader = new StreamReader(stream, Encoding.Unicode))
                            {

                                RunScript(scriptToRun, dataContext, streamReader.ReadToEnd());
                            }
                        }

                        dataContext.SubmitChanges();
                    }

                    tx.Commit();
                }

                OnAfterUpgrade(dataContext);
            }
            finally
            {
                if (shouldCloseConnection)
                    dataContext.CloseConnection();
            }

        }

        protected virtual void OnBeforeUpgrade(TDataContext dbContext)
        {

        }

        protected virtual void OnAfterUpgrade(TDataContext dbContext)
        {

        }

        private string[] GetAlreadyRunnedScripts(TDataContext dataCotnext, string[] resourceNames)
        {
            var alreadyRunnedScript = GetUpgradeScriptTable(dataCotnext)
                                           .Where(us => resourceNames.Select(s => s.Replace(UpgradeScriptsResourcePath, ""))
                                                                     .ToArray()
                                                                     .Contains(us.ScriptName))
                                           .Select(row => row.ScriptName)
                                           .ToArray()
                                           .Select(scriptName => UpgradeScriptsResourcePath + scriptName)
                                           .ToArray();
            return alreadyRunnedScript;
        }

        private string UpgradeScriptsResourcePath
        {
            get
            {
                string path = GetUpgradeScriptsResourcePath();
                if (!path.EndsWith("."))
                    path = path + ".";

                return path;
            }
        }

        private void RunScript(string scriptName, TDataContext dbContext, string sqlScript)
        {
            System.Text.RegularExpressions.Regex expr = CreateRegularExpression();
            string[] allCommands = expr.Split(sqlScript);

            foreach (var cmdText in allCommands)
            {
                if (string.IsNullOrEmpty(cmdText) || cmdText.Trim().Length == 0)
                    continue;

                dbContext.ExecuteCommand(cmdText);

            }

            GetUpgradeScriptTable(dbContext).InsertOnSubmit(new TUpgradeScriptTable()
            {
                RunDateTime = DateTime.Now,
                ScriptContent = sqlScript,
                ScriptName = scriptName.Replace(UpgradeScriptsResourcePath, string.Empty)
            });
        }


        private System.Text.RegularExpressions.Regex _regEx;
        private System.Text.RegularExpressions.Regex CreateRegularExpression()
        {
            if (_regEx == null)
                _regEx = new System.Text.RegularExpressions.Regex(@"\bGO\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return _regEx;
        }


        private void CreateUpgradeScriptsTableIfNotExists(TDataContext ggpVersioningDB)
        {
            var countResult = ggpVersioningDB.ExecuteQuery<int>("SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UpgradeScripts'")
                                                                .ToArray()
                                                                .First();
            if (0 < countResult)
            {
                return;
            }

            ggpVersioningDB.ExecuteCommand(
                @"CREATE TABLE [dbo].[UpgradeScripts](
	                                    [Script_ID] [int] IDENTITY(1,1) NOT NULL,
	                                    [ScriptName] [nvarchar](50) NOT NULL,
	                                    [RunDateTime] [datetime] NOT NULL,
	                                    [ScriptContent] [ntext] NULL,
                                     CONSTRAINT [PK_UpgradeScripts] PRIMARY KEY CLUSTERED 
                                    (
	                                    [Script_ID] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");


            ggpVersioningDB.ExecuteCommand(@"CREATE UNIQUE NONCLUSTERED INDEX [ScriptName] ON [dbo].[UpgradeScripts]
                                            (
	                                            [ScriptName] ASC
                                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }


    }
}
