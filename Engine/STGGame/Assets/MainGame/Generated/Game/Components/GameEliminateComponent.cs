//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.EliminateComponent eliminate { get { return (STGU3D.EliminateComponent)GetComponent(GameComponentsLookup.Eliminate); } }
    public bool hasEliminate { get { return HasComponent(GameComponentsLookup.Eliminate); } }

    public void AddEliminate(float newRadius, float newMaxHoldTime, float newConsumeRate, float newChargeRate, float newHoldTime, float newCdTime, float newNextChargeTime, bool newIsEliminating) {
        var index = GameComponentsLookup.Eliminate;
        var component = (STGU3D.EliminateComponent)CreateComponent(index, typeof(STGU3D.EliminateComponent));
        component.radius = newRadius;
        component.maxHoldTime = newMaxHoldTime;
        component.consumeRate = newConsumeRate;
        component.chargeRate = newChargeRate;
        component.holdTime = newHoldTime;
        component.cdTime = newCdTime;
        component.nextChargeTime = newNextChargeTime;
        component.isEliminating = newIsEliminating;
        AddComponent(index, component);
    }

    public void ReplaceEliminate(float newRadius, float newMaxHoldTime, float newConsumeRate, float newChargeRate, float newHoldTime, float newCdTime, float newNextChargeTime, bool newIsEliminating) {
        var index = GameComponentsLookup.Eliminate;
        var component = (STGU3D.EliminateComponent)CreateComponent(index, typeof(STGU3D.EliminateComponent));
        component.radius = newRadius;
        component.maxHoldTime = newMaxHoldTime;
        component.consumeRate = newConsumeRate;
        component.chargeRate = newChargeRate;
        component.holdTime = newHoldTime;
        component.cdTime = newCdTime;
        component.nextChargeTime = newNextChargeTime;
        component.isEliminating = newIsEliminating;
        ReplaceComponent(index, component);
    }

    public void RemoveEliminate() {
        RemoveComponent(GameComponentsLookup.Eliminate);
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

    static Entitas.IMatcher<GameEntity> _matcherEliminate;

    public static Entitas.IMatcher<GameEntity> Eliminate {
        get {
            if (_matcherEliminate == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Eliminate);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherEliminate = matcher;
            }

            return _matcherEliminate;
        }
    }
}
