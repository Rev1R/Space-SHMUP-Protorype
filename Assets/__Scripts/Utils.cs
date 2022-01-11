﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //==================== Функции для работы с материалом ====================\\

    //Возвращает список всех материалов в данном игровом объекте и его дочерних объектах

    static public Material[] GetAllMaterials(GameObject go)              //a
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();       //b

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)                   //c
        {
            mats.Add(rend.material);
        }
        return (mats.ToArray());        //d
    }
}
