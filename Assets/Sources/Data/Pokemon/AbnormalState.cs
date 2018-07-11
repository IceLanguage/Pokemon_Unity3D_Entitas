using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PokemonBattelePokemon
{
    /// <summary>
    /// 异常状态
    /// </summary>
    [Serializable]
    public enum AbnormalState
    {
        Normal,
        Burns,
        Frostbite,
        Poisoning,
        Sleeping,
        paralysis,
        death,
        BadlyPoison,
        confusion
    }

}