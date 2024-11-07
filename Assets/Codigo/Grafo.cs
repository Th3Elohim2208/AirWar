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
        origen.conexiones.Add(new Arista(destino, peso));
    }

    public bool VerificarConectividad()
    {
        if (nodos.Count == 0) return false;

        HashSet<Nodo> visitados = new HashSet<Nodo>();

        DFS(nodos[0], visitados);

        // Si el número de nodos visitados es igual al número total de nodos, el grafo está conectado
        bool conectado = visitados.Count == nodos.Count;
        Debug.Log(conectado ? "El grafo está completamente conectado." : "El grafo NO está completamente conectado.");
        return conectado;
    }

    private void DFS(Nodo nodo, HashSet<Nodo> visitados)
    {
        if (visitados.Contains(nodo)) return;

        visitados.Add(nodo);

        foreach (var arista in nodo.conexiones)
        {
            DFS(arista.destino, visitados);
        }
    }

    public bool ExisteRuta(Nodo origen, Nodo destino)
    {
        HashSet<Nodo> visitados = new HashSet<Nodo>();
        return DFSBuscarRuta(origen, destino, visitados);
    }

    private bool DFSBuscarRuta(Nodo actual, Nodo destino, HashSet<Nodo> visitados)
    {
        if (actual == destino) return true;
        if (visitados.Contains(actual)) return false;

        visitados.Add(actual);

        foreach (var arista in actual.conexiones)
        {
            if (DFSBuscarRuta(arista.destino, destino, visitados))
                return true;
        }

        return false;
    }

    // Método Dijkstra 
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
                    Debug.Log($"Actualizando distancia de {arista.destino.tipo} a {nuevaDistancia}");
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

        if (ruta.Count > 1 && ruta[0] == inicio)
        {
            Debug.Log("Ruta encontrada");
            return ruta;
        }
        else
        {
            Debug.LogWarning("No se encontró una ruta en Dijkstra.");
            return null;
        }
    }



    // Método ImprimirConexiones (sin cambios)
    public void ImprimirConexiones()
    {
        foreach (Nodo nodo in nodos)
        {
            Debug.Log($"Nodo {nodo.tipo} en posición {nodo.posicion} tiene {nodo.conexiones.Count} conexiones:");
            foreach (Arista conexion in nodo.conexiones)
            {
                Debug.Log($"    Conexión hacia {conexion.destino.tipo} en posición {conexion.destino.posicion} con peso {conexion.peso}");
            }
        }
    }
}
