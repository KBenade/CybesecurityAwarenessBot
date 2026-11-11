using Microsoft.Data.SqlClient;

namespace CybersecurityBot;

internal sealed class SqlServerTaskRepository
{
    private const string DatabaseName = "CybersecurityBot";
    private readonly string _connectionString;

    public SqlServerTaskRepository()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = @"(localdb)\MSSQLLocalDB",
            InitialCatalog = "master",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };

        using (var connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}]";
            command.ExecuteNonQuery();
        }

        builder.InitialCatalog = DatabaseName;
        _connectionString = builder.ConnectionString;
    }

    public void EnsureCreated()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            IF OBJECT_ID(N'dbo.cybersecurity_tasks', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.cybersecurity_tasks (
                    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    title NVARCHAR(200) NOT NULL,
                    description NVARCHAR(MAX) NOT NULL,
                    reminder_at DATETIME2 NULL,
                    is_completed BIT NOT NULL CONSTRAINT DF_cybersecurity_tasks_is_completed DEFAULT 0,
                    created_at DATETIME2 NOT NULL CONSTRAINT DF_cybersecurity_tasks_created_at DEFAULT SYSUTCDATETIME()
                )
            END
            """;
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<SecurityTask> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, title, description, reminder_at, is_completed FROM dbo.cybersecurity_tasks ORDER BY is_completed, CASE WHEN reminder_at IS NULL THEN 1 ELSE 0 END, reminder_at, id";
        using var reader = command.ExecuteReader();
        var tasks = new List<SecurityTask>();
        while (reader.Read())
        {
            int reminderOrdinal = reader.GetOrdinal("reminder_at");
            tasks.Add(new SecurityTask(
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("title")),
                reader.GetString(reader.GetOrdinal("description")),
                reader.IsDBNull(reminderOrdinal) ? null : reader.GetDateTime(reminderOrdinal),
                reader.GetBoolean(reader.GetOrdinal("is_completed"))));
        }

        return tasks;
    }

    public void Add(string title, string description, DateTime? reminderAt)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dbo.cybersecurity_tasks (title, description, reminder_at) VALUES (@title, @description, @reminderAt)";
        command.Parameters.Add("@title", System.Data.SqlDbType.NVarChar, 200).Value = title;
        command.Parameters.Add("@description", System.Data.SqlDbType.NVarChar, -1).Value = description;
        command.Parameters.Add("@reminderAt", System.Data.SqlDbType.DateTime2).Value = reminderAt is null ? DBNull.Value : reminderAt.Value;
        command.ExecuteNonQuery();
    }

    public void SetCompleted(int id, bool isCompleted)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.cybersecurity_tasks SET is_completed = @completed WHERE id = @id";
        command.Parameters.Add("@completed", System.Data.SqlDbType.Bit).Value = isCompleted;
        command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.cybersecurity_tasks WHERE id = @id";
        command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();
    }
}
