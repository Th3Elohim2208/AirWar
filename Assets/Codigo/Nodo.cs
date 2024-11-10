using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo
{
    public Vector2 posicion;
    public string tipo;
    public List<Arista> conexiones;
    public float combustibleDisponible;
    public TextMesh textoCombustible;

    public Nodo(Vector2 posicion, string tipo, float combustibleInicial = 0)
    {
        this.posicion = posicion;
        this.tipo = tipo;
        this.conexiones = new List<Arista>();

        if (tipo == "aeropuerto")
        {
            combustibleDisponible = combustibleInicial;
        }
    }

    public bool ReabastecerAvion(float cantidad)
    {
        if (combustibleDisponible >= cantidad)
        {
            combustibleDisponible -= cantidad;
            ActualizarTextoCombustible();
            return true;
        }
        else
        {
            Debug.LogWarning("El aeropuerto no tiene suficiente combustible para reabastecer al avión.");
            return false;
        }
    }

    public void ActualizarTextoCombustible()
    {
        if (textoCombustible != null)
        {
            textoCombustible.text = "Combustible: " + combustibleDisponible;
        }
    }
}
