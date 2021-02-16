using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLocalisationBase : SingletonBase
{
    public GameLocalisationBase() : base(typeof(GameLocalisation), typeof(GameLocalisationData)) { }
}
