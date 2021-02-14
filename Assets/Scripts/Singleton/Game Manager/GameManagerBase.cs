public class GameManagerBase : SingletonBase {
    public GameManagerBase() : base(typeof(GameManager), typeof(GameManagerData)) { }
}
