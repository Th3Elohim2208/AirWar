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
        SeleccionarNuevoDestino();
        DibujarRuta();

        // Inicializar los módulos de AI si aún no están inicializados
        if (aiModules == null || aiModules.Count == 0)
        {
            InicializarAIModules();
        }
    }



    void Update()
    {
        if (puedeMoverse && rutaActual != null && indiceRuta < rutaActual.Count)
        {
            MoverHaciaDestino();
        }
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
            InicializarConsumoSegmento();
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
                distanciaRecorrida = 0;
            }
            else
            {
                Debug.LogWarning("No se encontró una arista para el segmento inicial. Intentando pasar al siguiente segmento...");
                indiceRuta++;
                if (indiceRuta < rutaActual.Count - 1) InicializarConsumoSegmento();
            }
        }
    }

    void MoverHaciaDestino()
    {
        if (indiceRuta < rutaActual.Count)
        {
            Nodo siguienteNodo = rutaActual[indiceRuta];

            if (distanciaRecorrida == 0 && consumoSegmento == 0)
            {
                InicializarConsumoSegmento();
            }

            float distanciaPaso = velocidadVuelo * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, siguienteNodo.posicion, distanciaPaso);
            distanciaRecorrida += distanciaPaso;

            if (distanciaSegmento > 0)
            {
                float consumoPorPaso = (consumoSegmento / distanciaSegmento) * distanciaPaso;
                combustible -= consumoPorPaso;
            }

            if (combustible <= 0)
            {
                DestruirAvion();
                return;
            }

            if (Vector2.Distance(transform.position, siguienteNodo.posicion) < 0.1f)
            {
                posicionActual = siguienteNodo;
                indiceRuta++;

                if (indiceRuta >= rutaActual.Count)
                {
                    estadoActual = EstadoAvion.EnEspera;
                    StartCoroutine(EsperarYReabastecer());
                }
                else
                {
                    InicializarConsumoSegmento();
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
        Debug.Log($"Avión con ID {id} ha sido destruido.");
        OnAvionDestruido?.Invoke(this);
        Destroy(gameObject);
    }

    IEnumerator EsperarYReabastecer()
    {
        if (posicionActual.tipo == "aeropuerto" && combustible < 50.0f)
        {
            // Intenta reabastecer el avión desde el tanque del aeropuerto
            posicionActual.ReabastecerAvion(ref combustible); // Pasa el combustible del avión por referencia

            Debug.Log($"Avión reabastecido a {combustible} en el aeropuerto.");
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