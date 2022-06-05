using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public Step[] steps;
    int curStep;
    public PlayerController PlayerController;
    bool isBatteriEmpted, isNextDayBegan;

    public void Start()
    {
        if (TimeManager.instance.day > 0)
            SaveLoadManager.instance.ClearSaveAndReload();
        curStep = -1;
        isBatteriEmpted = false;
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].Init();
        }
        NextStep();
    }

    public void NextStep()
    {
        curStep++;
        steps[curStep].Begin();
    }

    public void SkipTraining()
    {
        StartCoroutine(SkipTrainingCoroutine());

    }

    IEnumerator SkipTrainingCoroutine()
    {
        for (int i = curStep; i < steps.Length; i++)
        {
            steps[i].Begin();
            yield return new WaitForSeconds((1 - curStep/steps.Length));
        }
    }

    public void Update()
    {
       if(!isBatteriEmpted)
            if(PlayerController.curEnergy == 0)
            {
                NextStep();
                isBatteriEmpted = true;
            }
       if(!isNextDayBegan)
            if(TimeManager.instance.day == 1)
            {
                isNextDayBegan = true;
                NextStep();
            }


    }
}

[System.Serializable]
public class Step
{
    public GameObject[] toActivate;
    public GameObject[] toDeactivate;


    public void Begin()
    {
        for (int i = 0; i < toActivate.Length; i++)
        {
            toActivate[i].SetActive(true);
        }
        for (int i = 0; i < toDeactivate.Length; i++)
        {
            toDeactivate[i].SetActive(false);
        }
    }

    public void Init()
    {
        for (int i = 0; i < toActivate.Length; i++)
        {
            toActivate[i].SetActive(false);
        }
    }

}
