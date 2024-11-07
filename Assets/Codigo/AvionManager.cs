using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvionManager : MonoBehaviour
{
    public GameObject avionPrefab;
    public int cantidadAviones = 5;
    private Grafo grafo;
    public float velocidadAvion = 5.0f;
    private List<GameObject> aviones = new List<GameObject>();

    void OnEnable()
    {
        GeneradorGrafo.OnGrafoInicializado += InicializarAviones;
    }

    void OnDisable()
    {
        GeneradorGrafo.OnGrafoInicializado -= InicializarAviones;
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
            avion.velocidadVuelo = velocidadAvion; // Asigna la velocidad configurada
            avion.grafo = grafo; // Asigna el grafo al avión
        }

        aviones.Add(avionObj);
    }



    Vector3 ObtenerPosicionInicial()
    {
        // Filtrar los nodos de tipo "aeropuerto"
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");

        if (aeropuertos.Count > 0)
        {
            // Seleccionar un aeropuerto aleatorio y devolver su posición
            Nodo nodoInicial = aeropuertos[Random.Range(0, aeropuertos.Count)];
            return nodoInicial.posicion;
        }
        else
        {
            Debug.LogError("No se encontraron aeropuertos en el grafo para generar aviones.");
            return Vector3.zero; // O una posición por defecto
        }
    }

}
