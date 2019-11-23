//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.HealthComponent health { get { return (STGU3D.HealthComponent)GetComponent(GameComponentsLookup.Health); } }
    public bool hasHealth { get { return HasComponent(GameComponentsLookup.Health); } }

    public void AddHealth(float newMaxNearDeathTime, int newMaxLife, int newMaxBlood, int newLife, int newBlood, float newTrueDeathTime, bool newIsTrueDied) {
        var index = GameComponentsLookup.Health;
        var component = (STGU3D.HealthComponent)CreateComponent(index, typeof(STGU3D.HealthComponent));
        component.maxNearDeathTime = newMaxNearDeathTime;
        component.maxLife = newMaxLife;
        component.maxBlood = newMaxBlood;
        component.life = newLife;
        component.blood = newBlood;
        component.trueDeathTime = newTrueDeathTime;
        component.isTrueDied = newIsTrueDied;
        AddComponent(index, component);
    }

    public void ReplaceHealth(float newMaxNearDeathTime, int newMaxLife, int newMaxBlood, int newLife, int newBlood, float newTrueDeathTime, bool newIsTrueDied) {
        var index = GameComponentsLookup.Health;
        var component = (STGU3D.HealthComponent)CreateComponent(index, typeof(STGU3D.HealthComponent));
        component.maxNearDeathTime = newMaxNearDeathTime;
        component.maxLife = newMaxLife;
        component.maxBlood = newMaxBlood;
        component.life = newLife;
        component.blood = newBlood;
        component.trueDeathTime = newTrueDeathTime;
        component.isTrueDied = newIsTrueDied;
        ReplaceComponent(index, component);
    }

    public void RemoveHealth() {
        RemoveComponent(GameComponentsLookup.Health);
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

    static Entitas.IMatcher<GameEntity> _matcherHealth;

    public static Entitas.IMatcher<GameEntity> Health {
        get {
            if (_matcherHealth == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Health);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherHealth = matcher;
            }

            return _matcherHealth;
        }
    }
}
