﻿using System;
using Phenix.Core.Message;
using Phenix.Core.Security;

namespace Phenix.Services.Extend.Api.Message
{
    /// <summary>
    /// 用户消息服务
    /// </summary>
    public sealed class MessageService : IMessageService
    {
        #region 方法

        void IMessageService.OnConnected(Identity identity, string connectionId)
        {
            /*
             * 本函数被执行到，说明客户端已连接到 SignalR hub 中间件
             * 当前登录用户身份为 identity
             */
        }

        void IMessageService.OnDisconnected(Identity identity, string connectionId, Exception exception)
        {
            /*
             * 本函数被执行到，说明客户端已断开 SignalR hub 中间件
             * 当前登录用户身份为 identity
             */
        }

        #endregion
    }
}