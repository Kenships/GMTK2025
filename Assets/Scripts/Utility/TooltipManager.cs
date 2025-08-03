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
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {
        if (curTooltip != null)
        {
            curTooltip.transform.position = getTooltipPosition();
        }
    }

    public void DisplayTooltip(Sprite icon, string message) 
    {
        if (curTooltip != null) HideTooltip();

        curTooltip = Instantiate(IconTooltip, Vector2.zero, Quaternion.identity, transform);
        toolTipText = curTooltip.GetComponentInChildren<TextMeshProUGUI>();
        Vector2 size = toolTipText.GetPreferredValues(message);
        toolTipText.text = message;
        Image image = curTooltip.transform.GetChild(1).GetComponent<Image>();
        image.sprite = icon;
        toolTipText.GetPreferredValues(message);
        rect = curTooltip.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size.x + toolTipPadding * 2f, size.y + toolTipPadding * 3f + iconHeight);
        textBoxRect = toolTipText.GetComponent<RectTransform>();
        textBoxRect.sizeDelta = size;
        textBoxRect.anchoredPosition = new Vector2(size.x / 2 + toolTipPadding, -size.y / 2 - 2 * toolTipPadding - iconHeight);
        rect.anchoredPosition = getTooltipPosition();
        image.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -toolTipPadding - iconHeight / 2);
        curTooltip.SetActive(true);
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
        rect.sizeDelta = new Vector2(size.x + toolTipPadding * 2f, size.y + toolTipPadding * 2f);
        textBoxRect = toolTipText.GetComponent<RectTransform>();
        textBoxRect.sizeDelta = size;
        textBoxRect.anchoredPosition = new Vector2(size.x/2 + toolTipPadding, -size.y/2 - toolTipPadding);
        rect.anchoredPosition = getTooltipPosition();
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
}
