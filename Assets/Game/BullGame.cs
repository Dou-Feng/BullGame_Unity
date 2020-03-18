using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

class CardTypeAttr: Attribute
{
    internal CardTypeAttr(int time)
    {
        this.Time = time;
    }
    public int Time { get; private set; }

	
}

// 牌型
public enum CardType
{
	[CardTypeAttr(0)] NO_NIU = 0,
	[CardTypeAttr(1)] NORMAL = 1,
    [CardTypeAttr(2)] BULL8 = 2,
    [CardTypeAttr(3)] BULL9 = 3,
    [CardTypeAttr(4)] FULL = 4,
    [CardTypeAttr(5)] SIVIR = 5,
    [CardTypeAttr(6)] GOLD = 6,
    [CardTypeAttr(7)] SMALL5 = 7,
    [CardTypeAttr(8)] BOOM = 8,
}

public class Pcard : IComparable
{

	public CardType type { get; private set; }
	int point;
	int max_card; // use grade to indicate the max_card
	public Pcard(List<CardDef> cards)
	{
		eval(cards);
	}

	public Pcard()
	{
		Clear();
	}

	public void Clear()
	{
		type = CardType.NO_NIU;
		point = 0;
		max_card = 0;
	}

	public int CompareTo(object obj)
	{
		Pcard pcard = obj as Pcard;
		// type compare
		if (this.type > pcard.type) return 1;
		else if (this.type < pcard.type) return -1;

		// the normal type
		if (this.type == CardType.NORMAL) {
			if (this.point > pcard.point) return 1;
			else if (this.point<pcard.point) return -1;
		}

		// if the type is the same, and is not normal type
		if (this.type == CardType.BOOM)
		{
			if (this.max_card > pcard.max_card) return 1;
			else return -1;
			Debug.Assert(false); // 两个炸弹，这牌 出轨了！！！
		}
		if (this.max_card == pcard.max_card) return 0;
		else if ((this.max_card % 13) > (pcard.max_card % 13)) return 1;
		else if ((this.max_card % 13) == (pcard.max_card % 13))
		{
			if ((this.max_card / 13) > (pcard.max_card / 13)) return 1;
			else return -1;
		} else
		{
			return -1;
		}
		
	}

	override public string ToString()
	{
		switch(type)
		{
			case CardType.BOOM: return "炸弹";
			case CardType.SMALL5: return "五小";
			case CardType.GOLD: return "金花";
			case CardType.SIVIR: return "银花";
			case CardType.FULL: return "满上";
			case CardType.BULL9: return "牛九";
			case CardType.BULL8: return "牛八";
			case CardType.NORMAL: return "牛" + point;
			case CardType.NO_NIU: return "无牛";
			default: return "None";
		}
	}

	public void eval(List<Card> cards)
	{
		List<CardDef> l = new List<CardDef>();
		foreach(Card c in cards)
		{
			l.Add(c.Definition);
		}
		eval(l);
	}
	public void eval(List<CardDef> cards)
	{
		type = GetType(cards);
		point = GetScore(cards);
		if (point == 8)
		{
			type = CardType.BULL8;
		}
		else if (point == 9)
		{
			type = CardType.BULL9;
		}
		else if (point == 10)
		{
			type = CardType.FULL;
		} else if (point == 0)
		{
			type = CardType.NO_NIU;
		}

		// get Max card 
		int point1 = -1;
		int point2 = -1;
		foreach (CardDef c in cards)
		{
			int point = c.Grade % 13;
			int max_point = max_card % 13;
			if (type == CardType.BOOM)
			{
				if (point1 == -1)
				{
					point1 = point;
				}
				else if (point1 != point)
				{
					if (point2 == -1)
						point2 = point;
					else
					{
						max_card = point2;
						break;
					}
				}
				else if (point1 == point)
				{
					max_card = point1;
					break;
				}
			}
			else
			{
				if ((point > max_point) || (point == max_point && c.Grade > max_card))
				{
					max_card = c.Grade;
				}
			}
		}
		//Debug.Log((max_card / 13).ToString() + ", " + (max_card % 13 + 1).ToString());
	}

	static CardType GetType(List<CardDef> cards)
	{
		int sum = 0;
		Dictionary<int, int> identical = new Dictionary<int, int>();
		int gold_num = 0;
		int sivir_num = 0;
		foreach (CardDef c in cards)
		{
			int point = c.Grade % 13 + 1;
			sum += c.Pattern;
			if (point > 10)
			{
				gold_num++; sivir_num++;
			}
			else
			{
				if (c.Pattern == 10)
				{
					sivir_num++;
				}
			}
			if (identical.ContainsKey(point))
			{
				identical[point]++;
			}
			else
			{
				identical.Add(point, 1);
			}
		}
		// boom
		foreach (int i in identical.Values)
		{
			if (i == 4)
			{
				return CardType.BOOM;
			}
		}
		// small5
		if (sum < 10)
		{
			return CardType.SMALL5;
		}
		// Gold Flower
		if (gold_num == 5)
		{
			return CardType.GOLD;
		}
		// Sivir Flower
		if (sivir_num == 5)
		{
			return CardType.SIVIR;
		}
		return CardType.NORMAL;

	}

	static void GetCombination(List<CardDef> cards, List<List<CardDef>> ans, List<CardDef> con, int end, int index, int m)
	{
		if (end == m)
		{
			List<CardDef> l = new List<CardDef>();
			for (int i = 0; i < m; i++)
			{
				l.Add(new CardDef(con[i]));
			}
			ans.Add(l);
		}
		else
		{
			if (index >= cards.Count) return;
			con[end++] = cards[index];
			GetCombination(cards, ans, con, end, index + 1, m);
			end--;
			GetCombination(cards, ans, con, end, index + 1, m);
		}
	}


	static List<List<CardDef>> Get3Of5(List<CardDef> cards)
	{
		List<List<CardDef>> ans = new List<List<CardDef>>();
		List<CardDef> con = new List<CardDef>(5);
		for (int i = 0; i < 5; i++)
		{
			con.Add(cards[i]);
		}

		GetCombination(cards, ans, con, 0, 0, 3);

		return ans;
	}

	static List<CardDef> GetRestCard(List<CardDef> cards, List<CardDef> discard)
	{
		List<CardDef> ans = new List<CardDef>();
		foreach (CardDef c in cards)
		{
			bool find = false;
			foreach (CardDef d in discard)
			{
				if (c.compareTo(d) == 0)
				{
					find = true;
					break;
				}
			}
			if (!find)
			{
				ans.Add(c);
			}
		}
		return ans;
	}


	static int GetScore(List<CardDef> cards)
	{
		int res = 0;

		List<List<CardDef>> threes = Get3Of5(cards);

		foreach (List<CardDef> l in threes)
		{
			int sum = 0;
			foreach (CardDef c in l)
			{
				sum += c.Pattern;
			}

			if (sum % 10 == 0)
			{
				//Debug.Log(l[0].Pattern + ", " + l[1].Pattern + ", " + l[2].Pattern);
				List<CardDef> rest = GetRestCard(cards, l);
				sum += rest[0].Pattern + rest[1].Pattern;
				//Debug.Log(rest[0].Pattern + ", " + rest[1].Pattern);
				if (sum % 10 == 0)
				{
					res = 10;
				}
				else
				{
					sum %= 10;
					res = sum;
				}
				break;
			}
		}

		return res;
	}
}
public class BullGame : MonoBehaviour
{
	public CardDeck Deck; // 一副牌


	public List<Transform> slots; // players' location 
	public GameObject guest_prefab;
	public GameObject player_prefab;

	List<GameObject> players = new List<GameObject>();

	public int max_player_num;

	public int guest_num;

	enum GameState
	{
		Invalid,
		Started,
		PlayerBusted,
		Resolving,
		DealerWins,
		PlayerWins,
		NobodyWins,
	};

	GameState m_state;

	// Start is called before the first frame update
	void Start()
    {
		Deck.Initialize();
		GameObject player = Instantiate(player_prefab, slots[0].transform.position, Quaternion.identity);
		players.Add(player);
		// spawn some guest 
		for (int i = 1; i < guest_num; i++)
		{
			GameObject guest = Instantiate(guest_prefab, slots[i].transform.position, Quaternion.identity);
			//Debug.Log(slots[i].transform.position);
			//Debug.Assert(guest != null);
			players.Add(guest);
		}
		
	}
	

    // Update is called once per frame
    void Update()
    {
        
    }

    void Clear()
	{
		foreach (GameObject g in players)
		{
			Person p = g.GetComponent<Person>();
			p.Clear();
		}
		// Reset the deck
		Deck.Reset();
		Deck.Shuffle();
	}

	public Transform Desk;
	// use to hit the player
	Vector3 GetDeckPosition()
	{
		return Desk.position;
	}

	const float FlyTime = 0.5f;

	void HitPlayer(Person player, Vector3 scale, float interval)
	{
		CardDef c1 = Deck.Pop();
		if (c1 != null)
		{
			GameObject newObj = new GameObject();
			//newObj.transform.localScale = scale;
			newObj.name = "Card";
			Card newCard = newObj.AddComponent(typeof(Card)) as Card;
			newCard.Definition = c1;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild();
			float x = player.cards_trans.position.x + (player.cards.Count) * interval;
			//Debug.Log(player.transform.position);
			float z = (player.cards.Count) * -0.02f;
			Vector3 deckPos = GetDeckPosition();
			//Debug.Log(player.transform.position.ToString());
			newObj.transform.position = deckPos;
			player.cards.Add(newCard);
			newCard.SetFlyTarget(deckPos, new Vector3(x, player.cards_trans.position.y, z), FlyTime);
		}
	}

	const float DealTime = 0.2f;
	public void start_game()
	{
		Clear();
		players[0].GetComponent<Person>().switch_banker(true);
		StartCoroutine(Hit());
	}

	IEnumerator Hit()
	{
		if (m_state != GameState.Resolving)
		{
			Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
			m_state = GameState.Resolving;
			foreach (GameObject g in players)
			{
				Person p = g.GetComponent<Person>();
				for (int i = 0; i < 5; i++)
				{
					HitPlayer(p, scale, 0.6f);
					yield return new WaitForSeconds(DealTime);
				}
				// inform the player
				p.prepared();
			}
			m_state = GameState.Started;
		}
		StartCoroutine(Open_Ienumtor());
	}
	public void OnButton(string msg)
	{
		//Debug.Log("OnButton = " + msg);
		switch (msg)
		{
			case "Start":
				start_game();
				break;
			default: break;
		}
	}

	int countdown = 3;
	public TextMesh text_countdown;
	IEnumerator Open_Ienumtor()
	{
		text_countdown.gameObject.SetActive(true);
		for (int i = countdown; i > 0; i--)
		{
			text_countdown.text = i.ToString();
			yield return new WaitForSeconds(1.0f);
		}
		text_countdown.text = "开牌";
		Open();
		yield return new WaitForSeconds(0.8f);
		text_countdown.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.0f);
		StartCoroutine(Settlement());
	}

	public static int  bottom_score = 1;
	static int calculate(Pcard banker, Pcard player, int bet)
	{
		if (banker.CompareTo(player) > 0)
		{
			int times = (int)(banker.type == CardType.NO_NIU ? CardType.NORMAL : banker.type);
			return bet * bottom_score * times;
		} else
		{
			int times = (int)(player.type == CardType.NO_NIU ? CardType.NORMAL : player.type);
			return -bet * bottom_score * times;
		}
	}

	IEnumerator Settlement()
	{
		Person banker = null;
		foreach (GameObject g in players)
		{
			Person p = g.GetComponent<Person>();
			if (p.isBanker)
			{
				banker = p;
				break;
			}
		}
		List<int> deltas = new List<int>();
		int sum = 0;
		foreach (GameObject g in players)
		{
			Person p = g.GetComponent<Person>();
			if (banker == p)
			{
				deltas.Add(0);
				continue;
			}
			int delta = calculate(banker.pcard, p.pcard, p.bet);
			deltas.Add(delta);
			//banker.set_text_delta(delta);
			sum += delta;
			p.set_text_delta(-delta);
		}
		banker.set_text_delta(sum);
		yield return new WaitForSeconds(3f);

		banker.changeScore(sum);
		for (int i = 0; i < players.Count; i++)
		{
			Person p = players[i].GetComponent<Person>();
			if (p == banker) continue;
			p.changeScore(-deltas[i]);
		}
	}

	//Open the cards of player and players
	public void Open()
	{
		foreach (GameObject g in players)
		{
			Person p = g.GetComponent<Person>();
			p.show_point();
		}
	}

}
