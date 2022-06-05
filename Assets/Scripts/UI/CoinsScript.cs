using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsScript : MonoBehaviour
{
	public Text CointsText;

	public void UpdateCoins(int amount)
	{
		CointsText.text = amount.ToString();
	}
}
