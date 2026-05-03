using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement; 


public class ItemComponent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] public float speed = 10f; 
    [SerializeField] public GameObject Description; 
    private Canvas Canvas; 
    public Item item;
    private GameObject ImageObj;
    private RectTransform rt;
    public bool dragged = false;
    public bool chosen = false;
    public bool goBack = true;
    private Vector2 InitPosition;
    public Vector2 Destination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        ImageObj = transform.GetChild(1).gameObject;
        RawImage image = ImageObj.GetComponent<RawImage>();
        string path = "items/";
        image.texture = Resources.Load<Texture>(path + item.Image);
        image.SetNativeSize();
        rt = GetComponent<RectTransform>();
        rt.localScale = new Vector3(1, 1, 1);

        Description.transform.GetChild(0).GetComponent<TMP_Text>().SetText(item.ActiveDescription + "\n" + item.PassiveDescription);
        Description.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("icons/" + item.Icon);
        
    }

    public void SetUp(Item item, Vector2 Destination)
    {
        this.item = item;
        this.Destination = Destination;
        this.InitPosition = Destination;
    }
    // Update is called once per frame
    void Update()
    {
        if (rt.anchoredPosition != Destination && !dragged)
        {
            rt.anchoredPosition  = Vector2.MoveTowards(rt.anchoredPosition , Destination, Time.deltaTime*speed);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragged = true;
        UnChoose();
    }
    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / Canvas.scaleFactor;
        Description.SetActive(false);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        dragged = false;
    }
    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (rt.anchoredPosition == Destination && !dragged)
            Description.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        Description.SetActive(false);
    }
    
    public void Choose(Vector2 newDestination)
    {
        if (!chosen)
        {
            chosen = true;
            Destination = newDestination;
        }
    }
    public void UnChoose()
    {
        if (chosen)
        {
            chosen = false;
            Destination = InitPosition;
        }
    }
}
