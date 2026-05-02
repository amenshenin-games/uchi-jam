using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class EnemyGen : MonoBehaviour
{
    [SerializeField] public GameObject EnemyPrefab; 
    //[SerializeField] public GameObject EnemyHolder; 
    
    [SerializeField] public List<GameObject> EnemyPositions;
    [SerializeField] public int EncounterChallengeRating=4;
    [SerializeField] public List<Enemy> enemies;
    //[SerializeField] public GameObject Canvas; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IEnemyRepository enemyLoader = new GenericEnemyLoader(Application.dataPath + "/enemies.json");

        Entity player = new Entity(100);
        enemies = new List<Enemy>();

        //foreach (GenericEnemy enemy in enemyList.enemyList)

        int currentChallenge = 0;
        for(int i=0; i<enemyLoader.EnemyCount(); i++)
        {
            if (currentChallenge > EncounterChallengeRating)
                break;

            int randomIndex = UnityEngine.Random.Range(0, enemyLoader.EnemyCount());
            //TODO: плучить противника с классом опасности не выше максимума GetRandEnemyWLessCR(EncounterChallengeRating + currentChallenge), а не по id
            GenericEnemy enemy = enemyLoader.GetItemById(randomIndex);  
            enemy.Init();
            enemy.target = player;
            currentChallenge += enemy.ChallengeRating;
            enemies.Add(enemy);
            GameObject newItem = Instantiate(EnemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            //newItem.GetComponent<ItemComponent>().SetUp(item, ItemPositions[i].GetComponent<RectTransform>().localPosition);
            //newItem.transform.SetParent(EnemyHolder.transform);
            newItem.gameObject.SetActive(true);
            Debug.Log(enemy.id);
        }
        
        //Entity player = new Entity(100);
        Debug.Log(player.CurrentHealth);
        //BatEnemy bat = new BatEnemy(player);

        StatusAbility<Confused> a = new StatusAbility<Confused>(1, 0, 2);
        List<Entity> all = new List<Entity>();
        foreach (Enemy en in enemies)
        {
            all.Add(en);
            
        }
        a.ExecAbility(enemies[0], enemies.Cast<object>().ToArray());
        a.ExecAbility(enemies[1], enemies.Cast<object>().ToArray());
        a.ExecAbility(enemies[2], enemies.Cast<object>().ToArray());
        a.ExecAbility(enemies[3], enemies.Cast<object>().ToArray());
        //bat.TakeTurn();

        Debug.Log("===========================");
        foreach (Enemy en in enemies)
        {
            Debug.Log(en.target);
            en.StatusTick(enemies.Cast<object>().ToArray());
            en.StatusTick(enemies.Cast<object>().ToArray());
            en.StatusTick(enemies.Cast<object>().ToArray());
            en.StatusTick(enemies.Cast<object>().ToArray());
            Debug.Log(en.target);
            en.TakeTurn(enemies);
            
        }

        Debug.Log("===========================");
        

        
        foreach (Enemy en in enemies)
        {
            Debug.Log(en.CurrentHealth);
        }

        Debug.Log("===========================");
        Debug.Log(player.CurrentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IEnemyRepository
{
    public GenericEnemy GetItemById(int id);
    public GenericEnemy GetRandEnemyWLessCR(int challengeRating);
    public int EnemyCount();
}

public class GenericEnemyLoader : IEnemyRepository
{
    [System.Serializable]
    private class GenericEnemyListWrapper
    {
        public List<GenericEnemy> enemyList; 
    }

    private GenericEnemyListWrapper itemListWrapper;

    public GenericEnemyLoader(string fileName)
    {
        string jsonText = File.ReadAllText(fileName);
        itemListWrapper  = JsonUtility.FromJson<GenericEnemyListWrapper>(jsonText);
    }
    public int EnemyCount()
    {
        return itemListWrapper.enemyList.Count;
    }
    public GenericEnemy GetItemById(int id)
    {
        itemListWrapper.enemyList[id].Init();
        return itemListWrapper.enemyList[id];
    }
    public GenericEnemy GetRandEnemyWLessCR(int challengeRating)
    {
        return null;//TODO
    }
    
}
