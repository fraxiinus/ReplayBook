using Rofl.Reader.Models;
using Rofl.UI.Main.Extensions;
using System;
using System.Collections.Generic;

namespace Rofl.UI.Main.Models
{
    public class PlayerDetail
    {
        public PlayerDetail(Player player, PlayerPreview previewModel, bool isBlueTeamMember)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }

            PreviewModel = previewModel ?? throw new ArgumentNullException(nameof(previewModel));

            // Basic info
            Level = player.LEVEL.ToInt();
            IsBlueTeamMember = isBlueTeamMember;
            TotalExperience = player.EXP.ToInt();
            GoldEarned = player.GOLD_EARNED.ToInt();
            GoldSpent = player.GOLD_SPENT.ToInt();

            // Kills/Deaths/Assists
            MinionsKilled = player.MINIONS_KILLED.ToInt();
            NeutralMinionsKilled = player.NEUTRAL_MINIONS_KILLED.ToInt();
            TotalMinionsKilled = MinionsKilled + NeutralMinionsKilled;
            NeutralMinionsKilledFromOwnJungle = player.NEUTRAL_MINIONS_KILLED_YOUR_JUNGLE.ToInt();
            NeutralMinionsKilledFromEnemyJungle = player.NEUTRAL_MINIONS_KILLED_ENEMY_JUNGLE.ToInt();
            ChampionsKilled = player.CHAMPIONS_KILLED.ToInt();
            Deaths = player.NUM_DEATHS.ToInt();
            Assists = player.ASSISTS.ToInt();
            LargestKillingSpree = player.LARGEST_KILLING_SPREE.ToInt();
            KillingSprees = player.KILLING_SPREES.ToInt();
            LargestMultiKill = player.LARGEST_MULTI_KILL.ToInt();
            BountyLevel = player.BOUNTY_LEVEL.ToInt();
            DoubleKills = player.DOUBLE_KILLS.ToInt();
            TripleKills = player.TRIPLE_KILLS.ToInt();
            QuadraKills = player.QUADRA_KILLS.ToInt();
            PentaKills = player.PENTA_KILLS.ToInt();
            UnrealKills = player.UNREAL_KILLS.ToInt();
            InhibitorsKilled = player.BARRACKS_KILLED.ToInt();
            TurretsKilled = player.TURRETS_KILLED.ToInt();

            LastHitNexus = Convert.ToBoolean(player.HQ_KILLED.ToInt());

            ObjectivesStolen = player.OBJECTIVES_STOLEN.ToInt();
            BaronsKilled = player.BARON_KILLS.ToInt();
            DragonsKilled = player.DRAGON_KILLS.ToInt();

            // Items
            ItemsPurchased = player.ITEMS_PURCHASED.ToInt();
            ConsumablesPurchased = player.CONSUMABLES_PURCHASED.ToInt();
            VisionWardsPurchased = player.VISION_WARDS_BOUGHT_IN_GAME.ToInt();
            WardsPlaced = player.WARD_PLACED.ToInt();
            WardsKilled = player.WARD_KILLED.ToInt();
            VisionScore = player.VISION_SCORE.ToInt();

            // Spells
            Spell1Casts = player.SPELL1_CAST.ToInt();
            Spell2Casts = player.SPELL2_CAST.ToInt();
            Spell3Casts = player.SPELL3_CAST.ToInt();
            Spell4Casts = player.SPELL4_CAST.ToInt();
            SummonerSpell1Casts = player.SUMMON_SPELL1_CAST.ToInt();
            SummonerSpell2Casts = player.SUMMON_SPELL2_CAST.ToInt();

            // Damage/Healing/Shelding Stats
            TotalDamageDealt = player.TOTAL_DAMAGE_DEALT.ToInt();
            PhysicalDamageDealt = player.PHYSICAL_DAMAGE_DEALT_PLAYER.ToInt();
            MagicDamageDealt = player.MAGIC_DAMAGE_DEALT_PLAYER.ToInt();
            TrueDamageDealt = player.TRUE_DAMAGE_DEALT_PLAYER.ToInt();
            TotalDamageDealtToChampions = player.TOTAL_DAMAGE_DEALT_TO_CHAMPIONS.ToInt();
            PhysicalDamageDealtToChampions = player.PHYSICAL_DAMAGE_DEALT_TO_CHAMPIONS.ToInt();
            MagicDamageDealtToChampions = player.MAGIC_DAMAGE_DEALT_TO_CHAMPIONS.ToInt();
            TrueDamageDealtToChampions = player.TRUE_DAMAGE_DEALT_TO_CHAMPIONS.ToInt();
            TotalDamageTaken = player.TOTAL_DAMAGE_TAKEN.ToInt();
            PhysicalDamageTaken = player.PHYSICAL_DAMAGE_TAKEN.ToInt();
            MagicDamageTaken = player.MAGIC_DAMAGE_TAKEN.ToInt();
            TrueDamageTaken = player.TRUE_DAMAGE_TAKEN.ToInt();
            TotalDamageSelfMitigated = player.TOTAL_DAMAGE_SELF_MITIGATED.ToInt();
            TotalDamageShieldedToTeammates = player.TOTAL_DAMAGE_SHIELDED_ON_TEAMMATES.ToInt();
            // Same as towers...
            //TotalDamageToBuildings = player.TOTAL_DAMAGE_DEALT_TO_BUILDINGS.ToInt();
            TotalDamageToTurrets = player.TOTAL_DAMAGE_DEALT_TO_TURRETS.ToInt();
            TotalDamageToObjectives = player.TOTAL_DAMAGE_DEALT_TO_OBJECTIVES.ToInt();
            LargestCriticalStrike = player.LARGEST_CRITICAL_STRIKE.ToInt();
            TotalTimeCrowdControlDealt = player.TOTAL_TIME_CROWD_CONTROL_DEALT.ToInt();
            TotalHealingDone = player.TOTAL_HEAL.ToInt();
            TotalHealingDoneToTeammates = player.TOTAL_HEAL_ON_TEAMMATES.ToInt();
            TotalUnitsHealed = player.TOTAL_UNITS_HEALED.ToInt();

            // Other
            LongestTimeSpentAlive = player.LONGEST_TIME_SPENT_LIVING.ToInt();
            TotalTimeSpentDead = player.TOTAL_TIME_SPENT_DEAD.ToInt();
            TimeSpentDisconnected = player.TIME_SPENT_DISCONNECTED.ToInt();
            CrowdControlScore = player.TIME_CCING_OTHERS.ToInt();
            PlayersMuted = player.PLAYERS_I_MUTED.ToInt();
            MutedByPlayers = player.PLAYERS_THAT_MUTED_ME.ToInt();
            Ping = player.PING.ToInt();

            // Only capitalize first letter of position name JUNGLE -> Jungle
#pragma warning disable CA1308 // Normalize strings to uppercase
            IndividualPosition = player.INDIVIDUAL_POSITION == null ? "N/A" : player.INDIVIDUAL_POSITION[0] + player.INDIVIDUAL_POSITION.Substring(1).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

            TeamEarlySurrendered = Convert.ToBoolean(player.TEAM_EARLY_SURRENDERED.ToInt());
            TimeOfLastDisconnect = player.TIME_OF_FROM_LAST_DISCONNECT.ToInt();
            WasAFK = Convert.ToBoolean(player.WAS_AFK.ToInt());
            WasAFKAfterFailedSurrender = Convert.ToBoolean(player.WAS_AFK_AFTER_FAILED_SURRENDER.ToInt());
            WasEarlySurrenderAccomplice = Convert.ToBoolean(player.WAS_EARLY_SURRENDER_ACCOMPLICE.ToInt());

            // Create items
            Items = new List<Item>
            {
                new Item(player.ITEM0),
                new Item(player.ITEM1),
                new Item(player.ITEM2),
                new Item(player.ITEM3),
                new Item(player.ITEM4),
                new Item(player.ITEM5),
                new Item(player.ITEM6)
            };

            // Runes
            KeystoneRune = new Rune(player.PERK0, player.PERK0_VAR1, player.PERK0_VAR2, player.PERK0_VAR3);
            Runes = new List<Rune>
            {
                new Rune(player.PERK1, player.PERK1_VAR1, player.PERK1_VAR2, player.PERK1_VAR3),
                new Rune(player.PERK2, player.PERK2_VAR1, player.PERK2_VAR2, player.PERK2_VAR3),
                new Rune(player.PERK3, player.PERK3_VAR1, player.PERK3_VAR2, player.PERK3_VAR3),
                new Rune(player.PERK4, player.PERK4_VAR1, player.PERK4_VAR2, player.PERK4_VAR3),
                new Rune(player.PERK5, player.PERK5_VAR1, player.PERK5_VAR2, player.PERK5_VAR3)
            };
            StatsRunes = new List<Rune>
            {
                new Rune(player.STAT_PERK_0, "", "", ""),
                new Rune(player.STAT_PERK_1, "", "", ""),
                new Rune(player.STAT_PERK_2, "", "", "")
            };
        }

        public PlayerPreview PreviewModel { get; private set; }

        public int Level { get; private set; }

        public bool IsBlueTeamMember { get; private set; }

        public int TotalExperience { get; private set; }

        public int GoldEarned { get; private set; }

        public int GoldSpent { get; private set; }

        public int TotalMinionsKilled { get; private set; }

        public int MinionsKilled { get; private set; }

        public int NeutralMinionsKilled { get; private set; }

        public int NeutralMinionsKilledFromOwnJungle { get; private set; }

        public int NeutralMinionsKilledFromEnemyJungle { get; private set; }

        public int ChampionsKilled { get; private set; }

        public int Deaths { get; private set; }

        public int Assists { get; private set; }

        public int LargestKillingSpree { get; private set; }

        public int KillingSprees { get; private set; }

        public int LargestMultiKill { get; private set; }

        public int BountyLevel { get; private set; }

        public int DoubleKills { get; private set; }

        public int TripleKills { get; private set; }

        public int QuadraKills { get; private set; }

        public int PentaKills { get; private set; }

        public int UnrealKills { get; private set; }

        public int InhibitorsKilled { get; private set; }

        public int TurretsKilled { get; private set; }

        public bool LastHitNexus { get; private set; }

        public int ObjectivesStolen { get; private set; }

        public int BaronsKilled { get; private set; }

        public int DragonsKilled { get; private set; }

        public int ItemsPurchased { get; private set; }

        public int ConsumablesPurchased { get; private set; }

        public int VisionWardsPurchased { get; private set; }

        public int WardsPlaced { get; private set; }

        public int WardsKilled { get; private set; }

        public int VisionScore { get; private set; }

        public int Spell1Casts { get; private set; }

        public int Spell2Casts { get; private set; }

        public int Spell3Casts { get; private set; }

        public int Spell4Casts { get; private set; }

        public int SummonerSpell1Casts { get; private set; }

        public int SummonerSpell2Casts { get; private set; }

        public int TotalDamageDealt { get; private set; }

        public int PhysicalDamageDealt { get; private set; }

        public int MagicDamageDealt { get; private set; }

        public int TrueDamageDealt { get; private set; }

        public int TotalDamageDealtToChampions { get; private set; }

        public int PhysicalDamageDealtToChampions { get; private set; }

        public int MagicDamageDealtToChampions { get; private set; }

        public int TrueDamageDealtToChampions { get; private set; }

        public int TotalDamageTaken { get; private set; }

        public int PhysicalDamageTaken { get; private set; }

        public int MagicDamageTaken { get; private set; }

        public int TrueDamageTaken { get; private set; }

        public int TotalDamageSelfMitigated { get; private set; }

        public int TotalDamageShieldedToTeammates { get; private set; }

        public int TotalDamageToTurrets { get; private set; }

        public int TotalDamageToObjectives { get; private set; }

        public int LargestCriticalStrike { get; private set; }

        public int TotalTimeCrowdControlDealt { get; private set; }

        public int TotalHealingDone { get; private set; }

        public int TotalHealingDoneToTeammates { get; private set; }

        public int TotalUnitsHealed { get; private set; }

        public int LongestTimeSpentAlive { get; private set; }

        public int TotalTimeSpentDead { get; private set; }

        public int TimeSpentDisconnected { get; private set; }

        public int CrowdControlScore { get; private set; }

        public int PlayersMuted { get; private set; }

        public int MutedByPlayers { get; private set; }

        public int Ping { get; private set; }

        public string IndividualPosition { get; private set; }

        public bool TeamEarlySurrendered { get; private set; }

        public int TimeOfLastDisconnect { get; private set; }

        public bool WasAFK { get; private set; }

        public bool WasAFKAfterFailedSurrender { get; private set; }

        public bool WasEarlySurrenderAccomplice { get; private set; }

        public Rune KeystoneRune { get; private set; }

        public IList<Item> Items { get; private set; }

        // Does not include keystone rune or stats runes
        public IList<Rune> Runes { get; private set; }

        public IList<Rune> StatsRunes { get; private set; }

        public Rune PrimaryPathRune0 => Runes[0];

        public Rune PrimaryPathRune1 => Runes[1];

        public Rune PrimaryPathRune2 => Runes[2];

        public Rune SecondaryPathRune0 => Runes[3];

        public Rune SecondaryPathRune1 => Runes[4];

        public Rune StatsRunes0 => StatsRunes[0];

        public Rune StatsRunes1 => StatsRunes[1];

        public Rune StatsRunes2 => StatsRunes[2];
    }
}
