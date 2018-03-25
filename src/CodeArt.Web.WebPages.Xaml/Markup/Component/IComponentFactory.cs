﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IComponentFactory
    {
        object CreateComponent(string componentName);
    }
}
