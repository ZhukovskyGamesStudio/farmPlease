using System;
using UnityEngine;

public class InterAdRunner {
	private float _interAdCooldown;
	private IAdsProvider _ads;
	
	public bool IsInterAdRunEnabled;

	public InterAdRunner(float cooldown, IAdsProvider ads) {
		_interAdCooldown = cooldown;
		_ads = ads;
	}
	
	private float _timer;

	public void Update() {
		if (!IsInterAdRunEnabled) return;
		
		_timer += Time.deltaTime;
		if (_timer >= _interAdCooldown) {
			_ads.ShowInterAd("timed_inter_ad");
		}
	}
}	