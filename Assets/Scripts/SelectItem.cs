using UnityEngine;

public class SelectItem : MonoBehaviour
{
    ItemComponent chosenItem;

    public void OnTriggerStay2D(Collider2D col)
    {
        ItemComponent item = col.gameObject.GetComponent<ItemComponent>();
        if (item is not null && !item.dragged && !item.chosen)
        {
            if (chosenItem is not null)
                chosenItem.UnChoose();
            chosenItem = item;
            item.Choose(transform.localPosition);
        }
    }
}
