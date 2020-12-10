using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysUserService : ISysUserService
    {
        private readonly IBaseService<Sys_User> _baseSysUserService;
        private readonly IMySystemService _mySystemService;

        public SysUserService(IBaseService<Sys_User> baseSysUserService,
            IMySystemService mySystemService)
        {
            _baseSysUserService = baseSysUserService;
            _mySystemService = mySystemService;
        }

        /// <summary>
        /// 根据登录用户名查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Sys_User GetUserByUserName(string userName)
        {
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.username == userName).SingleOrDefault();
            if (sys_User != null)
            {
                sys_User.password = "";
            }
            return sys_User;
        }

        // TODO：查询


        /// <summary>
        /// 用户管理：修改
        /// </summary>
        /// <param name="sys_User"></param>
        public void UpdateUser(Sys_User sys_User)
        {
            _baseSysUserService.UpdateRange(sys_User);
        }

        /// <summary>
        /// 用户管理：新增
        /// </summary>
        /// <param name="sys_User"></param>
        public void AddUser(Sys_User sys_User)
        {

        }

        /// <summary>
        /// 用户管理：删除
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUser(long userId)
        {
            _baseSysUserService.DeleteRange(_baseSysUserService.GetModels(a => a.id == userId));
        }

        /// <summary>
        /// 用户管理：更新用户的激活状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        public void UpdateEnabled(long id, bool enabled)
        {
            Sys_User sys_User = new Sys_User
            {
                id = id,
                enabled = enabled
            };
            _baseSysUserService.UpdateRange(sys_User);
        }
    }
}
