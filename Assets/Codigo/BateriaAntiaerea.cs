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
            if (transform.position.x < -13.7f) // Límite izquierdo
                moviendoDerecha = true;
        }
    }

    void ControlarDisparo()
    {
        if (Input.GetMouseButtonDown(0) && !UIHelper.IsPointerOverUI())
        {
            estaPresionandoClick = true;
            tiempoPresionandoClick = 0.0f;
        }

        if (estaPresionandoClick && Input.GetMouseButton(0))
        {
            tiempoPresionandoClick += Time.deltaTime;

            // Calcular el progreso en función del tiempo máximo de carga
            float progresoCarga = Mathf.Clamp(tiempoPresionandoClick / tiempoMaximoDeCarga, 0, 1);
            barraCarga.fillAmount = progresoCarga;
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

        // Crear la bala y establecer su velocidad
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
        bala.GetComponent<Bala>().Inicializar(velocidadBala);

        // Mostrar la velocidad de la bala en la consola
    }
}
