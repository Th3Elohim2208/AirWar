using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Avion))]
public class AvionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Avion avion = (Avion)target;

        // Mostrar la lista de m�dulos de AI manualmente
        if (avion.aiModules != null)
        {
            EditorGUILayout.LabelField("M�dulos de AI:");
            foreach (var module in avion.aiModules)
            {
                EditorGUILayout.LabelField("Rol: " + module.Rol);
                EditorGUILayout.LabelField("ID: " + module.ID);
                EditorGUILayout.LabelField("Horas de Vuelo: " + module.HorasDeVuelo);
                EditorGUILayout.Space();
            }
        }
    }
}
