using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIModule
{
    public string ID { get; private set; }
    public string Rol { get; private set; } // Definición para Rol
    public int HorasDeVuelo { get; private set; } // Definición para HorasDeVuelo

    public AIModule(string rol, string id, int horasDeVuelo)
    {
        Rol = rol;
        ID = id;
        HorasDeVuelo = horasDeVuelo;
    }
}
