using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


using UnityEngine.UI;
using OpenCvSharp;
using OpenCvSharp.Demo;
public class DrawBox : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject ass;
    [SerializeField] public Button NextButton;

    private LineRenderer lineRenderer;
    private RectTransform rt;
    
    public Vector2 StartPos = new Vector2(0, 0);
    public Vector2 EndPos = new Vector2(0, 0);
    private bool mousePressed = false;
    private Vector2 mouseLocalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (mousePressed)
        {   
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, Mouse.current.position.ReadValue(), canvas.worldCamera, out mouseLocalPos);
            if(mouseLocalPos.x < transform.position.x - rt.rect.width/2)
                mouseLocalPos.x = transform.position.x - rt.rect.width/2;
            else if(mouseLocalPos.x > transform.position.x + rt.rect.width/2)
                mouseLocalPos.x = transform.position.x + rt.rect.width/2;
            if(mouseLocalPos.y > transform.position.y + rt.rect.height/2)
                mouseLocalPos.y = transform.position.y + rt.rect.height/2;
            else if(mouseLocalPos.y < transform.position.y - rt.rect.height/2)
                mouseLocalPos.y = transform.position.y - rt.rect.height/2;
            

            lineRenderer.positionCount = 5;
            lineRenderer.SetPosition(0, new Vector3(StartPos.x, StartPos.y, -1));
            lineRenderer.SetPosition(1, new Vector3(mouseLocalPos.x, StartPos.y, -1));
            lineRenderer.SetPosition(2, new Vector3(mouseLocalPos.x, mouseLocalPos.y, -1));
            lineRenderer.SetPosition(3, new Vector3(StartPos.x, mouseLocalPos.y, -1));
            lineRenderer.SetPosition(4, new Vector3(StartPos.x, StartPos.y, -1));

        
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out StartPos))
        {
            UnityEngine.Debug.Log("Somethiong wrong");
        }

        mousePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndPos = mouseLocalPos;
        mousePressed = false;
        NextButton.gameObject.SetActive(true);
    }


}
