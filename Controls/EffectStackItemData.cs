using EffectBlueprintEditorPlugin.Windows;
using Frosty.Core;
using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace EffectBlueprintEditorPlugin
{
    public class EffectStackItemData : BaseViewModel
    {

        #region -- Fields --

        public dynamic Value;

        private FrostyPropertyGrid propertyGrid;

        #endregion

        #region -- Properties --

        public Visibility HeaderVisiblity { get; set; } = Visibility.Collapsed;

        public Visibility ValuesVisiblity { get; set; } = Visibility.Collapsed;
        public Visibility SingleVisiblity { get; set; } = Visibility.Collapsed;

        public Visibility NewPropVisiblity { get; set; } = Visibility.Collapsed;

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

        public bool? ComponentEnabled {
            get {
                dynamic enableScalable = Value.Internal.Enable;
                if (enableScalable.Low && enableScalable.Medium && enableScalable.High && enableScalable.Ultra)
                    return true;
                else if (enableScalable.Low || enableScalable.Medium || enableScalable.High || enableScalable.Ultra)
                    return null;
                else
                    return false;
            }
            set {
                dynamic enableScalable = Value.Internal.Enable;
                enableScalable.GetType().GetProperty("Low").SetValue(enableScalable, value, null);
                enableScalable.GetType().GetProperty("Medium").SetValue(enableScalable, value, null);
                enableScalable.GetType().GetProperty("High").SetValue(enableScalable, value, null);
                enableScalable.GetType().GetProperty("Ultra").SetValue(enableScalable, value, null);
                RaisePropertyChanged("ComponentEnabled");
                propertyGrid.Modified = true;
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

        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }

        public EffectStackItemData(int propertyId, dynamic obj, FrostyPropertyGrid pg, Dictionary<int, string[]> vsfParams)
        {
            propertyGrid = pg;
            Value = obj;

            ItemToolTip = propertyId != -1 ? propertyId.ToString() : null;

            XName = $"X [{propertyId}]";
            YName = $"Y [{propertyId}]";
            ZName = $"Z [{propertyId}]";
            WName = $"W [{propertyId}]";

            ValuesVisiblity = Visibility.Visible;

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

            CopyCommand = new RelayCommand((_) => {
                try {
                    if (obj.GetType().Name == "Vec3")
                        Clipboard.SetText($"{XValue}/{YValue}/{ZValue}");
                    else
                        Clipboard.SetText($"{XValue}/{YValue}/{ZValue}/{WValue}");
                }
                catch (Exception e) {
                    SystemSounds.Hand.Play();
                    App.Logger.LogError($"Unable To Copy to Clipboard: {e.Message}");
                }
            });

            PasteCommand = new RelayCommand((_) => {
                string clipboard = Clipboard.GetText();
                string[] values = clipboard.Trim().Split('/');

                if (values.Length >= 3) {
                    if (float.TryParse(values[0], out float resultX))
                        XValue = resultX;

                    if (float.TryParse(values[1], out float resultY))
                        YValue = resultY;

                    if (float.TryParse(values[2], out float resultZ))
                        ZValue = resultZ;

                    if (values.Length == 4 && float.TryParse(values[3], out float resultW))
                        WValue = resultW;
                }
            });
        }

        public EffectStackItemData(dynamic obj, string valueName, FrostyPropertyGrid pg) {
            propertyGrid = pg;
            Value = obj;

            SingleName = valueName;
            SingleVisiblity = Visibility.Visible;
        }

        public EffectStackItemData(dynamic obj, string headerText, bool isEnabled, FrostyPropertyGrid pg) {
            propertyGrid = pg;
            Value = obj;

            HeaderText = headerText;
            HeaderVisiblity = Visibility.Visible;
        }


        public ICommand ButtonClickedCommand { get; set; }

        public EffectStackItemData(dynamic obj, FrostyPropertyGrid pg, Dictionary<int, string[]> vsfParams, Action<object> refreshAction) {
            propertyGrid = pg;
            Value = obj;

            ButtonClickedCommand = new RelayCommand((_) => {
                dynamic currentVsf = Value.EmitterGraphParams;

                dynamic[] egParams = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(Value.EmitterGraph.External.FileGuid)).RootObject.EmitterGraphParams.ToArray();

                NewParamWindow newParamWin = new NewParamWindow(vsfParams, currentVsf);

                if (newParamWin.ShowDialog() == true) {
                    // i think i have to clone it?
                    dynamic newParam = Activator.CreateInstance(currentVsf.GetType().GetGenericArguments()[0]);

                    dynamic newParamRef = egParams.Where(p => p.PropertyId == newParamWin.SelectedParam).FirstOrDefault();
                    newParam.PropertyId = newParamRef.PropertyId;
                    newParam.Value.x = newParamRef.Value.x;
                    newParam.Value.y = newParamRef.Value.y;
                    newParam.Value.z = newParamRef.Value.z;
                    newParam.Value.w = newParamRef.Value.w;

                    currentVsf.Add(newParam);

                    propertyGrid.Modified = true;

                }
            } + refreshAction);

            NewPropVisiblity = Visibility.Visible;
        }

        #endregion
    }
}
