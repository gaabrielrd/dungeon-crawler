namespace DungeonCrawler.Data.Definitions
{
    public enum DamageType
    {
        Physical,
        Magical,
        True
    }

    public enum SkillTargetType
    {
        Self,
        Ally,
        Enemy,
        AllAllies,
        AllEnemies,
        Any
    }

    public enum ItemType
    {
        Consumable,
        Material,
        KeyItem,
        CurrencyBundle
    }

    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum EncounterType
    {
        Common,
        Elite,
        Boss
    }

    public enum UpgradeType
    {
        Hero,
        Skill,
        Equipment,
        Shop,
        Run
    }

    public enum StatusEffectType
    {
        Buff,
        Debuff,
        DamageOverTime,
        HealOverTime,
        Control
    }

    public enum StatusEffectStackPolicy
    {
        RefreshDuration,
        AddStacks,
        Replace,
        Ignore
    }

    public enum ItemUseTarget
    {
        None,
        Self,
        Ally,
        Party,
        Enemy
    }
}
