using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

public class View : MonoBehaviour
{

    SSDirector one;
    UserAction action;

    // Start is called before the first frame update
    void Start()
    {
        one = SSDirector.GetInstance();
        action = SSDirector.GetInstance() as UserAction;
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        if (one.state == State.Win)
        {
            if (GUI.Button(new Rect(650, 140, 300, 100), "WIN\n(click here to reset)"))
            {
                action.reset();
            }
        }
        if (one.state == State.Lose)
        {
            if (GUI.Button(new Rect(650, 140, 300, 100), "LOSE\n(click here to reset)"))
            {
                action.reset();
            }
        }

        if (GUI.Button(new Rect(700, 80, 100, 50), "GO"))
        {
            action.moveBoat();
        }
        if (GUI.Button(new Rect(550, 250, 90, 50), "LPriestsOFF"))
        {
            action.offBoatLP();
        }
        if (GUI.Button(new Rect(950, 250, 90, 50), "RPriestsOFF"))
        {
            action.offBoatRP();
        }
        if (GUI.Button(new Rect(650, 250, 90, 50), "LDevilsOFF"))
        {
            action.offBoatLD();
        }
        if (GUI.Button(new Rect(850, 250, 90, 50), "RDevilsOFF"))
        {
            action.offBoatRD();
        }
        if (GUI.Button(new Rect(350, 130, 75, 50), "PriestsON"))
        {
            action.priestSOnSide();
        }
        if (GUI.Button(new Rect(500, 130, 75, 50), "DevilsON"))
        {
            action.devilSOnSide();
        }
        if (GUI.Button(new Rect(1050, 130, 75, 50), "DevilsON"))
        {
            action.devilEOnSide();
        }
        if (GUI.Button(new Rect(1200, 130, 75, 50), "PriestsON"))
        {
            action.priestEOnSide();
        }
    }
}