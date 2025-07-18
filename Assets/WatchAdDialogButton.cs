using System.Collections.Generic;
using UnityEngine;
using MadPixel;
using Tables;
using UI;
using UnityEngine.UI;
public class WatchAdDialogButton : MonoBehaviour
{
    private Button m_myButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_myButton = GetComponent<Button>();
        if (m_myButton != null) {
            m_myButton.onClick.AddListener(OnClick);
        } else {
            Debug.LogError("[MadPixel] Please add a Button component!");
        }
    }

    
    
    
    public void OnClick()
    {
        m_myButton.enabled = false;
        
        AdsManager.EResultCode code =  AdsManager.ShowRewarded(this.gameObject, OnFinishAds, "ChangeAdForClock");
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
            DialogsManager.Instance.ShowDialogWithData(typeof(RewardDialog), new RewardDialogData() {
                Reward = new Reward() {
                    Items = new List<RewardItem>() {
                        new RewardItem() {
                            Type = ToolBuff.WeekBattery.ToString(),
                            Amount = 1
                        }
                    }
                },
                OnClaim = () => { UIHud.Instance.BackpackAttention.ShowAttention(); }
            });
            m_myButton.enabled = true;
        }
    }
    // Update is called once per frame
    
}
