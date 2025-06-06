using System;
using Tables;
using UnityEngine;

public class OpenCroponomButtonView : MonoBehaviour {
    [SerializeField]
    private GameObject _cropsTag, _toolsTag, _weatherTag, _buildingsTag;

    private void Start() {
        UpdateTags();
    }

    public void OnClick() { }

    public void ShowAttention() { }

    public void HideAttention() { }

    public void UpdateTags() {
        _cropsTag.gameObject.SetActive(true);
        _toolsTag.gameObject.SetActive(UnlockableUtils.HasUnlockable(ToolBuff.WeekBattery));
        _weatherTag.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.Weather));
        _buildingsTag.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.FoodMarket));
    }
}