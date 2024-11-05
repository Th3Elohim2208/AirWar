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
        // Movimiento recto de la bala en la dirección hacia arriba
        transform.Translate(Vector2.up * velocidad * Time.deltaTime);

        // Destruir la bala si se sale de la pantalla
        if (transform.position.y > 10.0f)
        {
            Destroy(gameObject);
        }
    }
}
