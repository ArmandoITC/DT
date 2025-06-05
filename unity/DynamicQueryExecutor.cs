using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Npgsql;

public class DynamicQueryExecutor : MonoBehaviour
{
    // Conexión activa
    private string host = "66.179.211.230";
    private string port = "5432";
    private string dbName = "Unity";
    private string username = "postgres";
    private string password = "Armando18";

    private string connString;

    private void Awake()
    {
        connString = $"Host={host};Port={port};Username={username};Password={password};Database={dbName}";
    }

    // Ejecuta cualquier consulta de SELECT y devuelve los resultados en una lista de diccionarios
    public async Task<List<Dictionary<string, object>>> ExecuteQuery(string query, Dictionary<string, object> parameters = null)
    {
        List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

        using (var connection = new NpgsqlConnection(connString))
        {
            try
            {
                await connection.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            results.Add(row);
                        }
                    }
                }

                Debug.Log($"Consulta ejecutada con éxito desde: {gameObject.name} ({GetType().Name})\nQuery: {query}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al ejecutar la consulta desde: {gameObject.name} ({GetType().Name})\nError: {ex.Message}\nQuery: {query}");
            }
        }

        return results;
    }

    // Ejecuta una consulta que no devuelve datos (INSERT, UPDATE, DELETE)
    public async Task<int> ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        int rowsAffected = 0;

        using (var connection = new NpgsqlConnection(connString))
        {
            try
            {
                await connection.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    rowsAffected = await cmd.ExecuteNonQueryAsync();
                    Debug.Log($"Consulta no SELECT ejecutada con éxito desde: {gameObject.name} ({GetType().Name})\nQuery: {query}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error en consulta no SELECT desde: {gameObject.name} ({GetType().Name})\nError: {ex.Message}\nQuery: {query}");
            }
        }

        return rowsAffected;
    }
}

