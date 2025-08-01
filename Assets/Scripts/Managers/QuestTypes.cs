﻿using System;

[Serializable]
public enum QuestTypes {
    None = 0,
    Collect,
    Send,
    Special
}

[Serializable]
public enum TargetTypes {
    None = 0,
    Crop,
    Tool,
}

[Serializable]
public enum SpecialTargetTypes {
    None = 0,
    StrawberryWateredTomato,
    GiantEggplant,
    DandellionHoedStrawberry,
    DigAllField,
    ChangeNickname,
    CollectNSeedTypes,
    BuildBuilding,
    CollectNToolTypes,
    EroseNWeeds,
    CollectNFromOneTile,
    SellOnDoubleDay,
    LocateInsects,
}
