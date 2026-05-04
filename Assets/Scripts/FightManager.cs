
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.Networking;

using OpenCVForUnity.UnityUtils;

public class FightManager : MonoBehaviour
{
    [SerializeField] public List<Button> AbilityButtons; 
    [SerializeField] public Image Passives; 
    [SerializeField] public TMP_Text CurrentEnergy; 
    [SerializeField] public Slider Healthbar; 
    [SerializeField] public Button EndTurnButton;
    [SerializeField] public GameObject EnemyPrefab; 
    [SerializeField] public List<GameObject> EnemyPositions;
    [SerializeField] public int EncounterChallengeRating=4;
    [SerializeField] public RawImage Chel; 
    [SerializeField] public AudioSource audioSource; 
    [SerializeField] public GameObject End; 


    private Player player;

    private int SelectedAbility;
    private Enemy SelectedEnemy;
    private List<EnemyComponent> enemies;
    private List<Enemy> EnemyData;
    List<string> passives;
    private CreatureData creatureData;
    
    async Task  Start()
    {
        SelectedAbility = -1;

        player = new Player(20);
        Healthbar.maxValue = 20;
        ItemLoader itemLoader = new ItemLoader();
        //itemLoader.
        passives = new List<string>();
        int i = 0;
        foreach (Item item in itemLoader.GetChosenItems())
        {
            Debug.Log(item);
            player.SetAbility(item.ActiveName, item.ActiveStrength, item.ActiveCost);
            //actives.Add(item.ActiveDescription);
            player.SetAbility(item.PassiveName, item.PassiveStrength, 0);
            passives.Add(item.PassiveDescription);

            
            SetUpButton(AbilityButtons[i], i, item.ActiveDescription, item.Icon);
            i++;
        }

        
        GameObject hint = Passives.transform.GetChild(0).gameObject;
        hint.GetComponentInChildren<TMP_Text>().SetText(string.Join("\n", passives));

        EventTrigger trigger = Passives.GetComponent<EventTrigger>();
        // --- Pointer Enter ---
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { hint.SetActive(true); });
        trigger.triggers.Add(enterEntry);

        // --- Pointer Exit ---
        EventTrigger.Entry exitEntry = new EventTrigger.Entry(); // Создаем НОВЫЙ объект
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { hint.SetActive(false); });
        trigger.triggers.Add(exitEntry);

        GenericEnemyLoader enemyLoader = new GenericEnemyLoader();
        await enemyLoader.LoadText("enemies.json");
        enemies = new List<EnemyComponent>();
        EnemyData = new List<Enemy>();

        int currentChallenge = 0;
        for(i=0; i<enemyLoader.EnemyCount(); i++)
        {
            if (currentChallenge > EncounterChallengeRating || i >= EnemyPositions.Count)
                break;

            int randomIndex = UnityEngine.Random.Range(0, enemyLoader.EnemyCount());
            //TODO: плучить противника с классом опасности не выше максимума GetRandEnemyWLessCR(EncounterChallengeRating + currentChallenge), а не по id
            GenericEnemy enemy = enemyLoader.GetItemById(randomIndex);  
            enemy.Init();
            enemy.target = player;
            EnemyData.Add(enemy);
            currentChallenge += enemy.ChallengeRating;
            GameObject newEnemy = Instantiate(EnemyPrefab, EnemyPositions[i].transform);
            enemies.Add(newEnemy.GetComponent<EnemyComponent>());
            enemies[enemies.Count-1].SetUp(enemy);
            newEnemy.gameObject.SetActive(true);
            Debug.Log(enemy.id);
        }


        EndTurnButton.onClick.AddListener(EndTurn);
        player.StartOfBattle();
        CurrentEnergy.SetText(player.CurrentEnergy.ToString());
        CheckHealth();
        
        creatureData = GameObject.Find("CreatureObject").GetComponent<CreatureData>();
        Chel.texture = creatureData.image;
        Chel.SetNativeSize();
        float currentWidth = Chel.rectTransform.rect.width;
        float currentHeight = Chel.rectTransform.rect.height;
        float currentArea = currentWidth * currentHeight;
        if (currentArea > 300000f)
        {
            float targetScale = Mathf.Sqrt(300000f / (Chel.rectTransform.rect.width * Chel.rectTransform.rect.height));
            Chel.rectTransform.localScale = new Vector3(targetScale, targetScale, 1f);
        }
    }
    void Update()
    {
        if (SelectedAbility >= 0)
        {
            int i = 0;
            if (enemies is not null)
                foreach (EnemyComponent ec in enemies)
                {
                    if (ec.selected)
                        Select(i);
                    i++; 
                }
        }
        if (Healthbar.value != player.CurrentHealth)
            Healthbar.value = player.CurrentHealth;
    }

    public void CheckHealth()
    {
        if (player.CurrentHealth <= 0)
        {
            Defeat();
            return;
        }
        List<int> deadEnemies = new List<int>();
        int i = 0;
        foreach (Enemy en in EnemyData)
        {
            if (en.CurrentHealth <= 0)
            {
                deadEnemies.Add(i);
            }
            i++;
        }
        foreach (int dead in deadEnemies)
        {
            EnemyData.RemoveAt(dead);
            enemies[dead].gameObject.SetActive(false);
            Destroy(enemies[dead]);
            enemies.RemoveAt(dead);
        }
        if (EnemyData.Count == 0)
        {
            Victory();
        }
    }
    public void Defeat()
    {
        End.transform.GetChild(1).gameObject.SetActive(true);
        End.transform.GetChild(2).gameObject.SetActive(true);
    }
    public void Victory()
    {
        End.transform.GetChild(0).gameObject.SetActive(true);
        End.transform.GetChild(2).gameObject.SetActive(true);
    }
    private void SetUpButton(Button button, int abilityNum, string description, string Icon)
    {
        
        string path = "icons/";
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>(path + Icon);

        button.onClick.AddListener(()=>{
            SelectedAbility=abilityNum;
            OnAbilitySelect(); 
            });
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        GameObject hint = button.transform.GetChild(0).gameObject;
        hint.GetComponentInChildren<TMP_Text>().SetText(description);

        // --- Pointer Enter ---
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { hint.SetActive(true); });
        trigger.triggers.Add(enterEntry);

        // --- Pointer Exit ---
        EventTrigger.Entry exitEntry = new EventTrigger.Entry(); // Создаем НОВЫЙ объект
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { hint.SetActive(false); });
        trigger.triggers.Add(exitEntry);

        
    }
    public void Select(int enemy)
    {
        
        player.UseAbility(SelectedAbility, enemies[enemy].enemyData, EnemyData.ToArray());
        foreach (EnemyComponent ec in enemies)
        {
            ec.GetComponentInChildren<Button>().gameObject.SetActive(false);
            ec.selected = false;
        }
        SelectedAbility = -1;
        CurrentEnergy.SetText(player.CurrentEnergy.ToString());
        CheckHealth();
        if (creatureData.sound != null)
            audioSource.PlayOneShot(creatureData.sound);
    }

    private void OnAbilitySelect()
    {
        if (player.abilityList[SelectedAbility * 2].tags.Contains(Tags.Positive))
        {
            player.UseAbility(SelectedAbility, player, EnemyData, EnemyData.ToArray());
            CurrentEnergy.SetText(player.CurrentEnergy.ToString());
            CheckHealth();
            return;
        }
        foreach (EnemyComponent ec in enemies)
        {
            ec.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void EndTurn()
    {
        foreach (EnemyComponent ec in enemies)
        {
            ec.enemyData.StatusTick(EnemyData.ToArray());
            ec.enemyData.TakeTurn(EnemyData.ToArray());
            ec.UpdateIcon();
        }
        player.StatusTick(EnemyData.ToArray());
        player.StartOfTurn();
        CurrentEnergy.SetText(player.CurrentEnergy.ToString());
        CheckHealth();
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

    public GenericEnemyLoader()
    {
    }
    public int EnemyCount()
    {
        return itemListWrapper.enemyList.Count;
    }
    public GenericEnemy GetItemById(int id)
    {
        GenericEnemy ge = new GenericEnemy(itemListWrapper.enemyList[id]);
        ge.Init();
        return ge;
    }
    public GenericEnemy GetRandEnemyWLessCR(int challengeRating)
    {
        return null;//TODO
    }
    
    async public Task LoadText(string fileName)
    {
        //string jsonText = File.ReadAllText(fileName);
        string jsonText = await LoadDialogsAsync(fileName);
        itemListWrapper  = JsonUtility.FromJson<GenericEnemyListWrapper>(jsonText);
    }
    async Task<string> LoadDialogsAsync(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
        {
            var operation = webRequest.SendWebRequest();

            // Ждем завершения без блокировки потока
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
                return webRequest.downloadHandler.text;
            
            return null;
        }
    }
}