using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlePassItem 
{
    void SetupUi(BattlePassItemData bpData,int level,bool canClaim, bool hasClaimed);
    int GetItemSpriteIndex(int itemId);
    void OnClaim();
}
