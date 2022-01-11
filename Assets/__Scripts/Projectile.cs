using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]                     //a
    private WeaponType _type;            //b


    //это общедоступное свойство маскирует поле _type и обрабатывает 
    //операции присваивания ему новые значения 
    public WeaponType type                      //c
    {
        get { return (_type); }
        set { SetType(value); }                 //c
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundCheck>();
        rend = GetComponent<Renderer>();                     //d
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);              //a
        }
    }
    ///<summary>
    ///Изменяет скрытое поле _type и устанавливает цвет этого снаряда,
    ///как определенно в WeaponDefinition.
    ///</summary>
    ///<param name="eType">Тип WeaponType используемого оружия.</param>
    public void SetType (WeaponType eType)                  //e
    {
        //Установить _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
