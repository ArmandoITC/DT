using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpotlightIntensityController : MonoBehaviour
{
    public DynamicQueryExecutor queryExecutor;

    [Header("Consulta SQL")]
    [TextArea]
    public string query = "SELECT iluminacion FROM sesiones WHERE sesion = @sesion";

    [Header("Configuración de Rango")]
    public float minDBValue = 0f;
    public float maxDBValue = 400f;

    [Header("Configuración de Intensidad")]
    public float minIntensity = 0f;
    public float maxIntensity = 11f;

    [Header("Consulta cada X minutos")]
    public float intervalInMinutes = 1f;

    private void Start()
    {
        StartCoroutine(IntensityUpdateLoop());
    }

    // --------- Corrutina principal ----------
    private System.Collections.IEnumerator IntensityUpdateLoop()
    {
        while (true)
        {
            // Llamada asíncrona sin bloquear el hilo principal
            Task task = UpdateSpotlightIntensity();
            while (!task.IsCompleted) yield return null;

            yield return new WaitForSeconds(intervalInMinutes * 60f);
        }
    }

    // --------- Consulta y mapeo ----------
    private async Task UpdateSpotlightIntensity()
    {
        var parameters = new Dictionary<string, object>
        {
            { "@sesion", GlobalSession.SelectedSession }
        };

        List<Dictionary<string, object>> result =
            await queryExecutor.ExecuteQuery(query, parameters);

        if (result.Count > 0 && result[0].Count > 0)
        {
            object firstValue = null;
            foreach (var v in result[0].Values) { firstValue = v; break; }

            if (firstValue != null &&
                float.TryParse(firstValue.ToString(), out float rawValue))
            {
                float scaled = Mathf.InverseLerp(minDBValue, maxDBValue, rawValue);
                float intensity = Mathf.Lerp(minIntensity, maxIntensity, scaled);

                ApplyIntensityToChildren(intensity);

                Debug.Log($"Iluminación de sesión #{GlobalSession.SelectedSession} = {rawValue}");
                Debug.Log($"Intensidad aplicada a spots = {intensity}");
            }
            else
            {
                Debug.LogWarning("No se pudo convertir 'iluminacion' a float.");
            }
        }
        else
        {
            Debug.LogWarning("La consulta no devolvió resultados.");
        }
    }

    // --------- Aplicar a luces hijo ----------
    private void ApplyIntensityToChildren(float intensity)
    {
        Light[] lights = GetComponentsInChildren<Light>(true);
        foreach (var l in lights)
            if (l.type == LightType.Spot)
                l.intensity = intensity;

        Debug.Log($"Se aplicó intensidad {intensity} a {lights.Length} luces Spot.");
    }
}
