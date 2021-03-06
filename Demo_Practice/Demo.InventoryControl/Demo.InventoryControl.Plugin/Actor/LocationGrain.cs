﻿using System.Threading.Tasks;
using Demo.InventoryControl.Plugin.Business;
using Phenix.Actor;

namespace Demo.InventoryControl.Plugin.Actor
{
    /// <summary>
    /// 货架Grain
    /// </summary>
    public class LocationGrain : EntityGrainBase<IcLocation>, ILocationGrain
    {
        #region 属性

        private string _area;

        /// <summary>
        /// 库区
        /// </summary>
        protected string Area
        {
            get { return _area ?? (_area = AppConfig.ExtractArea(Name)); }
        }

        private string _alley;

        /// <summary>
        /// 巷道
        /// </summary>
        protected string Alley
        {
            get { return _alley ?? (_alley = AppConfig.ExtractAlley(Name)); }
        }

        private string _ordinal;

        /// <summary>
        /// 序号
        /// </summary>
        protected string Ordinal
        {
            get { return _ordinal ?? (_ordinal = AppConfig.ExtractOrdinal(Name)); }
        }

        /// <summary>
        /// ID(映射表ID字段)
        /// </summary>
        protected override long Id
        {
            get { return Kernel.Id; }
        }
        
        /// <summary>
        /// 根实体对象
        /// </summary>
        protected override IcLocation Kernel
        {
            get
            {
                return base.Kernel ?? (base.Kernel = IcLocation.FetchRoot(Database,
                           p => p.Area == Area && p.Alley == Alley && p.Ordinal == Ordinal,
                           () => IcLocation.New(Database,
                               IcLocation.Set(p => p.Area, Area).
                                   Set(p => p.Alley, Alley).
                                   Set(p => p.Ordinal, Ordinal))));
            }
        }

        #endregion

        #region 方法

        Task<long> ILocationGrain.GetStackOrdinal()
        {
            return Task.FromResult(Kernel.GetStackOrdinal());
        }

        Task<int> ILocationGrain.GetUnloadValue(string brand, string cardNumber, string transportNumber)
        {
            return Task.FromResult(Kernel.GetUnloadValue(brand, cardNumber, transportNumber));
        }

        Task ILocationGrain.Refresh()
        {
            Kernel.Refresh();
            return Task.CompletedTask;
        }

        #endregion
    }
}
