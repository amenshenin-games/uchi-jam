using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

using OpenCVForUnity.ImgprocModule; 
using OpenCVForUnity.UnityUtils;

public class Countur : MonoBehaviour
{
    [SerializeField] public Slider ThresholdSlider;
    [SerializeField] public Slider MinAreaSlider;
    [SerializeField] public RawImage PhotoSurface;
    [SerializeField] public Button NextButton;
    [SerializeField] public Toggle ShowOrig;
    
    private Mat image;
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
        Texture2D passedTexture = creatureData.image;

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
        
        image = new Mat(passedTexture.height, passedTexture.width, CvType.CV_8UC4);
        //processedImage = new Mat(passedTexture.height, passedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(passedTexture, image);

        ProcessTexture(0);
    }
    
    private void ProcessTexture(float valueFromChangedSlider)
    {
        

        float Threshold = ThresholdSlider.value;
        float MinArea = MinAreaSlider.value;

        Mat processedImage = new Mat(); 
        image.copyTo(processedImage);
        Imgproc.cvtColor(processedImage, processedImage, Imgproc.COLOR_RGBA2GRAY);
        Imgproc.threshold(processedImage, processedImage, Threshold, 225, Imgproc.THRESH_BINARY_INV );
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(processedImage, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        Mat mask = new Mat(processedImage.size(), CvType.CV_8UC1, new Scalar(0));
        foreach (MatOfPoint contour in contours)
        {
            MatOfPoint2f contour2f = new MatOfPoint2f(contour.toArray());
            MatOfPoint2f approx2f = new MatOfPoint2f();
            double epsilon = 0.02 * Imgproc.arcLength(contour2f, true);
            Imgproc.approxPolyDP(contour2f, approx2f, epsilon, true);
            if (Imgproc.contourArea(contour2f, false) > MinArea)
            {
                Imgproc.drawContours(mask, new List<MatOfPoint> { contour}, -1, new Scalar(255), thickness: -1);
            }
        }


        Mat dst = new Mat(image.size(), CvType.CV_8UC4, new Scalar(0, 0, 0, 0));
        image.copyTo(dst, mask); 
        //OpenCVForUnity.CoreModule.Rect roi = Imgproc.boundingRect(contours[maxIdx]);
        //Mat finalCropped = new Mat(dst, roi);
        Texture2D newTexture = new Texture2D(dst.cols(), dst.rows(), TextureFormat.RGBA32, false);

        if (!ShowOrig.isOn)
        {
            Utils.matToTexture2D(dst, newTexture);

            //Utils.matToTexture2D(transparentMat, newTexture);
            //CanvasImage.texture = OpenCvSharp.Unity.MatToTexture(transparentMat);
            
        }
        else
        {
            Utils.matToTexture2D(processedImage, newTexture);
            //CanvasImage.texture = OpenCvSharp.Unity.MatToTexture(processedImage);
        }
        PhotoSurface.texture = newTexture;
        //processedImage = dst;
    }

    private void OnNextButton()
    {
        creatureObject.GetComponent<CreatureData>().image = (Texture2D)PhotoSurface.texture;
        SceneManager.LoadScene("Sound");
    }

}
