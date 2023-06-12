﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryView : MonoBehaviour {
    public Image[] chargeImages;
    public Sprite green, red;
    private Coroutine _coroutine;

    public void NoEnergy() {
        EndCoroutine();
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(NoEnergyCoroutine());
    }

    private void EndCoroutine() {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        for (int i = 0; i < chargeImages.Length; i++) chargeImages[i].sprite = green;
    }

    private IEnumerator NoEnergyCoroutine() {
        for (int i = 0; i < chargeImages.Length; i++) {
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

        for (int i = 0; i < chargeImages.Length; i++) {
            chargeImages[i].enabled = false;
            chargeImages[i].sprite = green;
        }
    }

    public void UpdateCharge(int amount) {
        EndCoroutine();
        if (amount < 0) {
            UnityEngine.Debug.Log("энергии меньше нуля?!");
            amount = 0;
        }

        for (int i = 0; i < chargeImages.Length; i++)
            if (i < amount)
                chargeImages[i].enabled = true;
            else
                chargeImages[i].enabled = false;
    }
}