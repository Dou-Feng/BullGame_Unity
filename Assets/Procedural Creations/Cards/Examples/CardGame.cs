using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardGame : MonoBehaviour
{
	public CardDeck Deck;
	//List<CardDefinition> m_deck = new List<CardDefinition>();
	
	List<Card> m_dealer = new List<Card>();
	List<Card> m_player = new List<Card>();

	GameObject PlayerWins;
	GameObject DealerWins;
	GameObject NobodyWins;
	
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
	
	GameObject [] Buttons;
	
	// Use this for initialization
	void Start ()
	{
		m_state = GameState.Invalid;
		Deck.Initialize();
		
		PlayerWins = this.transform.Find("MessagePlayerWins").gameObject;
		DealerWins = this.transform.Find("MessageDealerWins").gameObject;
		NobodyWins = this.transform.Find("MessageTie").gameObject;
		PlayerWins.SetActive(false);
		DealerWins.SetActive(false);
		NobodyWins.SetActive(false);
		Buttons = new GameObject[3];
		Buttons[0] = this.transform.Find("Button1").gameObject;
		Buttons[1] = this.transform.Find("Button2").gameObject;
		Buttons[2] = this.transform.Find("Button3").gameObject;
		UpdateButtons();
	}
	
	void UpdateButtons()
	{
		Buttons[0].GetComponent<Renderer>().material.color = Color.blue;
		Buttons[1].GetComponent<Renderer>().material.color = (m_state == GameState.Started) ? Color.blue : Color.red;
		Buttons[2].GetComponent<Renderer>().material.color = (m_state == GameState.Started || m_state == GameState.PlayerBusted) ? Color.blue : Color.red;
	}
	
	void ShowMessage(string msg)
	{
		if (msg == "Dealer")
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(true);
			NobodyWins.SetActive(false);
		}
		else if (msg == "Player")
		{
			PlayerWins.SetActive(true);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(false);
		}
		else if (msg == "Nobody")
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(true);
		}
		else
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			OnReset();
		}
		else if (Input.GetKeyDown(KeyCode.F2))
		{
			OnHitMe();
		}
		else if (Input.GetKeyDown(KeyCode.F3))
		{
			OnStop();
		}
		UpdateButtons();
	}
	
	void Shuffle()
	{
		if (m_state != GameState.Invalid)
		{
		}
	}
	
	void Clear()
	{
		foreach (Card c in m_dealer)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		foreach (Card c in m_player)
		{
			GameObject.Destroy(c.gameObject);
		}
		m_dealer.Clear();
		m_player.Clear();
		Deck.Reset();
	}
	
	Vector3 GetDeckPosition()
	{
		return new UnityEngine.Vector3(-7,0,0);
	}
	
	const float FlyTime = 0.5f;
	
	void HitDealer()
	{
		CardDef c1 = Deck.Pop();
		if (c1 != null)
		{
			GameObject newObj = new GameObject();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent(typeof(Card)) as Card;
			newCard.Definition = c1;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild();
			float x = -3+(m_dealer.Count)*2.0f;
			float z = (m_dealer.Count)*-0.1f;
			Vector3 deckPos = GetDeckPosition();
			newObj.transform.position = deckPos;
			m_dealer.Add(newCard);
			newCard.SetFlyTarget(deckPos,new Vector3(x,2,z),FlyTime);
		}
	}
	void HitPlayer()
	{
		CardDef c1 = Deck.Pop();
		if (c1 != null)
		{
			GameObject newObj = new GameObject();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent(typeof(Card)) as Card;
			newCard.Definition = c1;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild();
			float x = -3+(m_player.Count)*1.5f;
			float y = -3-m_player.Count*0.15f;
			float z = (m_player.Count)*-0.1f;
			//newObj.transform.position = new Vector3(x,-3,z);
			m_player.Add(newCard);
			Vector3 deckPos = GetDeckPosition();
			newCard.transform.position = deckPos;
			newCard.SetFlyTarget(deckPos,new Vector3(x,y,z),FlyTime);
		}
	}
	
	static int Value(Card c)
	{
		if (c != null)
		{
			switch (c.Definition.Pattern)
			{
			case 0:
				return 10;
			case 1:
				return 11;
			}
			return c.Definition.Pattern;
		}
		return 0;
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
		} else
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


	static int GetScore(List<Card> cards)
	{
		int res = 0;
		List<CardDef> cards_def = new List<CardDef>();
		foreach (Card c in cards)
		{
			cards_def.Add(c.Definition);
		}
		List<List<CardDef>> threes = Get3Of5(cards_def);

		foreach (List<CardDef> l in threes)
		{
			int sum = 0;
			foreach (CardDef c in l) {
				sum += c.Pattern;
			}
			
			if (sum % 10 == 0)
			{
				Debug.Log(l[0].Pattern + ", " + l[1].Pattern + ", " + l[2].Pattern);
				List<CardDef> rest = GetRestCard(cards_def, l);
				sum += rest[0].Pattern + rest[1].Pattern;
				Debug.Log(rest[0].Pattern + ", " + rest[1].Pattern);
				if (sum % 10 == 0)
				{
					res = 10;
				} else {
					sum %= 10;
					res = sum;
				}
				break;
			}
		}

		return res;
	}
	
	int GetDealerScore()
	{
		return GetScore(m_dealer);
	}
	
	int GetPlayerScore()
	{
		return GetScore(m_player);
	}
	
	const float DealTime = 0.35f;
	
	IEnumerator OnReset()
	{
		if (m_state != GameState.Resolving)
		{
			m_state = GameState.Resolving;
			ShowMessage("");
			Clear();
			Deck.Shuffle();
			for (int i = 0; i < 5; i++)
            {
				HitDealer();
				yield return new WaitForSeconds(DealTime);
			}
			for (int i = 0; i < 5; i++)
			{
				HitPlayer();
				yield return new WaitForSeconds(DealTime);
			}
			//HitDealer();
			//yield return new WaitForSeconds(DealTime);
			//HitDealer();
			//yield return new WaitForSeconds(DealTime);
			//HitPlayer();
			//yield return new WaitForSeconds(DealTime);
			//HitPlayer();
			m_state = GameState.Started;
		}
	}
	void OnHitMe()
	{
		if (m_state == GameState.Started)
		{
			HitPlayer();
			if (GetPlayerScore() > 21)
			{
				m_state = GameState.PlayerBusted;
			}
		}
	}
	
	IEnumerator OnStop()
	{
		if (m_state == GameState.Started || m_state == GameState.PlayerBusted)
		{
			m_state = GameState.Resolving;
			UpdateButtons();
			int playerScore = GetPlayerScore();
			int d = GetDealerScore();
			Debug.Log(string.Format("Player={0}  Dealer={1}",playerScore,d));
		}
		yield return new WaitForSeconds(DealTime * 1.5f);
	}
	
	public void OnButton(string msg)
	{
		Debug.Log("OnButton = "+msg);
		switch (msg)
		{
		case "Reset":
			StartCoroutine(OnReset());
			break;
		case "Hit":
			OnHitMe();
			break;
		case "Stop":
			StartCoroutine(OnStop());
			break;
		}
	}
}
