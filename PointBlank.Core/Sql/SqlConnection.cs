using System;
using Npgsql;
using System.Runtime.Remoting.Contexts;

namespace PointBlank.Core.Sql
{
    [Synchronization]
    public class SqlConnection
    {
        private static SqlConnection sql = new SqlConnection();
        protected NpgsqlConnectionStringBuilder connBuilder;

        static SqlConnection()
        {

        }

        public SqlConnection()
        {
            connBuilder = new NpgsqlConnectionStringBuilder
            {
                Database = Config.dbName,
                Host = Config.dbHost,
                Username = Config.dbUser,
                Password = Config.dbPass,
                Port = Config.dbPort
            };

            // ========== INÍCIO DO CÓDIGO DE DEBUG ==========
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  🔍 DEBUG CONEXÃO BANCO DE DADOS");
            Console.WriteLine($"  Host: {Config.dbHost}");
            Console.WriteLine($"  Port: {Config.dbPort}");
            Console.WriteLine($"  Database: {Config.dbName}");
            Console.WriteLine($"  User: {Config.dbUser}");
            Console.WriteLine($"  Pass: '{Config.dbPass}' (len={Config.dbPass?.Length ?? 0})");
            Console.WriteLine($"  ConnectionString: {connBuilder.ConnectionString}");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();
            // ========== FIM DO CÓDIGO DE DEBUG ==========
        }

        public static SqlConnection getInstance()
        {
            return sql;
        }

        public NpgsqlConnection conn()
        {
            return new NpgsqlConnection(connBuilder.ConnectionString);
        }
    }
}