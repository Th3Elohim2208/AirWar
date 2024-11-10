using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo
{
    public Vector2 posicion;
    public string tipo; // "aeropuerto" o "portaviones"
    public List<Arista> conexiones; // Rutas conectadas a este nodo

    public Nodo(Vector2 posicion, string tipo)
    {
        this.posicion = posicion;
        this.tipo = tipo;
        conexiones = new List<Arista>();
    }
}