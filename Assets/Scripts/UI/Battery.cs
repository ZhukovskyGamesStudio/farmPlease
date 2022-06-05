using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    public Image[] chargeImages;
    public Sprite green, red;
    Coroutine coroutine;

    public void NoEnergy()
    {
        EndCoroutine();
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(NoEnergyCoroutine());

    }

    void EndCoroutine()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        for (int i = 0; i < chargeImages.Length; i++)
        {
            chargeImages[i].sprite = green;
        }
    }

    IEnumerator NoEnergyCoroutine()
    {
        for (int i = 0; i < chargeImages.Length; i++)
        {
            chargeImages[i].enabled = true;
            chargeImages[i].sprite = red;
        }
        yield return new WaitForSeconds(0.30f);


        for (int i = 0; i < chargeImages.Length; i++)         
            chargeImages[i].enabled = false;
        

        yield return new WaitForSeconds(0.30f);

        for (int i = 0; i < chargeImages.Length; i++)         
            chargeImages[i].enabled = true;
        

        yield return new WaitForSeconds(0.30f);

        for (int i = 0; i < chargeImages.Length; i++)
        {
            chargeImages[i].enabled = false;
            chargeImages[i].sprite = green;
        }
    }




    public void UpdateCharge(int amount)
    {
        EndCoroutine();
        if (amount < 0)
        {
            Debug.Log("энергии меньше нуля?!");
            amount = 0;
        }

        for (int i = 0; i < chargeImages.Length; i++)
        {
            if (i < amount)
                chargeImages[i].enabled = true;
            else
                chargeImages[i].enabled = false;
        }
    }
}
