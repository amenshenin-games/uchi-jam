using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems;

public class TakePhoto : WebCamera
{
    [SerializeField] public TMP_Dropdown dropDownList;
    [SerializeField] public TMP_Text Instructions;
    [SerializeField] public Button TakePhotoButton;
    [SerializeField] public Button NextButton;
    [SerializeField] public GameObject PhotoSurface;
    private CreatureData creatureData;
    private GameObject creatureObject;

    private Mat VideoImage;
    public Mat PhotoImage;
    

    public void Start()
    {
        Instructions.SetText("Сделай снимок своего рисунка");
        creatureObject = GameObject.Find("CreatureObject");
        if (creatureObject is not null)
        {
            creatureData = creatureObject.GetComponent<CreatureData>();
            PhotoSurface.GetComponent<RawImage>().texture = OpenCvSharp.Unity.MatToTexture(creatureData.image);
        }

        List<string> camList = new List<string>();
        foreach (WebCamDevice cam in WebCamTexture.devices)
        {
            camList.Add(cam.name);
        }
		
        dropDownList.ClearOptions();
        dropDownList.AddOptions(camList);
        dropDownList.value = -1; 
        dropDownList.onValueChanged.AddListener(delegate {
                DeviceName = WebCamTexture.devices[dropDownList.value].name;
            });

        TakePhotoButton.onClick.AddListener(OnPhotoButton);
        NextButton.onClick.AddListener(OnNextButton);
        
    }
    
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        
        VideoImage = OpenCvSharp.Unity.TextureToMat(input);

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(VideoImage);
        else
            OpenCvSharp.Unity.MatToTexture(VideoImage, output);

        
        return true;
    }

    
    private void OnPhotoButton()
    {
        PhotoImage = VideoImage;
        PhotoSurface.GetComponent<RawImage>().texture =  OpenCvSharp.Unity.MatToTexture(PhotoImage);
        Instructions.SetText("Выдели на фото своего чудика");
    }

    private void OnNextButton()
    {
        if (creatureObject is null)
        {
            creatureObject = new GameObject("CreatureObject");
            creatureData = creatureObject.AddComponent<CreatureData>();
            DontDestroyOnLoad(creatureObject);
        } else
        {
            creatureData = creatureObject.GetComponent<CreatureData>();
        }
        

        Vector2 StartPos = PhotoSurface.GetComponent<DrawBox>().StartPos;
        Vector2 EndPos = PhotoSurface.GetComponent<DrawBox>().EndPos;
        Texture textureFromImage = PhotoSurface.GetComponent<RawImage>().texture;
        
        if (StartPos != EndPos) // Обрезание
        {
            float coef = textureFromImage.width / PhotoSurface.GetComponent<RectTransform>().rect.width; //Разница окна с фото и разрешением фото
            StartPos *= coef;
            EndPos *= coef;
            
            StartPos.x = textureFromImage.width/2 + StartPos.x; // Преобразование в систему координат от верхнего левого угла
            StartPos.y = textureFromImage.height/2 - StartPos.y;
            EndPos.x = textureFromImage.width/2 + EndPos.x;
            EndPos.y = textureFromImage.height/2 - EndPos.y;
            
            float tmp;
            if (StartPos.x > EndPos.x) // В какую бы сторону не провели квадрат, нам всегда нужен левый верхний уол
            {
                tmp = StartPos.x;
                StartPos.x = EndPos.x;
                EndPos.x = tmp;
            }
            if (StartPos.y > EndPos.y)
            {
                tmp = StartPos.y;
                StartPos.y = EndPos.y;
                EndPos.y = tmp;
            }

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect((int)StartPos.x, //x top left
                                                         (int)StartPos.y, //y top left
                                                         (int)EndPos.x - (int)StartPos.x, //width
                                                         (int)EndPos.y - (int)StartPos.y); //height
 
            creatureData.image = new Mat(PhotoImage, rect);
        }

        webCamTexture.Stop();
        webCamTexture = null;
        
        SceneManager.LoadScene("Edit");
    }

    

}
