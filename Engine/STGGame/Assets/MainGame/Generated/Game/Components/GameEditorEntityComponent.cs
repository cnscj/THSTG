//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly STGU3D.EditorEntityComponent editorEntityComponent = new STGU3D.EditorEntityComponent();

    public bool isEditorEntity {
        get { return HasComponent(GameComponentsLookup.EditorEntity); }
        set {
            if (value != isEditorEntity) {
                var index = GameComponentsLookup.EditorEntity;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : editorEntityComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
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

    static Entitas.IMatcher<GameEntity> _matcherEditorEntity;

    public static Entitas.IMatcher<GameEntity> EditorEntity {
        get {
            if (_matcherEditorEntity == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.EditorEntity);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherEditorEntity = matcher;
            }

            return _matcherEditorEntity;
        }
    }
}
