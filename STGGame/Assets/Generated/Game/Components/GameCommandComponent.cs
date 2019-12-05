//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly STGU3D.CommandComponent commandComponent = new STGU3D.CommandComponent();

    public bool isCommand {
        get { return HasComponent(GameComponentsLookup.Command); }
        set {
            if (value != isCommand) {
                var index = GameComponentsLookup.Command;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : commandComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherCommand;

    public static Entitas.IMatcher<GameEntity> Command {
        get {
            if (_matcherCommand == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Command);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherCommand = matcher;
            }

            return _matcherCommand;
        }
    }
}
