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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NextSceneButton is not null)
        {
            NextSceneButton.onClick.AddListener(GoToNextScene);
        }
    }


    public void GoToNextScene()
    {
        StartCoroutine(LoadNext(NextScene));
    }
    
    IEnumerator LoadNext(string scene)
    {
        CurtainAnimator.SetTrigger("start");

        yield return new WaitForSeconds(1);
        
        SceneManager.LoadScene(scene);
    }
}
