using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    // 願いのDef
    public class WishSelectDef : Def
    {
        // 呼び出す処理
        public WishBase WishCalc = null;
        // 確率アップ処理
        public WishCountUP WishCountUP = null;
        // 確率アップ時何個候補に増やすか
        public int CountUP = 0;
    }

    // 願いのベース
    public abstract class WishBase
    {
        // この条件を満たしてる場合候補に選ばれる
        public virtual bool Test(Map map)
        {
            return true;
        }

        // 実際に実行される処理
        public virtual void Run(Map map, WishSelectDef def)
        {

        }
    }

    // 願いのベース
    public abstract class WishCountUP
    {
        // 確率がアップするかどうか
        public virtual bool CountUP(Map map)
        {
            return false;
        }
    }

}
