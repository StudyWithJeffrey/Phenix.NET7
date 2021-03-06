﻿using System;
using System.Threading.Tasks;
using Demo.InspectionStation.Plugin.Business;
using Orleans.Streams;
using Phenix.Actor;

namespace Demo.InspectionStation.Plugin.Actor
{
    /// <summary>
    /// 作业点Grain
    /// </summary>
    public class OperationPointGrain : StreamEntityGrainBase<IsOperationPoint, IsOperationPoint>, IOperationPointGrain
    {
        #region 属性

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
        protected override IsOperationPoint Kernel
        {
            get
            {
                return base.Kernel ?? (base.Kernel = IsOperationPoint.FetchRoot(Database,
                           p => p.Name == Name,
                           () => IsOperationPoint.New(Database,
                               IsOperationPoint.Set(p => p.Name, Name))));
            }
        }

        /// <summary>
        /// StreamId
        /// </summary>
        protected override Guid StreamId
        {
            get { return AppConfig.OperationPointStreamId; }
        }

        /// <summary>
        /// StreamNamespace
        /// </summary>
        protected override string StreamNamespace
        {
            get { return Name; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="token">StreamSequenceToken</param>
        protected override Task OnReceiving(IsOperationPoint content, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        Task<OperationPointStatus> IOperationPointGrain.GetStatus()
        {
            return Task.FromResult(Kernel.OperationPointStatus);
        }

        Task<int> IOperationPointGrain.GetWeighbridge()
        {
            return Task.FromResult(Kernel.Weighbridge);
        }

        Task IOperationPointGrain.SetWeighbridge(int value)
        {
            Kernel.SetWeighbridge(value);
            Send(Kernel);
            return Task.CompletedTask;
        }
        
        Task IOperationPointGrain.WeighbridgeAlive()
        {
            Kernel.WeighbridgeAlive();
            Send(Kernel);
            return Task.CompletedTask;
        }
        
        Task<string> IOperationPointGrain.GetLicensePlate()
        {
            return Task.FromResult(Kernel.LicensePlate);
        }

        Task IOperationPointGrain.SetLicensePlate(string value)
        {
            Kernel.SetLicensePlate(value);
            Send(Kernel);
            return Task.CompletedTask;
        }

        Task IOperationPointGrain.LicensePlateAlive()
        {
            Kernel.LicensePlateAlive();
            Send(Kernel);
            return Task.CompletedTask;
        }

        Task IOperationPointGrain.PermitThrough()
        {
            Kernel.PermitThrough();
            Send(Kernel);
            return Task.CompletedTask;
        }

        #endregion
    }
}