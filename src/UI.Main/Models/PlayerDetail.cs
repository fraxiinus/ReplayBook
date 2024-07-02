using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Extensions;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Models;
public class PlayerDetail
{
    private readonly StaticDataManager _staticDataManager;
    private readonly string _patchVersion;
    private readonly PlayerStats2 _player;

    public PlayerDetail(StaticDataManager staticData,
        string patchVersion,
        PlayerStats2 player,
        PlayerPreview previewModel,
        bool isBlueTeamMember)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        PreviewModel = previewModel ?? throw new ArgumentNullException(nameof(previewModel));
        _staticDataManager = staticData;
        _patchVersion = patchVersion.VersionSubstring();

        // Basic info
        Level = player.Level.ToInt();
        IsBlueTeamMember = isBlueTeamMember;
        TotalExperience = player.Exp.ToInt();
        GoldEarned = player.GoldEarned.ToInt();
        GoldSpent = player.GoldSpent.ToInt();
        PlayerSubteam = player.PlayerSubteam.ToInt();
        PlayerSubteamPlacement = player.PlayerSubteamPlacement.ToInt();
        PUUID = player.PUUID;

        GoldFromTurretPlatesTaken = player.MissionsGoldFromTurretPlatesTaken.ToInt();
        GoldFromStructuresDestroyed = player.MissionsGoldFromStructuresDestroyed.ToInt();

        // Kills/Deaths/Assists
        MinionsKilled = player.MinionsKilled.ToInt();
        NeutralMinionsKilled = player.NeutralMinionsKilled.ToInt();
        TotalMinionsKilled = MinionsKilled + NeutralMinionsKilled;
        NeutralMinionsKilledFromOwnJungle = player.NeutralMinionsKilledYourJungle.ToInt();
        NeutralMinionsKilledFromEnemyJungle = player.NeutralMinionsKilledEnemyJungle.ToInt();
        ChampionsKilled = player.ChampionsKilled.ToInt();
        Deaths = player.NumDeaths.ToInt();
        Assists = player.Assists.ToInt();
        LargestKillingSpree = player.LargestKillingSpree.ToInt();
        KillingSprees = player.KillingSprees.ToInt();
        LargestMultiKill = player.LargestMultiKill.ToInt();
        BountyLevel = player.BountyLevel.ToInt();
        DoubleKills = player.DoubleKills.ToInt();
        TripleKills = player.TripleKills.ToInt();
        QuadraKills = player.QuadraKills.ToInt();
        PentaKills = player.PentaKills.ToInt();
        UnrealKills = player.UnrealKills.ToInt();
        InhibitorsKilled = player.BarracksKilled.ToInt();
        TurretsKilled = player.TurretsKilled.ToInt();

        LastHitNexus = Convert.ToBoolean(player.HQKilled.ToInt());

        ObjectivesStolen = player.ObjectivesStolen.ToInt();
        VoidGrubKills = player.HordeKills.ToInt();
        RiftHeraldKills = player.RiftHeraldKills.ToInt();
        BaronsKilled = player.BaronKills.ToInt();
        DragonsKilled = player.DragonKills.ToInt();

        // Items
        ItemsPurchased = player.ItemsPurchased.ToInt();
        ConsumablesPurchased = player.ConsumablesPurchased.ToInt();
        VisionWardsPurchased = player.VisionWardsBoughtInGame.ToInt();
        WardsPlaced = player.WardPlaced.ToInt();
        WardsKilled = player.WardKilled.ToInt();
        VisionScore = player.VisionScore.ToInt();

        // Spells
        Spell1Casts = player.Spell1Cast.ToInt();
        Spell2Casts = player.Spell2Cast.ToInt();
        Spell3Casts = player.Spell3Cast.ToInt();
        Spell4Casts = player.Spell4Cast.ToInt();
        SummonerSpell1Casts = player.SummonSpell1Cast.ToInt();
        SummonerSpell2Casts = player.SummonSpell2Cast.ToInt();

        // Damage/Healing/Shelding Stats
        TotalDamageDealt = player.TotalDamageDealt.ToInt();
        PhysicalDamageDealt = player.PhysicalDamageDealtPlayer.ToInt();
        MagicDamageDealt = player.MagicDamageDealtPlayer.ToInt();
        TrueDamageDealt = player.TrueDamageDealtPlayer.ToInt();
        TotalDamageDealtToChampions = player.TotalDamageDealtToChampions.ToInt();
        PhysicalDamageDealtToChampions = player.PhysicalDamageDealtToChampions.ToInt();
        MagicDamageDealtToChampions = player.MagicDamageDealtToChampions.ToInt();
        TrueDamageDealtToChampions = player.TrueDamageDealtToChampions.ToInt();
        TotalDamageTaken = player.TotalDamageTaken.ToInt();
        PhysicalDamageTaken = player.PhysicalDamageTaken.ToInt();
        MagicDamageTaken = player.MagicDamageTaken.ToInt();
        TrueDamageTaken = player.TrueDamageTaken.ToInt();
        TotalDamageSelfMitigated = player.TotalDamageSelfMitigated.ToInt();
        TotalDamageShieldedToTeammates = player.TotalDamageShieldedOnTeammates.ToInt();
        // Same as towers...
        //TotalDamageToBuildings = player.TOTAL_DAMAGE_DEALT_TO_BUILDINGS.ToInt();
        TotalDamageToTurrets = player.TotalDamageDealtToTurrets.ToInt();
        TurretPlatesDestroyed = player.MissionsTurretPlatesDestroyed.ToInt();
        TotalDamageToObjectives = player.TotalDamageDealtToObjectives.ToInt();
        LargestCriticalStrike = player.LargestCriticalStrike.ToInt();
        LargestAbilityDamage = player.LargestAbilityDamage.ToInt();
        LargestAttackDamage = player.LargestAttackDamage.ToInt();
        TotalTimeCrowdControlDealt = player.TotalTimeCrowdControlDealt.ToInt();
        TotalTimeCrowdControlDealtToChampions = player.TotalTimeCrowdControlDealtToChampions.ToInt();
        TotalHealingDone = player.TotalHeal.ToInt();
        TotalHealingDoneToTeammates = player.TotalHealOnTeammates.ToInt();
        TotalUnitsHealed = player.TotalUnitsHealed.ToInt();

        // Other
        LastTakedownTime = player.LastTakedownTime.ToInt();
        LongestTimeSpentAlive = player.LongestTimeSpentLiving.ToInt();
        TotalTimeSpentDead = player.TotalTimeSpentDead.ToInt();
        TimeSpentDisconnected = player.TimeSpentDisconnected.ToInt();
        CrowdControlScore = player.TimeCCingOthers.ToInt();
        PlayersMuted = player.PlayersIMuted.ToInt();
        MutedByPlayers = player.PlayersThatMutedMe.ToInt();
        Ping = player.Ping.ToInt();
        AllInPings = player.AllInPings.ToInt();
        AssistMePings = player.AssistMePings.ToInt();
        BaitPings = player.BaitPings.ToInt();
        BasicPings = player.BasicPings.ToInt();
        CommandPings = player.CommandPings.ToInt();
        DangerPings = player.DangerPings.ToInt();
        EnemyMissingPings = player.EnemyMissingPings.ToInt();
        EnemyVisionPings = player.EnemyVisionPings.ToInt();
        GetBackPings = player.GetBackPings.ToInt();
        HoldPings = player.HoldPings.ToInt();
        NeedVisionPings = player.NeedVisionPings.ToInt();
        OnMyWayPings = player.OnMyWayPings.ToInt();
        PushPings = player.PushPings.ToInt();
        RetreatPings = player.RetreatPings.ToInt();
        VisionClearedPings = player.VisionClearedPings.ToInt();

        PlayerAugment1 = player.PlayerAugment1.ToInt();
        PlayerAugment2 = player.PlayerAugment2.ToInt();
        PlayerAugment3 = player.PlayerAugment3.ToInt();
        PlayerAugment4 = player.PlayerAugment4.ToInt();

        // Only capitalize first letter of position name JUNGLE -> Jungle
        IndividualPosition = player.IndividualPosition == null ? "N/A" : player.IndividualPosition[0] + player.IndividualPosition[1..].ToLowerInvariant();
        TeamEarlySurrendered = Convert.ToBoolean(player.TeamEarlySurrendered.ToInt());
        TimeOfLastDisconnect = player.TimeOfFromLastDisconnect.ToInt();
        WasAFK = Convert.ToBoolean(player.WasAfk.ToInt());
        WasAFKAfterFailedSurrender = Convert.ToBoolean(player.WasAfkAfterFailedSurrender.ToInt());
        WasEarlySurrenderAccomplice = Convert.ToBoolean(player.WasEarlySurrenderAccomplice.ToInt());

        // Create items
        Items = new List<Item>
            {
                new Item(player.Item0),
                new Item(player.Item1),
                new Item(player.Item2),
                new Item(player.Item3),
                new Item(player.Item4),
                new Item(player.Item5),
                new Item(player.Item6)
            };
    }

    public async Task LoadRunes()
    {
        KeystoneRune = new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk0, _patchVersion),
            _player.Perk0, _player.Perk0Var1, _player.Perk0Var2, _player.Perk0Var3);

        Runes = new List<RuneStat>
            {
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk1, _patchVersion),
                    _player.Perk1, _player.Perk1Var1, _player.Perk1Var2, _player.Perk1Var3),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk2, _patchVersion),
                    _player.Perk2, _player.Perk2Var1, _player.Perk2Var2, _player.Perk2Var3),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk3, _patchVersion),
                    _player.Perk3, _player.Perk3Var1, _player.Perk3Var2, _player.Perk3Var3),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk4, _patchVersion),
                    _player.Perk4, _player.Perk4Var1, _player.Perk4Var2, _player.Perk4Var3),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.Perk5, _patchVersion),
                    _player.Perk5, _player.Perk5Var1, _player.Perk5Var2, _player.Perk5Var3)
            };
        StatsRunes = new List<RuneStat>
            {
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.StatPerk0, _patchVersion),
                    _player.StatPerk0, "", "", ""),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.StatPerk1, _patchVersion),
                    _player.StatPerk1, "", "", ""),
                new RuneStat(await _staticDataManager.GetRuneDataForCurrentLanguage(_player.StatPerk2, _patchVersion),
                    _player.StatPerk2, "", "", "")
            };
    }

    public PlayerPreview PreviewModel { get; private set; }

    public int Level { get; private set; }

    public bool IsBlueTeamMember { get; private set; }

    // Unknown value/type
    public int PlayerSubteam { get; private set; }
    
    // Unknown value/type
    public int PlayerSubteamPlacement { get; private set; }

    // Player Universally Unique IDentifiers. The value is unencrypted.
    public string PUUID { get; private set; }

    public int TotalExperience { get; private set; }

    public int GoldEarned { get; private set; }

    public int GoldFromTurretPlatesTaken { get; private set; }

    public int GoldFromStructuresDestroyed { get; private set; }

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

    // This is called 'Horde Kills' in the replay file
    public int VoidGrubKills { get; private set; }

    public int RiftHeraldKills { get; private set; }

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

    public int TurretPlatesDestroyed { get; private set; }

    public int TotalDamageToObjectives { get; private set; }

    public int LargestCriticalStrike { get; private set; }

    public int LargestAbilityDamage { get; private set; }

    public int LargestAttackDamage { get; private set; }

    public int TotalTimeCrowdControlDealt { get; private set; }

    public int TotalTimeCrowdControlDealtToChampions { get; private set; }

    public int TotalHealingDone { get; private set; }

    public int TotalHealingDoneToTeammates { get; private set; }

    public int TotalUnitsHealed { get; private set; }

    public int LongestTimeSpentAlive { get; private set; }

    public int LastTakedownTime { get; private set; }

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

    public int AllInPings { get; private set; }

    public int AssistMePings { get; private set; }

    public int BaitPings { get; private set; }

    public int BasicPings { get; private set; }

    public int CommandPings { get; private set; }

    public int DangerPings { get; private set; }

    public int EnemyMissingPings { get; private set; }

    public int EnemyVisionPings { get; private set; }

    public int GetBackPings { get; private set; }

    public int HoldPings { get; private set; }

    public int NeedVisionPings { get; private set; }

    public int OnMyWayPings { get; private set; }

    public int PushPings { get; private set; }

    public int RetreatPings { get; private set; }

    public int VisionClearedPings { get; private set; }

    public RuneStat KeystoneRune { get; private set; }

    public IList<Item> Items { get; private set; }

    // Does not include keystone rune or stats runes
    public IList<RuneStat> Runes { get; private set; }

    public IList<RuneStat> StatsRunes { get; private set; }

    // Unknown value/type, perhaps arena?
    public int PlayerAugment1 { get; private set; }

    public int PlayerAugment2 { get; private set; }

    public int PlayerAugment3 { get; private set; }

    public int PlayerAugment4 { get; private set; }

    public RuneStat PrimaryPathRune0 => Runes[0];

    public RuneStat PrimaryPathRune1 => Runes[1];

    public RuneStat PrimaryPathRune2 => Runes[2];

    public RuneStat SecondaryPathRune0 => Runes[3];

    public RuneStat SecondaryPathRune1 => Runes[4];

    public RuneStat StatsRunes0 => StatsRunes[0];

    public RuneStat StatsRunes1 => StatsRunes[1];

    public RuneStat StatsRunes2 => StatsRunes[2];
}
