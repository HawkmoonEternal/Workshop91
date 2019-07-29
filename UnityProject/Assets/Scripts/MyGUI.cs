using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyGUI : MonoBehaviour
{
    public Text[] lastActionTexts;
    public Text ActionsOverviewText;
    public GameObject[] contentPanels;
    public static float fadeDuration=10;
    public static GameObject infoTextPanel;
    public GameObject infoTextPAnelNonStatic;
    public static Text infoText;
    public Text infoTextNonStatic;
    public Text cashText;
    public Text xpText;
    public Text levelText;
    public Text resultText;
    public Text summaryText;
    GameObject activeContent=null;
    public TouchInput touchScript;
    private protected Image defaultImage = null;
    public Sprite[] smileys;
    public Image resultSmileyImage;
    private AudioSource radio;
    public GameObject audioUI;
    public AudioClip[] channels;
    private int curChannelIndex;
    private StaticCoroutine routine;

    
    void Start()
    {
        routine = new StaticCoroutine();
        foreach (GameObject o in contentPanels) {
            o.SetActive(false);
        }
        infoText = infoTextNonStatic;
        infoTextPanel = infoTextPAnelNonStatic;
    }
 
    //Conetent-Panels 
    public void openContentPanel(int index) {
        if (contentPanels[index].name.Contains("Dummy")) {
            setInfoTextFadeout("You haven't unlocked this Action yet");
            return;
        }
        touchScript.disableTouchControl();
        Debug.Log("Button");
        if (activeContent != null) {
            if (activeContent == contentPanels[index]) return;
            else activeContent.SetActive(false);
        }
        contentPanels[index].SetActive(true);
        activeContent = contentPanels[index];
        Debug.Log(activeContent + " " + activeContent.activeInHierarchy);
    }
    public void closeAllContentPanels() {
        foreach (GameObject o in contentPanels)
        {
            o.SetActive(false);
        }
        activeContent = null;
        touchScript.enableTouchControl();
    }

    //OnClickFunctions
    public void onBuyButton(ShopItem item) {
        if (item.price > GameData.getCash())
        {
            setInfoTextFadeout("Not enough money. Work harder!");
            return;
        }
        if (!item.buyable) {
            setInfoTextFadeout("You've already bought the item '" + item.name + "'");
            return;
        }
        item.disableShopItem();
        gameObject.GetComponent<GameControl>().buyItem(item);
    }

    public void OnTakeActionParse(string action) {
        QuestItem.Action actionEnum = Parse(action);
        if (actionEnum != QuestItem.Action.idleAction) OnTakeActionButton((int)actionEnum);
    }
     public QuestItem.Action Parse(string myString)
    {
        QuestItem.Action enumerable=QuestItem.Action.idleAction;
        try
        {
            enumerable = (QuestItem.Action)System.Enum.Parse(typeof(QuestItem.Action), myString);
            //Foo(enumerable); //Now you have your enum, do whatever you want.
            
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Parse: Can't convert {0} to enum, please check the spell.", myString);
        }
        return enumerable;
    }

    public void OnTakeActionButton(int a)
    {
        QuestItem.Action action = (QuestItem.Action)a;
        QuestItem currentQuest = gameObject.GetComponent<GameControl>().getCurrentQuest().GetComponent<QuestItem>();
        currentQuest.controlAction(action);
        SetLastActionTexts();
        setInfoTextFadeout("Performed Action: '" + QuestItem.ActionIdentifiers[(int)action]+"'");
    }
    public void OnRevertAll(bool trueRevert) {
        GameObject questObj = gameObject.GetComponent<GameControl>().getCurrentQuest();
        foreach(ArrayOfLists l in questObj.GetComponent<QuestItem>().getActionsDone()) {
            l.actions = new List<QuestItem.Action>();
        }
        questObj.GetComponent<QuestItem>().getOverflowActions().Clear();

        questObj.GetComponent<QuestItem>().setCurIndex(0);
        questObj.GetComponent<QuestItem>().setDefaultToDo();
        questObj.GetComponent<QuestItem>().resetErrors();
        foreach (Text t in lastActionTexts)
        {
            t.text = "Last: No Action Done";
        }
        ActionsOverviewText.text = "";
        if(trueRevert)setInfoTextFadeout("Reverted all Actions done. Now try it again.");
    }
    public void OnAcceptQuest(GameObject Quest)
    {
        
        if (Quest.GetComponent<QuestItem>().buttonText.text.Equals("Finished")) return;
        if (gameObject.GetComponent<GameControl>().getCurrentQuest() == null)
        {
            if (Quest.GetComponent<QuestItem>().level > GameData.getLevel()) {
                setInfoTextFadeout("You have to be level " + Quest.GetComponent<QuestItem>().level + " to accept this Order");
                return;
            }
            gameObject.GetComponent<GameControl>().setCurrentQuest(Quest);
             
            setInfoTextFadeout("Accepted Quest");
            Quest.GetComponent<QuestItem>().buttonText.text = "Finish Quest";
        }
        else if (gameObject.GetComponent<GameControl>().getCurrentQuest() == Quest)
        {
            OnFinishQuestPreface();

        }
        else
        {
            Debug.Log("You already have an active Quest");
            setInfoTextFadeout("You already have an active Quest");
        }
    }

    public void OnFinishQuestPreface() {
        QuestItem  quest = gameObject.GetComponent<GameControl>().getCurrentQuest().GetComponent<QuestItem>();
        int index=1;
        string text = "";
        foreach (ArrayOfLists l in quest.getActionsDone()) {
            foreach (QuestItem.Action action in l.actions) {
                text += index + ". " + QuestItem.ActionIdentifiers[(int)action] + "\n";
                index++;
            }
        }
        foreach (QuestItem.Action a in quest.getOverflowActions()) {
            text += index + ". " + QuestItem.ActionIdentifiers[(int)a] + "\n";
            index++;
        }

        ActionsOverviewText.text = text;
        openContentPanel(17);
    }
    public void OnFinishQuest()
    {
        GameObject questObj = gameObject.GetComponent<GameControl>().getCurrentQuest();
        if (questObj.GetComponent<QuestItem>().getFinalErrorCount() == 0)
        {
            questObj.GetComponent<QuestItem>().buttonText.text = "Finished";
            questObj.GetComponent<QuestItem>().acceptButton.interactable = false;
        }
        else
        {
            questObj.GetComponent<QuestItem>().buttonText.text = "Accept Quest";
        }
        setInfoTextFadeout("Finished Quest");
        //TODO: Auswertung
        int errors = questObj.GetComponent<QuestItem>().getFinalErrorCount();
        Debug.Log("Errors: " + errors);
       
        string result = "";
        string summary = "";
        QuestItem quest = questObj.GetComponent<QuestItem>();
        quest.destroyCarInstance();
        ArrayOfLists[] order = quest.getOrderOfActios();
        ArrayOfLists[] done = quest.getActionsDone();
        int cash = errors > 0 ? 0 : quest.cashReward;
        int xp = errors > 0 ? 0 : quest.xpReward;
        resultSmileyImage.sprite = errors > 0 ? smileys[1] : smileys[0];
        
        summary += "Overall Mistakes: "+quest.getFinalErrorCount()+"\n";
        summary += "Wrong Actions: " + (quest.getFinalErrorCount() - quest.getUnnecessaryActionsCount())+"\n";
        //summary += "Unnecessary Actions: " + quest.getUnnecessaryActionsCount()+"\n";

        summary += "Cash Reward: " + cash + "\n";
        summary += "XP Reward: " + xp + "\n";
        summaryText.text = summary;
        int outer = 0;
        foreach (ArrayOfLists l in order) {
            int index = 0;
            
            foreach (QuestItem.Action action in l.actions) {
                
                result += (outer+1)+". "+  QuestItem.ActionIdentifiers[(int)action]+ "\n";
                if (index < done[outer].actions.Count)
                {
                    if (l.actions.Contains(done[outer].actions.ToArray()[index])){
                        result += "✓ " + "<color=green>" + QuestItem.ActionIdentifiers[(int)done[outer].actions.ToArray()[index]]+"</color>";
                    }
                    else
                    {
                        result += "X " + "<color=red>" + QuestItem.ActionIdentifiers[(int)done[outer].actions.ToArray()[index]]+ "</color>";
                    }
                }
                else {
                    result += "X No Action";
                }
                index++;
                
                result += "\n\n";
            }
            outer++;
        }
        resultText.text = result;
        openContentPanel(4);
        if (errors == 0)
        {
            GameData.updateCash(quest.cashReward);
            GameData.updateXP(quest.xpReward);
            updateTopPanelElements();
        }
        OnRevertAll(false);
        gameObject.GetComponent<GameControl>().setCurrentQuest(null);

    }

    public void OnRevert() {
        QuestItem quest = GetComponent<GameControl>().getCurrentQuest().GetComponent<QuestItem>();
        QuestItem.Action revertedAction = quest.revertLastAction();
        SetLastActionTexts();
        if (revertedAction==QuestItem.Action.idleAction) {
            setInfoTextFadeout("Couldn't revert last action.\n No action done yet.");
        }
        else
        {
            setInfoTextFadeout("Reverted Action '" + QuestItem.ActionIdentifiers[(int)revertedAction] + "'");
        }
    }

    public void OnChooseOwnAudioClip() {
        openContentPanel(8);
    }
    //Setting Texts
    public void OnRadioPlay() {
        
        if (radio.clip == null)
        {
            if (channels.Length != 0)
            {
                radio.clip = channels[curChannelIndex % channels.Length];
            }
            else
            {
                setInfoTextFadeout("No clip choosen!");
                return;
            }
        }
        radio.Play();
    }
    public void OnRadioPause()
    {
        if (radio.clip == null)
        {
            setInfoTextFadeout("No clip choosen");
            return;
        }
        radio.Pause();
    }
    public void OnChangeChannel() {
        if (channels.Length == 0) {
            setInfoTextFadeout("No legal channels. Thats Soviet ;)");
            return;
        }
        curChannelIndex = ++curChannelIndex % channels.Length;
        radio.clip = channels[curChannelIndex];
        radio.Play();
        setInfoTextFadeout("Switched to Channel " + radio.clip.name);
    }
    public void OnMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public static void setInfoTextFadeout(string text) {
        Debug.Log("setInfoTextFadeout");
        infoText.text = text;      
        StaticCoroutine.DoCoroutine(new GameObject[] { infoTextPanel }, new Text[] { infoText }, infoTextPanel.GetComponentsInChildren<Image>(), fadeDuration);
    }

    void updateTopPanelElements() {
        setCashText();
        setXPText();
        setLevelText();
    }

    public void setCashText()
    {
        cashText.text = "₽ " + GameData.getCash();
    }
    public void setXPText()
    {
        xpText.text = "XP " + GameData.getXP();
    }
    public void setLevelText()
    {
        levelText.text = "" + GameData.getLevel();
    }
    public void setRadio(AudioSource r) {
        radio = r;
    }
    private void SetLastActionTexts() {
        QuestItem quest = GetComponent<GameControl>().getCurrentQuest().GetComponent<QuestItem>();
        
        foreach (Text t in lastActionTexts)
        {
            t.text = quest.getLastActionDone() == QuestItem.Action.idleAction ? "Last: No Action Done" : "Last: " + QuestItem.ActionIdentifiers[(int)quest.getLastActionDone()];
        }
    }
    
}
