using UnityEngine;

public class GameModeManager : MonoBehaviour
{
	public bool UnlimitedMoneyCrops;
	public bool InfiniteEnergy;
	public bool UnlimitedSeeds;
	public bool ShowTileType;
	public bool DoNotSave;
	public bool DisableStrongWind;
	public bool IsBuildingsShopAlwaysOpen;

	public GameMode GameMode;


	public static GameModeManager instance = null;

	/**********/

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance == this)
			Destroy(gameObject);
	}
}

public enum GameMode
{
	Training, RealTime, FakeTime, Online
}
