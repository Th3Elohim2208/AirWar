using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionAvionesDerribados : MonoBehaviour
{
    private List<Avion> avionesDerribados = new List<Avion>();

    // Agregar un avión a la lista de derribados
    public void AgregarAvionDerribado(Avion avion)
    {
        avionesDerribados.Add(avion);
        Debug.Log($"Avión con ID {avion.id} ha sido añadido a la lista de aviones derribados.");
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
}
