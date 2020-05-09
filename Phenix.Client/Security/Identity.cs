using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Phenix.Client.Security
{
    /// <summary>
    /// �û����
    /// </summary>
    public sealed class Identity : IIdentity
    {
        internal Identity(HttpClient httpClient, string name, string password)
        {
            _httpClient = httpClient;
            _user = new User(this, name, password);
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

        private readonly HttpClient _httpClient;

        /// <summary>
        /// HttpClient
        /// </summary>
        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

        private User _user;

        /// <summary>
        /// �û�����
        /// </summary>
        public User User
        {
            get { return _user; }
        }

        /// <summary>
        /// ��¼��
        /// </summary>
        public string Name
        {
            get { return _user.Name; }
        }

        /// <summary>
        /// �������֤?
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _user.IsAuthenticated; }
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

        internal async Task LogonAsync(string tag)
        {
            await _user.LogonAsync(tag);
            _user = await ReFetchUserAsync();
        }

        /// <summary>
        /// ���»�ȡ�û�����
        /// </summary>
        public async Task<User> ReFetchUserAsync()
        {
            _user = await _user.ReFetchAsync();
            return _user;
        }

        #endregion
    }
}