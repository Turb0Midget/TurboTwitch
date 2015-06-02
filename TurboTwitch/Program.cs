﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

namespace TurboTwitch
{
    class Program
    {
        public const string ChampName = "Twitch";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R, Recall;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("TurboTwitch Loaded", 950);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 850);
            Recall = new Spell(SpellSlot.Recall);

            W.SetSkillshot(0.25f, 250f, 1400f, false, SkillshotType.SkillshotCircle);
            // R buff : TwitchFullAutomatic
            // Q buff : TwitchHideInShadows
            // Passive : TwitchDeadlyVenomMarker

            Config = new Menu("TurboTwitch", "Twitch", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            var combo = Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            var harass = Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            var killsteal = Config.AddSubMenu(new Menu("Killsteal Settings", "Killsteal Settings"));
            var lasthit = Config.AddSubMenu(new Menu("Lasthit Settings", "Lasthit Settings"));
            var laneclear = Config.AddSubMenu(new Menu("Laneclear Settings", "Laneclear Settings"));
            var jungleclear = Config.AddSubMenu(new Menu("Jungle Settings", "Jungle Settings"));
            var misc = Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            var drawing = Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));

            combo.SubMenu("Settings Q").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            combo.SubMenu("Settings Q").AddItem(new MenuItem("UseQMslider", "Mana % for Q").SetValue(new Slider(10, 100, 0)));
            combo.SubMenu("Settings Q").AddItem(new MenuItem("UseQslider", "Max enemies around to use Q").SetValue(new Slider(1, 5, 1)));
            combo.SubMenu("Settings Q").AddItem(new MenuItem("UseQTF", "Use Q Teamfight mode").SetValue(true));
            combo.SubMenu("Settings Q").AddItem(new MenuItem("UseQ1v1", "Use Q in 1 v 1?").SetValue(true));

            combo.SubMenu("Settings W").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            combo.SubMenu("Settings W").AddItem(new MenuItem("UseWslider", "Minimum enemies to use W").SetValue(new Slider(1, 5, 1)));
            combo.SubMenu("Settings W").AddItem(new MenuItem("AutoW", "Auto W when hit enemy").SetValue(true));
            combo.SubMenu("Settings W").AddItem(new MenuItem("AutoWslider", "Enemy count").SetValue(new Slider(4, 5, 1)));

            combo.SubMenu("Settings E").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            combo.SubMenu("Settings E").AddItem(new MenuItem("ComboEMana", "Mana % for E").SetValue(new Slider(10, 100, 0)));
            combo.SubMenu("Settings E").AddItem(new MenuItem("UseEmaxdmg", "Use E at max damage").SetValue(false));
            combo.SubMenu("Settings E").AddItem(new MenuItem("UseEkillonly", "Use E if kill only").SetValue(true));
            combo.SubMenu("Settings E").AddItem(new MenuItem("AutoE", "Auto E at max stacks").SetValue(false));
            combo.SubMenu("Settings E").AddItem(new MenuItem("UseEOOA", "E if enemy is out of AA range and has stacks (Lane Pressure)").SetValue(true));
            combo.SubMenu("Settings E").AddItem(new MenuItem("UseEOOAslider", "Stacks Amount").SetValue(new Slider(4, 6, 1)));

            combo.SubMenu("Item Settings").AddItem(new MenuItem("UseItems", "Use Items").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("ghostblade", "Use Youmuu's Ghostblade").SetValue(false));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("ghostbladeownhp", "Own HP %").SetValue(new Slider(60, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("ghostbladeenemyhp", "Enemy HP %").SetValue(new Slider(60, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("bork", "Use Blade Of The Ruined King").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("borkownhp", "Own HP %").SetValue(new Slider(60, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("borkenemyhp", "Enemy HP %").SetValue(new Slider(75, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("cutlass", "Use Bilgewater Cutlass").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("cutlassownhp", "Own HP %").SetValue(new Slider(60, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("cutlassenemyhp", "Enemy HP %").SetValue(new Slider(75, 100, 0)));

            lasthit.AddItem(new MenuItem("LasthitE", "Lasthit with E").SetValue(false));
            lasthit.AddItem(new MenuItem("LasthitEslider", "Minimun minions to kill with E").SetValue(new Slider(3, 10, 0)));
            lasthit.AddItem(new MenuItem("LasthitEMana", "Mana % for E").SetValue(new Slider(40, 100, 0)));

            laneclear.AddItem(new MenuItem("LaneclearW", "Laneclear with W").SetValue(true));
            laneclear.AddItem(new MenuItem("LaneclearWhit", "Minions to hit with W").SetValue(new Slider(3, 10, 0)));
            laneclear.AddItem(new MenuItem("LaneclearE", "Laneclear with E").SetValue(true));
            laneclear.AddItem(new MenuItem("LaneclearEpoison", "Minimum minions to be poisoned").SetValue(new Slider(3, 10, 0)));
            laneclear.AddItem(new MenuItem("LaneclearEstacks", "Minimum Poison stacks per minion to use E").SetValue(new Slider(3, 10, 0)));          
            laneclear.AddItem(new MenuItem("LaneclearMana", "Mana % for laneclear").SetValue(new Slider(30, 100, 0)));

            jungleclear.AddItem(new MenuItem("JungleclearW", "Jungleclear with W").SetValue(true));
            jungleclear.AddItem(new MenuItem("JungleclearE", "Jungleclear with E").SetValue(true));
            jungleclear.AddItem(new MenuItem("JungleclearMana", "Jungleclear Mana").SetValue(new Slider(40, 100, 0)));
            
            harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HarassEMana", "Mana % for E").SetValue(new Slider(50, 100, 0)));

            killsteal.AddItem(new MenuItem("KSE", "Killsteal with E").SetValue(true));

            misc.AddItem(new MenuItem("AntiGapW", "Auto W in gapclosers").SetValue(true));
            misc.AddItem(new MenuItem("AutoQ", "Auto use Q when below hp %").SetValue(false));
            misc.AddItem(new MenuItem("AutoQHP", "HP %").SetValue(new Slider(10, 100, 0)));

            drawing.AddItem(new MenuItem("DisableDraw", "Disable All Drawings").SetValue(false));
            drawing.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            drawing.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.Blue)));
            drawing.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.Blue)));

            // misc > Auto blue trinket
            // misc > Auto Q on zed ult / caitultbuff
            // misc > Steal baron / dragon / red / blue with E
            // misc > additem stealth recall
            // draw > Draw E stacks on enemy
            // draw > Draw ult seconds under player
            // Combo > R = line skillshot with no collision > use r if enemy.hitcount >=2 or 3
            // Combo > if enemy is out of aa range and killable and is in R range && config item R  > R.Cast > enemies to use R
            // Combo > USE Q SNEAKY 3000 RANGE if Q.level == 5 / 2500 RANGE if Q.level == 3 etc
            // Combo > Q logic if play has ghostblade && config item ghostblade cast ghostblade && calc edmg + aa * ? + 5


            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    QLogic();
                    WLogic();
                    ELogic();
                    Items();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Lasthit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;

            }
            if (Config.Item("KSE").GetValue<bool>())
            {
                Killsteal();
            }
            if (Config.Item("AutoQ").GetValue<bool>())
            {
                AutoQ();
            }
            if (Config.Item("AutoW").GetValue<bool>())
            {
                AutoW();
            }
            if (Config.Item("AutoE").GetValue<bool>())
            {
                AutoE();
            }

        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range)
                && Config.Item("AntiGapW").GetValue<bool>())
                W.Cast(gapcloser.Sender);

        }
        private static void AutoW()
        {
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(W.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                var wprediction = W.GetPrediction(enemy);

                if (Config.Item("AutoW").GetValue<bool>())
                {
                    if (W.IsReady() && wprediction.Hitchance >= HitChance.High
                        && enemy.Position.CountEnemiesInRange(W.Width) >= Config.Item("AutoWslider").GetValue<Slider>().Value)
                        W.Cast(enemy);
                }
            }

        }
        private static void WLogic()
        {
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(W.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                var wprediction = W.GetPrediction(enemy);

                if (Config.Item("UseW").GetValue<bool>())
                {
                    if (W.IsReady() && wprediction.Hitchance >= HitChance.High
                        && enemy.Position.CountEnemiesInRange(W.Width) >= Config.Item("UseWslider").GetValue<Slider>().Value)
                        W.Cast(enemy);
                }
            }

        }
        private static void AutoE()
        {
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(E.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                if (Config.Item("AutoE").GetValue<bool>())
                {
                    var edmg = E.GetDamage(enemy);

                    if (enemy.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= 6 && E.IsReady()
                        && Player.ManaPercent >= Config.Item("ComboEMana").GetValue<Slider>().Value
                        && Config.Item("AutoE").GetValue<bool>())

                        E.Cast(enemy);
                }
            }
        }
        private static void ELogic()
        {
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(E.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                var edmg = E.GetDamage(enemy);

                if (enemy.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= 6 && E.IsReady()
                    && Config.Item("UseEmaxdmg").GetValue<bool>()
                    && Player.ManaPercent >= Config.Item("ComboEMana").GetValue<Slider>().Value
                    && Config.Item("UseE").GetValue<bool>())
                    E.Cast();

                if (enemy.Health <= edmg + 5 && E.IsReady() && Config.Item("UseEkillonly").GetValue<bool>()
                    && Player.ManaPercent >= Config.Item("ComboEMana").GetValue<Slider>().Value
                    && Config.Item("UseE").GetValue<bool>())
                    E.Cast();

                if (Config.Item("UseE").GetValue<bool>() && E.IsReady()
                    && enemy.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= Config.Item("UseEOOAslider").GetValue<Slider>().Value
                    && Config.Item("UseEOOA").GetValue<bool>() && enemy.Distance(Player.Position) > Orbwalking.GetRealAutoAttackRange(Player))
                    E.Cast();

            }
        }
        private static void AutoQ()
        {
            if (Config.Item("AutoQ").GetValue<bool>())
            {
                if (Player.HealthPercent <= Config.Item("AutoQHP").GetValue<Slider>().Value)

                    Q.Cast();
            }
        }
        private static void QLogic()
        {
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(E.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                var edmg = E.GetDamage(enemy);
                var aa = Player.GetAutoAttackDamage(enemy, true);
                var damage = aa;
                // aa range = 550, R range = 300, Tot range = 850 

                //TURRET CHECK
                if (enemy.Position.UnderTurret(true) && Config.Item("UseQtower").GetValue<bool>()
                    && Q.IsReady())
                    return;

                //Q 1V1 MODE
                if (Config.Item("UseQ1v1").GetValue<bool>()
                    && enemy.Position.CountEnemiesInRange(1200) == 1
                    && Q.IsReady() && Player.ManaPercent >= Config.Item("UseQMslider").GetValue<Slider>().Value)
                    Q.Cast();

                //WHEN TO NOT USE Q
                if (Player.HasBuff("TwitchHideInShadows") || !Player.IsVisible ||
                    Player.HasBuff("CHECKCORKIQ") || Player.HasBuff("BlindMonkTempest") ||
                    Player.HasBuff("ORACLELENS") || Player.HasBuff("BlindMonkSonicWave"))
                    //Pinkward
                    return;

                //WHEN TO USE Q
                if (Config.Item("UseQ").GetValue<bool>() && Q.IsReady()
                    && enemy.Health < edmg + 6 * aa + 5
                    && enemy.Position.CountEnemiesInRange(1200) <= Config.Item("UseQslider").GetValue<Slider>().Value
                    && Player.ManaPercent >= Config.Item("UseQMslider").GetValue<Slider>().Value ||
                    //&& Config.Item("ComboEMana").GetValue<Slider>().Value  DONT Q IF IF NOT HAVE ENOUGH MANA FOR E TO KILL ENEMY 
                    Config.Item("UseQ").GetValue<bool>() && enemy.Health < edmg + 8 * aa + 5 && Q.IsReady()
                    && enemy.Position.CountEnemiesInRange(1200) <= Config.Item("UseQslider").GetValue<Slider>().Value
                    && Config.Item("UseW").GetValue<bool>() && W.IsReady()
                    && Player.ManaPercent >= Config.Item("UseQMslider").GetValue<Slider>().Value)
                    Q.Cast();

                //Q TEAMFIGHT MODE
                if (Config.Item("UseQ").GetValue<bool>() && Q.IsReady()
                    && Config.Item("UseQTF").GetValue<bool>() && Player.ManaPercent >= Config.Item("UseQMslider").GetValue<Slider>().Value
                    && enemy.Position.CountEnemiesInRange(1400) >= 3 && enemy.Health < edmg + aa * 10)
                    Q.Cast();

            }
        }
        private static void Items()
        {

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var ghostblade = ItemData.Youmuus_Ghostblade.GetItem();
            var bork = ItemData.Blade_of_the_Ruined_King.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (target == null || !target.IsValidTarget()
               || target.IsZombie || target.IsDead)
                return;

            //BORK
            if (Player.HealthPercent <= Config.Item("borkownhp").GetValue<Slider>().Value
                && bork.IsReady() && bork.IsOwned(Player) && Config.Item("bork").GetValue<bool>()
                && bork.IsInRange(target) || target.HealthPercent <= Config.Item("borkenemyhp").GetValue<Slider>().Value
                && bork.IsReady() && bork.IsOwned(Player) && Config.Item("bork").GetValue<bool>()
                && bork.IsInRange(target))

                bork.Cast(target);

            //CUTLASS
            if (Player.HealthPercent <= Config.Item("cutlassownhp").GetValue<Slider>().Value
                && cutlass.IsReady() && cutlass.IsOwned(Player) && Config.Item("cutlass").GetValue<bool>()
                && cutlass.IsInRange(target) || target.HealthPercent <= Config.Item("cutlassenemyhp").GetValue<Slider>().Value
                && cutlass.IsReady() && cutlass.IsOwned(Player) && Config.Item("cutlass").GetValue<bool>()
                && bork.IsInRange(target))

                cutlass.Cast(target);

            //GHOSTBLADE
            if (ghostblade.IsReady() && ghostblade.IsOwned(Player) && target.IsValidTarget(E.Range -50)
                && Config.Item("ghostblade").GetValue<bool>())

                ghostblade.Cast();

        }
        private static void Harass()
        {
            foreach (var enemy in
                 ObjectManager.Get<Obj_AI_Hero>()
                     .Where(x => x.IsValidTarget(E.Range))
                     .Where(x => !x.IsZombie)
                     .Where(x => !x.IsDead))
            {
                var edmg = E.GetDamage(enemy);

                if (enemy.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= 6 && E.IsReady()
                && Config.Item("HarassE").GetValue<bool>()
                && Player.ManaPercent >= Config.Item("HarassEMana").GetValue<Slider>().Value)
                    E.Cast();

            }
        }
        private static void Killsteal()
        {
            foreach (var enemy in
               ObjectManager.Get<Obj_AI_Hero>()
                   .Where(x => x.IsValidTarget(E.Range))
                   .Where(x => !x.IsZombie)
                   .Where(x => !x.IsDead))
            {
                var edmg = E.GetDamage(enemy);
                var aa = Player.GetAutoAttackDamage(enemy, true);

                if (enemy.Health < aa + 2 && enemy.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player)
                    && E.IsReady())
                    return;

                if (enemy.Health < edmg && E.IsReady())
                    E.Cast(enemy);
            }
        }
        private static void Lasthit()
        {
            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                                                                   minion.Distance(Player.ServerPosition) <= E.Range))
            {
                var aa = Player.GetAutoAttackDamage(minion, true);
                var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                float predictedHealtMinionE = HealthPrediction.GetHealthPrediction(minion,
                    (int)(E.Delay + (Player.Distance(minion.ServerPosition) / E.Speed)));

                if (minion.Health <= aa + 20 && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player))
                    return;

                if (Config.Item("LasthitE").GetValue<bool>() && E.IsReady() && Player.ManaPercent >= Config.Item("LasthitEMana").GetValue<Slider>().Value
                    && predictedHealtMinionE < E.GetDamage(minion)
                    && MinionsE.Count >= Config.Item("LasthitEslider").GetValue<Slider>().Value)
                    E.Cast(minion);
            }
        }
        private static void Laneclear()
        {
            foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                    minion.Distance(Player.ServerPosition) <= E.Range))
            {
                var aa = Player.GetAutoAttackDamage(minion, true);
                var MinionsA = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
                var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width);
                var Wfarmpos = Q.GetCircularFarmLocation(allMinionsW, W.Width);
 
                if (MinionsA.Count == 0)
                    return;

                float predictedHealtMinionE = HealthPrediction.GetHealthPrediction(minion,
                    (int)(E.Delay + (Player.Distance(minion.ServerPosition) / E.Speed)));

                if (minion.Team == GameObjectTeam.Neutral)
                    return;

                if (Config.Item("LaneclearE").GetValue<bool>())
                {
                    if (E.IsReady())
                    {
                        if (Player.ManaPercent >= Config.Item("LaneclearMana").GetValue<Slider>().Value
                            && minion.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= Config.Item("LaneclearEstacks").GetValue<Slider>().Value
                            && MinionsA.Count >= Config.Item("LaneclearEpoison").GetValue<Slider>().Value && minion.HasBuff("twitchdeadlyvenom"))
                            E.Cast();
                    }
                }
                if (Config.Item("LaneclearW").GetValue<bool>() && W.IsReady() &&
                    Wfarmpos.MinionsHit >= Config.Item("LaneclearWhit").GetValue<Slider>().Value
                    && Player.ManaPercent >= Config.Item("LaneclearMana").GetValue<Slider>().Value)
                    W.Cast(Wfarmpos.Position);
            }
        }
        private static void Jungleclear()
        {         
            foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                    minion.Distance(Player.ServerPosition) <= E.Range))
            {
                var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);       
                var Efarmpos = E.GetCircularFarmLocation(MinionsE, Q.Width);
                var MinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                var Wfarmpos = E.GetCircularFarmLocation(MinionsE, Q.Width);

                if (Config.Item("JungleclearE").GetValue<bool>() && E.IsReady()
                    && minion.Buffs.Find(buff => buff.Name == "twitchdeadlyvenom").Count >= 6
                    && Efarmpos.MinionsHit >= 1 && Player.ManaPercent >= Config.Item("JungleclearMana").GetValue<Slider>().Value)
                    E.Cast();

                if (Config.Item("JungleclearW").GetValue<bool>() && W.IsReady()
                    && Wfarmpos.MinionsHit >= 1 && minion.IsValidTarget(W.Range)
                    && Player.ManaPercent >= Config.Item("JungleclearMana").GetValue<Slider>().Value)
                    W.Cast(Wfarmpos.Position);
            }         
        }
        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("DrawDisable").GetValue<bool>())
                return;

            //DRAW SPELL RANGES
            if (Config.Item("DrawW").GetValue<Circle>().Active)
            {
                if (W.Level > 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? 
                    Config.Item("DrawW").GetValue<Circle>().Color : System.Drawing.Color.Red);                 
            }

            if (Config.Item("DrawW").GetValue<Circle>().Active)
                if (W.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range,
                        W.IsReady() ? Config.Item("DrawW").GetValue<Circle>().Color : System.Drawing.Color.Red);


            if (Config.Item("DrawE").GetValue<Circle>().Active)
            {
                if (E.Level > 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ?
                    Config.Item("DrawE").GetValue<Circle>().Color : System.Drawing.Color.Red);
            }


            if (Config.Item("DrawR").GetValue<Circle>().Active)
            {
                if (R.Level > 0)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, R.IsReady() ?
                    Config.Item("DrawR").GetValue<Circle>().Color : System.Drawing.Color.Red);
            }                                       
        }
    }
}
