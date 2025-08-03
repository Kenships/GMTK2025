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
    public int maxBuyTrack;
    public int maxBuyItem;
    public int maxOwnedTrack;
    public int maxOwnedItem;

    private GameObject selectedTrackGO;
    private TrackSO selectedTrackSO;

    [SerializeField] private IntVariable dollars;

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
            e.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.itemName} - ${item.price}";

            var holder = e.GetComponent<ItemHolder>();
            holder.Item = item;

            var tooltip = e.AddComponent<Tooltip>();
            tooltip.Message = $"{item.itemName}\nBuy: ${item.price}\n{item.description}";
            //tooltip.sprite = track.icon;

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
            e.GetComponentInChildren<TextMeshProUGUI>().text = $"{track.trackName} - ${track.price}";

            var holder = e.GetComponent<TrackHolder>();
            holder.Track = track;

            var tooltip = e.AddComponent<Tooltip>();
            tooltip.Message = $"{track.trackName}\nBuy: ${track.price}\n{track.description}";
            //tooltip.sprite = track.icon;

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
            
            var tooltip = e.AddComponent<Tooltip>();
            tooltip.Message = $"{item.itemName}\n{item.description}";
            //tooltip.sprite = track.icon;

        }
        foreach (var track in playerInventory.tracks)
        {
            var e = Instantiate(BTrack, ownedTracksUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = track.trackName;

            var holder = e.GetComponent<TrackHolder>();
            holder.Track = track;

            var tooltip = e.AddComponent<Tooltip>();
            tooltip.Message = $"{track.trackName}\nSell: ${track.price}\n{track.description}";
            //tooltip.sprite = track.icon;
            
            e.GetComponentInChildren<Button>().onClick.AddListener(() => SelectTrack(e));
        }
    }


    void BuyTrack(TrackSO track, GameObject e)
    {
        //MAKE SURE TO ADD A VISUAL TO SHOW U CANT BUY IT
        if (dollars.Value < track.price) return;
        if (playerInventory.tracks.Count >= maxOwnedTrack) return;

        playerInventory.tracks.Add(track);
        dollars.Value -= track.price;

        shopBank.availableTracks.Remove(track);

        //Destroy(e);
        e.GetComponentInChildren<Button>().interactable = false;
        e.GetComponent<CanvasGroup>().alpha = 0f;

        UpdateInventory();
    }

    void BuyItem(ItemSO item, GameObject e)
    {
        if (dollars.Value < item.price) return;
        if (playerInventory.items.Count >= maxOwnedItem) return;

        playerInventory.items.Add(item);
        dollars.Value -= item.price;

        shopBank.availableItems.Remove(item);

        // Destroy(e);
        e.GetComponentInChildren<Button>().interactable = false;
        e.GetComponent<CanvasGroup>().alpha = 0f;

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
