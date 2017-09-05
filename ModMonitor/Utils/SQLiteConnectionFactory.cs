using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;

namespace ModMonitor.Utils
{
    public class SQLiteConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new SQLiteConnection(nameOrConnectionString);
        }
    }
}
