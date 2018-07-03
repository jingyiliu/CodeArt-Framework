﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface IDependencyPropertyGotEventArgs
    {
        /// <summary>
        /// 赋予的值
        /// </summary>
        object Value { get; set; }

        DependencyProperty Property { get; }
    }
}