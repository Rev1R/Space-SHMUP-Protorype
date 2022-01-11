using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//В каждой из четырех следующих строк сначала введите /// и затем нажмите Tab
/// <summary>
///  предотвращает выход игрового объекта за границы экрана.
///  Важно : работает ТОЛЬКО с ортографической камерой Main Camera [0 , 0 , 0].
/// </summary>

public class BoundCheck : MonoBehaviour                       //a
{
    [Header("Set In Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;     //a
    [Header("Set Dynamically")]
    public bool isOnScreen = true;       //b
    public float camWidth;
    public float camHeight;
    [HideInInspector]
    public bool offRight, offLeft, offUp, offDawn;        //a


    void Awake()
    {
        camHeight = Camera.main.orthographicSize;           //b
        camWidth = camHeight * Camera.main.aspect;          //c
    }

    void LateUpdate()                                        //d
    {
        Vector3 pos = transform.position;                     //c
        isOnScreen = true;                               //d
        offRight = offLeft = offUp = offDawn = false;              //b

        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;                               //c
        }
        if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;                              //c

        }
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;                            //c
        }
        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDawn = true;                                //c
        }
        isOnScreen = !(offRight || offLeft || offUp || offDawn);  //d
        if (keepOnScreen && !isOnScreen)                        //f
        {
            transform.position = pos;                           //g
            isOnScreen = true;
            offRight = offLeft = offUp = offDawn = false;    //e
        }

    }
        
        //Рисует границы в панели Scene (Сцена) с помощью OnDrawGizmos()
    void OnDrawGizmos()                                                //e
    {
            if (!Application.isPlaying) return;
            Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
            Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
      
    
}

