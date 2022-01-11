using System.Collections;                             //необходимо для доступа к массивам и другим коллекциям
using System.Collections.Generic;                     //необходимо для доступа к спискам и словарям
using UnityEngine;                                    //необходимо для доступа к Unity

public class Enemy : MonoBehaviour
{
    [Header("Set In Inspector: Enemy")]
    public float speed = 10f;      //Скорос в м/c 
    public float fireRate = 0.3f;  //Секунд между выстрелами (не используется)
    public float health = 10;
    public int score = 100;   //Очки за уничтожение этого корабля
    public float showDamageDuration = 0.1f;    //а    //длительность эффекта // попадания в секундах

    public float powerUpDropChance = 1f;  //вероятность сбросить бонус //a

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;   //Все материалы игрового объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime;   //время прекращения отображения эффекта
    public bool notifiedOfDestruction = false;  // будет использованно позже


    protected BoundCheck bndCheck;      //a
    void Awake()
    {
        bndCheck = GetComponent<BoundCheck>();        //b
        //Получить материалы и цветэтого игровоого обьекта и его потомков 
        materials = Utils.GetAllMaterials(gameObject);      //b
        originalColors = new Color[materials.Length];
        for(int i =0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }



    //Это свойство: метод, действующий как поле
    public Vector3 pos                //a
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    
    void Update()
    {
        Move();
        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        if(bndCheck != null && bndCheck.offDawn)                 //c
        { //Корабль за нижней границей. поэтому его нужно уничтожить
                Destroy(gameObject);
        }
    }
    public virtual void Move()                  //b
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    void OnCollisionEnter(Collision coll )
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":               //b
                Projectile p = otherGO.GetComponent<Projectile>();
                //Если вражеский корабль за границами экрана, не наносить ему повреждений.
                if (!bndCheck.isOnScreen)             //c
                {
                    Destroy(otherGO);
                    break;
                }
                //поразить вражеский корабль
                ShowDamage();          //d
                //получить разрушающую силу из WEAP_DICT в классе Main.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)                         //d
                {
                    //Сообщить обьекту-одиночке Main об уничтожении    //b
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    //уничтожить этот вражеский корабль
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);                //e
                break;
            default:
                print("Enemy hit by non-ProjetileHero: " + otherGO.name);   //f
                break;
        }
    }
    void ShowDamage()                        //e
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()                  //f
    {
        for(int i=0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
