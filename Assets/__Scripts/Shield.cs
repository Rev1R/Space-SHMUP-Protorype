using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set In Inspector")]
    public float rotationPerSecond = 0.1f;
    [Header("Set Dynamically")]
    public int levelShown = 0;
    //Скрытые переменные, не появляющиеся в инспекторе 
    Material mat;                                        //a

    void Start()
    {
        mat = GetComponent<Renderer>().material;           //b
    }

    void Update()
    {
        //прочитать текущую мощность защитного поля из объекта-одиночки Hero
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);              //c
        //Если она отлиается от levelShown ...
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            //скорректировать смещение в текстуре, чтобы отобразить поле с другой мощностью
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);         //d
        }
        //поворачивать поле в каждом кадре с постоянной скоростью
        float rZ = -(rotationPerSecond * Time.time * 360) % 360f;      //e
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
