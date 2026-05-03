using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 


public class Show : MonoBehaviour
{
    [SerializeField] public RawImage Image;
    [SerializeField] public AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatureData cd = GameObject.Find("CreatureObject").GetComponent<CreatureData>();
        Image.texture = cd.image;
        Image.SetNativeSize();
        audioSource.clip = cd.sound;
        audioSource.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
