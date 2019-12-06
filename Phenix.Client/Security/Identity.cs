using System.Security.Principal;
using System.Threading;

namespace Phenix.Client.Security
{
    /// <summary>
    /// �û����
    /// </summary>
    public sealed class Identity : IIdentity
    {
        internal Identity(User user)
        {
            _user = user;
        }

        #region ����

        /// <summary>
        /// ��ǰ�û����
        /// </summary>
        public static Identity CurrentIdentity
        {
            get { return HttpClient.Default != null ? HttpClient.Default.Identity : Thread.CurrentPrincipal != null ? Thread.CurrentPrincipal.Identity as Identity : null; }
            set { Thread.CurrentPrincipal = value != null ? new Principal(value) : null; }
        }

        #endregion

        #region ����

        private User _user;

        /// <summary>
        /// �û�����
        /// </summary>
        public User User
        {
            get { return _user; }
            internal set { _user = value; }
        }

        /// <summary>
        /// ��¼��
        /// </summary>
        public string Name
        {
            get { return _user.Name; }
        }

        private bool _isAuthenticated;

        /// <summary>
        /// �������֤?
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated && !_user.Disabled && !_user.Locked; }
            internal set { _isAuthenticated = value; }
        }

        /// <summary>
        /// �����֤����
        /// </summary>
        public string AuthenticationType
        {
            get { return "Phenix-Authorization"; }
        }

        #endregion

        #region ����

        /// <summary>
        /// ȷ���Ƿ�����ָ���Ľ�ɫ
        /// </summary>
        /// <param name="role">��ɫ</param>
        public bool IsInRole(string role)
        {
            if (!IsAuthenticated)
                return false;
            return User.IsAdmin || User.Position != null && User.Position.Roles.Contains(role);
        }

        #endregion
    }
}