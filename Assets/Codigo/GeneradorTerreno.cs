using System.Collections.Generic;
using UnityEngine;

public class GeneradorTerreno : MonoBehaviour
{
    public GameObject aeropuertoPrefab;
    public GameObject portavionesPrefab;
    public int cantidadAeropuertos = 3;
    public int cantidadPortaviones = 2;
    public float distanciaMinimaEntrePortaviones = 2.0f;
    public float distanciaMinimaEntreAeropuertos = 2.0f;
    private List<Vector2> posicionesPortaviones = new List<Vector2>();
    private List<Vector2> posicionesAeropuertos = new List<Vector2>();
    private Grafo grafo;

    private Rect areaTierra1 = new Rect(-9.8f, -4.2f, 5.2f, 9.3f); // Área de tierra izquierda
    private Rect areaTierra2 = new Rect(4.5f, -4.2f, 5.2f, 9.3f);  // Área de tierra derecha
    private Rect areaMar = new Rect(-2.8f, -4.2f, 5.2f, 9.3f); // Área de mar en el centro

    void Start()
    {
        grafo = new Grafo();
        GenerarAeropuertos();
        GenerarPortaviones();

        // Inicializar el grafo en GeneradorGrafo si es necesario
        var generadorGrafo = FindObjectOfType<GeneradorGrafo>();
        if (generadorGrafo != null)
        {
            generadorGrafo.InicializarGrafo(grafo);
        }
        else
        {
            Debug.LogError("GeneradorGrafo no encontrado en la escena.");
        }
    }

    void GenerarAeropuertos()
    {
        for (int i = 0; i < cantidadAeropuertos; i++)
        {
            Vector2 posicion;
            bool posicionValida = false;
            int intentos = 0;
            int maxIntentos = 50;

            do
            {
                intentos++;
                posicion = ObtenerPosicionEnTierra();
                posicionValida = EsPosicionValida(posicion, posicionesAeropuertos, distanciaMinimaEntreAeropuertos) &&
                                 (areaTierra1.Contains(posicion) || areaTierra2.Contains(posicion));

                if (intentos >= maxIntentos && !posicionValida)
                {
                    break;
                }
            }
            while (!posicionValida);

            if (posicionValida)
            {
                posicionesAeropuertos.Add(posicion);
                float combustibleInicial = 500f; // Cantidad inicial de combustible para el aeropuerto

                // Crear el nodo del aeropuerto con combustible inicial y añadirlo al grafo
                Nodo nodo = new Nodo(posicion, "aeropuerto", combustibleInicial);
                grafo.AgregarNodo(nodo);

                // Instanciar el prefab del aeropuerto en la posición aleatoria
                GameObject aeropuertoObj = Instantiate(aeropuertoPrefab, posicion, Quaternion.identity);

                // Crear el TextMesh para mostrar el combustible restante
                GameObject textoCombustible = new GameObject("TextoCombustible");
                textoCombustible.transform.position = new Vector3(posicion.x, posicion.y + 0.5f, 0); // Ajusta la posición en Y para que esté encima
                TextMesh textMesh = textoCombustible.AddComponent<TextMesh>();
                textMesh.text = "Combustible: " + combustibleInicial;
                textMesh.fontSize = 30;
                textMesh.characterSize = 0.1f;
                textMesh.alignment = TextAlignment.Center;
                textMesh.anchor = TextAnchor.MiddleCenter;

                // Vincula el TextMesh al Nodo del aeropuerto
                nodo.textoCombustible = textMesh;
            }
        }
    }

    void GenerarPortaviones()
    {
        for (int i = 0; i < cantidadPortaviones; i++)
        {
            Vector2 posicion;
            bool posicionValida = false;
            int intentos = 0;
            int maxIntentos = 50;

            do
            {
                intentos++;
                posicion = ObtenerPosicionEnMar();
                posicionValida = EsPosicionValida(posicion, posicionesPortaviones, distanciaMinimaEntrePortaviones) &&
                                 areaMar.Contains(posicion);

                if (intentos >= maxIntentos && !posicionValida)
                {
                    Debug.LogWarning("No se encontró una posición válida en el mar para el portaaviones tras varios intentos.");
                    break;
                }
            }
            while (!posicionValida);

            if (posicionValida)
            {
                posicionesPortaviones.Add(posicion);
                Nodo nodo = new Nodo(posicion, "portaviones");
                grafo.AgregarNodo(nodo);
                Instantiate(portavionesPrefab, posicion, Quaternion.identity);
            }
        }
    }

    bool EsPosicionValida(Vector2 posicion, List<Vector2> posicionesExistentes, float distanciaMinima)
    {
        foreach (Vector2 pos in posicionesExistentes)
        {
            if (Vector2.Distance(posicion, pos) < distanciaMinima)
            {
                return false;
            }
        }
        return true;
    }

    Vector2 ObtenerPosicionEnMar()
    {
        float x = Random.Range(areaMar.xMin + 1.0f, areaMar.xMax - 1.0f);
        float y = Random.Range(areaMar.yMin + 1.0f, areaMar.yMax - 1.0f);
        return new Vector2(x, y);
    }

    Vector2 ObtenerPosicionEnTierra()
    {
        Rect areaSeleccionada = (Random.Range(0, 2) == 0) ? areaTierra1 : areaTierra2;
        float x = Random.Range(areaSeleccionada.xMin + 1.0f, areaSeleccionada.xMax - 1.0f);
        float y = Random.Range(areaSeleccionada.yMin + 1.0f, areaSeleccionada.yMax - 1.0f);
        return new Vector2(x, y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(areaTierra1.center.x, areaTierra1.center.y, 0), new Vector3(areaTierra1.width, areaTierra1.height, 1));
        Gizmos.DrawWireCube(new Vector3(areaTierra2.center.x, areaTierra2.center.y, 0), new Vector3(areaTierra2.width, areaTierra2.height, 1));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(areaMar.center.x, areaMar.center.y, 0), new Vector3(areaMar.width, areaMar.height, 1));
    }
}
