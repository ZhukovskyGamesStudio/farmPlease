using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinsCounter : MonoBehaviour {
        public TextMeshProUGUI CointsText;

        public void UpdateCoins(int amount) {
            CointsText.text = amount.ToString();
        }
    }
}