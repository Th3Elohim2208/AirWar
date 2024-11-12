using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorGrafo : MonoBehaviour
{
    public GameObject aeropuertoPrefab;
    public GameObject portavionesPrefab;
    public int cantidadAeropuertos = 3;
    public int cantidadPortaviones = 2;

    public Grafo grafo; // Cambiado a público
    public GameObject textoPesoPrefab; // Prefab de texto para los pesos

    // Evento para indicar que el grafo ha sido inicializado
    public static event Action OnGrafoInicializado;

    public void InicializarGrafo(Grafo grafo)
    {
        this.grafo = grafo;
    }

    void Start()
    {
        GenerarRutas();
        if (grafo != null && grafo.nodos.Count > 0)
        {
            OnGrafoInicializado?.Invoke(); // Notifica que el grafo está listo
        }
        else
        {
            Debug.LogWarning("No se encontraron nodos en el grafo al inicializar en GeneradorGrafo.");
        }
    }

    void GenerarRutas()
    {
        // Obtener listas de aeropuertos y portaviones
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");
        List<Nodo> portaviones = grafo.nodos.FindAll(n => n.tipo == "portaviones");

        // Crear conexiones entre aeropuertos
        foreach (Nodo origen in aeropuertos)
        {
            int conexiones = UnityEngine.Random.Range(2, 5); // Intenta entre 2 y 5 conexiones
            for (int i = 0; i < conexiones; i++)
            {
                Nodo destino = ObtenerNodoAleatorio(origen, aeropuertos, portaviones);
                if (destino != null)
                {
                    float peso = CalcularPesoRuta(origen, destino);
                    grafo.AgregarArista(origen, destino, peso);
                    grafo.AgregarArista(destino, origen, peso); // Conexión bidireccional
                    CrearTextoPeso(origen, destino, peso);
                    CrearRutaVisual(origen, destino);
                }
            }
        }

        // Crear conexiones entre portaaviones y otros portaaviones
        foreach (Nodo origen in portaviones)
        {
            int conexiones = UnityEngine.Random.Range(1, 3); // Entre 1 y 3 conexiones para portaaviones
            for (int i = 0; i < conexiones; i++)
            {
                Nodo destino = ObtenerNodoAleatorio(origen, aeropuertos, portaviones);
                if (destino != null)
                {
                    float peso = CalcularPesoRuta(origen, destino);
                    grafo.AgregarArista(origen, destino, peso);
                    grafo.AgregarArista(destino, origen, peso); // Conexión bidireccional
                    CrearTextoPeso(origen, destino, peso);
                    CrearRutaVisual(origen, destino);
                }
            }
        }

        // Verificar y reforzar la conectividad del grafo
        if (!grafo.VerificarConectividad())
        {
            Debug.LogWarning("El grafo no está completamente conectado. Añadiendo conexiones adicionales...");
            AsegurarConectividadCompleta(aeropuertos, portaviones);
        }
    }


    void AsegurarConectividadCompleta(List<Nodo> aeropuertos, List<Nodo> portaviones)
    {
        List<Nodo> todosNodos = new List<Nodo>(aeropuertos);
        todosNodos.AddRange(portaviones);

        for (int i = 0; i < todosNodos.Count; i++)
        {
            for (int j = i + 1; j < todosNodos.Count; j++)
            {
                if (!grafo.ExisteRuta(todosNodos[i], todosNodos[j]))
                {
                    float peso = CalcularPesoRuta(todosNodos[i], todosNodos[j]);
                    grafo.AgregarArista(todosNodos[i], todosNodos[j], peso);
                    grafo.AgregarArista(todosNodos[j], todosNodos[i], peso); // Conexión bidireccional
                }
            }
        }
    }

    Nodo ObtenerNodoAleatorio(Nodo actual, List<Nodo> aeropuertos, List<Nodo> portaviones)
    {
        List<Nodo> posiblesDestinos = new List<Nodo>(aeropuertos);
        posiblesDestinos.AddRange(portaviones);

        Nodo nodoAleatorio;
        do
        {
            nodoAleatorio = posiblesDestinos[UnityEngine.Random.Range(0, posiblesDestinos.Count)];
        } while (nodoAleatorio == actual);

        return nodoAleatorio;
    }

    float CalcularPesoRuta(Nodo origen, Nodo destino)
    {
        float distancia = Vector2.Distance(origen.posicion, destino.posicion);
        float peso = distancia;

        float costoAeropuertoAPortaaviones = 5.0f;
        float costoPortaavionesAPortaaviones = 10.0f;
        float costoAeropuertoDiferenteBloque = 20.0f;

        if (origen.tipo == "aeropuerto" && destino.tipo == "aeropuerto")
        {
            if (EstanEnElMismoBloque(origen, destino))
            {
                peso = distancia;
            }
            else
            {
                peso = distancia + costoAeropuertoDiferenteBloque;
            }
        }
        else if ((origen.tipo == "aeropuerto" && destino.tipo == "portaviones") || (origen.tipo == "portaviones" && destino.tipo == "aeropuerto"))
        {
            peso = distancia + costoAeropuertoAPortaaviones;
        }
        else if (origen.tipo == "portaviones" && destino.tipo == "portaviones")
        {
            peso = distancia + costoPortaavionesAPortaaviones;
        }

        return peso;
    }

    bool EstanEnElMismoBloque(Nodo nodo1, Nodo nodo2)
    {
        Rect areaTierra1 = new Rect(-13.0f, -5.7f, 6.5f, 12.5f);
        Rect areaTierra2 = new Rect(3.5f, -5.7f, 6.5f, 12.5f);

        bool enMismoBloque = (areaTierra1.Contains(nodo1.posicion) && areaTierra1.Contains(nodo2.posicion)) ||
                             (areaTierra2.Contains(nodo1.posicion) && areaTierra2.Contains(nodo2.posicion));

        return enMismoBloque;
    }

    void CrearTextoPeso(Nodo origen, Nodo destino, float peso)
    {
        Vector2 posicionTexto = (origen.posicion + destino.posicion) / 2;
        GameObject textoPeso = Instantiate(textoPesoPrefab, posicionTexto, Quaternion.identity);
        textoPeso.GetComponent<TextMesh>().text = peso.ToString("F1");
    }

    void CrearRutaVisual(Nodo origen, Nodo destino)
    {
        GameObject lineaObj = new GameObject("LineaRuta");
        LineRenderer lineRenderer = lineaObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origen.posicion);
        lineRenderer.SetPosition(1, destino.posicion);

        Material material = new Material(Shader.Find("Sprites/Default"));

        Texture2D texture = new Texture2D(2, 1);
        texture.SetPixel(0, 0, Color.clear);
        texture.SetPixel(1, 0, Color.white);
        texture.Apply();
        material.mainTexture = texture;

        material.mainTextureScale = new Vector2(10.0f, 1);

        lineRenderer.material = material;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
}
