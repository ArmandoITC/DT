using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SkyboxIntensityController : MonoBehaviour
{
    public DynamicQueryExecutor queryExecutor;

    [Header("Umbrales e Intensidades")]
    public float threshold = 10f;
    public float lowIntensity = 0.5f;
    public float highIntensity = 1.5f;

    [Header("Frecuencia de Consulta")]
    public float intervalInMinutes = 1f;

    [Header("Consulta SQL")]
    [TextArea]
    public string query = "SELECT iluminacion FROM sesiones WHERE sesion = @sesion";

    private void Start()
    {
        StartCoroutine(IntensityUpdateLoop());
    }

    private System.Collections.IEnumerator IntensityUpdateLoop()
    {
        while (true)
        {
            Task task = UpdateSkyboxIntensity();
            while (!task.IsCompleted) yield return null;

            yield return new WaitForSeconds(intervalInMinutes * 60f);
        }
    }

    private async Task UpdateSkyboxIntensity()
    {
        var parameters = new Dictionary<string, object>
        {
            { "@sesion", GlobalSession.SelectedSession }
        };

        List<Dictionary<string, object>> result = await queryExecutor.ExecuteQuery(query, parameters);

        if (result.Count > 0 && result[0].Count > 0)
        {
            object firstValue = null;
            foreach (var val in result[0].Values)
            {
                firstValue = val;
                break;
            }

            if (firstValue != null && float.TryParse(firstValue.ToString(), out float value))
            {
                float intensityToApply = (value > threshold) ? lowIntensity : highIntensity;
                SetSkyboxIntensity(intensityToApply);

                Debug.Log($"[Skybox] Sesión #{GlobalSession.SelectedSession} → Iluminación: {value} " +
                          $"(Umbral: {threshold}) → Intensidad aplicada: {intensityToApply}");
            }
            else
            {
                Debug.LogWarning("[Skybox] No se pudo convertir el valor de 'iluminacion' a float.");
            }
        }
        else
        {
            Debug.LogWarning("[Skybox] La consulta no devolvió resultados válidos.");
        }
    }

    private void SetSkyboxIntensity(float intensity)
    {
        RenderSettings.ambientIntensity = intensity;
    }
}

