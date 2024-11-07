using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Avion : MonoBehaviour
{
    public Grafo grafo;
    public Nodo posicionActual;
    public Nodo destino;
    public List<Nodo> rutaActual;
    public float velocidadVuelo = 3.0f;
    public enum EstadoAvion { EnVuelo, EnEspera }
    public EstadoAvion estadoActual = EstadoAvion.EnVuelo;
    public static event Action OnAvionDestruido;
    private int indiceRuta = 0;
    private LineRenderer lineRenderer;
    private bool puedeMoverse = true;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        posicionActual = ObtenerNodoInicial();
        SeleccionarNuevoDestino();
        DibujarRuta();
    }

    void Update()
    {
        if (puedeMoverse && rutaActual != null && indiceRuta < rutaActual.Count)
        {
            MoverHaciaDestino();
        }
    }


    public void DetenerMovimiento()
    {
        puedeMoverse = false;
    }


    Nodo ObtenerNodoInicial()
    {
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");

        if (aeropuertos.Count > 0)
        {
            Nodo nodoSeleccionado = aeropuertos[UnityEngine.Random.Range(0, aeropuertos.Count)];
            return nodoSeleccionado;
        }
        else
        {
            Debug.LogError("No se encontraron nodos de tipo aeropuerto en el grafo.");
            return null;
        }
    }

    void SeleccionarNuevoDestino()
    {
        List<Nodo> posiblesDestinos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto" || n.tipo == "portaviones");
        Nodo nuevoDestino;

        do
        {
            nuevoDestino = posiblesDestinos[UnityEngine.Random.Range(0, posiblesDestinos.Count)];
        } while (nuevoDestino == posicionActual);

        destino = nuevoDestino;
        rutaActual = grafo.CalcularRutaDijkstra(posicionActual, destino);
        indiceRuta = 0;

        if (rutaActual != null && rutaActual.Count > 1)
        {
            DibujarRuta();
        }
        else
        {
            Debug.LogWarning("No se encontró una ruta válida.");
        }
    }

    void MoverHaciaDestino()
    {
        if (indiceRuta < rutaActual.Count)
        {
            Nodo siguienteNodo = rutaActual[indiceRuta];
            transform.position = Vector2.MoveTowards(transform.position, siguienteNodo.posicion, velocidadVuelo * Time.deltaTime);

            if (Vector2.Distance(transform.position, siguienteNodo.posicion) < 0.1f)
            {
                posicionActual = siguienteNodo;
                indiceRuta++;

                // Cambia a EnEspera solo si ha llegado a su destino final
                if (indiceRuta >= rutaActual.Count)
                {
                    estadoActual = EstadoAvion.EnEspera;
                    StartCoroutine(EsperarYReabastecer());
                }
            }
        }
        else
        {
            estadoActual = EstadoAvion.EnVuelo;
        }
    }

    public void DestruirAvion()
    {
        Debug.Log("Avión destruido");
        OnAvionDestruido?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator EsperarYReabastecer()
    {
        float tiempoEspera = UnityEngine.Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(tiempoEspera);

        float combustible = UnityEngine.Random.Range(50.0f, 100.0f);

        // Cambia el estado a EnVuelo después de reabastecer y antes de seleccionar un nuevo destino
        estadoActual = EstadoAvion.EnVuelo;
        SeleccionarNuevoDestino();
    }


    void DibujarRuta()
    {
        if (lineRenderer != null && rutaActual != null)
        {
            lineRenderer.positionCount = rutaActual.Count;
            for (int i = 0; i < rutaActual.Count; i++)
            {
                lineRenderer.SetPosition(i, rutaActual[i].posicion);
            }
        }
    }
}
