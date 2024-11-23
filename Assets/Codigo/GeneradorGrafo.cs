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

    public Grafo grafo;
    public GameObject textoPesoPrefab;

    public static event Action OnGrafoInicializado;

    public void InicializarGrafo(Grafo grafo)
    {
        this.grafo = grafo;
    }

    void Start()
    {
        GenerarRutas();

        // Imprimir todas las conexiones del grafo
        grafo.ImprimirConexiones();

        if (grafo != null && grafo.nodos.Count > 0)
        {
            OnGrafoInicializado?.Invoke();
        }
        else
        {
            Debug.LogWarning("No se encontraron nodos en el grafo al inicializar en GeneradorGrafo.");
        }
    }
    void GenerarRutas()
    {
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");
        List<Nodo> portaviones = grafo.nodos.FindAll(n => n.tipo == "portaviones");

        // Generar conexiones entre aeropuertos
        foreach (Nodo origen in aeropuertos)
        {
            int conexiones = UnityEngine.Random.Range(2, 5);
            for (int i = 0; i < conexiones; i++)
            {
                Nodo destino = ObtenerNodoAleatorio(origen, aeropuertos, portaviones);

                if (destino != null && !grafo.ExisteArista(origen, destino))
                {
                    float peso = CalcularPesoRuta(origen, destino);
                    grafo.AgregarArista(origen, destino, peso);
                    CrearTextoPeso(origen, destino, peso); // Crear texto de peso
                    CrearRutaVisual(origen, destino);      // Dibujar la ruta visualmente
                }
            }
        }

        // Generar conexiones entre portaviones
        foreach (Nodo origen in portaviones)
        {
            int conexiones = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < conexiones; i++)
            {
                Nodo destino = ObtenerNodoAleatorio(origen, aeropuertos, portaviones);

                if (destino != null && !grafo.ExisteArista(origen, destino))
                {
                    float peso = CalcularPesoRuta(origen, destino);
                    grafo.AgregarArista(origen, destino, peso);
                    CrearTextoPeso(origen, destino, peso); // Crear texto de peso
                    CrearRutaVisual(origen, destino);      // Dibujar la ruta visualmente
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

        return (areaTierra1.Contains(nodo1.posicion) && areaTierra1.Contains(nodo2.posicion)) ||
               (areaTierra2.Contains(nodo1.posicion) && areaTierra2.Contains(nodo2.posicion));
    }

    void AsegurarConectividadCompleta(List<Nodo> aeropuertos, List<Nodo> portaviones)
    {
        foreach (Nodo nodo in aeropuertos)
        {
            if (nodo.conexiones.Count == 0)
            {
                Nodo conexion = ObtenerNodoAleatorio(nodo, aeropuertos, portaviones);
                if (conexion != null)
                {
                    float peso = CalcularPesoRuta(nodo, conexion);
                    grafo.AgregarArista(nodo, conexion, peso);
                }
            }
        }
    }


    void CrearTextoPeso(Nodo origen, Nodo destino, float peso)
    {
        if (textoPesoPrefab == null)
        {
            Debug.LogError("Prefab para el texto de peso no asignado.");
            return;
        }

        Vector2 posicionTexto = (origen.posicion + destino.posicion) / 2;
        GameObject textoPeso = Instantiate(textoPesoPrefab, posicionTexto, Quaternion.identity);
        TextMesh textMesh = textoPeso.GetComponent<TextMesh>();

        if (textMesh != null)
        {
            textMesh.text = peso.ToString("F1");
            textMesh.fontSize = 10;
            textMesh.color = Color.black;
            Debug.Log($"Texto de peso creado en {posicionTexto} con valor {peso}");
        }
        else
        {
            Debug.LogWarning("El prefab de texto no contiene un componente TextMesh.");
        }
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
