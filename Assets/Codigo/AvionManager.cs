using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvionManager : MonoBehaviour
{
    public GameObject avionPrefab;
    public int cantidadAviones = 5;
    private Grafo grafo;
    public float velocidadAvion = 5.0f;
    private List<GameObject> aviones = new List<GameObject>();
    public int contadorAvionesDestruidos = 0;
    public Text textoContadorAviones;

    void OnEnable()
    {
        GeneradorGrafo.OnGrafoInicializado += InicializarAviones;
        Avion.OnAvionDestruido += IncrementarContador;
    }

    void OnDisable()
    {
        GeneradorGrafo.OnGrafoInicializado -= InicializarAviones;
        Avion.OnAvionDestruido -= IncrementarContador;
    }

    void InicializarAviones()
    {
        grafo = FindObjectOfType<GeneradorGrafo>().grafo;

        if (grafo != null && grafo.nodos.Count > 0)
        {
            for (int i = 0; i < cantidadAviones; i++)
            {
                GenerarAvion();
            }
        }
        else
        {
            Debug.LogError("No se pudo acceder al grafo en AvionManager.");
        }
    }

    void GenerarAvion()
    {
        GameObject avionObj = Instantiate(avionPrefab, ObtenerPosicionInicial(), Quaternion.identity);
        Avion avion = avionObj.GetComponent<Avion>();

        if (avion != null)
        {
            avion.velocidadVuelo = velocidadAvion;
            avion.grafo = grafo;
        }

        aviones.Add(avionObj);
    }

    private void IncrementarContador()
    {
        contadorAvionesDestruidos++;
        textoContadorAviones.text = "Aviones Destruidos: " + contadorAvionesDestruidos;
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
}
