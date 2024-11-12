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

    public bool ReabastecerAvion(ref float combustibleAvion)
    {
        if (combustibleDisponible > 0)
        {
            float cantidadReabastecida = Mathf.Min(combustibleDisponible, 100.0f - combustibleAvion);
            combustibleAvion += cantidadReabastecida;
            combustibleDisponible -= cantidadReabastecida;

            ActualizarTextoCombustible();

            if (combustibleDisponible <= 0)
            {
                Debug.Log("El aeropuerto se ha quedado sin combustible.");
            }

            return true;
        }
        else
        {
            Debug.LogWarning("El aeropuerto no tiene combustible disponible para reabastecer al avión.");
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
