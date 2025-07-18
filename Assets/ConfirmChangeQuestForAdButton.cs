using UnityEngine;
using MadPixel;
using UnityEngine.UI;

public class ConfirmChangeQuestForAdButton : MonoBehaviour
{
    private Button m_myButton;
    private void Start() {
        m_myButton = GetComponent<Button>();
        if (m_myButton != null) {
            m_myButton.onClick.AddListener(OnClick);
        } else {
            Debug.LogError("[MadPixel] Please add a Button component!");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnClick()
    {
        m_myButton.enabled = false;
        
            AdsManager.EResultCode code =  AdsManager.ShowRewarded(this.gameObject, OnFinishAds, "ChangeAdForQuest");
            if (code != AdsManager.EResultCode.OK)
            {
                // Не сыграли рекламу, плашку, насрать return;  я не вижу куда начало анимаций пихать, пока скипну
                m_myButton.enabled = true;
            }
        }

        private void OnFinishAds(bool bsuccess)
        {
            if (bsuccess)
            {
                var dialog = FindObjectOfType<QuestsDialog>();
                if (dialog != null)
                {
                    dialog.ChangeSelectedQuestForAd();
                }
                else
                {
                    Debug.LogWarning("QuestsDialog не найден на сцене");
                }
                m_myButton.enabled = true;
            }
        }
    
}
