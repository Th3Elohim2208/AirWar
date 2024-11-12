using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AvionManager : MonoBehaviour
{
    public GameObject avionPrefab;
    public int cantidadAvionesInicial = 5; // Cantidad inicial de aviones a generar
    public int capacidadHangar = 12; // Capacidad máxima de hangares
    public float velocidadAvion = 5.0f;
    public float intervaloGeneracionAviones = 10.0f;
    public Text textoContadorAviones;
    public Text textoAvionesDerribados;

    private List<GameObject> aviones = new List<GameObject>();
    private Grafo grafo;
    private int contadorAvionesDestruidos = 0;
    private GestionAvionesDerribados gestionAvionesDerribados;

    public int ContadorAvionesDestruidos => contadorAvionesDestruidos;

    void OnEnable()
    {
        GeneradorGrafo.OnGrafoInicializado += InicializarAviones;
        Avion.OnAvionDestruido += ManejarAvionDestruido;
    }

    void OnDisable()
    {
        GeneradorGrafo.OnGrafoInicializado -= InicializarAviones;
        Avion.OnAvionDestruido -= ManejarAvionDestruido;
    }

    void Start()
    {
        gestionAvionesDerribados = GetComponent<GestionAvionesDerribados>();
        if (gestionAvionesDerribados == null)
        {
            Debug.LogError("No se encontró el componente GestionAvionesDerribados en AvionManager.");
            return;
        }

        StartCoroutine(GenerarAvionesPeriodicamente());
    }

    void InicializarAviones()
    {
        grafo = FindObjectOfType<GeneradorGrafo>().grafo;

        if (grafo != null && grafo.nodos.Count > 0)
        {
            for (int i = 0; i < cantidadAvionesInicial; i++)
            {
                GenerarAvion();
            }
        }
        else
        {
            Debug.LogError("No se pudo acceder al grafo en AvionManager.");
        }
    }

    IEnumerator GenerarAvionesPeriodicamente()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloGeneracionAviones);

            // Verifica cuántos aviones se pueden generar sin exceder el límite de hangares
            int espacioDisponible = capacidadHangar - aviones.Count;
            if (espacioDisponible > 0)
            {
                List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");
                int avionesACrear = Mathf.Min(3, espacioDisponible, aeropuertos.Count); // Limita a 3 o menos, según el espacio y número de aeropuertos

                for (int i = 0; i < avionesACrear; i++)
                {
                    GenerarAvionEnPosicion(aeropuertos[i].posicion);
                }
            }
            else
            {
                Debug.LogWarning("Capacidad de hangares alcanzada. No se pueden generar más aviones.");
            }
        }
    }

    void GenerarAvion()
    {
        Vector3 posicionInicial = ObtenerPosicionInicial();
        GenerarAvionEnPosicion(posicionInicial);
    }

    void GenerarAvionEnPosicion(Vector3 posicion)
    {
        if (aviones.Count < capacidadHangar)
        {
            GameObject avionObj = Instantiate(avionPrefab, posicion, Quaternion.identity);
            Avion avion = avionObj.GetComponent<Avion>();

            if (avion != null)
            {
                avion.id = Guid.NewGuid().ToString();
                avion.velocidadVuelo = velocidadAvion;
                avion.grafo = grafo;
                aviones.Add(avionObj);
            }
        }
        else
        {
            Debug.LogWarning("Capacidad de hangares alcanzada. No se pueden generar más aviones.");
        }
    }

    Vector3 ObtenerPosicionInicial()
    {
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");

        if (aeropuertos.Count > 0)
        {
            Nodo nodoInicial = aeropuertos[UnityEngine.Random.Range(0, aeropuertos.Count)];
            return nodoInicial.posicion;
        }
        else
        {
            Debug.LogError("No se encontraron aeropuertos en el grafo para generar aviones.");
            return Vector3.zero;
        }
    }

    private void ManejarAvionDestruido(Avion avion)
    {
        contadorAvionesDestruidos++;
        textoContadorAviones.text = "Aviones Destruidos: " + contadorAvionesDestruidos;

        // Agregar el avión destruido a la lista en GestionAvionesDerribados
        gestionAvionesDerribados.AgregarAvionDerribado(avion);

        // Eliminar el avión de la lista de aviones activos
        aviones.Remove(avion.gameObject);

        // Actualizar la lista en la interfaz de usuario
        ActualizarListaAvionesDerribados();
    }

    private void ActualizarListaAvionesDerribados()
    {
        List<Avion> listaOrdenada = gestionAvionesDerribados.ObtenerAvionesDerribadosOrdenadosPorID();
        textoAvionesDerribados.text = "Aviones Derribados (Ordenados por ID):\n";

        foreach (Avion avion in listaOrdenada)
        {
            textoAvionesDerribados.text += $"ID: {avion.id}\n";
        }
    }
}
