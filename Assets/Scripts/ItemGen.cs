using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI; 
using System.Linq;

public class ItemGen : MonoBehaviour
{
    [SerializeField] public GameObject ItemPrefab; 
    [SerializeField] public CurtainsAndTransitions Curtains; 
    [SerializeField] public GameObject ItemHolder; 
    [SerializeField] public Button NextSceneButton;
    
    [SerializeField] public List<GameObject> ItemPositions;
    [SerializeField] public List<ItemComponent> CurrentItems;

    private IItemRepository itemLoader;
    
    void Awake()
    {
        itemLoader = new ItemLoader(Application.dataPath + "/items.json");

        HashSet<int> exclude = new HashSet<int>();
        for(int i=0; i < ItemPositions.Count; i++)
        {
            if (i >= itemLoader.ItemCount())
                break;

            Item item = itemLoader.GetRandomItemExcluding(exclude);
            exclude.Add(item.id);
 
            GameObject newItem = Instantiate(ItemPrefab, new Vector3(0,0,0), Quaternion.identity);
            ItemComponent ic = newItem.GetComponent<ItemComponent>();
            ic.SetUp(item, ItemPositions[i].GetComponent<RectTransform>().localPosition);
            CurrentItems.Add(ic);
            newItem.transform.SetParent(ItemHolder.transform);
            newItem.gameObject.SetActive(true);
            Debug.Log(item.ToString());
        }
    }

    void Start()
    {
        NextSceneButton.onClick.AddListener(OnNextButton);
    }

    private void OnNextButton()
    {
        int chosenCount = 0;
        foreach (ItemComponent item in CurrentItems)
        {
            if (item.chosen)
            {
                chosenCount++;
            }
        }
        if (chosenCount < 2)
        {
            Debug.Log("Wrong");
            //TODO
        }
        else
        {
            Debug.Log(chosenCount);
            List<Item> chosenItems = new List<Item>();
            foreach (ItemComponent item in CurrentItems)
            {
                if (item.chosen)
                {
                    chosenItems.Add(item.item);
                }
            }
            itemLoader.SaveChosenItems(chosenItems);
            Curtains.GoToNextScene();
        }
    }
}

public interface IItemRepository
{
    public List<Item> GetAll();
    public void SaveChosenItems(List<Item> items);
    public List<Item> GetChosenItems();
    public Item GetItemById(int id);
    public Item GetRandomItemExcluding(HashSet<int> ExcludeIds);
    public int ItemCount();
}

public class ItemLoader : IItemRepository
{
    [System.Serializable]
    private class ItemListWrapper
    {
        public List<Item> itemList; 
    }

    private ItemListWrapper itemListWrapper;

    public ItemLoader(string fileName)
    {
        string jsonText = File.ReadAllText(fileName);
        itemListWrapper  = JsonUtility.FromJson<ItemListWrapper>(jsonText);
    }
    public int ItemCount()
    {
        return itemListWrapper.itemList.Count;
    }
    public Item GetItemById(int id)
    {
        return itemListWrapper.itemList[id];
    }
    public List<Item> GetAll()
    {
        return itemListWrapper.itemList;
    }
    public void SaveChosenItems(List<Item> items)
    {
        string file = Application.dataPath + "/chosenItems.json";
        ItemListWrapper smallItemListWrapper = new ItemListWrapper();
        smallItemListWrapper.itemList = items;
        string json = JsonUtility.ToJson(smallItemListWrapper);
        File.WriteAllText(file, json);
    }
    public List<Item> GetChosenItems()
    {
        string file = Application.dataPath + "/chosenItems.json";
        string json = File.ReadAllText(file);
        ItemListWrapper chosenItems = JsonUtility.FromJson<ItemListWrapper>(json);
        return chosenItems.itemList;
    }

    public Item GetRandomItemExcluding(HashSet<int> ExcludeIds)
    {
        var range = Enumerable.Range(0, ItemCount()).Where(i => !ExcludeIds.Contains(i));
        var rand = new System.Random();
        int randomIndex = range.ElementAt(rand.Next(0, ItemCount() - ExcludeIds.Count));
        return GetItemById(randomIndex);
    }
}
