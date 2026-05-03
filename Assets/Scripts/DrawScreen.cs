using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class DrawScreen : MonoBehaviour 
{
    IItemRepository itemLoader;
    [SerializeField] public List<GameObject> ItemPositions;
    [SerializeField] public GameObject ItemPrefab; 
    [SerializeField] public GameObject ItemHolder; 

    async Task Start()
    {
        //IItemRepository itemLoader = new ItemLoader(Application.streamingAssetsPath + "/chosenItems.json");
        ItemLoader itemLoader = new ItemLoader();
        //await itemLoader.LoadText(Application.streamingAssetsPath + "/chosenItems.json");
        
        List<Item> chosenItems = itemLoader.GetChosenItems();
        HashSet<int> exclude = new HashSet<int>();

        int i = 0;
        foreach (Item item in chosenItems)
        {
            exclude.Add(item.id);

            if (i < 2)
            {
                GameObject newItem = Instantiate(ItemPrefab, new Vector3(0,0,0), Quaternion.identity);
                ItemComponent ic = newItem.GetComponent<ItemComponent>();
                ic.SetUp(item, ItemPositions[i].GetComponent<RectTransform>().localPosition);
                newItem.transform.SetParent(ItemHolder.transform);
                newItem.gameObject.SetActive(true);
            }
            i++;
        }
    }
}
