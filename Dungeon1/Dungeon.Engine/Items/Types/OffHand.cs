﻿namespace Dungeon.Items.Types
{
    using Dungeon.Items.Enums;

    public class OffHand : Item
    {
        public override Stats AvailableStats => Stats.Attack & Stats.Damage & Stats.Defence;
        public override ItemKind Kind => ItemKind.OffHand;
    }
}