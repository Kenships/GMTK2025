using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Obvious.Soap;
using DefaultNamespace;
using TrackScripts;

public class ShopController : MonoBehaviour
{
    //is my naming trash? yes.
    public InventorySO playerInventory;
    public ShopBankSO shopBank;

    public Transform buyTracksUI;
    public Transform ownedTracksUI;
    public Transform buyItemsUI;
    public Transform ownedItemsUI;
    public GameObject BItem;
    public GameObject BTrack;
    public GameObject OTrack;
    public GameObject BuyEffect;
    public int maxBuyTrack;
    public int maxBuyItem;
    public int maxOwnedTrack;
    public int maxOwnedItem;

    private GameObject selectedTrackGO;
    private TrackSO selectedTrackSO;

    [SerializeField] private IntVariable dollars;
    [SerializeField] private TMP_Text buttonText;
    private int rerollPrice;

    void Start()
    {
        var tempInventory = Instantiate(playerInventory);
        // tempInventory.tracks = new List<TrackSO>(playerInventory.tracks);
        // tempInventory.items = new List<ItemSO>(playerInventory.items);
        playerInventory = tempInventory;

        var tempBank = Instantiate(shopBank);
        // tempBank.tracks = new List<TrackSO>(playerInventory.tracks);
        // tempBank.items = new List<ItemSO>(playerInventory.items);
        shopBank = tempBank;
        rerollPrice = 1;
        buttonText.text = "REROLL: $"+rerollPrice;
        UpdateShop();
        UpdateInventory();
    }

    void UpdateShop()
    {
        foreach (Transform t in buyItemsUI) Destroy(t.gameObject);
        foreach (Transform t in buyTracksUI) Destroy(t.gameObject);

        //create buttons, need a price thign in a sec, codes getting bad 🤮🤮🤮 idgaf ill fight my own demons
        int count = 0;
        foreach (var item in shopBank.availableItems)
        {
            //no dupes
            if (playerInventory.items.Contains(item)) continue;
            if (count >= maxBuyItem) break;
            count++;

            var e = Instantiate(BItem, buyItemsUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text =  $"${item.price}";

            var holder = e.GetComponent<ItemHolder>();
            holder.Item = item;

            e.GetComponentInChildren<Button>().onClick.AddListener(() => BuyItem(item, e));
        }
        count = 0; //this is insanity
        foreach (var track in shopBank.availableTracks)
        {
            //no dupes, it really shouldnt even be here cuz not possible technically
            if (playerInventory.tracks.Contains(track)) continue;
            if (count >= maxBuyTrack) break;
            count++;

            var e = Instantiate(BTrack, buyTracksUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = $"${track.price}";


            var holder = e.GetComponent<TrackHolder>();
            holder.Track = track;


            e.GetComponentInChildren<Button>().onClick.AddListener(() => BuyTrack(track, e));
        }
    }
     void UpdateInventory()
    {
        foreach (Transform t in ownedItemsUI) Destroy(t.gameObject);
        foreach (Transform t in ownedTracksUI) Destroy(t.gameObject);


        foreach (var item in playerInventory.items)
        {
            var e = Instantiate(BItem, ownedItemsUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;

            var holder = e.GetComponent<ItemHolder>();
            holder.Item = item;
    

        }
        foreach (var track in playerInventory.tracks)
        {
            var e = Instantiate(OTrack, ownedTracksUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = track.trackName;

            var holder = e.GetComponent<TrackHolder>();
            holder.Track = track;

            Debug.Log(track.albumCover);
            var albumImage = e.transform.Find("Image").GetComponent<Image>();
            albumImage.sprite = track.albumCover;

            
            e.GetComponentInChildren<Button>().onClick.AddListener(() => SelectTrack(e));
        }
    }


    void BuyTrack(TrackSO track, GameObject e)
    {
        //MAKE SURE TO ADD A VISUAL TO SHOW U CANT BUY IT
        if (dollars.Value < track.price) return;
        if (playerInventory.tracks.Count >= maxOwnedTrack) return;

        Canvas mainCanvas = FindObjectOfType<Canvas>();

        var effect = Instantiate(BuyEffect, e.transform.position, Quaternion.identity);

        RectTransform effectRect = effect.GetComponent<RectTransform>();
        Vector2 originalSize = effectRect.sizeDelta;
        Vector3 originalScale = effectRect.localScale;

        effect.transform.SetParent(mainCanvas.transform, worldPositionStays: true);

        effectRect.sizeDelta = originalSize;
        effectRect.localScale = originalScale;


        playerInventory.tracks.Add(track);
        dollars.Value -= track.price;

        shopBank.availableTracks.Remove(track);

        //Destroy(e);
        e.GetComponentInChildren<Button>().interactable = false;
        var cg = e.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;

        UpdateInventory();
    }

    void BuyItem(ItemSO item, GameObject e)
    {
        if (dollars.Value < item.price) return;
        if (playerInventory.items.Count >= maxOwnedItem) return;

        Canvas mainCanvas = FindObjectOfType<Canvas>();

        var effect = Instantiate(BuyEffect, e.transform.position, Quaternion.identity);

        RectTransform effectRect = effect.GetComponent<RectTransform>();
        Vector2 originalSize = effectRect.sizeDelta;
        Vector3 originalScale = effectRect.localScale;

        effect.transform.SetParent(mainCanvas.transform, worldPositionStays: true);

        effectRect.sizeDelta = originalSize;
        effectRect.localScale = originalScale;

        playerInventory.items.Add(item);
        dollars.Value -= item.price;

        shopBank.availableItems.Remove(item);

        // Destroy(e);
        e.GetComponentInChildren<Button>().interactable = false;
        var cg = e.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;

        UpdateInventory();
    }

    void SelectTrack(GameObject e)
    {
        if (selectedTrackGO == e) return;
        if (selectedTrackGO != null)
        {
            //reset colour of prev selection
        }

        selectedTrackGO = e;
        selectedTrackSO = e.GetComponent<TrackHolder>().Track;
        //add highlight logic here
    }

    public void SellTrack()
    {
        if (selectedTrackGO == null) return;

        if (playerInventory.tracks.Contains(selectedTrackSO))
        {
            playerInventory.tracks.Remove(selectedTrackSO);
            shopBank.availableTracks.Add(selectedTrackSO);
            dollars.Value += selectedTrackSO.price;
        }

        selectedTrackGO = null;
        selectedTrackSO = null;

        UpdateInventory();
    }

    public void Reroll()
    {
        if (dollars.Value < rerollPrice) return;
        dollars.Value -= rerollPrice;
        rerollPrice = rerollPrice + 1;
        buttonText.text = "REROLL: $"+rerollPrice;
        Shuffle(shopBank.availableTracks);
        UpdateShop();
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

   
    // Update is called once per frame
    void Update()
    {

    }
}
