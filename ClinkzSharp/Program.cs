using System;
using System.Collections.Generic;

using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;


namespace ClinkzSharp
{

    internal class Program
    {


        private static Ability _strafe, _arrows;
        private static Item _bkb, _orchid, _hex, _medallion;
        private static readonly Menu Menu = new Menu("ClinkzSharp", "clinkzsharp", true, "npc_dota_hero_Clinkz", true);
        private static Hero _me, _target;
        private static bool _autoKillz;
        private static AbilityToggler _menuValue;

        private static bool _menuvalueSet;


        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            var menuThingy = new Menu("Options", "opsi");
            menuThingy.AddItem(new MenuItem("enable", "enable").SetValue(true));
            Menu.AddSubMenu(menuThingy);
            menuThingy.AddItem(new MenuItem("comboKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddToMainMenu();
            var dict = new Dictionary<string, bool>
            {
                {"item_black_king_bar", true },
                { "item_medallion", true },
                { "item_orchid", true },
                { "item_sheepstick", true }
            };
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(dict)));
        }


        public static void Game_OnUpdate(EventArgs args)
        {
            _me = ObjectMgr.LocalHero;

            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

            if (_me.ClassID != ClassID.CDOTA_Unit_Hero_Clinkz)
                return;

            if (_me == null)
                return;

            if (_arrows == null)
                _arrows = _me.Spellbook.SpellW;


            if (_bkb == null)
                _bkb = _me.FindItem("item_black_king_bar");

            if (_strafe == null)
                _strafe = _me.Spellbook.Spell1;

            if (_hex == null)
                _hex = _me.FindItem("item_sheepstick");

            if (_orchid == null)
                _orchid = _me.FindItem("item_orchid");

            if (_medallion == null)
                _medallion = _me.FindItem("item_medallion");

            if (!_menuvalueSet)
            {
                _menuValue = Menu.Item("Items").GetValue<AbilityToggler>();
                _menuvalueSet = true;
            }


            if (!_autoKillz || !Menu.Item("enable").GetValue<bool>()) return;
            _target = _me.ClosestToMouseTarget(1000);

            if (_target == null || !_target.IsAlive || _target.IsInvul() || _target.IsIllusion) return;

            if (!_me.CanAttack() || !_me.CanCast()) return;

            if (
                _arrows != null &&
                _arrows.CanBeCasted() &&
                _me.CanCast() &&
                Utils.SleepCheck("_arrows"))
            {
                _arrows.UseAbility(_target);
                Utils.Sleep(330, "_arrows");
            }

            if (_strafe.CanBeCasted() && _me.CanCast() &&
                Utils.SleepCheck("_strafe"))

            {
                _strafe.UseAbility();
                Utils.Sleep(150 + Game.Ping, "_strafe");
            }

            if (_bkb != null && _bkb.CanBeCasted() && Utils.SleepCheck("_bkb") && _menuValue.IsEnabled(_bkb.Name) && _me.Distance2D(_target) <= 620)
            {
                _bkb.UseAbility();
                Utils.Sleep(150 + Game.Ping, "_bkb");
            }

            if (_hex == null || !_hex.CanBeCasted() || !_menuValue.IsEnabled(_hex.Name) || !Utils.SleepCheck("_hex")) return;
            _hex.UseAbility(_target);
            Utils.Sleep(150 + Game.Ping, "_hex");

            if (_medallion == null || !_medallion.CanBeCasted() || !_menuValue.IsEnabled(_medallion.Name) || !Utils.SleepCheck("_medallion")) return;
            _medallion.UseAbility(_target);
            Utils.Sleep(150 + Game.Ping, "_medallion");

            if (_orchid != null && _orchid.CanBeCasted() && _menuValue.IsEnabled(_orchid.Name) && Utils.SleepCheck("_orchid") && !_target.IsSilenced() && !_target.IsStunned() && !_target.IsHexed())
            {
                _orchid.UseAbility(_target);
                Utils.Sleep(150 + Game.Ping, "_orchid");
            }
        }   
    private static void Game_OnWndProc(WndEventArgs args)
    {
        if (Game.IsChatOpen) return;
        _autoKillz = Menu.Item("comboKey").GetValue<KeyBind>().Active;
    }
    }
}