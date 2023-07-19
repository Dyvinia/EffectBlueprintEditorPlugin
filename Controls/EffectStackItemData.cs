using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EffectBlueprintEditorPlugin
{
    public class EffectStackItemData : BaseViewModel
    {

        #region -- Fields --

        public dynamic Value;

        private FrostyPropertyGrid propertyGrid;

        #endregion

        #region -- Properties --

        public Visibility HeaderVisiblity { get; set; }
        public Visibility ValuesVisiblity { get; set; }
        public Visibility SingleVisiblity { get; set; } = Visibility.Collapsed;

        public GridLength WWidth { get; set; } = new GridLength(1, GridUnitType.Star);

        public string ItemToolTip { get; set; }

        public string HeaderText { get; set; }

        public string XName { get; set; }
        public string YName { get; set; }
        public string ZName { get; set; }
        public string WName { get; set; }
        public string SingleName { get; set; }

        public float XValue {
            get {
                return Value.x;
            }
            set {
                if (Value.x != value) {
                    Value.x = value;
                    RaisePropertyChanged("XValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public float YValue {
            get {
                return Value.y;
            }
            set {
                if (Value.y != value) {
                    Value.y = value;
                    RaisePropertyChanged("YValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public float ZValue {
            get {
                return Value.z;
            }
            set {
                if (Value.z != value) {
                    Value.z = value;
                    RaisePropertyChanged("ZValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public float WValue {
            get {
                return Value.w;
            }
            set {
                if (Value.w != value) {
                    Value.w = value;
                    RaisePropertyChanged("WValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public float SingleValue {
            get {
                return Value.GetType().GetProperty(SingleName).GetValue(Value, null);
            }
            set {
                if (Value.GetType().GetProperty(SingleName).GetValue(Value, null) != value) {
                    Value.GetType().GetProperty(SingleName).SetValue(Value, value, null);
                    RaisePropertyChanged("SingleValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        #endregion

        #region -- Constructors --

        public EffectStackItemData(int propertyId, dynamic obj, FrostyPropertyGrid pg, Dictionary<int, string[]> vsfParams)
        {
            propertyGrid = pg;
            Value = obj;

            ItemToolTip = propertyId != -1 ? propertyId.ToString() : null;

            XName = $"X [{propertyId}]";
            YName = $"Y [{propertyId}]";
            ZName = $"Z [{propertyId}]";
            WName = $"W [{propertyId}]";

            HeaderVisiblity = Visibility.Collapsed;

            if (vsfParams.TryGetValue(propertyId, out string[] egParams)) {
                switch (egParams?.Length) {
                    // Assume Floatx4
                    case 4:
                        XName = egParams[0];
                        YName = egParams[1];
                        ZName = egParams[2];
                        WName = egParams[3];
                        break;
                    // Assume Floatx3
                    case 3:
                        XName = egParams[0];
                        YName = egParams[1];
                        ZName = egParams[2];
                        WName = "";
                        break;
                    // Assume Vec3 + Float
                    case 2:
                        XName = egParams[0];
                        YName = "";
                        ZName = "";
                        WName = egParams[1];
                        break;
                    // Assume Vec4
                    case 1:
                        XName = egParams[0];
                        YName = "";
                        ZName = "";
                        WName = "";
                        break;
                    // Display as a single string
                    default:
                        SingleName = String.Join("_", egParams);
                        XName = "";
                        YName = "";
                        ZName = "";
                        WName = "";
                        break;
                }
            }

            if (obj.GetType().Name == "Vec3") {
                WWidth = new GridLength(0, GridUnitType.Star);
            }
        }

        public EffectStackItemData(dynamic obj, string valueName, FrostyPropertyGrid pg) {
            propertyGrid = pg;
            Value = obj;

            SingleName = valueName;

            HeaderVisiblity = Visibility.Collapsed;
            ValuesVisiblity = Visibility.Collapsed;
            SingleVisiblity = Visibility.Visible;
        }

        public EffectStackItemData(dynamic obj, string headerText, bool isEnabled, FrostyPropertyGrid pg) {
            propertyGrid = pg;
            Value = obj;

            HeaderText = headerText;
            ValuesVisiblity = Visibility.Collapsed;
        }

        #endregion
    }
}
