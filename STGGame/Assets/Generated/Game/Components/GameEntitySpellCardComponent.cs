//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly STGU3D.EntitySpellCardComponent entitySpellCardComponent = new STGU3D.EntitySpellCardComponent();

    public bool isEntitySpellCard {
        get { return HasComponent(GameComponentsLookup.EntitySpellCard); }
        set {
            if (value != isEntitySpellCard) {
                var index = GameComponentsLookup.EntitySpellCard;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : entitySpellCardComponent;

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

    static Entitas.IMatcher<GameEntity> _matcherEntitySpellCard;

    public static Entitas.IMatcher<GameEntity> EntitySpellCard {
        get {
            if (_matcherEntitySpellCard == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.EntitySpellCard);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherEntitySpellCard = matcher;
            }

            return _matcherEntitySpellCard;
        }
    }
}