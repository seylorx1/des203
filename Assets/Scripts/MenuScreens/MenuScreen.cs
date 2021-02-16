using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen {
    public MenuScreen PreviousMenuScreen {
        get;
        private set;
    }
    public MenuScreen NextMenuScreen {
        get;
        private set;
    }

    public MenuScreen(MenuScreen next) : this(null, next) { }
    public MenuScreen(MenuScreen previous, MenuScreen next) {
        PreviousMenuScreen = previous;
        NextMenuScreen = next;
    }
}
