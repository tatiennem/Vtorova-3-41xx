namespace DAL.DbContext
{
    public class DatabaseOptions
    {
        /// <summary>
        /// Either Postgres or Sqlite.
        /// </summary>
        public string Provider { get; set; } = "Postgres";

        public string ConnectionString { get; set; } =
            "Host=localhost;Port=5432;Database=autosalon;Username=postgres;Password=123";

        public string SqlitePath { get; set; } = "autosalon.db";

        public bool EnableSensitiveLogging { get; set; }
        public bool FallbackToSqlite { get; set; } = true;
    }
}
