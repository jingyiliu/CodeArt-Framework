﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class AssetFileBindingExtensionConverter : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            var assetFileKey = GetAssetFileKey(value);
            if (string.IsNullOrEmpty(assetFileKey)) throw new XamlException("资产绑定表达式" + value + "格式不正确");
            return new AssetFileBindingExtension(assetFileKey);
        }

        /// <summary>
        /// 获取资产的编号
        /// </summary>
        /// <returns></returns>
        private string GetAssetFileKey(string value)
        {
            var pos = value.IndexOf(" ");
            if (pos > -1) return value.Substring(pos + 1, value.Length - pos - 2);
            return string.Empty;
        }

    }
}