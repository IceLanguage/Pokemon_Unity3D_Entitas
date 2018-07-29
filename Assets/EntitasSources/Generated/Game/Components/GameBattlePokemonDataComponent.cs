//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public BattlePokemonDataComponent battlePokemonData { get { return (BattlePokemonDataComponent)GetComponent(GameComponentsLookup.BattlePokemonData); } }
    public bool hasBattlePokemonData { get { return HasComponent(GameComponentsLookup.BattlePokemonData); } }

    public void AddBattlePokemonData(BattlePokemonData newData) {
        var index = GameComponentsLookup.BattlePokemonData;
        var component = CreateComponent<BattlePokemonDataComponent>(index);
        component.data = newData;
        AddComponent(index, component);
    }

    public void ReplaceBattlePokemonData(BattlePokemonData newData) {
        var index = GameComponentsLookup.BattlePokemonData;
        var component = CreateComponent<BattlePokemonDataComponent>(index);
        component.data = newData;
        ReplaceComponent(index, component);
    }

    public void RemoveBattlePokemonData() {
        RemoveComponent(GameComponentsLookup.BattlePokemonData);
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

    static Entitas.IMatcher<GameEntity> _matcherBattlePokemonData;

    public static Entitas.IMatcher<GameEntity> BattlePokemonData {
        get {
            if (_matcherBattlePokemonData == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.BattlePokemonData);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBattlePokemonData = matcher;
            }

            return _matcherBattlePokemonData;
        }
    }
}