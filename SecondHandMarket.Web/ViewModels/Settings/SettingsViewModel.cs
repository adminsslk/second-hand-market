using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecondHandMarket.Web.ViewModels.Settings
{
    public abstract class SettingsViewModel : ViewModel
    {
        public string SelectedSettingsTab { get; set; }
    }
}