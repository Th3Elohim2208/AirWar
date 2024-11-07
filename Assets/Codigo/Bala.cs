using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    private float velocidad;

    public void Inicializar(float velocidadInicial)
    {
        velocidad = velocidadInicial;
    }

    void Update()
    {
        transform.Translate(Vector2.up * velocidad * Time.deltaTime);

        if (transform.position.y > 10.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Avion avion = other.GetComponent<Avion>();
        if (avion != null)
        {
            Debug.Log("Colisi�n detectada con un avi�n.");
            // Solo destruye el avi�n si est� en vuelo
            if (avion.estadoActual == Avion.EstadoAvion.EnVuelo)
            {
                avion.DestruirAvion();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("El avi�n est� en espera y no puede ser destruido.");
            }
        }
    }
}
