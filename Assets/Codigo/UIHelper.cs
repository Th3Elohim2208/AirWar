using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIHelper
{
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("No se encontró un EventSystem en la escena.");
            return false;
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}