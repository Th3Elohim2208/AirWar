using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avion : MonoBehaviour
{
    public Grafo grafo;
    public Nodo posicionActual;
    public Nodo destino;
    public List<Nodo> rutaActual;
    public float velocidadVuelo = 3.0f;

    private int indiceRuta = 0;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        posicionActual = ObtenerNodoInicial();
        SeleccionarNuevoDestino();
        DibujarRuta();
    }

    void Update()
    {
        if (rutaActual != null && indiceRuta < rutaActual.Count)
        {
            MoverHaciaDestino();
        }
    }

    Nodo ObtenerNodoInicial()
    {
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");

        if (aeropuertos.Count > 0)
        {
            Nodo nodoSeleccionado = aeropuertos[Random.Range(0, aeropuertos.Count)];
            Debug.Log($"Nodo inicial seleccionado: {nodoSeleccionado.tipo} en posición {nodoSeleccionado.posicion}");
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
        // Filtra los nodos de tipo "aeropuerto" y "portaviones" para seleccionar el destino
        List<Nodo> posiblesDestinos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto" || n.tipo == "portaviones");
        Nodo nuevoDestino;

        do
        {
            nuevoDestino = posiblesDestinos[Random.Range(0, posiblesDestinos.Count)];
        } while (nuevoDestino == posicionActual);

        destino = nuevoDestino;
        rutaActual = grafo.CalcularRutaDijkstra(posicionActual, destino);
        indiceRuta = 0;

        if (rutaActual != null && rutaActual.Count > 1)
        {
            Debug.Log($"Destino seleccionado: {destino.tipo} en posición {destino.posicion}");
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

                if (indiceRuta >= rutaActual.Count)
                {
                    StartCoroutine(EsperarYReabastecer());
                }
            }
        }
    }

    IEnumerator EsperarYReabastecer()
    {
        float tiempoEspera = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(tiempoEspera);

        float combustible = Random.Range(50.0f, 100.0f);
        Debug.Log($"Avión reabastecido a {combustible} unidades de combustible.");

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
