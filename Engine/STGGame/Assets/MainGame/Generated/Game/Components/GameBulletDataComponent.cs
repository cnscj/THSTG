//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.BulletDataComponent bulletData { get { return (STGU3D.BulletDataComponent)GetComponent(GameComponentsLookup.BulletData); } }
    public bool hasBulletData { get { return HasComponent(GameComponentsLookup.BulletData); } }

    public void AddBulletData(STGU3D.ECampType newCampType) {
        var index = GameComponentsLookup.BulletData;
        var component = (STGU3D.BulletDataComponent)CreateComponent(index, typeof(STGU3D.BulletDataComponent));
        component.campType = newCampType;
        AddComponent(index, component);
    }

    public void ReplaceBulletData(STGU3D.ECampType newCampType) {
        var index = GameComponentsLookup.BulletData;
        var component = (STGU3D.BulletDataComponent)CreateComponent(index, typeof(STGU3D.BulletDataComponent));
        component.campType = newCampType;
        ReplaceComponent(index, component);
    }

    public void RemoveBulletData() {
        RemoveComponent(GameComponentsLookup.BulletData);
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

    static Entitas.IMatcher<GameEntity> _matcherBulletData;

    public static Entitas.IMatcher<GameEntity> BulletData {
        get {
            if (_matcherBulletData == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.BulletData);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBulletData = matcher;
            }

            return _matcherBulletData;
        }
    }
}