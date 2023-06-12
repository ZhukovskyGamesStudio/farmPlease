using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CoinsView : MonoBehaviour {
        public Text CointsText;

        public void UpdateCoins(int amount) {
            CointsText.text = amount.ToString();
        }
    }
}