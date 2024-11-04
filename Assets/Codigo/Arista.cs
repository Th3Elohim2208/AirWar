using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arista 
{
    public Nodo destino;
    public float peso;

    public Arista(Nodo destino, float peso)
    {
        this.destino = destino;
        this.peso = peso;
    }
}
