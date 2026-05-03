using UnityEngine;
using UnityEngine.UI;

public class EnemyComponent : MonoBehaviour
{
    private Image image;
    public GenericEnemy enemyData;
    [SerializeField] public Slider Healthbar; 
    [SerializeField] public Button Select; 
    [SerializeField] public Image Icon; 
    public bool selected;

    public void Start()
    {
        image = GetComponent<Image>();
        Select.onClick.AddListener(() => {selected = true;});
    } 
    public void Update()
    {
        if (Healthbar.value != enemyData.CurrentHealth)
            Healthbar.value = enemyData.CurrentHealth;
    }
    public void SetUp(GenericEnemy enemyData)
    {
        image = GetComponent<Image>();
        string path = "enemies/";
        this.enemyData = enemyData;
        image.sprite = Resources.Load<Sprite>(path + enemyData.Image);
        image.SetNativeSize();

        Healthbar.minValue = 0f;
        Healthbar.maxValue = enemyData.MaxHealth;
        UpdateIcon();
    }

    public void UpdateIcon()
    { 
        Icon.sprite = Resources.Load<Sprite>("icons/" + enemyData.Icons[enemyData.currAbility]);
    }
}
