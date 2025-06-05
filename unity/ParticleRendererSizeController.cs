using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ParticleRendererSizeController : MonoBehaviour
{
    public DynamicQueryExecutor queryExecutor;

    [Header("Rango del valor en la BD")]
    public float minDBValue = 10f;
    public float maxDBValue = 1000f;

    [Header("Rango de Min Particle Size")]
    public float minParticleSize = 0.1f;
    public float maxParticleSize = 2f;

    [Header("Frecuencia de actualización (minutos)")]
    public float intervalInMinutes = 1f;

    [Header("Sistema de partículas")]
    public ParticleSystem targetParticleSystem;

    private void Start()
    {
        StartCoroutine(UpdateLoop());
    }

    private System.Collections.IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return UpdateMinParticleSize();
            yield return new WaitForSeconds(intervalInMinutes * 60f);
        }
    }

    private async Task UpdateMinParticleSize()
    {
        int sesionID = GlobalSession.SelectedSession;

        if (sesionID <= 0)
        {
            Debug.LogWarning("No hay sesión seleccionada todavía.");
            return;
        }

        string sql = "SELECT humedad FROM sesiones WHERE sesion = @sesion";

        var parameters = new Dictionary<string, object>
        {
            { "@sesion", sesionID }
        };

        List<Dictionary<string, object>> result = await queryExecutor.ExecuteQuery(sql, parameters);

        if (result.Count > 0 && result[0].Count > 0)
        {
            object firstValue = null;
            foreach (var val in result[0].Values)
            {
                firstValue = val;
                break;
            }

            if (firstValue != null && float.TryParse(firstValue.ToString(), out float rawValue))
            {
                float scaled = Mathf.InverseLerp(minDBValue, maxDBValue, rawValue);
                float finalSize = Mathf.Lerp(minParticleSize, maxParticleSize, scaled);

                ApplyMinParticleSize(finalSize);
                Debug.Log($"Sesión {sesionID} → Humedad: {rawValue} → Particle Size: {finalSize}");
            }
            else
            {
                Debug.LogWarning("No se pudo convertir el valor de humedad a float.");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró humedad para la sesión {sesionID}.");
        }
    }

    private void ApplyMinParticleSize(float size)
    {
        if (targetParticleSystem == null)
        {
            Debug.LogWarning("No se ha asignado un sistema de partículas.");
            return;
        }

        var renderer = targetParticleSystem.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.minParticleSize = size;
        }
        else
        {
            Debug.LogWarning("No se encontró el componente ParticleSystemRenderer.");
        }
    }
}
