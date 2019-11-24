//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.RecycleComponent recycle { get { return (STGU3D.RecycleComponent)GetComponent(GameComponentsLookup.Recycle); } }
    public bool hasRecycle { get { return HasComponent(GameComponentsLookup.Recycle); } }

    public void AddRecycle(UnityEngine.Rect newBoundary, float newMaxStayTime, float newStayTime, bool newIsRecycled) {
        var index = GameComponentsLookup.Recycle;
        var component = (STGU3D.RecycleComponent)CreateComponent(index, typeof(STGU3D.RecycleComponent));
        component.boundary = newBoundary;
        component.maxStayTime = newMaxStayTime;
        component.stayTime = newStayTime;
        component.isRecycled = newIsRecycled;
        AddComponent(index, component);
    }

    public void ReplaceRecycle(UnityEngine.Rect newBoundary, float newMaxStayTime, float newStayTime, bool newIsRecycled) {
        var index = GameComponentsLookup.Recycle;
        var component = (STGU3D.RecycleComponent)CreateComponent(index, typeof(STGU3D.RecycleComponent));
        component.boundary = newBoundary;
        component.maxStayTime = newMaxStayTime;
        component.stayTime = newStayTime;
        component.isRecycled = newIsRecycled;
        ReplaceComponent(index, component);
    }

    public void RemoveRecycle() {
        RemoveComponent(GameComponentsLookup.Recycle);
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

    static Entitas.IMatcher<GameEntity> _matcherRecycle;

    public static Entitas.IMatcher<GameEntity> Recycle {
        get {
            if (_matcherRecycle == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Recycle);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRecycle = matcher;
            }

            return _matcherRecycle;
        }
    }
}
