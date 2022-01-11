﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это перечисление всех возможных типов оружия.
/// Также включает тип "Shield", чтобы дать возможность совершенствовать защиту.
/// Аббривиатурой [HP] ниже отмеченны элементы, не реализованные в этой книге.
/// </summary>

    public enum WeaponType
{
    none,                  //по умолчанию / нет оружия
    blaster,               //простой бластер
    spread,                //веерная пушка, стрелябщаяя несколькими снарядами
    phase,                 //[HP] воновой фазер
    missile,               //[HP] самонаводящиеся ракеты
    laser,                 //[HP] наносит повреждения при долговременном воздействии
    shield                 //увеличивает shieldLevel
}

/// <summary>
/// Класс WeaponDefinition позволяет настраивать свойства
/// конкретного вида оружия в инспекторе. Для этого класс Main
/// будет хранить массив элементов типа WeaponDefinition.
/// </summary>

    [System.Serializable]                             //a
    public class WeaponDefinition                     //b
{
    public WeaponType type = WeaponType.none;
    public string letter;                     //буква на кубике изображающем бонус
    public Color color = Color.white;         //цвет ствола оружия и кубика бонуса
    public GameObject projectilePrefab;       //шаблон снарядов 
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;             //разрушительная мощность 
    public float countinuousDamage = 0;       //степень разрушения в секунду (для Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20;               //корость полета снарядов
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]   [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; //Время последнего выстрела
    private Renderer collarRend;

    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        //Вызвать SetType(), чтобы заменить тип оружия по умолчанию
        //WeaponType.none 
        SetType(_type);         //a
        //Динамически создать точку привязки для всех снарядов
        if (PROJECTILE_ANCHOR == null)         //b
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //Найти fireDelegate в корневом игровом объекте
        GameObject rootGO = transform.root.gameObject;          //c
        if (rootGO.GetComponent<Hero>() != null)                //d
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }
    public void SetType( WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)           //e
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);              //f
        collarRend.material.color = def.color;
        lastShotTime = 0;   //Сразу после установки _type можно выстрелить  //g
    }
    public void Fire()
    {
        //Если this.gameObject неактивен, выйти
        if (!gameObject.activeInHierarchy) return;          //h
        //Если между выстрелами прошло недостаточно много времени, выйти
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;             //j
        if(transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();  //снаряд летящий прямо
                p.rigid.velocity = vel;
                p = MakeProjectile(); //снаряд летящий вправо
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p= MakeProjectile(); //снаряд летящий влево
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }
    public Projectile MakeProjectile()         //m
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")    //n
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return (p);
    }
}
