using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 需要强制替换一些被玩家选择的技能
    /// </summary>
    public static class NeedReplaceSKill
    {
        public static Dictionary<int, int> context = new Dictionary<int, int>(10);
    }

    /// <summary>
    /// 不允许使用的技能
    /// </summary>
    public static class DisableSkill
    {
        public static Dictionary<int, int> context = new Dictionary<int, int>(10);
    }
}
