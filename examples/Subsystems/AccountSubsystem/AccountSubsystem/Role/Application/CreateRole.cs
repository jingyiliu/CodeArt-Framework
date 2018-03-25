﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class CreateRole : Command<Role>
    {
        public Guid OrganizationId
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }


        private string _name;

        private IEnumerable<Guid> _permissionIds = null;



        private bool _isSystem;

        public CreateRole(string name, IEnumerable<Guid> permissionIds, bool isSystem)
        {
            _name = name;
            _permissionIds = permissionIds;
            _isSystem = isSystem;
        }

        protected override Role ExecuteProcedure()
        {
            var role = BuildRole();
            AddRole(role);
            return role;
        }

        private Role BuildRole()
        {
            Role role = new Role(Guid.NewGuid(), _isSystem)
            {
                Name = _name ?? string.Empty,
                Description = this.Description ?? string.Empty,
                MarkedCode = this.MarkedCode ?? string.Empty
            };

            //为角色分配权限，由于权限和角色的关系是松散的，所以我们不必锁定权限对象
            if(_permissionIds != null && _permissionIds.Count() > 0)
            {
                var permissions = PermissionCommon.FindsBy(_permissionIds);
                role.SetPermissions(permissions);
            }
            return role;
        }

        private void AddRole(Role role)
        {
            //向仓储中追加角色对象
            var repository = Repository.Create<IRoleRepository>();
            repository.Add(role);
        }
    }
}
