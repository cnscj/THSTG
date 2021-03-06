//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public STGU3D.PlayerDataComponent playerData { get { return (STGU3D.PlayerDataComponent)GetComponent(GameComponentsLookup.PlayerData); } }
    public bool hasPlayerData { get { return HasComponent(GameComponentsLookup.PlayerData); } }

    public void AddPlayerData(STGU3D.EPlayerType newPlayerType, STGU3D.EHeroType newHeroType, GameEntity[] newWingmans, float newMoveSpeed, DG.Tweening.Tween newFlashTween) {
        var index = GameComponentsLookup.PlayerData;
        var component = (STGU3D.PlayerDataComponent)CreateComponent(index, typeof(STGU3D.PlayerDataComponent));
        component.playerType = newPlayerType;
        component.heroType = newHeroType;
        component.wingmans = newWingmans;
        component.moveSpeed = newMoveSpeed;
        component.flashTween = newFlashTween;
        AddComponent(index, component);
    }

    public void ReplacePlayerData(STGU3D.EPlayerType newPlayerType, STGU3D.EHeroType newHeroType, GameEntity[] newWingmans, float newMoveSpeed, DG.Tweening.Tween newFlashTween) {
        var index = GameComponentsLookup.PlayerData;
        var component = (STGU3D.PlayerDataComponent)CreateComponent(index, typeof(STGU3D.PlayerDataComponent));
        component.playerType = newPlayerType;
        component.heroType = newHeroType;
        component.wingmans = newWingmans;
        component.moveSpeed = newMoveSpeed;
        component.flashTween = newFlashTween;
        ReplaceComponent(index, component);
    }

    public void RemovePlayerData() {
        RemoveComponent(GameComponentsLookup.PlayerData);
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

    static Entitas.IMatcher<GameEntity> _matcherPlayerData;

    public static Entitas.IMatcher<GameEntity> PlayerData {
        get {
            if (_matcherPlayerData == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.PlayerData);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherPlayerData = matcher;
            }

            return _matcherPlayerData;
        }
    }
}
