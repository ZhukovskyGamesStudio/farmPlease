using System;
using System.Collections;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

[Obsolete]
public class TrainingManager : MonoBehaviour {
    public Step[] steps;
    private int _curStep;
    private bool _isBatteriEmpted, _isNextDayBegan, _isSeedBought;

    public void Start() {
        if (SaveLoadManager.CurrentSave.CurrentDay > 0)
            SaveLoadManager.Instance.ClearSaveAndReload();
        _curStep = -1;
        _isBatteriEmpted = false;
        for (int i = 0; i < steps.Length; i++) steps[i].Init();
        NextStep();
    }

    public void Update() {
        if (!_isBatteriEmpted)
            if (Energy.Instance.CurEnergy == 0) {
                NextStep();
                _isBatteriEmpted = true;
            }

        if (!_isNextDayBegan)
            if (SaveLoadManager.CurrentSave.CurrentDay == 1) {
                _isNextDayBegan = true;
                NextStep();
            }

        /*
        if (!isSeedBought)
            if (InventoryManager.instance.seedsInventory.Any(crop => crop.Value > 0)) {
                isSeedBought = true;
                NextStep();
            }*/
    }

    public void NextStep() {
        _curStep++;
        steps[_curStep].Begin();
    }

    public void SkipTraining() {
        StartCoroutine(SkipTrainingCoroutine());
    }

    private IEnumerator SkipTrainingCoroutine() {
        for (int i = _curStep; i < steps.Length; i++) {
            steps[i].Begin();
            yield return new WaitForSeconds(1 - _curStep / steps.Length);
        }

        SceneManager.LoadScene("Game");
    }
}

[Serializable]
public class Step {
    public GameObject[] toActivate;
    public GameObject[] toDeactivate;

    public void Begin() {
        for (int i = 0; i < toActivate.Length; i++) toActivate[i].SetActive(true);
        for (int i = 0; i < toDeactivate.Length; i++) toDeactivate[i].SetActive(false);
    }

    public void Init() {
        for (int i = 0; i < toActivate.Length; i++) toActivate[i].SetActive(false);
    }
}