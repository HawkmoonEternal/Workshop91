using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.UIElements;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image img;
    public Text title;
    public Text priceText;
    public int price;
    public Button buyButton;
    public Image backImg;
    public GameObject itemPrfab;
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    private Transform itemTransform;
    public bool buyable { get; set; } = true;
    
    // Start is called before the first frame update
    void Start()
    {
        itemPrfab.transform.position = itemPosition;
        itemPrfab.transform.eulerAngles = itemRotation;
        priceText.text = price+ "₽";

    }

    // Update is called once per frame
    public void disableShopItem() {
        buyable = false;
        buyButton.GetComponentInChildren<Text>().text = "Bought";
        Color c1 = Color.grey;
        c1.a = 0.5f;
        buyButton.GetComponent<Image>().color = c1;
        Color c=backImg.color;
        c.a = 0.5f;
        backImg.color = c;
    }
    public GameObject InstatiateItem() {
        GameObject o=Instantiate(itemPrfab,itemPosition,Quaternion.Euler(itemRotation));
        Debug.Log(o);
        return o;
    }

}
