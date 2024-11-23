using System.Collections.Generic;
using UnityEngine;

public class Grafo
{
    public List<Nodo> nodos = new List<Nodo>();

    public void AgregarNodo(Nodo nodo)
    {
        nodos.Add(nodo);
    }

    public void AgregarArista(Nodo origen, Nodo destino, float peso)
    {
        if (peso <= 0)
        {
            Debug.LogWarning("El peso de la arista debe ser mayor a 0.");
            return;
        }

        if (!ExisteArista(origen, destino))
        {
            origen.conexiones.Add(new Arista(destino, peso));
            destino.conexiones.Add(new Arista(origen, peso));
        }
    }

    public bool ExisteArista(Nodo origen, Nodo destino)
    {
        return origen.conexiones.Exists(a => a.destino == destino);
    }

    public void VerificarConexionesDelGrafo()
    {
        foreach (Nodo nodo in nodos)
        {
            if (nodo.conexiones.Count == 0)
            {
                Nodo conexion = nodos[UnityEngine.Random.Range(0, nodos.Count)];
                if (conexion != nodo)
                {
                    float peso = Vector2.Distance(nodo.posicion, conexion.posicion);
                    AgregarArista(nodo, conexion, peso);
                    Debug.Log($"Conexión automática añadida entre {nodo.posicion} y {conexion.posicion}.");
                }
            }
        }
    }


    public List<Nodo> CalcularRutaDijkstra(Nodo inicio, Nodo objetivo)
    {
        Dictionary<Nodo, float> distancias = new Dictionary<Nodo, float>();
        Dictionary<Nodo, Nodo> previos = new Dictionary<Nodo, Nodo>();
        HashSet<Nodo> visitados = new HashSet<Nodo>();

        foreach (Nodo nodo in nodos)
        {
            distancias[nodo] = float.MaxValue;
            previos[nodo] = null;
        }

        distancias[inicio] = 0;
        Nodo actual = inicio;

        while (actual != null)
        {
            visitados.Add(actual);

            foreach (Arista arista in actual.conexiones)
            {
                if (visitados.Contains(arista.destino)) continue;

                float nuevaDistancia = distancias[actual] + arista.peso;

                if (nuevaDistancia < distancias[arista.destino])
                {
                    distancias[arista.destino] = nuevaDistancia;
                    previos[arista.destino] = actual;
                }
            }

            actual = null;
            float menorDistancia = float.MaxValue;

            foreach (var nodo in distancias.Keys)
            {
                if (!visitados.Contains(nodo) && distancias[nodo] < menorDistancia)
                {
                    menorDistancia = distancias[nodo];
                    actual = nodo;
                }
            }

            if (actual == objetivo) break;
        }

        List<Nodo> ruta = new List<Nodo>();
        for (Nodo nodo = objetivo; nodo != null; nodo = previos[nodo])
        {
            ruta.Insert(0, nodo);
        }

        for (int i = 0; i < ruta.Count - 1; i++)
        {
            if (!ExisteArista(ruta[i], ruta[i + 1]))
            {
                Debug.LogError($"[Error Dijkstra] Ruta inválida: no hay conexión entre {ruta[i].posicion} y {ruta[i + 1].posicion}.");
                return null;
            }
        }

        if (ruta.Count > 1 && ruta[0] == inicio)
        {
            Debug.Log("Ruta encontrada y validada.");
            return ruta;
        }
        else
        {
            Debug.LogWarning("No se encontró una ruta válida.");
            return null;
        }
    }


    public void ImprimirConexiones()
    {
        foreach (Nodo nodo in nodos)
        {
            Debug.Log($"Nodo {nodo.tipo} en posición {nodo.posicion} tiene {nodo.conexiones.Count} conexiones:");
            foreach (Arista conexion in nodo.conexiones)
            {
                Debug.Log($"  Conexión hacia {conexion.destino.tipo} en posición {conexion.destino.posicion}, peso: {conexion.peso}");
            }
        }
    }
}




