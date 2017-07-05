﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.DomainDriven;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    internal class DataProxyPro : DataProxy
    {
        /// <summary>
        /// 从数据库中加载的数据
        /// </summary>
        public DynamicData OriginalData
        {
            get;
            private set;
        }

        /// <summary>
        /// 对象对应的数据表
        /// </summary>
        internal DataTable Table
        {
            get;
            private set;
        }

        public DataProxyPro(DynamicData originalData, DataTable table)
        {
            this.OriginalData = originalData;
            this.Table = table;
        }

        protected override object LoadData(DomainProperty property)
        {
            var tip = property.RepositoryTip;
            if (tip != null && tip.Lazy)
                return this.Table.ReadPropertyValue(this.Owner, tip, null, this.OriginalData);
            return null;
        }


        public override bool IsSnapshot
        {
            get
            {
                if (this.IsFromSnapshot) return true;
                //通过对比数据版本号判定数据是否为快照
                var current = (int)this.OriginalData.Get(GeneratedField.DataVersionName);
                var latest = this.Table.GetDataVersion(this.OriginalData);
                return current != latest; //当对象已经被删除，对象版本号大于数据库版本号，当对象被修改，当前对象版本号小于数据库版本号
            }
        }

        public override bool IsFromSnapshot
        {
            get
            {
                //如果这个对象来自快照表，那么它就是来自于仓储的快照存储区
                return this.Table.IsSnapshot;
            }
        }


        //public override void Clear()
        //{
        //    base.Clear();
        //    this.OriginalData = null;
        //    this.Table = null;
        //}

        //private static Pool<DataProxyPro> _pool = new Pool<DataProxyPro>(() =>
        //{
        //    return new DataProxyPro();
        //}, (data, phase) =>
        //{
        //    data.Clear();
        //    return true;
        //}, new PoolConfig()
        //{
        //    MaxRemainTime = 300 //闲置时间300秒
        //});


        ///// <summary>
        ///// 创建代理对象，生命周期与当前的共生器同步
        ///// </summary>
        ///// <returns></returns>
        //internal static DataProxyPro CreateWithSymbiosis()
        //{
        //    var temp = _pool.Borrow();
        //    Symbiosis.Current.Mark(temp);
        //    return temp.Item;
        //}
    }
    }
