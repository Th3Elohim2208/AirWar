using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneradorGrafo : MonoBehaviour
{
    public GameObject aeropuertoPrefab;
    public GameObject portavionesPrefab;
    public int cantidadAeropuertos = 3;
    public int cantidadPortaviones = 2;

    private Grafo grafo;

    void Start()
    {
        grafo = new Grafo();
        GenerarRutas();
    }


    void GenerarRutas()
    {
        foreach (Nodo origen in grafo.nodos)
        {
            int conexiones = Random.Range(1, 3); // Número aleatorio de conexiones por nodo
            for (int i = 0; i < conexiones; i++)
            {
                Nodo destino = ObtenerNodoAleatorio(origen);
                if (destino != null)
                {
                    float peso = CalcularPesoRuta(origen, destino);
                    grafo.AgregarArista(origen, destino, peso);
                    Debug.Log($"Ruta generada de {origen.tipo} a {destino.tipo} con peso {peso}");
                }
            }
        }
    }

    Nodo ObtenerNodoAleatorio(Nodo actual)
    {
        Nodo nodoAleatorio = grafo.nodos[Random.Range(0, grafo.nodos.Count)];
        return nodoAleatorio != actual ? nodoAleatorio : null;
    }

    float CalcularPesoRuta(Nodo origen, Nodo destino)
    {
        float distancia = Vector2.Distance(origen.posicion, destino.posicion);
        float peso = distancia;

        if (destino.tipo == "portaviones")
        {
            peso += 10; // Aumenta el peso si es un portaviones
        }

        if (origen.tipo == "portaviones" && destino.tipo == "aeropuerto")
        {
            peso += 5; // Penalización adicional por ruta interoceánica
        }

        return peso;
    }

}