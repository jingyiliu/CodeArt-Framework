﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ComponentLoader(typeof(InputHelpLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.InputHelp.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class InputHelp : ContentControl
    {
        static InputHelp()
        {
        }

 
    }
}
