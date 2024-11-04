using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grafo
{
    public List<Nodo> nodos;

    public Grafo()
    {
        nodos = new List<Nodo>();
    }

    public void AgregarNodo(Nodo nodo)
    {
        nodos.Add(nodo);
    }

    public void AgregarArista(Nodo origen, Nodo destino, float peso)
    {
        origen.conexiones.Add(new Arista(destino, peso));
    }
}
