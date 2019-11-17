//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.BombComponent bomb { get { return (STGU3D.BombComponent)GetComponent(GameComponentsLookup.Bomb); } }
    public bool hasBomb { get { return HasComponent(GameComponentsLookup.Bomb); } }

    public void AddBomb(int newMaxTimes, float newMaxCdTime, int newDyingBombUse, int newTimes, float newCdTime, bool newIsBomb) {
        var index = GameComponentsLookup.Bomb;
        var component = (STGU3D.BombComponent)CreateComponent(index, typeof(STGU3D.BombComponent));
        component.maxTimes = newMaxTimes;
        component.maxCdTime = newMaxCdTime;
        component.dyingBombUse = newDyingBombUse;
        component.times = newTimes;
        component.cdTime = newCdTime;
        component.isBomb = newIsBomb;
        AddComponent(index, component);
    }

    public void ReplaceBomb(int newMaxTimes, float newMaxCdTime, int newDyingBombUse, int newTimes, float newCdTime, bool newIsBomb) {
        var index = GameComponentsLookup.Bomb;
        var component = (STGU3D.BombComponent)CreateComponent(index, typeof(STGU3D.BombComponent));
        component.maxTimes = newMaxTimes;
        component.maxCdTime = newMaxCdTime;
        component.dyingBombUse = newDyingBombUse;
        component.times = newTimes;
        component.cdTime = newCdTime;
        component.isBomb = newIsBomb;
        ReplaceComponent(index, component);
    }

    public void RemoveBomb() {
        RemoveComponent(GameComponentsLookup.Bomb);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherBomb;

    public static Entitas.IMatcher<GameEntity> Bomb {
        get {
            if (_matcherBomb == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Bomb);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBomb = matcher;
            }

            return _matcherBomb;
        }
    }
}
