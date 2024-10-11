using System;
using Npgsql;

namespace TemboDatabaseManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Host=concretely-fair-mudskipper.data-1.use1.tembo.io;Port=5432;Username=postgres;Password=yP3GpCB2eEbn1vIw\r\n";

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
                                Console.WriteLine($"{affectedRows} rows affected.");
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
    }
}
