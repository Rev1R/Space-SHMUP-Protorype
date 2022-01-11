using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //Одиночка    //a
    [Header("Set In Inspector")]
    //Поля, управляющие движением корабля
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamycally")]
    [SerializeField]
    private float _shieldLevel = 1;   //обратите внимание на символ подчеркивания

    //Эта переменная хранит ссылку на последний столкнувшийся объект 
    private GameObject lastTriggerGo = null;     //a

    //Объявление нового делегата типа WeaponFireDelegate 
    public delegate void WeaponFireDelegate();               //a
    //Создать поле типа WeaponFireDelegate с именем fireDelegate.
    public WeaponFireDelegate fireDelegate;

    void Start()
    {
        if (S == null)
        {
            S = this;  // Сохранить ссылку на одиночку   //a
        }
        else
        {
            Debug.LogError("Hero.Awake()-Attempted to assign second Hero.S1");
        }
        // fireDelegate += TempFire;       //b
        //Очистить массив weapons и начать игру с 1 бластером
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }



    void Update()
    {
        //Извлечь информацию из класса Input 
        float xAxis = Input.GetAxis("Horizontal");     //b
        float yAxis = Input.GetAxis("Vertical");        //b
        // Изменить transform.position, опираясь на информацию по осям 
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        //Повернуть корабль чтобы придать ощущение динамизма    //c
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //позволит кораблю выстреллить
       /* if (Input.GetKeyDown(KeyCode.Space))               //a
        {
            TempFire();
        }*/

        /*произвести выстрел из всех видов оружия вызовом fireDelegate, сначала проверить нажатие клавиши: Axis("Jump")
         Затем убедиться, что значение fireDelegate не равно null, чтобы избежать ошибки.*/
         if (Input.GetAxis("Jump")==1 && fireDelegate != null)          //d
        {
            fireDelegate();                //e
        }
    }
    void TempFire()                                        //b
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        //rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGO.GetComponent<Projectile>();           //h
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }
    void OnTriggerEnter(Collider other)
    {
        // print("Triggered: " + other.gameObject.name);
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);            //b
        //Гарантировать невозможность повторного столкновения с тем же объектом 
        if (go == lastTriggerGo)                      //c
        {
            return;
        }
        lastTriggerGo = go;   //d
        if (go.tag == "Enemy")               //если защитное поле столкнулось с вражеским
        {                                    //кораблем
            shieldLevel--;                   //уменьшить уровень защиты на 1
            Destroy(go);                     //... и уничтожить врага                 //e
        }else if(go.tag == "PowerUp")
        {
            //Если защитное поле столкнулось с бонусом
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);                              //f
        }

    }
    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:               //a
                shieldLevel++;
                break;
            default:                              //b
                if (pu.type == weapons[0].type)   //c
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        //Установить в pu.Type
                        w.SetType(pu.type);
                    }
                }
                else        //d
                {
                    //Если оружие другого типа 
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
      pu.AbsorbedBy(this.gameObject);
        
    }
    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);              //a
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);  //b
                                                 //если уровень поля упал до нуля или ниже
            if (value < 0)                       //c
            {
                Destroy(this.gameObject);
                //Сообщить объекту Main.S о необходимости перезапустить игру
                Main.S.DelayedRestart(gameRestartDelay);        //a
            }
        }
    }
    Weapon GetEmptyWeaponSlot()
    {
        for(int i=0; i<weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }
    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
