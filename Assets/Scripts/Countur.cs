using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class Countur : MonoBehaviour
{
    [SerializeField] public Slider ThresholdSlider;
    [SerializeField] public Slider MinAreaSlider;
    [SerializeField] public GameObject PhotoSurface;
    [SerializeField] public float CurveAccuracy = 5;
    [SerializeField] public Button NextButton;
    [SerializeField] public Toggle ShowOrig;
    

    private OpenCvSharp.Point[][] Contours;
    private Mat image;
    private Mat processedImage = new Mat();
    private RawImage CanvasImage;
    private RectTransform rt;
    private GameObject creatureObject;

    public void Start()
    {
        ThresholdSlider.onValueChanged.AddListener(ProcessTexture);
        MinAreaSlider.onValueChanged.AddListener(ProcessTexture);
        NextButton.onClick.AddListener(OnNextButton);
        ShowOrig.onValueChanged.AddListener(delegate {
            ProcessTexture(0);
        });


        CanvasImage = GetComponent<RawImage>();
        rt = GetComponent<RectTransform>();
        creatureObject = GameObject.Find("CreatureObject"); 
        CreatureData creatureData = creatureObject.GetComponent<CreatureData>();
        Texture passedTexture = OpenCvSharp.Unity.MatToTexture(creatureData.image);
        image = creatureData.image;

        float newWidth = passedTexture.width;
        float newHeight = passedTexture.height;

        if (newHeight > rt.rect.height)
        {
            float coef = rt.rect.height / newHeight;
            newWidth *= coef;
            newHeight = rt.rect.height;
        }
        
        if (newWidth > rt.rect.width)
        {
            float coef = rt.rect.width / newWidth;
            newWidth = rt.rect.width;
            newHeight *= coef;
        }
        

        CanvasImage.texture = passedTexture;
        rt.sizeDelta = new Vector2(newWidth, newHeight);

    }
    
    private void ProcessTexture(float valueFromChangedSlider)
    {
        float Threshold = ThresholdSlider.value;
        float MinArea = MinAreaSlider.value;

        Cv2.CvtColor(image, processedImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processedImage, processedImage, Threshold, 225, ThresholdTypes.BinaryInv);
        Cv2.FindContours(processedImage, out Contours, out _, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
        Mat mask = new Mat(processedImage.Size(), MatType.CV_8UC1, Scalar.All(0));
        foreach (OpenCvSharp.Point[] contour in Contours)
        {
            OpenCvSharp.Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            if (Cv2.ContourArea(contour) > MinArea)
            {
                Cv2.DrawContours(mask, new[] { contour}, -1, Scalar.All(225), thickness: -1);
            }
        }
        
        Mat bgra = new Mat();
        Cv2.CvtColor(image, bgra, ColorConversionCodes.BGR2BGRA);
        Mat transparentMat = new Mat(bgra.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
        bgra.CopyTo(transparentMat, mask);

        if (!ShowOrig.isOn)
        {
            CanvasImage.texture = OpenCvSharp.Unity.MatToTexture(transparentMat);
            
        }
        else
        {
            CanvasImage.texture = OpenCvSharp.Unity.MatToTexture(processedImage);
        }
        
        processedImage = transparentMat;
    }

    private void OnNextButton()
    {
        creatureObject.GetComponent<CreatureData>().image = processedImage;
        SceneManager.LoadScene("Sound");
    }

}
