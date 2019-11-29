//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class UIEntity {

    public STGU3D.DestroyedComponent destroyed { get { return (STGU3D.DestroyedComponent)GetComponent(UIComponentsLookup.Destroyed); } }
    public bool hasDestroyed { get { return HasComponent(UIComponentsLookup.Destroyed); } }

    public void AddDestroyed(int newWhat, bool newIsDestroyed) {
        var index = UIComponentsLookup.Destroyed;
        var component = (STGU3D.DestroyedComponent)CreateComponent(index, typeof(STGU3D.DestroyedComponent));
        component.what = newWhat;
        component.isDestroyed = newIsDestroyed;
        AddComponent(index, component);
    }

    public void ReplaceDestroyed(int newWhat, bool newIsDestroyed) {
        var index = UIComponentsLookup.Destroyed;
        var component = (STGU3D.DestroyedComponent)CreateComponent(index, typeof(STGU3D.DestroyedComponent));
        component.what = newWhat;
        component.isDestroyed = newIsDestroyed;
        ReplaceComponent(index, component);
    }

    public void RemoveDestroyed() {
        RemoveComponent(UIComponentsLookup.Destroyed);
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
public partial class UIEntity : IDestroyedEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class UIMatcher {

    static Entitas.IMatcher<UIEntity> _matcherDestroyed;

    public static Entitas.IMatcher<UIEntity> Destroyed {
        get {
            if (_matcherDestroyed == null) {
                var matcher = (Entitas.Matcher<UIEntity>)Entitas.Matcher<UIEntity>.AllOf(UIComponentsLookup.Destroyed);
                matcher.componentNames = UIComponentsLookup.componentNames;
                _matcherDestroyed = matcher;
            }

            return _matcherDestroyed;
        }
    }
}
