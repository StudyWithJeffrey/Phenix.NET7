using System.Security.Principal;
using System.Threading;

namespace Phenix.Client.Security
{
    /// <summary>
    /// �û�
    /// </summary>
    public sealed class Principal : IPrincipal
    {
        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="identity">�û����</param>
        public Principal(Identity identity)
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
        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsInRole(role);
        }

        #endregion

        #endregion
    }
}