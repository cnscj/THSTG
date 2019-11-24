//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.DestroyedComponent destroyed { get { return (STGU3D.DestroyedComponent)GetComponent(GameComponentsLookup.Destroyed); } }
    public bool hasDestroyed { get { return HasComponent(GameComponentsLookup.Destroyed); } }

    public void AddDestroyed(int newCode, bool newIsDestroyed) {
        var index = GameComponentsLookup.Destroyed;
        var component = (STGU3D.DestroyedComponent)CreateComponent(index, typeof(STGU3D.DestroyedComponent));
        component.code = newCode;
        component.isDestroyed = newIsDestroyed;
        AddComponent(index, component);
    }

    public void ReplaceDestroyed(int newCode, bool newIsDestroyed) {
        var index = GameComponentsLookup.Destroyed;
        var component = (STGU3D.DestroyedComponent)CreateComponent(index, typeof(STGU3D.DestroyedComponent));
        component.code = newCode;
        component.isDestroyed = newIsDestroyed;
        ReplaceComponent(index, component);
    }

    public void RemoveDestroyed() {
        RemoveComponent(GameComponentsLookup.Destroyed);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity : IDestroyedEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherDestroyed;

    public static Entitas.IMatcher<GameEntity> Destroyed {
        get {
            if (_matcherDestroyed == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Destroyed);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherDestroyed = matcher;
            }

            return _matcherDestroyed;
        }
    }
}
