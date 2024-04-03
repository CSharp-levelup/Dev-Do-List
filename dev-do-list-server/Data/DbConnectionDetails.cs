namespace DevDoListServer.Data
{
    public class DbConnectionDetails
    {
        public DbConnectionDetails(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public string ConnectionString { get; }
    }
}
