using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Obvious.Soap;

public class ShopController : MonoBehaviour
{
    //is my naming trash? yes.
    public Transform buyTracksUI;
    public Transform ownedTracksUI;
    public Transform buyItemsUI;
    public Transform ownedItemsUI;
    public GameObject BItem;
    public GameObject BTrack;

    private GameObject selectedTrack;


    private List<string> tempShopItems = new() { "a", "b", "c" };
    private List<string> tempShopTracks = new() { "e","d","f"};
    private List<string> tempOwnedItems = new() { "1","2","3"};
    private List<string> tempOwnedTracks = new() { "hol","lee","fukl"};

    [SerializeField] private IntVariable dollars;

    void Start()
    {
        UpdateShop();
    }

    void BuyTrack(GameObject e)
    {
        string thing = e.GetComponentInChildren<TextMeshProUGUI>().text;
        tempOwnedTracks.Add(thing);
        var f = Instantiate(BTrack, ownedTracksUI);
        f.GetComponentInChildren<TextMeshProUGUI>().text = thing;

        dollars.Value -= 1;

        Destroy(e);
    }

    void BuyItem(GameObject e)
    {
        string thing = e.GetComponentInChildren<TextMeshProUGUI>().text;
        tempOwnedItems.Add(thing);
        var f = Instantiate(BItem, ownedItemsUI);
        f.GetComponentInChildren<TextMeshProUGUI>().text = thing;

        dollars.Value -= 1;

        Destroy(e);
    }

    void SelectTrack(GameObject e)
    {
        if (selectedTrack == e) return;
        if (selectedTrack != null)
        {
            //reset colour of prev selection
        }

        selectedTrack = e;
        //add highlight logic here
    }

    public void SellTrack()
    {
        if (selectedTrack == null) return;
        string thing = selectedTrack.GetComponentInChildren<TextMeshProUGUI>().text;
        tempOwnedTracks.Remove(thing);

        Destroy(selectedTrack);
        selectedTrack = null;

        dollars.Value += 1;
    }

    public void Reroll()
    {
        Shuffle(tempShopTracks);
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

    void UpdateShop()
    {

        foreach (Transform child in buyItemsUI) Destroy(child.gameObject);
        foreach (Transform child in buyTracksUI) Destroy(child.gameObject);
        foreach (Transform child in ownedItemsUI) Destroy(child.gameObject);
        foreach (Transform child in ownedTracksUI) Destroy(child.gameObject);
        //create buttons, need a price thign in a sec, its getting bad
        foreach (string thing in tempShopItems)
        {
            Debug.Log(thing);
            var e = Instantiate(BItem, buyItemsUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = thing;

            var copy = e;
            e.GetComponentInChildren<Button>().onClick.AddListener(() => BuyItem(copy));
        }
        foreach (string thing in tempShopTracks)
        {
            var e = Instantiate(BTrack, buyTracksUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = thing;

            var copy = e;
            e.GetComponentInChildren<Button>().onClick.AddListener(() => BuyTrack(copy));
        }
        foreach (string thing in tempOwnedItems)
        {
            var e = Instantiate(BItem, ownedItemsUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = thing;
            //e.GetComponentInChildren<Button>().onClick.AddListener(() => BuyTrack(e));
        }
        foreach (string thing in tempOwnedTracks)
        {
            var e = Instantiate(BTrack, ownedTracksUI);
            e.GetComponentInChildren<TextMeshProUGUI>().text = thing;
            e.GetComponentInChildren<Button>().onClick.AddListener(() => SelectTrack(e));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
