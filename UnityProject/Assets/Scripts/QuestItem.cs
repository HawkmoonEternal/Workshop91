using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    
    private GameObject carInstance;
    private int curIndex=0;
    private int errors=0;
    public Button acceptButton;
    public Text buttonText;
    private int unnecessaryActions = 0;
    public int cashReward;
    public int xpReward;
    public int level;
    public GameObject car;
    public static Vector3 carPos;
    [Tooltip("Shows Index of Action used in OnPerformAction")]
    public static string[] ActionIdentifiers = new string[] {
        "idleAction",
        "neue Bremsbeläge",
        "Ölwechsel",
        "Behälter für das Öl holen",
        "Ölablassschraube öffnen",
        "Ölfilter ausbauen",
        "Dichtringe erneuern",
        "Neuen Ölfilter anschrauben",
        "Ölablassschraube festmachen",
        "Ölreste reinigen",
        "Neues Öl einfüllen",
        "Ölstand messen",
        "Motor laufen lassen kurz",
        "Schraube und Ölfilter checken",
        "Kraftstofffilter reinigen",
        "Staubschutzsatz erneuern",
        "Geberzylinder reinigen",
        "Druck am Einlassventil prüfen",
        "Zündkerze wechseln",
        "Schrauben öffnen",
        "Räder entfernen",
        "Radnarben auf Rost überprüfen",
        "Rost entfernen",
        "Schrauben überprüfen",
        "Schrauben reinigen",
        "Neue Räder anbringen",
        "Schrauben handfest anziehen",
        "Schrauben mit Drehmoment über Kreuz festziehen",
        "Alle Reifen überprüfen",
        "Schrauben festziehen",
        "Radnarben polieren",
        "Dichtungen erneuern",
        "Reifenprofil prüfen",
               //Reifen montieren
        "Reifen montieren",
        "Reifen putzen",
        //Sto?stange wechseln
        "Alte Stoßstange entfernen",
        "Lüftungsgitter putzen",
        "Neue Stoßstange erwärmen",
        "Neue Stoßstange anbringen",
        //Batterie prüfen/wechseln
        "Alte Batterie ausbauen",
        "Neue Batterie einbauen",
        "Eingebaute Batterie überprüfen",
        //Dummys Batterie
        "Alte Batterie leeren",
        "Kabel austauschen",
                //Motorkontrollleuchte
        "Diagnose Gerät an OBD Stecker anschließen",
        "Fahrzeugdaten aus Fahrzeugschein ins Gerät übertragen",
        "Motorsteuergerät auslesen",
        "Fehleranzeige beachten",
        "Nach Herstellervorgaben Fehler beheben",
        //Dummys Motorkontrollleuchte
        "Diagnose Gerät an ABD Stecker anschließen",
       "Motor laufen lassen",
        "Motorkontrollleuchte austauschen",
        "Motor ausbauen"

    };
    //Enum for all actions that can be done on a car:
    public enum Action
    {
        idleAction,
        neueBremsbeläge,
        Ölwechsel,
        //Ölwechel
        Behälter_fürs_Öl_holen,
        Ölablassschraube_öffnen,
        Ölfilter_ausbauen,
        Dichtringe_erneuern,
        Neuen_Ölfilter_anschrauben,
        Ölablassschraube_festmachen,
        Ölreste_reinigen,
        Neues_Öl_einfüllen,
        Ölstand_messen,
        Motor_laufen_lassen_kurz,
        Schraube_und_Ölfilter_checken,
            //Dummys_Ölwechsel
        Kraftstofffilter_reinigen,
        Staubschutzsatz_erneuern,
        Geberzylinder_reinigen,
        Druck_am_Einlassventil_prüfen,
        //Zündkerze
        Zündkerze_wechseln,
        //Räder wechseln
        Schrauben_öffnen, //6
        Räder_entfernen, //9
        Radnarben_auf_Rost_überprüfen, //1
        Rost_entfernen, //10
        Schrauben_überprüfen,//7
        Schrauben_reinigen, //11
        Neue_Räder_anbringen, //12
        Schrauben_handfest_anziehen, //3
        Schrauben_mit_Drehmoment_über_Kreuz_festziehen, //8
        Alle_Reifen_überprüfen,//4
            //Dummys Räder wechseln
        Schrauben_festziehen, //2
        Radnarben_polieren, //13
        Dichtungen_erneuern, //5
        Reifenprofil_prüfen, //4
        //Reifen montieren
        Reifen_montieren,
        Reifen_putzen,
        //Sto?stange wechseln
        Alte_Stoßstange_entfernen,
        Lüftungsgitter_putzen,
        Neue_Stoßstange_erwärmen,
        Neue_Stoßstange_anbringen,
        //Batterie prüfen/wechseln
        Alte_Batterie_ausbauen,
        Neue_Batterie_einbauen,
        Eingebaute_Batterie_überprüfen,
        //Dummys Batterie
        Alte_Batterie_leeren,
        Kabel_austauschen,
        //Motorkontrollleuchte
        Diagnose_Gerät_an_OBD_Stecker_anschließen, 
        Fahrzeugdaten_aus_Fahrzeugschein_ins_Gerät_übertragen,
        Motorsteuergerät_auslesen,
        Fehleranzeige_beachten, 
        Nach_Herstellervorgaben_Fehler_beheben,
        //Dummys Motorkontrollleuchte
        Diagnose_Gerät_an_ABD_Stecker_anschließen,
        Motor_laufen_lassen, 
        Motorkontrollleuchte_austauschen, 
        Motor_ausbauen 
    };
    [Tooltip("Every Entry represents a Working Step in the WorkingStep Order of the Quest. A Working Step can contain one ore more Actions, that can be done in arbitrary order within the WorkingStep ")]
    public ArrayOfLists[] order;
    private ArrayOfLists[] toDo;
    private ArrayOfLists[] done;
    private List<Action> overflowActions= new List<Action>();
    
    // Start is called before the first frame update
    void Start()
    {
      
        toDo =new ArrayOfLists[order.Length];
        
        for(int index=0; index<order.Length;index++)
        {
            Debug.Log("index " + index);
            ArrayOfLists ar = new ArrayOfLists();
            ar.actions.AddRange(order[index].actions);
            toDo[index] = ar;
            
        }
        Debug.Log("OrderLength " + order.Length);
        done = new ArrayOfLists[order.Length];
        for (int i = 0; i < done.Length; i++) {
            done[i] = new ArrayOfLists();
        }
       
        /*if(toDo.Length==2)Debug.Log(toDo[1].ToString());
        foreach (ArrayOfLists l in toDo) {
            Debug.Log("Length: " + l.actions.Count);
            foreach (Action a in l.actions) {
                Debug.Log("ToDo: " + a.ToString());
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setDefaultToDo() {
        toDo = new ArrayOfLists[order.Length];

        for (int index = 0; index < order.Length; index++)
        {
            Debug.Log("index " + index);
            ArrayOfLists ar = new ArrayOfLists();
            ar.actions.AddRange(order[index].actions);
            toDo[index] = ar;

        }
    }
    public void startQuest() {
        curIndex = 0;
        carInstance=Instantiate(car);
    }
    public void controlAction(Action actionDone)
    {
        if (curIndex >= toDo.Length) {
            unnecessaryActions++;
            errors++;
            overflowActions.Add(actionDone);
            //MyGUI.setInfoTextFadeout("False Action! All Actions allready done");
            return;
        }
        
        Action tmp=Action.idleAction;
        done[curIndex].actions.Add(actionDone);
        foreach (Action a in toDo[curIndex].actions) {
            Debug.Log("curTodo" + a.ToString());
            if ((int)a == (int)actionDone) {
                tmp = a;
                Debug.Log("Correct Action");
            }
        }
        if (tmp==Action.idleAction)
        {
            Debug.Log("Mistake");
            errors++;
            MyGUI.setInfoTextFadeout("False Action!");
        }
        else toDo[curIndex].actions.Remove(tmp);

        if (done[curIndex].actions.Count == order[curIndex].actions.Count) curIndex++;

    }

    public int getErrorCount() {
        return errors;
    }

    public int getFinalErrorCount() {
        return curIndex >= order.Length ? errors : errors + (order.Length - curIndex); 
    }
    public int getUnnecessaryActionsCount() {
        return unnecessaryActions;
    }
    public ArrayOfLists[] getActionsDone()
    {
        return done;
    }
    public ArrayOfLists[] getOrderOfActios()
    {
        return order;
    }
    public Action revertLastAction() {
        if (curIndex >= done.Length &&overflowActions.Count>0) {
            unnecessaryActions--;
            errors--;
            Action revertedAction2 = overflowActions[overflowActions.Count - 1];
            overflowActions.RemoveAt(overflowActions.Count-1);
            return revertedAction2;

        }
        
        if(curIndex==done.Length || done[curIndex].actions.Count == 0) {
            curIndex--;
        }
        if (curIndex < 0)
        {
            curIndex++;
            return Action.idleAction;
        }
        Action revertedAction=done[curIndex].actions[done[curIndex].actions.Count - 1];
        toDo[curIndex].actions.Add(revertedAction);
        done[curIndex].actions.RemoveRange(done[curIndex].actions.Count - 1, 1);
        if (!order[curIndex].actions.Contains(revertedAction)) errors--;
        return revertedAction;
    }
    public void destroyCarInstance() {
        Destroy(carInstance);
    }
    public Action getLastActionDone() {
        Debug.Log("CurIndex "+curIndex);
        Debug.Log("doneLength "+done.Length);
        if (done.Length == 0 || done[0].actions.Count==0) return Action.idleAction;
        if (overflowActions.Count != 0) return overflowActions[overflowActions.Count - 1];
        if (done[curIndex-1].actions.Count == 0) {
            Debug.Log("done[curIndex-1].Count==0");
            return Action.idleAction;
        }
        else return done[curIndex-1].actions[done[curIndex-1].actions.Count - 1];
    }
    public void setCurIndex(int index) {
        curIndex = index;
    }
    public void resetErrors() {
        errors = 0;
        unnecessaryActions = 0;
    }
    public List<Action> getOverflowActions() {
        return overflowActions;
    }
}

[System.Serializable]
public class ArrayOfLists{
    public List<QuestItem.Action> actions=new List<QuestItem.Action>();    
}
