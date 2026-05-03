using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems;


using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;

public class TakePhoto : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown dropDownList;
    [SerializeField] public TMP_Text Instructions;
    [SerializeField] public Button TakePhotoButton;
    [SerializeField] public Button NextButton;
    [SerializeField] public RawImage PhotoSurface;
    [SerializeField] public RawImage VideoSurface;
    private CreatureData creatureData;
    private GameObject creatureObject;

    private WebCamTexture webcamTexture;
    //private Mat VideoImage;
    //public Mat PhotoImage;
    

    public void Start()
    {

        Instructions.SetText("Сделай снимок своего рисунка");
        

        List<string> camList = new List<string>();
        foreach (WebCamDevice cam in WebCamTexture.devices)
        {
            camList.Add(cam.name);
        }
		
        dropDownList.ClearOptions();
        dropDownList.AddOptions(camList);
        dropDownList.value = -1; 
        dropDownList.onValueChanged.AddListener(delegate {
                SetCamera(dropDownList.value);
                //DeviceName = WebCamTexture.devices[dropDownList.value].name;
            });
        WebCamTexture webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        TakePhotoButton.onClick.AddListener(OnPhotoButton);
        NextButton.onClick.AddListener(OnNextButton);
    }
    

    void Update()
    {
        //.texture = webcamTexture;
    }
    public void SetCamera(int index)
    {
        if (WebCamTexture.devices.Length <= index) return;

        // Останавливаем текущую камеру, если она работает
        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Stop();

        // Создаем текстуру для выбранного устройства по его имени
        string deviceName = WebCamTexture.devices[index].name;
        webcamTexture = new WebCamTexture(deviceName);

        VideoSurface.texture = webcamTexture;
        webcamTexture.Play();
    }
    private void OnPhotoButton()
    {
        //PhotoImage = VideoImage;
        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height); 
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();
        PhotoSurface.texture = photo;
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
        Texture textureFromImage = PhotoSurface.texture;
        
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

            OpenCVForUnity.CoreModule.Rect rect = new OpenCVForUnity.CoreModule.Rect((int)StartPos.x, //x top left
                                                         (int)StartPos.y, //y top left
                                                         (int)EndPos.x - (int)StartPos.x, //width
                                                         (int)EndPos.y - (int)StartPos.y); //height

            
            Mat fullMat = new Mat(PhotoSurface.texture.height, PhotoSurface.texture.width, CvType.CV_8UC4);
            Utils.texture2DToMat((Texture2D)PhotoSurface.texture, fullMat);
            Mat croppedMat = new Mat(fullMat, rect);
            Texture2D croppedTex = new Texture2D(rect.width, rect.height, TextureFormat.RGBA32, false);
            Utils.matToTexture2D(croppedMat, croppedTex);
            fullMat.release();
            croppedMat.release();

            creatureData.image = croppedTex;
        }

        webcamTexture.Stop();
        webcamTexture = null;
        
        SceneManager.LoadScene("Edit");
    }

    

}
