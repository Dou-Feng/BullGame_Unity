using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMesh pname;

    public TextMesh score;

    public TextMesh point;

    public List<Card> cards = new List<Card>(5); // 玩家手牌

    public Pcard pcard = new Pcard();

    public Transform cards_trans;

    public GameObject banker_signature;

    public bool isBanker;

    public void Start()
    { 
        point.gameObject.SetActive(false);
        banker_signature.SetActive(false);
        text_delta.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    virtual public void Clear()
    {
        point.gameObject.SetActive(false);
        foreach (Card c in cards)
        {
            GameObject.DestroyImmediate(c.gameObject);
        }
        cards.Clear();
        pcard.Clear();
    }
    public void switch_banker(bool nextBanker)
    {
        //Debug.Log("Swich_banker");
        isBanker = nextBanker;
        banker_signature.SetActive(nextBanker);
    }
    virtual public void show_point()
    {
        point.text = pcard.ToString();
        point.gameObject.SetActive(true);

    }

    // inform the player that cards ared sent.
    virtual public void prepared()
    {
        pcard.eval(cards);

    }

    // delta text
    public TextMesh text_delta;

    public int bet = 1; // 下注大小

    public void set_text_delta(int delta)
    {
        text_delta.text = "";
        if (delta >= 0)
        {
            text_delta.text = "+";
        }
        text_delta.text += delta.ToString();
        //Debug.Log(text_delta.text);
        text_delta.gameObject.SetActive(true);
    }

    public void changeScore(int delta)
    {
        // 
        score.text = (int.Parse(score.text) + delta).ToString();

        text_delta.gameObject.SetActive(false);
    }

}
