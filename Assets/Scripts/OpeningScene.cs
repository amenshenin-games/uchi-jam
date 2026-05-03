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
    private int CurrentLine = 0;
    private List<string> Lines;
    private List<AudioClip> DubbingLines;
    
    async Task Awake()
    {
        LineLoader lineLoader = new LineLoader(); 
        await lineLoader.LoadText(Application.streamingAssetsPath + "/dialogLines.json");// Установите свой репозиторий сюда
        Lines = lineLoader.GetDialogLines();
        //TODO: Dub GetDubbingFiles()
        NextLine();
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
            CurrentLine++;
        }
        else
        {
            SpeechBubble.SetActive(false);
            CreateButton.SetActive(true);
        }
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
        string jsonText = await LoadDialogsAsync();
        linesListWrapper  = JsonUtility.FromJson<LinesListWrapper>(jsonText);
    }
    async Task<string> LoadDialogsAsync()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "dialogLines.json");
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

