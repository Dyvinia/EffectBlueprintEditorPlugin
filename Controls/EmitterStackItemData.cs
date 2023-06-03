using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScalableEmitterEditorPlugin
{
    public class EmitterStackItemData : BaseViewModel
    {

        #region -- Fields --

        public dynamic Value;
        public object EvaluatorObj;

        private FrostyPropertyGrid propertyGrid;

        private string xName;
        private string yName;
        private string zName;
        private string wName;

        #endregion

        #region -- Properties --

        public Visibility HeaderVisiblity { get; set; }
        public Visibility ValuesVisiblity { get; set; }

        public Visibility WVisiblity { get; set; }

        public string ItemToolTip { get; set; }

        public string HeaderText { get; set; }

        public string XName {
            get {
                return xName;
            }
            set {
                if (xName != value) {
                    xName = value;
                    RaisePropertyChanged("XName");
                }
            }
        }

        public string YName {
            get {
                return yName;
            }
            set {
                if (yName != value) {
                    yName = value;
                    RaisePropertyChanged("YName");
                }
            }
        }

        public string ZName {
            get {
                return zName;
            }
            set {
                if (zName != value) {
                    zName = value;
                    RaisePropertyChanged("ZName");
                }
            }
        }

        public string WName {
            get {
                return wName;
            }
            set {
                if (wName != value) {
                    wName = value;
                    RaisePropertyChanged("WName");
                }
            }
        }

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

        #endregion

        #region -- Constructors --

        public EmitterStackItemData(int propertyId, dynamic obj, FrostyPropertyGrid pg, Dictionary<int, string[]> vsfParams, string header = null)
        {
            propertyGrid = pg;
            Value = obj;

            ItemToolTip = propertyId != -1 ? propertyId.ToString() : null;

            XName = "X";
            YName = "Y";
            ZName = "Z";
            WName = "W";

            if (header != null) {
                HeaderText = header;
                ValuesVisiblity = Visibility.Collapsed;
            }
            else {
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
                    }
                }

                if (obj.GetType().Name == "Vec3") {
                    XName = "";
                    YName = egParams[0];
                    ZName = "";
                    WName = "";
                    WVisiblity = Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
