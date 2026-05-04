using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutor : MonoBehaviour
{
    public int CurrentImage = 0;
    public List<Sprite> Images;
    public List<AudioClip> audioInstructions;
    public Image Image;
    public AudioSource AudioSource;
    void Start()
    {
        Image = GetComponent<Image>();
        AudioSource.clip = audioInstructions[CurrentImage];
        AudioSource.Play();
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
        AudioSource.PlayOneShot(audioInstructions[CurrentImage]);
    }
}
