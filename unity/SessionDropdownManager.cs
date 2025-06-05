using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class SessionDropdownManager : MonoBehaviour
{
    public DynamicQueryExecutor queryExecutor;
    public TMP_Dropdown sessionDropdown;

    [Header("Intervalo de actualización (segundos)")]
    public float refreshIntervalSeconds = 60f; // Editable desde la GUI

    private List<int> sessionIds = new List<int>();

    private void Start()
    {
        StartCoroutine(RefreshLoop());
    }

    private System.Collections.IEnumerator RefreshLoop()
    {
        while (true)
        {
            yield return LoadSessions();
            yield return new WaitForSeconds(refreshIntervalSeconds);
        }
    }

    private async Task LoadSessions()
    {
        string query = "SELECT sesion FROM sesiones ORDER BY sesion ASC";

        List<Dictionary<string, object>> results = await queryExecutor.ExecuteQuery(query);

        int currentIndex = sessionDropdown.value;
        int previouslySelectedId = (currentIndex >= 0 && currentIndex < sessionIds.Count) ? sessionIds[currentIndex] : -1;

        sessionIds.Clear();
        sessionDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var row in results)
        {
            if (row.ContainsKey("sesion") && row["sesion"] != null)
            {
                int sessionId = System.Convert.ToInt32(row["sesion"]);
                sessionIds.Add(sessionId);
                options.Add("Sesión " + sessionId);
            }
        }

        sessionDropdown.AddOptions(options);

        // Restaurar selección previa si aún existe
        int restoredIndex = sessionIds.IndexOf(previouslySelectedId);
        if (restoredIndex != -1)
        {
            sessionDropdown.value = restoredIndex;
        }
        else if (sessionIds.Count > 0)
        {
            sessionDropdown.value = 0;
            GlobalSession.SelectedSession = sessionIds[0];
        }

        sessionDropdown.RefreshShownValue(); // Forzar redibujado

        sessionDropdown.onValueChanged.RemoveAllListeners();
        sessionDropdown.onValueChanged.AddListener(OnSessionSelected);
    }

    private void OnSessionSelected(int index)
    {
        if (index >= 0 && index < sessionIds.Count)
        {
            int selectedSession = sessionIds[index];
            GlobalSession.SelectedSession = selectedSession;
            Debug.Log("Sesión seleccionada: " + selectedSession);
        }
    }
}
