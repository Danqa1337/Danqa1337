using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnatomyScreen : Singleton<AnatomyScreen>
{
    public AnatomyPartUi RArm;
    public AnatomyPartUi LArm;
    public AnatomyPartUi Head;
    public AnatomyPartUi Body;

    public void UpdateBars()
    {
        var rarmDur = PlayerAbilitiesSystem.RightArm.GetComponentData<DurabilityComponent>();
        RArm.DurabilityBar.SetNewValue(rarmDur.currentDurability, rarmDur.MaxDurability);

        var larmDur = PlayerAbilitiesSystem.LeftArm.GetComponentData<DurabilityComponent>();
        LArm.DurabilityBar.SetNewValue(larmDur.currentDurability, larmDur.MaxDurability);

        var headDur = PlayerAbilitiesSystem.Head.GetComponentData<DurabilityComponent>();
        Head.DurabilityBar.SetNewValue(headDur.currentDurability, headDur.MaxDurability);

        var bodyDur = PlayerAbilitiesSystem.Body.GetComponentData<DurabilityComponent>();
        Body.DurabilityBar.SetNewValue(bodyDur.currentDurability, bodyDur.MaxDurability);
    }
}
