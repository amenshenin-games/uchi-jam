using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutor : MonoBehaviour
{
    public int CurrentImage = 0;
    public List<Sprite> Images;
    public Image Image;
    void Start()
    {
        Image = GetComponent<Image>();
    }
    public void Next()
    {
        CurrentImage++;
        if (CurrentImage == Images.Count)
        {
            gameObject.SetActive(false);
            return;
        }
        Image.sprite = Images[CurrentImage];
    }
}
