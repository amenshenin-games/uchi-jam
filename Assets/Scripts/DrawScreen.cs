using UnityEngine;
using System.Collections.Generic;

public class DrawScreen : MonoBehaviour 
{
    IItemRepository itemLoader;
    [SerializeField] public List<GameObject> ItemPositions;
    [SerializeField] public GameObject ItemPrefab; 
    [SerializeField] public GameObject ItemHolder; 

    void Start()
    {
        IItemRepository itemLoader = new ItemLoader(Application.dataPath + "/chosenItems.json");
        List<Item> chosenItems = itemLoader.GetAll();
        HashSet<int> exclude = new HashSet<int>();

        int i = 0;
        foreach (Item item in chosenItems)
        {
            exclude.Add(item.id);

            if (i <= 2)
            {
                GameObject newItem = Instantiate(ItemPrefab, new Vector3(0,0,0), Quaternion.identity);
                ItemComponent ic = newItem.GetComponent<ItemComponent>();
                ic.SetUp(item, ItemPositions[i].GetComponent<RectTransform>().localPosition);
                newItem.transform.SetParent(ItemHolder.transform);
                newItem.gameObject.SetActive(true);
            }
            i++;
        }
        //TODO +1 Item
    }
}
