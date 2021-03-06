//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public STGU3D.InputComponent input { get { return (STGU3D.InputComponent)GetComponent(InputComponentsLookup.Input); } }
    public bool hasInput { get { return HasComponent(InputComponentsLookup.Input); } }

    public void AddInput(int newType) {
        var index = InputComponentsLookup.Input;
        var component = (STGU3D.InputComponent)CreateComponent(index, typeof(STGU3D.InputComponent));
        component.type = newType;
        AddComponent(index, component);
    }

    public void ReplaceInput(int newType) {
        var index = InputComponentsLookup.Input;
        var component = (STGU3D.InputComponent)CreateComponent(index, typeof(STGU3D.InputComponent));
        component.type = newType;
        ReplaceComponent(index, component);
    }

    public void RemoveInput() {
        RemoveComponent(InputComponentsLookup.Input);
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
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherInput;

    public static Entitas.IMatcher<InputEntity> Input {
        get {
            if (_matcherInput == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.Input);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherInput = matcher;
            }

            return _matcherInput;
        }
    }
}
