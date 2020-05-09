using System;
using System.Security.Principal;
using System.Threading;

namespace Phenix.Client.Security
{
    /// <summary>
    /// �û�
    /// </summary>
    public sealed class Principal : IPrincipal
    {
        internal Principal(Identity identity)
        {
            _identity = identity;
        }

        #region ����

        /// <summary>
        /// ��ǰ�û�
        /// </summary>
        public static Principal CurrentPrincipal
        {
            get
            {
                if (Thread.CurrentPrincipal is Principal result)
                    return result;
                Identity identity = Identity.CurrentIdentity;
                return identity != null ? new Principal(identity) : null;
            }
            set { Thread.CurrentPrincipal = value; }
        }

        #endregion

        #region ����

        private readonly Identity _identity;

        /// <summary>
        /// �û����
        /// </summary>
        public Identity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// �û����
        /// </summary>
        IIdentity IPrincipal.Identity
        {
            get { return Identity; }
        }

        #endregion

        #region ����

        #region IPrincipal ��Ա

        /// <summary>
        /// ȷ���Ƿ�����ָ���Ľ�ɫ
        /// </summary>
        /// <param name="role">��ɫ</param>
        /// <returns>����ָ���Ľ�ɫ</returns>
        public bool IsInRole(string role)
        {
            Identity identity = Identity;
            if (identity == null)
                return false;
            if (!identity.IsAuthenticated)
                return false;
            if (String.IsNullOrEmpty(role))
                return true;
            return identity.User.Position != null && identity.User.Position.IsInRole(role);
        }

        #endregion

        #endregion
    }
}