using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Ebx;
using EffectBlueprintEditorPlugin.Windows;

namespace EffectBlueprintEditorPlugin {
    public class EffectStackItemData : BaseViewModel {

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

        public string XValue {
            get {
                if (Value.GetType().Name == "Vec4" || Value.GetType().Name == "Vec3")
                    return Value.x.ToString();
                else
                    return string.Empty;
            }
            set {
                if (float.TryParse(value, out float result) && Value.x != result) {
                    Value.x = result;
                    RaisePropertyChanged("XValue");
                    propertyGrid.Modified = true;
                }
                else if (TrySolve(value, out float mathResult)) {
                    Value.x = mathResult;
                    RaisePropertyChanged("XValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public string YValue {
            get {
                if (Value.GetType().Name == "Vec4" || Value.GetType().Name == "Vec3")
                    return Value.y.ToString();
                else
                    return string.Empty;
            }
            set {
                if (float.TryParse(value, out float result) && Value.y != result) {
                    Value.y = result;
                    RaisePropertyChanged("YValue");
                    propertyGrid.Modified = true;
                }
                else if (TrySolve(value, out float mathResult)) {
                    Value.y = mathResult;
                    RaisePropertyChanged("YValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public string ZValue {
            get {
                if (Value.GetType().Name == "Vec4" || Value.GetType().Name == "Vec3")
                    return Value.z.ToString();
                else
                    return string.Empty;
            }
            set {
                if (float.TryParse(value, out float result) && Value.z != result) {
                    Value.z = result;
                    RaisePropertyChanged("ZValue");
                    propertyGrid.Modified = true;
                }
                else if (TrySolve(value, out float mathResult)) {
                    Value.z = mathResult;
                    RaisePropertyChanged("ZValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public string WValue {
            get {
                if (Value.GetType().Name == "Vec4")
                    return Value.w.ToString();
                else
                    return string.Empty;
            }
            set {
                if (float.TryParse(value, out float result) && Value.w != result) {
                    Value.w = result;
                    RaisePropertyChanged("WValue");
                    propertyGrid.Modified = true;
                }
                else if (TrySolve(value, out float mathResult)) {
                    Value.w = mathResult;
                    RaisePropertyChanged("WValue");
                    propertyGrid.Modified = true;
                }
            }
        }

        public bool? ComponentEnabled {
            get {
                if (Value.GetType().Name == "PointerRef") {
                    dynamic enableScalable = Value.Internal.Enable;
                    if (enableScalable.Low && enableScalable.Medium && enableScalable.High && enableScalable.Ultra)
                        return true;
                    else if (enableScalable.Low || enableScalable.Medium || enableScalable.High || enableScalable.Ultra)
                        return null;
                    else
                        return false;
                }
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
                if (SingleName is null)
                    return float.NaN;
                else
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

        public bool TrySolve(string expression, out float result) {
            try {
                result = (float)Convert.ToDouble(new DataTable().Compute(expression, null));
                return true;
            }
            catch {
                result = float.NaN;
                return false;
            }
        }

        #endregion

        #region -- Constructors --

        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }

        public EffectStackItemData(int propertyId, dynamic obj, FrostyPropertyGrid pg, Dictionary<int, string[]> vsfParams) {
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
                    if (float.TryParse(values[0], out float _))
                        XValue = values[0];

                    if (float.TryParse(values[1], out float _))
                        YValue = values[1];

                    if (float.TryParse(values[2], out float _))
                        ZValue = values[2];

                    if (values.Length == 4 && obj.GetType().Name != "Vec3" && float.TryParse(values[3], out float _))
                        WValue = values[3];
                }
            });
        }

        public EffectStackItemData(dynamic obj, string valueName, FrostyPropertyGrid pg) {
            propertyGrid = pg;
            Value = obj;

            SingleName = valueName;
            SingleVisiblity = Visibility.Visible;
        }

        public EffectStackItemData(dynamic obj, string headerText, List<PointerRef> components, FrostyPropertyGrid pg, Action<object> refreshAction) {
            propertyGrid = pg;
            Value = obj;

            HeaderText = headerText;
            HeaderVisiblity = Visibility.Visible;

            CopyCommand = new RelayCommand((_) => {
                try {
                    FrostyClipboard.Current.SetData(new PointerRef(Value.Internal));
                }
                catch (Exception e) {
                    SystemSounds.Hand.Play();
                    App.Logger.LogError($"Unable To Copy to Clipboard: {e.Message}");
                }
            });

            PasteCommand = new RelayCommand((_) => {
                if (FrostyClipboard.Current.HasData) {
                    dynamic clipboardData = FrostyClipboard.Current.GetData(pg.Asset, App.AssetManager.GetEbxEntry(pg.Asset.FileGuid));
                    clipboardData = clipboardData?.Internal;

                    if (clipboardData is null)
                        return;

                    components[components.IndexOf(obj)] = new PointerRef((object)clipboardData);

                    propertyGrid.Modified = true;
                }
            } + refreshAction);
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

                    dynamic newParamRef = egParams.FirstOrDefault(p => p.PropertyId == newParamWin.SelectedParam);
                    newParam.PropertyId = newParamRef.PropertyId;
                    newParam.Value.x = newParamRef.Value.x;
                    newParam.Value.y = newParamRef.Value.y;
                    newParam.Value.z = newParamRef.Value.z;
                    newParam.Value.w = newParamRef.Value.w;

                    currentVsf.Add(newParam);

                    propertyGrid.Modified = true;

                }
            }
            + refreshAction);

            NewPropVisiblity = Visibility.Visible;
        }

        #endregion
    }
}
