using System;
using Npgsql;

namespace TemboDatabaseManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString; // "Host=concretely-fair-mudskipper.data-1.use1.tembo.io;Port=5432;Username=postgres;Password=yP3GpCB2eEbn1vIw\r\n";

            Console.WriteLine("Write your connection string: ");
            connectionString = Console.ReadLine();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                Console.WriteLine("Connected to the database. You can enter SQL commands. Type 'exit' to quit.");

                while (true)
                {
                    Console.Write("SQL> ");
                    string commandText = Console.ReadLine();

                    if (commandText.Trim().ToLower() == "exit")
                        break;

                    if (commandText.Trim().StartsWith(@"\t ", StringComparison.OrdinalIgnoreCase))
                    {
                        string tableName = commandText.Trim().Substring(3).Trim();
                        GetTableDetails(conn, tableName);
                        continue;
                    }

                    try
                    {
                        using (var cmd = new NpgsqlCommand(commandText, conn))
                        {
                            if (commandText.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                            {
                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        Console.WriteLine(new string('-', reader.FieldCount * 18));
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            Console.Write($"{reader.GetName(i),-20}");
                                        }
                                        Console.WriteLine();
                                        Console.WriteLine(new string('-', reader.FieldCount * 18));

                                        while (reader.Read())
                                        {
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                Console.Write($"{reader[i],-20}");
                                            }
                                            Console.WriteLine();
                                        }
                                        Console.WriteLine(new string('-', reader.FieldCount * 18));
                                    }
                                    else
                                    {
                                        Console.WriteLine("No rows found.");
                                    }
                                }
                            }
                            else
                            {
                                int affectedRows = cmd.ExecuteNonQuery();
                                if (affectedRows != -1)
                                {
                                    Console.WriteLine($"{affectedRows} rows affected.");
                                }
                                else
                                {
                                    Console.WriteLine("\n");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("Disconnected from the database.");
        }

        static void GetTableDetails(NpgsqlConnection conn, string tableName)
        {
            string query = @"
                SELECT 
                    column_name, 
                    data_type, 
                    character_maximum_length, 
                    is_nullable 
                FROM 
                    information_schema.columns 
                WHERE 
                    table_name = @tableName";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("tableName", tableName);

                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine($"Details for table: {tableName}");
                            Console.WriteLine(new string('-', 100));
                            Console.WriteLine($"{"Column Name",-30} {"Data Type",-20} {"Max Length",-15} {"Is Nullable",-15}");

                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["column_name"],-30} {reader["data_type"],-20} {reader["character_maximum_length"],-15} {reader["is_nullable"],-15}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Table '{tableName}' not found or has no columns.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving table details: {ex.Message}");
                }
            }
        }
    }
}
