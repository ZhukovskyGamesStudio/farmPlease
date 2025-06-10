using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ScalesDialog : DialogWithData<int> {
    [SerializeField]
    private ScalesView scalesView;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private SellTabletView _sellTablet;

    public SellTabletView SellTabletView => _sellTablet;

    public bool IsSellingAnimation { get; private set; }
    public bool IsRainingCropsAnimation { get; private set; }
    public Button CloseButton => _closeButton;

    public override void SetData(int data) {
        //TODO add proper data handling if needed
    }

    public override void Show(Action onClose) {
        base.Show(onClose);
        IsSellingAnimation = false;
        //ShowRainingCrops();
        _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue, scalesView.OnSelectedAmountChange);
        _sellTablet.Open();
    }

    public override void Close() {
        if (IsSellingAnimation || IsRainingCropsAnimation) {
            return;
        }

        base.Close();
    }

    public void SellSelected(List<Crop> crops) {
        if (IsSellingAnimation || IsRainingCropsAnimation) {
            return;
        }

        IsSellingAnimation = true;

        StartCoroutine(SellCoroutine(crops));
    }

    private IEnumerator SellCoroutine(List<Crop> crops) {
        _sellTablet.Close();
        _animation.Play("StartSelling");
        yield return new WaitWhile(() => _animation.isPlaying);
        yield return StartCoroutine(scalesView.SellCrops(crops));
        int cropsAmount = crops.Count;
        int coinsGain = cropsAmount * (TimeManager.Instance.IsTodayLoveDay ? 2 : 1);
        InventoryManager.Instance.AddCoins(coinsGain);
        InventoryManager.Instance.AddCropPoint(-cropsAmount);

        RemoveCropsFromCollected(crops);
        SaveLoadManager.SaveGame();
        IsSellingAnimation = false;
        _sellTablet.SetData(SaveLoadManager.CurrentSave.CropsCollectedQueue, scalesView.OnSelectedAmountChange);
        yield return new WaitWhile(() => _animation.isPlaying);
        _animation.Play("ContinueSelling");
        yield return new WaitWhile(() => _animation.isPlaying);
        _sellTablet.Open();

        //_animation.Play("MarkMission");
        // yield return new WaitWhile(() => _animation.isPlaying);

        _animation.Play("EndSelling");
        //yield return new WaitWhile(() => _animation.isPlaying);
    }

    private void RemoveCropsFromCollected(List<Crop> crops) {
        foreach (Crop crop in crops) {
            SaveLoadManager.CurrentSave.CropsCollected.Remove(crop);
        }
    }
}