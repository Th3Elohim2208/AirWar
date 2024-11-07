using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TemporizadorJuego : MonoBehaviour
{
    public Text textoTemporizador;       // Texto de UI para mostrar el temporizador
    public Text mensajeFinal;            // Texto de UI para mostrar el mensaje final
    public AvionManager avionManager;    // Referencia al AvionManager para contar aviones destruidos
    public GameObject controlDisparo;    // Objeto que controla las balas o el disparo, para desactivarlo al final

    private float tiempoRestante = 12f; // 2 minutos en segundos
    private bool juegoTerminado;


    void Start()
    {
        mensajeFinal.gameObject.SetActive(false); // Ocultar el mensaje al inicio
        ActualizarTextoTemporizador();
        StartCoroutine(ContarRegresivamente());
    }

    IEnumerator ContarRegresivamente()
    {
        while (tiempoRestante > 0 && !juegoTerminado) // Añadir la verificación de juegoTerminado
        {
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
            ActualizarTextoTemporizador();
        }

        if (!juegoTerminado) // Verificar si no ha terminado antes de llamar a TerminarJuego
        {
            TerminarJuego();
        }
    }


    void ActualizarTextoTemporizador()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60);
        textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void TerminarJuego()
    {
        juegoTerminado = true;

        // Desactivar el movimiento de los aviones
        Avion[] aviones = FindObjectsOfType<Avion>();
        foreach (Avion avion in aviones)
        {
            avion.DetenerMovimiento();
        }

        // Desactivar el disparo
        if (controlDisparo != null)
        {
            controlDisparo.SetActive(false);
        }

        // Eliminar todas las balas en el mapa
        GameObject[] balas = GameObject.FindGameObjectsWithTag("Bala");
        foreach (GameObject bala in balas)
        {
            Destroy(bala);
        }

        // Mostrar mensaje final con el contador de aviones destruidos
        mensajeFinal.text = "Juego terminado. Aviones eliminados: " + avionManager.contadorAvionesDestruidos;
        mensajeFinal.gameObject.SetActive(true);
    }

}
