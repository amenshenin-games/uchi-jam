using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class Openingscene : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public GameObject SpeechBubble;
    [SerializeField] public TMP_Text SpeechText;
    [SerializeField] public GameObject CreateButton;
    [SerializeField] public string dialogFile;
    [SerializeField] public string musicTrack;
    [SerializeField] public Image Mustage;
    [SerializeField] public CurtainsAndTransitions Curtains;
    [SerializeField] public AudioSource AudioSource;
    [SerializeField] public Button Replay;

    private int CurrentLine = 0;
    private List<string> Lines;
    private List<string> DubbingLines;
    
    async Task Awake()
    {
        GameObject bkgMusic = GameObject.Find("music");
        if (bkgMusic != null)
           bkgMusic.GetComponent<MusicPersistence>().SetTrack(musicTrack);
        LineLoader lineLoader = new LineLoader(); 
        Debug.Log(Application.streamingAssetsPath + dialogFile);
        await lineLoader.LoadText(Application.streamingAssetsPath + dialogFile);// Установите свой репозиторий сюда
        Lines = lineLoader.GetDialogLines();
        DubbingLines = lineLoader.GetDubbingFiles();
        NextLine();
        Replay.onClick.AddListener(()=>{AudioSource.Play();});
        //Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
    }

    void Start()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        NextLine();
    }

    public void NextLine()
    {
        if (CurrentLine < Lines.Count)
        {
            SpeechText.SetText(Lines[CurrentLine]);
        }
        else
        {
            SpeechBubble.SetActive(false);
            if (CreateButton != null)
                CreateButton.SetActive(true);
            else
            {
                Curtains.GoToNextScene();
            }
            
            return;
        }
        Debug.Log(Mustage.GetComponent<Animator>().GetBool("Mustage"));
        Debug.Log(Mustage.GetComponent<Animator>().GetBool("Mustage"));
        Mustage.GetComponent<Animator>().Play("mustg", 0, 0f); string path = "items/";

        AudioSource.clip = Resources.Load<AudioClip>("dubb/" + DubbingLines[CurrentLine]);
        AudioSource.Play();
        
            CurrentLine++;
        //TurnOffAfterDelay(2);
    }
    
    System.Collections.IEnumerator TurnOffAfterDelay(float delay)
    {
        //anim.Play("Base Layer.YourAnimationName", 0, 0f); 
        Mustage.GetComponent<Animator>().SetBool("Mustage", true);
        yield return new WaitForSeconds(delay);
        Mustage.GetComponent<Animator>().SetBool("Mustage", false);
        Debug.Log("Bool turned off!");
    }
}

public interface IDialogRepository
{
    public List<string> GetDialogLines();
    public List<string> GetDubbingFiles();
}

public class LineLoader : IDialogRepository
{
    [System.Serializable]
    private class LinesListWrapper
    {
        public List<string> dialogLines; 
        public List<string> dubFiles; 
    }

    private LinesListWrapper linesListWrapper;

    public LineLoader()
    {
        
    }

    public List<string> GetDialogLines()
    {
        return linesListWrapper.dialogLines;
    }
    public List<string> GetDubbingFiles()
    {
        return linesListWrapper.dubFiles;
    }

    async public Task LoadText(string fileName)
    {
        //string jsonText = File.ReadAllText(fileName);
        string jsonText = await LoadDialogsAsync(fileName);
        linesListWrapper  = JsonUtility.FromJson<LinesListWrapper>(jsonText);
    }
    async Task<string> LoadDialogsAsync(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
        {
            var operation = webRequest.SendWebRequest();

            // Ждем завершения без блокировки потока
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
                return webRequest.downloadHandler.text;
            
            return null;
        }
    }
}

