using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Person
{
    public GameObject btn_open;

    public GameObject bet_bar;
    void Start()
    {
        base.Start();
        btn_open.gameObject.SetActive(false);
        score.text = 111.ToString();
        pname.text = "Charitable Gambling King";
        bet_bar.SetActive(false);
        //banker_signature.SetActive(true);
    }


    // Update is called once per frame
    void Update() 
    {
        
    }

    public override void prepared()
    {
        //Debug.Log("Prepared");
        base.prepared();
        bet_bar.SetActive(!isBanker);
    }

    public override void Clear()
    {
        base.Clear();
        btn_open.gameObject.SetActive(true);
    }

    override public void show_point()
    {
        btn_open.gameObject.SetActive(false);
        base.show_point();
        
    }
    public void OnButton(string msg)
    {
        //Debug.Log("OnButton = " + msg);
        switch (msg)
        {
            case "Open":
                show_point();
                break;
            default: break;
        }
    }
}
