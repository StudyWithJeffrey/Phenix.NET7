using System;
using System.Collections.Generic;
using System.Data.Common;
using Phenix.Core.Data;
using Phenix.Core.Data.Common;
using Phenix.Core.SyncCollections;

namespace Phenix.Business
{
    /// <summary>
    /// ҵ�����
    /// </summary>
    [Serializable]
    public abstract class BusinessBase<T> : UndoableBase<T>, IBusiness, IRefinedBusiness
        where T : BusinessBase<T>
    {
        /// <summary>
        /// for CreateInstance
        /// </summary>
        protected BusinessBase()
        {
            //��ֹ��Ӵ���
        }

        /// <summary>
        /// for Newtonsoft.Json.JsonConstructor
        /// </summary>
        protected BusinessBase(bool? isNew, bool? isSelfDeleted, bool? isSelfDirty,
            IDictionary<string, object> oldPropertyValues, IDictionary<string, bool?> dirtyPropertyNames)
            : base(isNew, isSelfDeleted, isSelfDirty, oldPropertyValues, dirtyPropertyNames)
        {
        }

        #region ����

        #region �ṹ��ϵ

        /// <summary>
        /// ��ҵ�����
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public IBusiness Root
        {
            get { return Master == null ? this : Master.Root; }
        }

        [NonSerialized]
        private IBusiness _master;

        /// <summary>
        /// ��ҵ�����
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public IBusiness Master
        {
            get { return _master; }
            private set { _master = value; }
        }

        [NonSerialized]
        private readonly SynchronizedDictionary<Type, List<IBusiness>> _details = new SynchronizedDictionary<Type, List<IBusiness>>();

        #endregion

        /// <summary>
        /// ����״̬(����ҵ�����)
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsDirty
        {
            get
            {
                if (IsSelfDirty)
                    return true;
                foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
                foreach (IBusiness item in kvp.Value)
                    if (item.IsDirty)
                        return true;
                return false;
            }
        }

        #endregion

        #region ����

        #region Detail

        /// <summary>
        /// ���ڴ�ҵ�����
        /// </summary>
        public bool HaveDetail<TDetailBusiness>()
            where TDetailBusiness : BusinessBase<TDetailBusiness>
        {
            return _details.ContainsKey(typeof(TDetailBusiness));
        }

        /// <summary>
        /// ������ҵ�����
        /// </summary>
        public TDetailBusiness[] FindDetail<TDetailBusiness>()
            where TDetailBusiness : BusinessBase<TDetailBusiness>
        {
            if (_details.TryGetValue(typeof(TDetailBusiness), out List<IBusiness> detail))
            {
                List<TDetailBusiness> result = new List<TDetailBusiness>(detail.Count);
                foreach (IBusiness item in detail)
                    result.Add((TDetailBusiness) item);
                return result.ToArray();
            }

            return null;
        }

        /// <summary>
        /// ��Ӵ�ҵ�����
        /// </summary>
        /// <param name="detail">��ҵ�����</param>
        protected void AddDetail<TDetailBusiness>(params TDetailBusiness[] detail)
            where TDetailBusiness : BusinessBase<TDetailBusiness>
        {
            AddDetail(true, detail);
        }

        /// <summary>
        /// ��Ӵ�ҵ�����
        /// </summary>
        /// <param name="ignoreRepeat">�����ظ���</param>
        /// <param name="detail">��ҵ�����</param>
        protected void AddDetail<TDetailBusiness>(bool ignoreRepeat = true, params TDetailBusiness[] detail)
            where TDetailBusiness : BusinessBase<TDetailBusiness>
        {
            if (detail == null || detail.Length == 0)
                return;
            List<IBusiness> value = _details.GetValue(typeof(TDetailBusiness), () => new List<IBusiness>());
            foreach (TDetailBusiness item in detail)
                if (ignoreRepeat || !value.Contains(item))
                {
                    item.Master = this;
                    value.Add(item);
                }
        }

        #endregion

        #region �༭״̬

        /// <summary>
        /// �����༭(������༭��IsSelfDirty״̬�Ķ���(����ҵ�����))
        /// ���յ�ǰ����
        /// </summary>
        public override void BeginEdit()
        {
            base.BeginEdit();

            foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
            foreach (IBusiness item in kvp.Value)
                item.BeginEdit();
        }

        /// <summary>
        /// �����༭(��������IsSelfDirty״̬�Ķ���(����ҵ�����))
        /// �ָ���������
        /// </summary>
        public override void CancelEdit()
        {
            base.CancelEdit();

            foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
            foreach (IBusiness item in kvp.Value)
                item.CancelEdit();
        }

        /// <summary>
        /// Ӧ�ñ༭(����ҵ�����)
        /// ������������
        /// </summary>
        public override void ApplyEdit()
        {
            base.ApplyEdit();

            foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
            foreach (IBusiness item in kvp.Value)
                item.ApplyEdit();
        }

        #endregion

        #region SaveDepth

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="checkTimestamp">�Ƿ���ʱ�������һ��ʱ�׳�Phenix.Core.Data.Validity.OutdatedDataException���������Ժ�ӳ��ʱ����ֶ�ʱ��Ч��</param>
        public virtual void SaveDepth(bool checkTimestamp = true)
        {
            Database.Execute((Action<DbTransaction, bool>) SaveDepth, checkTimestamp);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="connection">DbConnection(ע�������δ��У��)</param>
        /// <param name="checkTimestamp">�Ƿ���ʱ�������һ��ʱ�׳�Phenix.Core.Data.Validity.OutdatedDataException���������Ժ�ӳ��ʱ����ֶ�ʱ��Ч��</param>
        public virtual void SaveDepth(DbConnection connection, bool checkTimestamp = true)
        {
            DbConnectionHelper.Execute(connection, SaveDepth, checkTimestamp);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="transaction">DbTransaction(ע�������δ��У��)</param>
        /// <param name="checkTimestamp">�Ƿ���ʱ�������һ��ʱ�׳�Phenix.Core.Data.Validity.OutdatedDataException���������Ժ�ӳ��ʱ����ֶ�ʱ��Ч��</param>
        public virtual void SaveDepth(DbTransaction transaction, bool checkTimestamp = true)
        {
            if (IsNew)
                SaveDepth(transaction, ExecuteAction.Insert, checkTimestamp);
            else if (IsSelfDeleted)
                SaveDepth(transaction, ExecuteAction.Delete, checkTimestamp);
            else
                SaveDepth(transaction, ExecuteAction.Update, checkTimestamp);
            ApplyEdit();
        }

        void IRefinedBusiness.SaveDepth(DbTransaction transaction, bool checkTimestamp)
        {
            SaveDepth(transaction, checkTimestamp);
        }

        private void SaveDepth(DbTransaction transaction, ExecuteAction executeAction, bool checkTimestamp)
        {
            switch (executeAction)
            {
                case ExecuteAction.Insert:
                    if (IsSelfDeleted)
                        break;
                    InsertSelf(transaction);
                    foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
                    foreach (IBusiness item in kvp.Value)
                        ((IRefinedBusiness) item).SaveDepth(transaction, ExecuteAction.Insert, checkTimestamp);
                    break;
                case ExecuteAction.Delete:
                    if (IsNew)
                        break;
                    foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
                    foreach (IBusiness item in kvp.Value)
                        ((IRefinedBusiness) item).SaveDepth(transaction, ExecuteAction.Delete, checkTimestamp);
                    DeleteSelf(transaction, true);
                    break;
                case ExecuteAction.Update:
                    foreach (KeyValuePair<Type, List<IBusiness>> kvp in _details)
                    foreach (IBusiness item in kvp.Value)
                        if (item.IsNew)
                            ((IRefinedBusiness) item).SaveDepth(transaction, ExecuteAction.Insert, checkTimestamp);
                        else if (IsSelfDeleted)
                            ((IRefinedBusiness) item).SaveDepth(transaction, ExecuteAction.Delete, checkTimestamp);
                        else
                            ((IRefinedBusiness) item).SaveDepth(transaction, ExecuteAction.Update, checkTimestamp);
                    if (IsSelfDirty)
                        UpdateSelf(transaction, checkTimestamp, GetDirtValues());
                    break;
            }
        }

        void IRefinedBusiness.SaveDepth(DbTransaction transaction, ExecuteAction executeAction, bool checkTimestamp)
        {
            SaveDepth(transaction, executeAction, checkTimestamp);
        }

        #endregion

        #endregion
    }
}