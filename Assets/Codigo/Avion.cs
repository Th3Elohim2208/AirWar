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
    public float combustible = 100.0f; // Combustible inicial
    public enum EstadoAvion { EnVuelo, EnEspera }
    public EstadoAvion estadoActual = EstadoAvion.EnVuelo;
    public static event Action OnAvionDestruido;
    private int indiceRuta = 0;
    private LineRenderer lineRenderer;
    private bool puedeMoverse = true;
    private float consumoSegmento; // Combustible a consumir en el segmento actual
    private float distanciaSegmento; // Distancia total al siguiente nodo
    private float distanciaRecorrida; // Distancia recorrida hacia el siguiente nodo

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
        distanciaRecorrida = 0;

        if (rutaActual != null && rutaActual.Count > 1)
        {
            // Configura el primer segmento de la ruta
            InicializarConsumoSegmento();

            // Dibuja la ruta para visualizarla en pantalla
            DibujarRuta();
        }
        else
        {
            Debug.LogWarning("No se encontró una ruta válida.");
        }
    }

    void InicializarConsumoSegmento()
    {
        if (rutaActual != null && indiceRuta < rutaActual.Count - 1)
        {
            Nodo siguienteNodo = rutaActual[indiceRuta + 1];
            Arista aristaActual = posicionActual.conexiones.Find(a => a.destino == siguienteNodo);

            if (aristaActual != null)
            {
                consumoSegmento = aristaActual.peso;
                distanciaSegmento = Vector2.Distance(posicionActual.posicion, siguienteNodo.posicion);
                distanciaRecorrida = 0; // Reiniciar la distancia recorrida
            }
            else
            {
                Debug.LogWarning("No se encontró una arista para el segmento inicial. Intentando pasar al siguiente segmento...");
                indiceRuta++; // Intenta avanzar al siguiente segmento
                if (indiceRuta < rutaActual.Count - 1) InicializarConsumoSegmento(); // Reintenta con el siguiente segmento
            }
        }
    }


    void MoverHaciaDestino()
    {
        if (indiceRuta < rutaActual.Count)
        {
            Nodo siguienteNodo = rutaActual[indiceRuta];

            // Inicializa el consumo y la distancia si es el comienzo de un nuevo segmento
            if (distanciaRecorrida == 0 && consumoSegmento == 0)
            {
                InicializarConsumoSegmento();
            }

            // Avanza el avión y acumula la distancia recorrida
            float distanciaPaso = velocidadVuelo * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, siguienteNodo.posicion, distanciaPaso);
            distanciaRecorrida += distanciaPaso;

            // Calcula el consumo basado en el progreso en la distancia
            if (distanciaSegmento > 0)
            {
                float consumoPorPaso = (consumoSegmento / distanciaSegmento) * distanciaPaso;
                combustible -= consumoPorPaso; // Resta el consumo de combustible
            }

            if (combustible <= 0)
            {
                DestruirAvion();
                return;
            }

            // Si ha alcanzado el siguiente nodo, reinicia la distancia recorrida y avanza en la ruta
            if (Vector2.Distance(transform.position, siguienteNodo.posicion) < 0.1f)
            {
                posicionActual = siguienteNodo;
                indiceRuta++;

                // Si ha llegado al destino final, espera y reabastece si es necesario
                if (indiceRuta >= rutaActual.Count)
                {
                    estadoActual = EstadoAvion.EnEspera;
                    StartCoroutine(EsperarYReabastecer());
                }
                else
                {
                    InicializarConsumoSegmento(); // Inicializa el siguiente segmento
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
        Debug.Log("Avión destruido por falta de combustible");
        OnAvionDestruido?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator EsperarYReabastecer()
    {
        if (posicionActual.tipo == "aeropuerto" && combustible < 50.0f)
        {
            combustible = 100.0f; // Recargar combustible a 100 si está en un aeropuerto
            Debug.Log("Avión reabastecido a 100 en aeropuerto");
        }

        float tiempoEspera = UnityEngine.Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(tiempoEspera);

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
