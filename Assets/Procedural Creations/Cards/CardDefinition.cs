using UnityEngine;
using System.Collections;

public class CardDefinition : MonoBehaviour
{
	public CardDef Data;
}

[System.Serializable]
public class CardDef
{
	public CardAtlas Atlas;
	public CardStock Stock;
	public string Text;
	public string Symbol; // Atlas shape name
	public int Grade;
	public int Pattern;
	public string Image;
	public bool FullImage;
	
	public CardDef(CardAtlas atlas, CardStock stock, string text, string symbol, int pattern)
	{
		Atlas = atlas;
		Stock = stock;
		Text = text;
		Symbol = symbol;
		Pattern = pattern;
	}
	public CardDef(CardAtlas atlas, CardStock stock, string text, string symbol, int pattern, int grade)
	{
		Atlas = atlas;
		Stock = stock;
		Text = text;
		Symbol = symbol;
		Pattern = pattern;
		Grade = grade;
	}
	public CardDef(CardDef def)
	{
		Atlas = def.Atlas;
		Stock = def.Stock;
		Text = def.Text;
		Symbol = def.Symbol;
		Pattern = def.Pattern;
		Grade = def.Grade;
	}

	public int compareTo(CardDef def)
	{
		if (this.Grade == def.Grade) return 0;
		else if (this.Grade > def.Grade) return 1;
		else return -1;
	}
}