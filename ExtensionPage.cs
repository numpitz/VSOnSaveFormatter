using Microsoft.Build.Framework.XamlTypes;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace OnSaveFormatter
{
    public class ExtensionPage : DialogPage
    {
        public enum OptionsEnabled
        {
            Extension1,
            All,
            CPP,
            CS,
            XML,
            JSON,
            XAML,
            MD
        }

        private bool[] _properties = new bool[Enum.GetValues(typeof(OptionsEnabled)).Length];
        public bool this[OptionsEnabled index]
        {
            get => _properties[(int)index];
            set => _properties[(int)index] = value;
        }

        [Category("General")]
        [DisplayName("Enable Auto-Format On Save")]
        [Description("If disabed non of the format gets format on save.")]
        public bool EnableAutoFormatOnSave { get; set; } = true;

        [Category("General")]
        [DisplayName("auto format all files")]
        public bool EnableAll
        {
            get => this[OptionsEnabled.All];

            set
            {
                this[OptionsEnabled.All] = value;
            }
        }

        [Category("General")]
        [DisplayName("auto format for .h .cpp")]
        public bool EnableCPP
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.CPP];

            set
            {
                this[OptionsEnabled.CPP] = value;
            }
        }


        [Category("General")]
        [DisplayName("auto format for .cs")]
        public bool EnableCS
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.CS];

            set
            {
                this[OptionsEnabled.CS] = value;
            }
        }
        [Category("General")]
        [DisplayName("auto format for .xml")]
        public bool EnableXML
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.XML];

            set
            {
                this[OptionsEnabled.XML] = value;
            }
        }
        [Category("General")]
        [DisplayName("auto format for .json")]
        public bool EnableJSON
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.JSON];

            set
            {
                this[OptionsEnabled.JSON] = value;
            }
        }
        [Category("General")]
        [DisplayName("auto format for .xaml")]
        public bool EnableXAML
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.XAML];

            set
            {
                this[OptionsEnabled.XAML] = value;
            }
        }
        [Category("General")]
        [DisplayName("auto format for .md")]
        public bool EnableMD
        {
            get => this[OptionsEnabled.All] || this[OptionsEnabled.MD];

            set
            {
                this[OptionsEnabled.MD] = value;
            }
        }
    }
}
