using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AvionManager : MonoBehaviour
{
    public GameObject avionPrefab;
    public int cantidadAvionesInicial = 5; // Cantidad inicial de aviones a generar
    public int capacidadHangar = 12; // Capacidad m�xima de hangares
    public float velocidadAvion = 5.0f;
    public float intervaloGeneracionAviones = 10.0f;
    public Text textoContadorAviones;
    public Text textoAvionesDerribados;
    public Dropdown avionDropdown;
    public Dropdown criterioDropdown;
    public Text textoTripulacion;
    private List<GameObject> aviones = new List<GameObject>();
    private Grafo grafo;
    private int contadorAvionesDestruidos = 0;
    private GestionAvionesDerribados gestionAvionesDerribados;
    public int ContadorAvionesDestruidos => contadorAvionesDestruidos;

    void OnEnable()
    {
        GeneradorGrafo.OnGrafoInicializado += InicializarAviones;
        Avion.OnAvionDestruido += ManejarAvionDestruido;
    }

    void OnDisable()
    {
        GeneradorGrafo.OnGrafoInicializado -= InicializarAviones;
        Avion.OnAvionDestruido -= ManejarAvionDestruido;
    }

    void Start()
    {
        gestionAvionesDerribados = GetComponent<GestionAvionesDerribados>();
        if (gestionAvionesDerribados == null)
        {
            Debug.LogError("No se encontr� el componente GestionAvionesDerribados en AvionManager.");
            return;
        }

        // Configura el dropdown de selecci�n de criterio
        criterioDropdown.onValueChanged.AddListener(delegate { ActualizarTripulacion(); });
        avionDropdown.onValueChanged.AddListener(delegate { ActualizarTripulacion(); });

        // Llama a este m�todo despu�s de confirmar que gestionAvionesDerribados no es null
        ActualizarListaAvionesDerribados();

        StartCoroutine(GenerarAvionesPeriodicamente());
    }

    void InicializarAviones()
    {
        grafo = FindObjectOfType<GeneradorGrafo>().grafo;

        if (grafo != null && grafo.nodos.Count > 0)
        {
            for (int i = 0; i < cantidadAvionesInicial; i++)
            {
                GenerarAvion();
            }
        }
        else
        {
            Debug.LogError("No se pudo acceder al grafo en AvionManager.");
        }
    }

    IEnumerator GenerarAvionesPeriodicamente()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloGeneracionAviones);

            // Verifica cu�ntos aviones se pueden generar sin exceder el l�mite de hangares
            int espacioDisponible = capacidadHangar - aviones.Count;
            if (espacioDisponible > 0)
            {
                List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");
                int avionesACrear = Mathf.Min(3, espacioDisponible, aeropuertos.Count); // Limita a 3 o menos, seg�n el espacio y n�mero de aeropuertos

                for (int i = 0; i < avionesACrear; i++)
                {
                    GenerarAvionEnPosicion(aeropuertos[i].posicion);
                }
            }
            else
            {
                Debug.LogWarning("Capacidad de hangares alcanzada. No se pueden generar m�s aviones.");
            }
        }
    }

    void GenerarAvion()
    {
        Vector3 posicionInicial = ObtenerPosicionInicial();
        GenerarAvionEnPosicion(posicionInicial);
    }

    void GenerarAvionEnPosicion(Vector3 posicion)
    {
        if (aviones.Count < capacidadHangar)
        {
            GameObject avionObj = Instantiate(avionPrefab, posicion, Quaternion.identity);
            Avion avion = avionObj.GetComponent<Avion>();

            if (avion != null)
            {
                avion.id = Guid.NewGuid().ToString();
                avion.velocidadVuelo = velocidadAvion;
                avion.grafo = grafo;
                aviones.Add(avionObj);
            }
        }
        else
        {
            Debug.LogWarning("Capacidad de hangares alcanzada. No se pueden generar m�s aviones.");
        }
    }

    Vector3 ObtenerPosicionInicial()
    {
        List<Nodo> aeropuertos = grafo.nodos.FindAll(n => n.tipo == "aeropuerto");

        if (aeropuertos.Count > 0)
        {
            Nodo nodoInicial = aeropuertos[UnityEngine.Random.Range(0, aeropuertos.Count)];
            return nodoInicial.posicion;
        }
        else
        {
            Debug.LogError("No se encontraron aeropuertos en el grafo para generar aviones.");
            return Vector3.zero;
        }
    }

    private void ManejarAvionDestruido(Avion avion)
    {
        contadorAvionesDestruidos++;
        textoContadorAviones.text = "Aviones Destruidos: " + contadorAvionesDestruidos;

        // Asegura que el avi�n tenga m�dulos de AI
        if (avion.aiModules == null || avion.aiModules.Count == 0)
        {
            avion.InicializarAIModules();
          
        }

        // Agregar el avi�n destruido a la lista en GestionAvionesDerribados
        Debug.Log($"Agregando avi�n a avionesDerribados.- Timestamp: {Time.time}");
        gestionAvionesDerribados.AgregarAvionDerribado(avion);
        Debug.Log($"Avi�n con ID {avion.id} agregado a avionesDerribados.- Timestamp: {Time.time}");

        // Eliminar el avi�n de la lista de aviones activos
        aviones.Remove(avion.gameObject);

        // Actualizar la lista en la interfaz de usuario
        ActualizarListaAvionesDerribados();
    }

    private void ActualizarListaAvionesDerribados()
    {
        Debug.Log($"Actualizando lista de aviones derribados. IDs en avionesDerribados:- Timestamp: {Time.time}");
        foreach (Avion avion in gestionAvionesDerribados.ObtenerAvionesDerribados())
        {
            Debug.Log(avion.id + $"picha - Timestamp: {Time.time}");
        }

        List<Avion> avionesDerribados = gestionAvionesDerribados.ObtenerAvionesDerribados();
        avionDropdown.ClearOptions(); // Limpiar opciones previas

        foreach (Avion avion in avionesDerribados)
        {
            Dropdown.OptionData option = new Dropdown.OptionData
            {
                text = $"ID: {avion.id}" // Esto se muestra al usuario
            };
            avionDropdown.options.Add(option);
        }

        avionDropdown.RefreshShownValue();
        ActualizarTripulacion(); // Llama a la funci�n para actualizar la tripulaci�n mostrada
    }

    private void ActualizarTripulacion()
    {
        if (avionDropdown.options.Count == 0) return;

        // Obtener el ID directamente del texto seleccionado en el Dropdown
        string textoSeleccionado = avionDropdown.options[avionDropdown.value].text;
        string avionID = textoSeleccionado.Replace("ID: ", ""); // Elimina el prefijo "ID: "
        Debug.Log($"Avi�n seleccionado en dropdown: {avionID} - Timestamp: {Time.time}");
        gestionAvionesDerribados.MostrarAvionesDerribados(avionID);

        Avion avionSeleccionado1= gestionAvionesDerribados.ObtenerAvionPorID(avionID);
        Debug.Log($"nica- Timestamp: {Time.time}");
        Avion avionSeleccionado = gestionAvionesDerribados.MostrarAvionesDerribados(avionID);


        // Verifica que el avi�n seleccionado exista
        if (avionSeleccionado.id==null)
        {
            textoTripulacion.text = "No se encontr� la tripulaci�n para el avi�n seleccionado.";
            Debug.LogWarning($"El avi�n con ID {avionID} no se encontr� en avionesDerribados.");
            return;
        }

        // Verificar si aiModules est� inicializado y tiene elementos
        Debug.Log($"Avi�n {avionSeleccionado.id} tiene {avionSeleccionado.aiModules.Count} m�dulos de AI.");

        // Obtener el criterio de ordenamiento seleccionado
        string criterio = criterioDropdown.options[criterioDropdown.value].text;
        List<AIModule> tripulacionOrdenada = gestionAvionesDerribados.ObtenerTripulacionOrdenada(avionSeleccionado, criterio);

        // Mostrar la tripulaci�n del avi�n seleccionado
        textoTripulacion.text = $"Tripulaci�n de Avi�n ID {avionSeleccionado.id}:\n";
        foreach (AIModule module in tripulacionOrdenada)
        {
            textoTripulacion.text += $"ID: {module.ID}, Rol: {module.Rol}, Horas de Vuelo: {module.HorasDeVuelo}\n";
        }
    }
}
