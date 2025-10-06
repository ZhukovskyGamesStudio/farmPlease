using Dialogs;
using UnityEngine;

public class InterAdRunner {
    public bool IsInterAdRunEnabled;

    private float _interAdCooldown;
    private IAdsProvider _ads;
    private float _timer;
    private static bool _needShowInter;
    private string _placement_name = "timed_inter_ad";

    public InterAdRunner(float cooldown, IAdsProvider ads) {
        _interAdCooldown = cooldown;
        _ads = ads;
    }

    public void SubscribeToDialogsClose() {
        DialogsManager.Instance.OnQueueEmptied += TryShowInter;
    }

    public void Update() {
        if (!IsInterAdRunEnabled) {
            return;
        }

        if (_needShowInter) {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _interAdCooldown) {
            _needShowInter = true;
            _timer = 0;
        }
    }

    public void TryShowInter() {
        if (!_needShowInter) {
            return;
        }

        _needShowInter = false;
        _ads.ShowInterAd(_placement_name);
    }
}