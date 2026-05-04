using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class CurtainsAndTransitions : MonoBehaviour
{
    [SerializeField] public Button NextSceneButton;
    [SerializeField] public string NextScene;
    [SerializeField] public Animator CurtainAnimator;
    [SerializeField] public AudioSource source;
    [SerializeField] public AudioClip openSound;
    [SerializeField] public AudioClip closeSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NextSceneButton is not null)
        {
            if (NextScene != "Opening 1")
                source.PlayOneShot(openSound);
            Debug.Log("SET BUTTON");
            NextSceneButton.onClick.AddListener(GoToNextScene);
        }
    }


    public void GoToNextScene()
    {
        if (NextScene != "Opening 1")
            source.PlayOneShot(closeSound);
        StartCoroutine(LoadNext(NextScene));
    }
    
    IEnumerator LoadNext(string scene)
    {
        CurtainAnimator.SetTrigger("start");

        yield return new WaitForSeconds(1);
        
        SceneManager.LoadScene(scene);
    }
}
