using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BlazorCalculatorApp.Data
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS CalculationHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Operand1 REAL NOT NULL,
                    Operand2 REAL NOT NULL,
                    Operation TEXT NOT NULL,
                    Result TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL
                );
            ");
        }

        public void SaveCalculation(double operand1, double operand2, string operation, string result)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                INSERT INTO CalculationHistory (Operand1, Operand2, Operation, Result, CreatedAt)
                VALUES (@Operand1, @Operand2, @Operation, @Result, @CreatedAt);
            ", new { Operand1 = operand1, Operand2 = operand2, Operation = operation, Result = result, CreatedAt = DateTime.UtcNow });
        }

        public CalculationHistory GetLastCalculation()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<CalculationHistory>(@"
                SELECT * FROM CalculationHistory
                ORDER BY CreatedAt DESC
                LIMIT 1;
            ");
        }
    }

    public class CalculationHistory
    {
        public int Id { get; set; }
        public double Operand1 { get; set; }
        public double Operand2 { get; set; }
        public string Operation { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
