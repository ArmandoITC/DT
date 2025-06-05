using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class bidon : MonoBehaviour
{
    [TextArea(2, 5)]
    public List<string> mensajes = new List<string>(); // Ahora puedes agregar múltiples mensajes desde el Inspector

    void OnMouseDown()
    {
        if (mensajes.Count == 0)
        {
            Debug.LogWarning("No hay mensajes en la lista.");
            return;
        }

        // Selecciona un mensaje aleatorio
        string mensajeAleatorio = mensajes[Random.Range(0, mensajes.Count)];
        EnviarTextoAVoz(mensajeAleatorio);
    }

    void EnviarTextoAVoz(string texto)
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 50007))
            {
                byte[] data = Encoding.UTF8.GetBytes(texto);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                client.Close();
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("No se pudo conectar al servidor de voz Python: " + ex.Message);
        }
    }
}

