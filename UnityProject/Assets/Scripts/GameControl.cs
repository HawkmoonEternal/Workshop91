using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameControl : MonoBehaviour
{
    MyGUI gui;
    public GameObject currentQuest;
    public Scripts.DemoCaller posterDemoCaller;
    public GameObject posterUI;
    public Scripts.DemoCaller audioDemoCaller;
    public GameObject Bilderrahmen;
    // Start is called before the first frame update
    void Start()
    {
        gui = gameObject.GetComponent<MyGUI>();
        gui.setCashText();
        currentQuest = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void buyItem(ShopItem item) {

        GameObject instantiatedItem=item.InstatiateItem();
        GameData.updateCash(-item.price);
        gui.setCashText();
        MyGUI.setInfoTextFadeout("Successfully bought item " + item.title.text);
        
        if (item.itemPrfab.name.StartsWith("Poster"))
        {
            posterDemoCaller.setCurPoster(instantiatedItem.transform.GetChild(0).gameObject);
            GetComponent<MyGUI>().openContentPanel(9);
        }
        else if (item.itemPrfab.name.StartsWith("Radio")) {
            audioDemoCaller.setRadio(instantiatedItem.GetComponent<AudioSource>());
            GetComponent<MyGUI>().setRadio(instantiatedItem.GetComponent<AudioSource>());
        }

    }


    public GameObject getCurrentQuest() {
        return currentQuest;
    }
    public void setCurrentQuest(GameObject Quest) {
        currentQuest = Quest;
        if (currentQuest != null) currentQuest.GetComponent<QuestItem>().startQuest();
    }
}
