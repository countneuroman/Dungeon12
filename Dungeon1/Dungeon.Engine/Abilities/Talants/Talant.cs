﻿namespace Dungeon.Abilities.Talants
{
    using Dungeon.Abilities.Talants.NotAPI;
    using Dungeon.Classes;
    using Dungeon.Map;
    using Dungeon.Map.Objects;
    using Dungeon.Transactions;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using System;


    public abstract class Talant<TClass> : TalantBase
         where TClass : Character
    {
        public Talant(int order):base(order)
        {

        }

        public override string Image => $"Dungeon.Classes.{this.GetType().BaseType.GenericTypeArguments[0].Name}.Images.Talants.{GetType().Name}.png";

        /// <summary>
        /// Метод вызывается для того что бы забиндить параметры для <see cref="Applicable.Apply(object)"/> и <see cref="Applicable.Discard(object)"/>
        /// </summary>
        /// <param name="gameMap"></param>
        /// <param name="avatar"></param>
        /// <param name="class"></param>
        public void Bind(GameMap gameMap, Avatar avatar, TClass @class)
        {
            this.Class = @class;
            this.GameMap = gameMap;
            this.Avatar = avatar;
        }

        public TClass Class { get; set; }

        public Avatar Avatar { get; set; }

        public GameMap GameMap { get; set; }
        
        public virtual bool CanUse(object @object)
        {
            return this.CallCanUse(@object as dynamic);
        }

        protected abstract bool CallCanUse(dynamic obj);
    }
}