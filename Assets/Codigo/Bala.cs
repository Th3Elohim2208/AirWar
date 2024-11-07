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
            Debug.Log("Colisión detectada con un avión.");
            // Solo destruye el avión si está en vuelo
            if (avion.estadoActual == Avion.EstadoAvion.EnVuelo)
            {
                avion.DestruirAvion();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("El avión está en espera y no puede ser destruido.");
            }
        }
    }
}
