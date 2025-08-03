using DefaultNamespace;
using System.Collections.Generic;
using TMPro;
using TrackScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    [SerializeField] private GameObject ModifierTooltip;
    [SerializeField] private GameObject ShopTooltip;
    [SerializeField] private GameObject ItemTooltip;
    private GameObject curTooltip;
    private TextMeshProUGUI toolTipText;
    [SerializeField] private float toolTipPadding;
    [SerializeField] private float iconHeight;
    [SerializeField] private Color Electronic;
    [SerializeField] private Color Wind;
    [SerializeField] private Color String;
    [SerializeField] private Color Percussion;
    [SerializeField] private Color MusicBox;
    [SerializeField] private Color Anger;
    [SerializeField] private Color Joy;
    [SerializeField] private Color Fear;
    [SerializeField] private Color Envy;
    [SerializeField] private Color Sadness;
    private Dictionary<Tag, Color> tagColors;

    Camera cam;
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
        tagColors = new Dictionary<Tag, Color>
        {
            { Tag.Electronic, Electronic },
            { Tag.Wind, Wind },
            { Tag.String, String },
            { Tag.Percussion, Percussion },
            { Tag.MusicBox, MusicBox },
            { Tag.Anger, Anger },
            { Tag.Joy, Joy },
            { Tag.Fear, Fear },
            { Tag.Envy, Envy },
            { Tag.Sadness, Sadness },
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        curTooltip = null;
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {
        if (curTooltip != null)
        {
            rect = curTooltip.GetComponent<RectTransform>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                rect.anchoredPosition = getTooltipPositionCamera();
            }
            else
            {
                rect.transform.position = getTooltipPosition();
            }
        }
    }
    public void DisplayTrackTooltip(TrackSO track, bool shop)
    {
        if (curTooltip != null) HideTooltip();
        string color0 = "#" + ColorUtility.ToHtmlStringRGB(tagColors[track.tags[0]]);
        string color1 = "#" + ColorUtility.ToHtmlStringRGB(tagColors[track.tags[1]]);

        curTooltip = Instantiate(ShopTooltip, Vector2.zero, Quaternion.identity, transform);
        curTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = track.name;
        curTooltip.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + track.points;
        curTooltip.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + track.description;
        curTooltip.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=" + color0 + ">" + track.tags[0];
        curTooltip.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=" + color1 + ">" + track.tags[1];
        if (shop) curTooltip.transform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + track.price + "$";
        else curTooltip.transform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        if (track.description == null || track.description == "")
        {
            curTooltip.transform.GetChild(2).gameObject.SetActive(false);
            curTooltip.transform.GetChild(1).gameObject.SetActive(false);
        }
        rect = curTooltip.GetComponent<RectTransform>();
        curTooltip.SetActive(true);
        Canvas.ForceUpdateCanvases();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            rect.anchoredPosition = getTooltipPositionCamera();
        }
        else 
        {
            rect.transform.position = getTooltipPosition();
        }
    }
    public void DisplayItemTooltip(ItemSO item)
    {
        if (curTooltip != null) HideTooltip();

        curTooltip = Instantiate(ItemTooltip, Vector2.zero, Quaternion.identity, transform);
        curTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name;
        curTooltip.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + item.description;
        curTooltip.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + item.price + "$";
        rect = curTooltip.GetComponent<RectTransform>();
        curTooltip.SetActive(true);
        Canvas.ForceUpdateCanvases();

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            rect.anchoredPosition = getTooltipPositionCamera();
        }
        else
        {
            rect.transform.position = getTooltipPosition();
        }
    }
    public void DisplayModifierTooltip(string message)
    {
        if (curTooltip != null) HideTooltip();

        curTooltip = Instantiate(ModifierTooltip, Vector2.zero, Quaternion.identity, transform);
        curTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        rect = curTooltip.GetComponent<RectTransform>();
        curTooltip.SetActive(true);
        Canvas.ForceUpdateCanvases();

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            rect.anchoredPosition = getTooltipPositionCamera();
        }
        else
        {
            rect.transform.position = getTooltipPosition();
        }
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

        float xVal = Mouse.current.position.value.x + xSign * (rect.sizeDelta.x / canvasScaler.referenceResolution.x) * cam.pixelWidth * 0.5f + xSign * cam.pixelHeight * 0.04f;
        float yVal = Mouse.current.position.value.y + ySign * (rect.sizeDelta.y / canvasScaler.referenceResolution.y) * cam.pixelHeight * 0.5f + ySign * cam.pixelHeight * 0.04f;
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
                                                xSign * (rect.sizeDelta.x * 0.5f + toolTipPadding) + xSign * cam.pixelWidth * 0.02f,
                                                ySign * (rect.sizeDelta.y * 0.5f + toolTipPadding) + ySign * cam.pixelHeight * 0.02f  
                                            );

            Vector2 canvasOffset = GetCanvasOffset(screenOffset);
            float offsetX = xSign * (rect.sizeDelta.x * 0.5f + toolTipPadding) + xSign * canvasOffset.x;
            float offsetY = ySign * (rect.sizeDelta.y * 0.5f + toolTipPadding) + ySign * canvasOffset.y;

            return localPoint + screenOffset;
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