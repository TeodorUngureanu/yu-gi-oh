using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn {

    public enum Phase { Hold, Draw, Main1, Battle, Main2, End };

    private List<Phase> turnPhases;
    private int currentPhase = 0;

    public Turn()
    {
        turnPhases = new List<Phase>(new Phase[] { Phase.Hold, Phase.Draw, Phase.Main1,
                                                   Phase.Battle, Phase.Main2, Phase.End });
    }

    public Phase getCurrentPhase()
    {
        return turnPhases[currentPhase];
    }

    public bool isMainPhase()
    {
        return getCurrentPhase() == Phase.Main1 || getCurrentPhase() == Phase.Main2;
    }

    public void goToNextPhase()
    {
        currentPhase = (currentPhase == turnPhases.Count - 1) ? 0 : currentPhase + 1;
    }

}
