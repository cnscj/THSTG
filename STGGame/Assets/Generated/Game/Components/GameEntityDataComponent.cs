//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.EntityDataComponent entityData { get { return (STGU3D.EntityDataComponent)GetComponent(GameComponentsLookup.EntityData); } }
    public bool hasEntityData { get { return HasComponent(GameComponentsLookup.EntityData); } }

    public void AddEntityData(string newEntityCode, STGU3D.EEntityType newEntityType, XLibrary.CSVObject newEntityData) {
        var index = GameComponentsLookup.EntityData;
        var component = (STGU3D.EntityDataComponent)CreateComponent(index, typeof(STGU3D.EntityDataComponent));
        component.entityCode = newEntityCode;
        component.entityType = newEntityType;
        component.entityData = newEntityData;
        AddComponent(index, component);
    }

    public void ReplaceEntityData(string newEntityCode, STGU3D.EEntityType newEntityType, XLibrary.CSVObject newEntityData) {
        var index = GameComponentsLookup.EntityData;
        var component = (STGU3D.EntityDataComponent)CreateComponent(index, typeof(STGU3D.EntityDataComponent));
        component.entityCode = newEntityCode;
        component.entityType = newEntityType;
        component.entityData = newEntityData;
        ReplaceComponent(index, component);
    }

    public void RemoveEntityData() {
        RemoveComponent(GameComponentsLookup.EntityData);
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

    static Entitas.IMatcher<GameEntity> _matcherEntityData;

    public static Entitas.IMatcher<GameEntity> EntityData {
        get {
            if (_matcherEntityData == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.EntityData);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherEntityData = matcher;
            }

            return _matcherEntityData;
        }
    }
}
