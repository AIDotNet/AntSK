using AntDesign;
using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace AntSK.Pages.Setting.Role
{
    public partial class AddRole
    {
        [Parameter]
        public string RoleId { get; set; }
        [Inject] protected IRoles_Repositories _roles_Repositories { get; set; }
        [Inject] protected IPermissions_Repositories _permissions_Repositories { get; set; }
        [Inject] protected IRolePermissions_Repositories _rolePermissions_Repositories { get; set; }
        [Inject] protected MessageService? Message { get; set; }

        private Roles _roleModel = new Roles();
        private List<Permissions> _allPermissions = new List<Permissions>();
        private IEnumerable<string> _permissionIds;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            // 加载所有权限
            _allPermissions = _permissions_Repositories.GetList();

            if (!string.IsNullOrEmpty(RoleId))
            {
                _roleModel = _roles_Repositories.GetFirst(p => p.Id == RoleId);
                
                // 加载角色已有的权限
                var rolePermissions = _rolePermissions_Repositories.GetList(p => p.RoleId == RoleId);
                _permissionIds = rolePermissions.Select(rp => rp.PermissionId);
            }
        }

        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(RoleId))
            {
                //新增
                _roleModel.Id = Guid.NewGuid().ToString();
                _roleModel.CreateTime = DateTime.Now;

                if (_roles_Repositories.IsAny(p => p.Code == _roleModel.Code))
                {
                    _ = Message.Error("角色编码已存在！", 2);
                    return;
                }
                _roles_Repositories.Insert(_roleModel);

                // 添加角色权限关联
                if (_permissionIds != null)
                {
                    foreach (var permissionId in _permissionIds)
                    {
                        _rolePermissions_Repositories.Insert(new RolePermissions
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = _roleModel.Id,
                            PermissionId = permissionId,
                            CreateTime = DateTime.Now
                        });
                    }
                }
            }
            else
            {
                //修改
                _roles_Repositories.Update(_roleModel);

                // 先删除旧的角色权限关联
                var oldRolePermissions = _rolePermissions_Repositories.GetList(p => p.RoleId == RoleId);
                foreach (var rp in oldRolePermissions)
                {
                    _rolePermissions_Repositories.Delete(rp.Id);
                }

                // 添加新的角色权限关联
                if (_permissionIds != null)
                {
                    foreach (var permissionId in _permissionIds)
                    {
                        _rolePermissions_Repositories.Insert(new RolePermissions
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = _roleModel.Id,
                            PermissionId = permissionId,
                            CreateTime = DateTime.Now
                        });
                    }
                }
            }

            Back();
        }

        private void Back()
        {
            NavigationManager.NavigateTo("/setting/rolelist");
        }
    }
}
