using System.Collections;                          //необходимо для доступа к массивам и другим коллекциям
using System.Collections.Generic;                  //необходимо для доступа к спискам и словарям 
using UnityEngine;                                 //необходимо для доступа к Unity
using UnityEngine.SceneManagement;                 //для загрузки и перезагрузки сцен

public class Main : MonoBehaviour
{
    static public Main S;                           //объект-одиночка Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;     //a
    [Header("Set In inspector")]
    public GameObject[] prefabEnemies;        //Массив шаблонов Enemy
    public float enemySpawnPerSecond = 0.5f;  //вражеских кораблей в секунду
    public float enemyDefaultPadding = 1.5f;  //отступ для позиционирования
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;       //a
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield }; //b

    private BoundCheck bndCheck;

    public void ShipDestroyed(Enemy e)         //c
    {
        //Сгенерировать юонус с заданной вероятностью
        if(Random.value <= e.powerUpDropChance)      //d
        {
            //Выбрать тип бонуса
            //Выбрать один из элементов в powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];        //e

            //Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Установить соответствующий тип WeaponType
            pu.SetType(puType);

            //Поместить в место, где находился разрушенный корабль 
            pu.transform.position = e.transform.position;
        }
    }
    


    void Awake()
    {
        S = this;
        //Записать в bndCheck ссылку на компонент BondCheck этого игрового объекта
        bndCheck = GetComponent<BoundCheck>();
        //Вызвать SpawnEnemy() один раз в секунду(в 2 секунды при значениях по умолчанию)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);        //a

        //Словарь с ключами типа WeaponType 
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();       //a
        foreach(WeaponDefinition def in weaponDefinitions)                //b
        {
            WEAP_DICT[def.type] = def;
        }
    }
   public void SpawnEnemy()
    {
        //Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, prefabEnemies.Length);    //b
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]); //c

        //разместить вражеский корабль над экраном в случайной позиции x
        float enemyPadding = enemyDefaultPadding;     //d
        if(go.GetComponent<BoundCheck>() != null)     //e
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundCheck>().radius);
        }
        //установить начальные координаты созданного вражеского корабля    //f
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);     //g

    }
    public void DelayedRestart(float delay)
    {
        //вызвать метод Restart() через delay секунд
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        //перзапустить _Scene_0, чтобы перезапустить игру
        SceneManager.LoadScene("_Scene_0");
    }
    ///<summary>
    ///Статистическая функция, возвращаяющая WeaponDefinition из статистического защищенного поля
    ///WEAP_DICT класса Main.</summary>
    ///<returns> Экземпляр WeaponDefinition или, если нет такого определения для указанного
    /// WeaponType , возвращает новый экземпляр WeaponDefinition с типом none.</returns> 
    /// <param name="wt">Тип оружия WeaponType, для которого требуется получить WeaponDefinition </param>

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)          //a
    {
        //Проверить наличие указанного ключа в словаре
        //Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
        //поэтому следующая инструкция играет важную роль.
        if (WEAP_DICT.ContainsKey(wt))                                         //b
        {
            return (WEAP_DICT[wt]);
        }
        /* следующая инструкция возвращает новый экземпляр WeaponDefinition 
         с типом оружия WeaponType.none, что означает неудачную попытку найти требуемое определение 
         WeaponDefinition*/
        return (new WeaponDefinition());                                       //c
    }
}
