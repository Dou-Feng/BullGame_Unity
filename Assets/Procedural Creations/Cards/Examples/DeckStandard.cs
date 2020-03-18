using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckStandard : CardDeck
{
	public CardAtlas Atlas;
	public CardStock Stock;
	
	public override void Initialize()
	{
		//Debug.Log("Atlas = "+Atlas.name);
		string [] suits = new string[]{ "Diamond", "Club", "Heart", "Spade"};
		string [] prefixes = new string[]{ "D-", "C-", "H-","S-",};
		List<CardDef> defs = new List<CardDef>();
		int grade = 0;
		for (int i=0; i<4; ++i)
		{
			int ii = i*13;
			string symbol = suits[i];
			defs.Add( new CardDef(Atlas,Stock,"A",symbol,1, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"2",symbol,2, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"3",symbol,3, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"4",symbol,4, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"5",symbol,5, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"6",symbol,6, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"7",symbol,7, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"8",symbol,8, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"9",symbol,9, grade++) );
			defs.Add( new CardDef(Atlas,Stock,"10",symbol,10, grade++) );
			string prefix = prefixes[i];
			CardDef jj = new CardDef(Atlas,Stock,"J",symbol,10, grade++);
			jj.Image = prefix+"Jack";
			defs.Add(jj);
			CardDef qq = new CardDef(Atlas,Stock,"Q",symbol,10, grade++);
			qq.Image = prefix+"Queen";
			defs.Add( qq );
			CardDef kk = new CardDef(Atlas,Stock,"K",symbol,10, grade++);
			kk.Image = prefix+"King";
			defs.Add( kk );
		}
		
		m_itemList = new DeckItem[52];
		for (int i=0; i<defs.Count; ++i)
		{
			DeckItem item = new DeckItem();
			item.Count = 1;
			item.Card = defs[i];
			m_itemList[i] = item;
		}
		//this.Shuffle();
	}
}
