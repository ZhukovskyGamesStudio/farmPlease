using System;
using System.Collections;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour {
    public Step[] steps;
    private int curStep;
    private bool isBatteriEmpted, isNextDayBegan, isSeedBought;

    public void Start() {
        if (Time.Instance.day > 0)
            SaveLoadManager.Instance.ClearSaveAndReload();
        curStep = -1;
        isBatteriEmpted = false;
        for (int i = 0; i < steps.Length; i++) steps[i].Init();
        NextStep();
    }

    public void Update() {
        if (!isBatteriEmpted)
            if (Energy.Instance.curEnergy == 0) {
                NextStep();
                isBatteriEmpted = true;
            }

        if (!isNextDayBegan)
            if (Time.Instance.day == 1) {
                isNextDayBegan = true;
                NextStep();
            }

        if (!isSeedBought)
            if (InventoryManager.instance.seedsInventory.Any(crop => crop.Value > 0)) {
                isSeedBought = true;
                NextStep();
            }
    }

    public void NextStep() {
        curStep++;
        steps[curStep].Begin();
    }

    public void SkipTraining() {
        StartCoroutine(SkipTrainingCoroutine());
    }

    private IEnumerator SkipTrainingCoroutine() {
        for (int i = curStep; i < steps.Length; i++) {
            steps[i].Begin();
            yield return new WaitForSeconds(1 - curStep / steps.Length);
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