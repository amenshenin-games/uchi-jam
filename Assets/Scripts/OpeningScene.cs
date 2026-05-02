using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

public class Openingscene : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public GameObject SpeechBubble;
    [SerializeField] public TMP_Text SpeechText;
    [SerializeField] public GameObject CreateButton;
    private int CurrentLine = 0;
    private List<string> Lines;
    private List<AudioClip> DubbingLines;
    
    void Awake()
    {
        IDialogRepository lineLoader = new LineLoader(Application.dataPath + "/dialogLines.json"); // Установите свой репозиторий сюда
        Lines = lineLoader.GetDialogLines();
        //TODO: Dub GetDubbingFiles()
    }

    void Start()
    {
        NextLine();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        NextLine();
    }

    public void NextLine()
    {
        Debug.Log(CurrentLine);
        Debug.Log(Lines.Count);

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

    public LineLoader(string fileName)
    {
        string jsonText = File.ReadAllText(fileName);
        linesListWrapper  = JsonUtility.FromJson<LinesListWrapper>(jsonText);
    }
    public List<string> GetDialogLines()
    {
        return linesListWrapper.dialogLines;
    }
    public List<string> GetDubbingFiles()
    {
        return linesListWrapper.dubFiles;
    }
}

