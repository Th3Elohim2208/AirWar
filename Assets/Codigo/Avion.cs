using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Avion : MonoBehaviour
{
    public Grafo grafo;
    public Nodo posicionActual;
    public Nodo destino;
    public List<Nodo> rutaActual;
    public float velocidadVuelo = 3.0f;
    public float combustible = 100.0f;
    public enum EstadoAvion { EnVuelo, EnEspera }
    public EstadoAvion estadoActual = EstadoAvion.EnVuelo;
    public static event Action<Avion> OnAvionDestruido;
    private int indiceRuta = 0;
    private LineRenderer lineRenderer;
    private bool puedeMoverse = true;
    private float consumoSegmento;
    private float distanciaSegmento;
    private float distanciaRecorrida;
    public string id;

    public List<AIModule> aiModules;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        posicionActual = ObtenerNodoInicial();

        if (posicionActual == null)
        {
            Debug.LogError("No se pudo obtener un nodo inicial v�lido para el avi�n.");
            return;
        }

        transform.position = posicionActual.posicion; // Coloca el avi�n en el nodo inicial
        SeleccionarNuevoDestino();
        DibujarRuta();

        if (aiModules == null || aiModules.Count == 0)
        {
            InicializarAIModules();
        }
    }

    void Update()
    {
        if (!puedeMoverse)
        {
            Debug.LogWarning($"[Avi�n {id}] El movimiento est� deshabilitado.");
            return;
        }

        if (rutaActual == null || rutaActual.Count == 0)
        {
            Debug.LogWarning($"[Avi�n {id}] No hay una ruta v�lida.");
            return;
        }

        if (indiceRuta >= rutaActual.Count)
        {
            Debug.LogWarning($"[Avi�n {id}] La ruta se ha completado.");
            return;
        }

        MoverHaciaDestino();
    }

    public Avion()
    {
        id = Guid.NewGuid().ToString();
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
        Nodo nuevoDestino = null;
        List<Nodo> nuevaRuta = null;

        for (int intentos = 0; intentos < 10; intentos++) // M�ximo 10 intentos
        {
            nuevoDestino = posiblesDestinos[UnityEngine.Random.Range(0, posiblesDestinos.Count)];

            if (nuevoDestino == posicionActual) continue;

            nuevaRuta = grafo.CalcularRutaDijkstra(posicionActual, nuevoDestino);

            if (nuevaRuta != null && nuevaRuta.Count > 1) break;
        }

        if (nuevaRuta != null && nuevaRuta.Count > 1)
        {
            destino = nuevoDestino;
            rutaActual = nuevaRuta;
            indiceRuta = 0;
            Debug.Log($"[Avi�n {id}] Nueva ruta asignada desde {posicionActual.posicion} hasta {destino.posicion}.");
            DibujarRuta();
        }
        else
        {
            Debug.LogWarning($"[Avi�n {id}] No se encontr� una ruta v�lida despu�s de varios intentos. Verificando grafo.");
            grafo.VerificarConexionesDelGrafo(); // Verificar y completar conexiones faltantes
            SeleccionarNuevoDestino();
        }
    }


    void MoverHaciaDestino()
    {
        if (rutaActual == null || indiceRuta >= rutaActual.Count)
        {
            Debug.LogWarning($"[Avi�n {id}] No hay ruta v�lida o se complet� la ruta.");
            SeleccionarNuevoDestino();
            return;
        }

        Nodo siguienteNodo = rutaActual[indiceRuta];
        float distanciaPaso = velocidadVuelo * Time.deltaTime;

        // Actualizar posici�n
        transform.position = Vector2.MoveTowards(transform.position, siguienteNodo.posicion, distanciaPaso);

        if (Vector2.Distance(transform.position, siguienteNodo.posicion) < 0.1f)
        {
            Debug.Log($"[Avi�n {id}] Alcanz� el nodo {siguienteNodo.posicion}.");
            posicionActual = siguienteNodo;
            indiceRuta++;

            // Si se complet� la ruta
            if (indiceRuta >= rutaActual.Count)
            {
                Debug.Log($"[Avi�n {id}] Ruta completada. Esperando reabastecimiento.");
                estadoActual = EstadoAvion.EnEspera;
                StartCoroutine(EsperarYReabastecer());
            }
        }
    }


    public void DestruirAvion()
    {
        OnAvionDestruido?.Invoke(this);
        Destroy(gameObject);
    }

    IEnumerator EsperarYReabastecer()
    {
        if (posicionActual.tipo == "aeropuerto" && combustible < 50.0f)
        {
            posicionActual.ReabastecerAvion(ref combustible);
            Debug.Log($"Avi�n reabastecido a {combustible} en el aeropuerto.");
        }

        float tiempoEspera = UnityEngine.Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(tiempoEspera);

        estadoActual = EstadoAvion.EnVuelo;
        SeleccionarNuevoDestino();
    }

    void DibujarRuta()
    {
        if (lineRenderer != null && rutaActual != null && rutaActual.Count > 1)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = rutaActual.Count;

            for (int i = 0; i < rutaActual.Count; i++)
            {
                lineRenderer.SetPosition(i, rutaActual[i].posicion);
            }

            Debug.Log($"[Avi�n {id}] Ruta dibujada con {rutaActual.Count} puntos.");
        }
        else
        {
            Debug.LogWarning($"[Avi�n {id}] No se puede dibujar la ruta. LineRenderer o ruta no v�lida.");
        }
    }


    public void InicializarAIModules()
    {
        aiModules = new List<AIModule>
        {
            new AIModule("Pilot", RandomID(), UnityEngine.Random.Range(45, 900)),
            new AIModule("Copilot", RandomID(), UnityEngine.Random.Range(45, 900)),
            new AIModule("Maintenance", RandomID(), UnityEngine.Random.Range(45, 900)),
            new AIModule("Space Awareness", RandomID(), UnityEngine.Random.Range(45, 900))
        };
    }

    string RandomID()
    {
        char[] letters = new char[3];
        for (int i = 0; i < 3; i++)
        {
            letters[i] = (char)UnityEngine.Random.Range('A', 'Z' + 1);
        }
        return new string(letters);
    }
}
