using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class DatosDisplay : MonoBehaviour
{
    private TextMeshProUGUI texto;

    [SerializeField] private DynamicQueryExecutor queryExecutor;
    [SerializeField] private float intervaloMinutos = 1f; // 🔁 Tiempo entre actualizaciones (en minutos)

    private void Start()
    {
        texto = GetComponent<TextMeshProUGUI>();
        if (queryExecutor == null)
        {
            queryExecutor = FindObjectOfType<DynamicQueryExecutor>();
        }

        if (queryExecutor == null || texto == null)
        {
            Debug.LogError("Faltan referencias al texto o al executor.");
            return;
        }

        StartCoroutine(ActualizarCadaTanto());
    }

    private IEnumerator ActualizarCadaTanto()
    {
        while (true)
        {
            // Ejecuta la consulta y espera a que termine
            yield return ActualizarTexto();

            // Espera la cantidad de minutos antes de repetir
            yield return new WaitForSeconds(intervaloMinutos * 60f);
        }
    }

    private async Task ActualizarTexto()
    {
        string query = "SELECT e.field2::numeric AS humedad FROM data_entries e JOIN channels c ON e.channel_id = c.id WHERE channel_id=2450378 ORDER BY e.created_at DESC LIMIT 1";
        var resultados = await queryExecutor.ExecuteQuery(query);

        if (resultados.Count > 0 && resultados[0].ContainsKey("humedad"))
        {
            string valor = resultados[0]["humedad"]?.ToString() ?? "Sin valor";
            texto.text = $"La humedad es\n{valor}%";
            Debug.LogError($"La humedad es\n{valor}%");
        }
        else
        {
            texto.text = "No se encontraron resultados.";
        }
    }
}
