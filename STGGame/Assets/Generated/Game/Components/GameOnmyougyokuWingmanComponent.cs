//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.OnmyougyokuWingmanComponent onmyougyokuWingman { get { return (STGU3D.OnmyougyokuWingmanComponent)GetComponent(GameComponentsLookup.OnmyougyokuWingman); } }
    public bool hasOnmyougyokuWingman { get { return HasComponent(GameComponentsLookup.OnmyougyokuWingman); } }

    public void AddOnmyougyokuWingman(int newId) {
        var index = GameComponentsLookup.OnmyougyokuWingman;
        var component = (STGU3D.OnmyougyokuWingmanComponent)CreateComponent(index, typeof(STGU3D.OnmyougyokuWingmanComponent));
        component.id = newId;
        AddComponent(index, component);
    }

    public void ReplaceOnmyougyokuWingman(int newId) {
        var index = GameComponentsLookup.OnmyougyokuWingman;
        var component = (STGU3D.OnmyougyokuWingmanComponent)CreateComponent(index, typeof(STGU3D.OnmyougyokuWingmanComponent));
        component.id = newId;
        ReplaceComponent(index, component);
    }

    public void RemoveOnmyougyokuWingman() {
        RemoveComponent(GameComponentsLookup.OnmyougyokuWingman);
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

    static Entitas.IMatcher<GameEntity> _matcherOnmyougyokuWingman;

    public static Entitas.IMatcher<GameEntity> OnmyougyokuWingman {
        get {
            if (_matcherOnmyougyokuWingman == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.OnmyougyokuWingman);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherOnmyougyokuWingman = matcher;
            }

            return _matcherOnmyougyokuWingman;
        }
    }
}
