using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GestionAvionesDerribados : MonoBehaviour
{
    private List<Avion> avionesDerribados = new List<Avion>();

    public List<Avion> ObtenerAvionesDerribados()
    {
        return avionesDerribados;
    }


    // Agregar un avión a la lista de derribados
    public void AgregarAvionDerribado(Avion avion)
    {
        if (avion == null)
        {
            Debug.LogError("Intento de agregar un avión nulo a la lista de aviones derribados.");
            return;
        }

        if (!avionesDerribados.Contains(avion))
        {
            avionesDerribados.Add(avion);
        }
        else
        {
            Debug.LogWarning($"Avión con ID {avion.id} ya estaba en la lista de aviones derribados.");
        }
    }


    public Avion ObtenerAvionPorID(string id)
    {
        foreach (Avion avion in avionesDerribados)
        {
            if (avion.id == id)
            {
                return avion;
            }
        }
        return null; // Si no encuentra el avión, devuelve null
    }

    public Avion MostrarAvionesDerribados(string id)
    {
        Debug.Log($"Contenido de avionesDerribados: - Timestamp: {Time.time}");

        if (avionesDerribados == null || avionesDerribados.Count == 0)
        {
            Debug.Log("No hay aviones derribados.");
            return null;
        }

        Avion temp = null;

        foreach (Avion avion in avionesDerribados)
        {
            if (avion.id == id)
            {
                temp = avion;
                Debug.Log("------------------------------------------------------------");
                Debug.Log($"ID: {temp.id}, Combustible: {temp.combustible}, Estado Actual: {temp.estadoActual} - Timestamp: {Time.time}");
                Debug.Log($"ID: {avion.id}, Combustible: {avion.combustible}, Estado Actual: {avion.estadoActual} - Timestamp: {Time.time}");
                if (temp.id == null)
                {
                    Debug.Log("Se fue nulo");
                }
                Debug.Log("------------------------------------------------------------");
                break; // Rompe el ciclo si encontró el avión
            }
        }



        return temp;
    }

    // Ordenar la lista de aviones derribados por ID usando Merge Sort
    public List<Avion> ObtenerAvionesDerribadosOrdenadosPorID()
    {
        return MergeSort(avionesDerribados);
    }

    private List<Avion> MergeSort(List<Avion> lista)
    {
        if (lista.Count <= 1) return lista;

        int mid = lista.Count / 2;
        List<Avion> izquierda = lista.GetRange(0, mid);
        List<Avion> derecha = lista.GetRange(mid, lista.Count - mid);

        izquierda = MergeSort(izquierda);
        derecha = MergeSort(derecha);

        return Merge(izquierda, derecha);
    }

    private List<Avion> Merge(List<Avion> izquierda, List<Avion> derecha)
    {
        List<Avion> resultado = new List<Avion>();
        int i = 0, j = 0;

        while (i < izquierda.Count && j < derecha.Count)
        {
            if (string.Compare(izquierda[i].id, derecha[j].id) <= 0)
            {
                resultado.Add(izquierda[i]);
                i++;
            }
            else
            {
                resultado.Add(derecha[j]);
                j++;
            }
        }

        while (i < izquierda.Count)
        {
            resultado.Add(izquierda[i]);
            i++;
        }

        while (j < derecha.Count)
        {
            resultado.Add(derecha[j]);
            j++;
        }

        return resultado;
    }

    public List<AIModule> ObtenerTripulacionOrdenada(Avion avion, string criterio)
    {
        List<AIModule> tripulacion = new List<AIModule>(avion.aiModules);

        switch (criterio)
        {
            case "ID":
                SelectionSort(tripulacion, (a, b) => a.ID.CompareTo(b.ID));
                break;
            case "Rol":
                SelectionSort(tripulacion, CompararRoles);
                break;
            case "HorasDeVuelo":
                SelectionSort(tripulacion, (a, b) => a.HorasDeVuelo.CompareTo(b.HorasDeVuelo));
                break;
            default:
                Debug.LogWarning("Criterio de orden no reconocido.");
                break;
        }

        return tripulacion;
    }

    // Función personalizada para comparar roles en el orden deseado
    private int CompararRoles(AIModule a, AIModule b)
    {
        // Define el orden de prioridad de los roles
        Dictionary<string, int> prioridades = new Dictionary<string, int>
    {
        { "Pilot", 1 },
        { "Copilot", 2 },
        { "Maintenance", 3 },
        { "Space Awareness", 4 }
    };

        // Obtén las prioridades de los roles, o un valor alto si el rol no está definido
        int prioridadA = prioridades.ContainsKey(a.Rol) ? prioridades[a.Rol] : int.MaxValue;
        int prioridadB = prioridades.ContainsKey(b.Rol) ? prioridades[b.Rol] : int.MaxValue;

        // Compara las prioridades
        return prioridadA.CompareTo(prioridadB);
    }

    private void SelectionSort(List<AIModule> list, Comparison<AIModule> comparison)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (comparison(list[j], list[minIndex]) < 0)
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                AIModule temp = list[i];
                list[i] = list[minIndex];
                list[minIndex] = temp;
            }
        }
    }
}

