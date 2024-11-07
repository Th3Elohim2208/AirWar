using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BateriaAntiaerea : MonoBehaviour
{
    public GameObject balaPrefab;
    public float velocidadMovimiento = 2.0f;
    public float velocidadMaximaBala = 10.0f;
    public float tiempoMaximoDeCarga = 2.0f; // Tiempo necesario para carga completa
    public Transform puntoDisparo;
    public Image barraCarga;

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
        if (moviendoDerecha)
        {
            transform.Translate(Vector2.right * velocidadMovimiento * Time.deltaTime);
            if (transform.position.x > 10.6f) // Límite derecho
                moviendoDerecha = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidadMovimiento * Time.deltaTime);
            if (transform.position.x < -10.6f) // Límite izquierdo
                moviendoDerecha = true;
        }
    }

    void ControlarDisparo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            estaPresionandoClick = true;
            tiempoPresionandoClick = 0.0f;
        }

        if (estaPresionandoClick)
        {
            tiempoPresionandoClick += Time.deltaTime;

            // Calcular el progreso en función del tiempo máximo de carga
            float progresoCarga = Mathf.Clamp(tiempoPresionandoClick / tiempoMaximoDeCarga, 0, 1);
            barraCarga.fillAmount = progresoCarga;

            // Calcular la velocidad de la bala en función del progreso de carga
            float velocidadBala = progresoCarga * velocidadMaximaBala;
        }

        if (Input.GetMouseButtonUp(0) && estaPresionandoClick)
        {
            estaPresionandoClick = false;
            DispararBala(tiempoPresionandoClick);
            barraCarga.fillAmount = 0;
        }
    }

    void DispararBala(float tiempoPresion)
    {
        // Calcular el progreso en función del tiempo máximo de carga
        float progresoCarga = Mathf.Clamp(tiempoPresion / tiempoMaximoDeCarga, 0, 1);
        float velocidadBala = progresoCarga * velocidadMaximaBala;

        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
        bala.GetComponent<Bala>().Inicializar(velocidadBala);
    }
}
