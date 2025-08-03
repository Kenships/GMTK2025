using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    [SerializeField] private GameObject IconTooltip;
    [SerializeField] private GameObject NoIconTooltip;
    private GameObject curTooltip;
    private TextMeshProUGUI toolTipText;
    private RectTransform textBoxRect;
    [SerializeField] private float toolTipPadding;
    [SerializeField] private float iconHeight;
    Camera cam;
    Vector3 min, max;
    RectTransform rect;
    Canvas canvas;
    CanvasScaler canvasScaler;

    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        min = new Vector3(0, 0, 0);
        max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
        curTooltip = null;
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {
        if (curTooltip != null)
        {
            rect = curTooltip.GetComponent<RectTransform>();
            rect.anchoredPosition = canvas.renderMode == RenderMode.ScreenSpaceCamera ? getTooltipPositionCamera() : getTooltipPosition();
        }
    }

    public void DisplayTooltip(Sprite icon, string message) 
    {
        if (curTooltip != null) HideTooltip();
        Debug.Log("Yays");
        string color0 = "#" + ColorUtility.ToHtmlStringRGB(tagColors[track.tags[0]]);
        string color1 = "#" + ColorUtility.ToHtmlStringRGB(tagColors[track.tags[1]]);

        curTooltip = Instantiate(NoIconTooltip, Vector2.zero, Quaternion.identity, transform);
        curTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = track.name;
        curTooltip.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + track.points;
        curTooltip.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + track.description;
        curTooltip.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=" + color0 + ">" + track.tags[0];
        curTooltip.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color="+ color1 + ">" + track.tags[1];
        curTooltip.transform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + track.price + "$";
        if (track.description == null || track.description == "") 
        {
            curTooltip.transform.GetChild(2).gameObject.SetActive(false);
            curTooltip.transform.GetChild(1).gameObject.SetActive(false);
        }
        rect = curTooltip.GetComponent<RectTransform>();
        curTooltip.SetActive(true);
        Canvas.ForceUpdateCanvases();

        rect.anchoredPosition = canvas.renderMode == RenderMode.ScreenSpaceCamera ? getTooltipPositionCamera() : getTooltipPosition();
    }
    public void DisplayTooltip(string message) 
    {
        if (curTooltip != null) HideTooltip();

        curTooltip = Instantiate(NoIconTooltip, Vector2.zero, Quaternion.identity, transform);
        toolTipText = curTooltip.GetComponentInChildren<TextMeshProUGUI>();
        Vector2 size = toolTipText.GetPreferredValues(message);
        toolTipText.text = message;
        toolTipText.GetPreferredValues(message);
        rect = curTooltip.GetComponent<RectTransform>();
        rect.anchoredPosition = canvas.renderMode == RenderMode.ScreenSpaceCamera ? getTooltipPositionCamera() : getTooltipPosition();
        curTooltip.SetActive(true);
    }
    public void HideTooltip() 
    {
        if (curTooltip == null) return;
        Destroy(curTooltip);
        curTooltip = null;
    }
    private Vector2 getTooltipPosition()
    {
        Vector2 mousePos;

        float xSign = (Mouse.current.position.value.x > cam.pixelWidth / 2.0f) ? -1f : 1f;
        float ySign = (Mouse.current.position.value.y > cam.pixelHeight / 2.0f) ? -1f : 1f;

        float xVal = Mouse.current.position.value.x + xSign * (rect.rect.width / canvasScaler.referenceResolution.x) * cam.pixelWidth * 0.5f + xSign * cam.pixelHeight * 0.04f;
        float yVal = Mouse.current.position.value.y + ySign * (rect.rect.height / canvasScaler.referenceResolution.y) * cam.pixelHeight * 0.5f + ySign * cam.pixelHeight * 0.04f;
        mousePos = new Vector2(xVal, yVal);
        return mousePos;
    }
    private Vector2 getTooltipPositionCamera()
    {
        Vector2 screenPoint = Mouse.current.position.value;
        RectTransform canvasRect = GetComponent<RectTransform>();

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint))
        {
            float xSign = (screenPoint.x > cam.pixelWidth / 2f) ? -1f : 1f;
            float ySign = (screenPoint.y > cam.pixelHeight / 2f) ? -1f : 1f;
            Vector2 screenOffset = new Vector2(
                                                xSign * cam.pixelWidth * 0.04f,   
                                                ySign * cam.pixelHeight * 0.04f  
                                            );

            Vector2 canvasOffset = GetCanvasOffset(screenOffset);
            float offsetX = xSign * (rect.sizeDelta.x * 0.5f + toolTipPadding) + xSign * canvasOffset.x;
            float offsetY = ySign * (rect.sizeDelta.y * 0.5f + toolTipPadding) + ySign * canvasOffset.y;

            return localPoint + new Vector2(offsetX, offsetY);
        }

        return Vector2.zero;
    }
    private Vector2 GetCanvasOffset(Vector2 screenOffset)
    {
        Vector2 screenCenter = new Vector2(cam.pixelWidth / 2f, cam.pixelHeight / 2f);
        Vector2 screenPoint = screenCenter + screenOffset;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 centerLocal;
        Vector2 offsetLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenCenter, cam, out centerLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out offsetLocal);

        return offsetLocal - centerLocal;
    }
}
