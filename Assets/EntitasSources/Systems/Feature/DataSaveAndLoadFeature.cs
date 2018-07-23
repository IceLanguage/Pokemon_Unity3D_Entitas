using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

class DataSaveAndLoadFeature:Feature
{
    public DataSaveAndLoadFeature(Contexts contexts)
    {
        
        Add(new InitBagItemEffectSystem());
        Add(new InitUseSkillSpecialEffectSystem());
        Add(new InitPokemonStateSystem());
        Add(new InitAbilityImpactSystem());
        Add(new SavePlayerDataSystem(contexts));
        Add(new InitTrainerSystem(contexts));
    }
}
