using System.Security.Claims;

namespace ApiServer.Application.Interfaces
{
    /// <summary>
    /// ��ǰ�û������Ľӿ�
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// �Ƿ�����֤
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// �û�ID����������������
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// �û�������������������
        /// </summary>
        string? Username { get; }

        /// <summary>
        /// ��ɫ�б�
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// ԭʼ�������ƣ�Bearer Token��
        /// </summary>
        string? Token { get; }

        /// <summary>
        /// ȫ������
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// ��ȡָ�����͵�����ֵ
        /// </summary>
        string? GetClaim(string type);

        /// <summary>
        /// �Ƿ�ӵ��ָ����ɫ
        /// </summary>
        bool HasRole(string role);
    }
}
