using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BateriaAntiaerea : MonoBehaviour
{
    public GameObject balaPrefab; // Prefab de la bala
    public float velocidadMovimiento = 2.0f; // Velocidad de la batería
    public float velocidadMaximaBala = 10.0f; // Velocidad máxima de la bala
    public Transform puntoDisparo; // Punto desde el que se dispara la bala

    private bool moviendoDerecha = true;
    private float tiempoPresionandoClick = 0.0f;
    private bool estaPresionandoClick = false;

    void Update()
    {
        MoverBateria();
        ControlarDisparo();
    }

    void MoverBateria()
    {
        // Movimiento de izquierda a derecha
        if (moviendoDerecha)
        {
            transform.Translate(Vector2.right * velocidadMovimiento * Time.deltaTime);
            if (transform.position.x > 10.8f) // Limite derecho
                moviendoDerecha = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidadMovimiento * Time.deltaTime);
            if (transform.position.x < -10.8f) // Limite izquierdo
                moviendoDerecha = true;
        }
    }

    void ControlarDisparo()
    {
        // Detectar cuando se mantiene presionado el clic
        if (Input.GetMouseButtonDown(0))
        {
            estaPresionandoClick = true;
            tiempoPresionandoClick = 0.0f;
        }

        // Acumular el tiempo de presionado
        if (estaPresionandoClick)
        {
            tiempoPresionandoClick += Time.deltaTime;
        }

        // Detectar cuando se suelta el clic para disparar la bala
        if (Input.GetMouseButtonUp(0) && estaPresionandoClick)
        {
            estaPresionandoClick = false;
            DispararBala(tiempoPresionandoClick);
        }
    }

    void DispararBala(float tiempoPresion)
    {
        // Limitar el tiempo de presionado para que no supere una velocidad máxima
        float velocidadBala = Mathf.Clamp(tiempoPresion * velocidadMaximaBala, 1.0f, velocidadMaximaBala);

        // Instanciar la bala y establecer su velocidad
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
        bala.GetComponent<Bala>().Inicializar(velocidadBala);
    }
}
