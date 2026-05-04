using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
//using WebGLMicrophone;
public class CaptureAudio : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown dropDownList;
    [SerializeField] public Button StartRecButton;
    [SerializeField] public Button ListenButton;
    [SerializeField] public Button NextButton;
    [SerializeField] public Sprite  RedRecord;
    [SerializeField] public Sprite  GreenListen;
    private GameObject creatureObject;
    private List<string> MicList;
    private string MicrophoneName;
    private AudioClip AudioClip;
    private Sprite orig;
    private bool imgChanged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creatureObject = GameObject.Find("CreatureObject");

        

        StartRecButton.onClick.AddListener(OnStartRecButton);
        ListenButton.onClick.AddListener(OnListenButton);
        NextButton.onClick.AddListener(OnNextButton);
    }

    private void OnStartRecButton()
    {
        /*List<string> micList = new List<string>();
        foreach (string mic in UnityWebGLMicrophone.Microphone.devices)
        {
            micList.Add(mic);
        }
        dropDownList.ClearOptions();
        dropDownList.AddOptions(micList);
        dropDownList.onValueChanged.AddListener(delegate {
                MicrophoneName = Microphone.devices[dropDownList.value];
            });*/

        if (!imgChanged)
        {
            StartCoroutine(SwapRoutine(true, StartRecButton, RedRecord));
            
        }
    }
    private void OnListenButton()
    {
        if (!imgChanged && AudioClip != null) 
        {
            StartCoroutine(SwapRoutine(false, ListenButton, GreenListen));
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = AudioClip;
            audioSource.Play();
        }
    }
    private void OnNextButton()
    {
        creatureObject.GetComponent<CreatureData>().sound = AudioClip;
    }



    System.Collections.IEnumerator SwapRoutine(bool startRec, Button button, Sprite newImage)
    {
        if (startRec)
            AudioClip = Microphone.Start(null, false, 2, AudioSettings.outputSampleRate);
        imgChanged = true;
        orig = button.image.sprite;

        button.image.sprite = newImage;

        yield return new WaitForSeconds(2f);

        button.image.sprite = orig;
        imgChanged = false;
        if (startRec)
            Microphone.End("0");
    }
}
