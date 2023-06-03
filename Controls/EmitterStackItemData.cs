using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScalableEmitterEditorPlugin
{
    public class EmitterStackItemData : BaseViewModel
    {

        #region -- Fields --

        public dynamic EmitterItemObj;
        public object EvaluatorObj;

        private FrostyPropertyGrid propertyGrid;

        private string xName;
        private string yName;
        private string zName;
        private string wName;

        #endregion

        #region -- Properties --

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
                return EmitterItemObj.Value.x;
            }
            set {
                if (EmitterItemObj.Value.x != value) {
                    EmitterItemObj.Value.x = value;
                    RaisePropertyChanged("XValueText");
                }
            }
        }

        public float YValue {
            get {
                return EmitterItemObj.Value.y;
            }
            set {
                if (EmitterItemObj.Value.y != value) {
                    EmitterItemObj.Value.y = value;
                    RaisePropertyChanged("YValueText");
                }
            }
        }

        public float ZValue {
            get {
                return EmitterItemObj.Value.z;
            }
            set {
                if (EmitterItemObj.Value.z != value) {
                    EmitterItemObj.Value.z = value;
                    RaisePropertyChanged("ZValueText");
                }
            }
        }

        public float WValue {
            get {
                return EmitterItemObj.Value.w;
            }
            set {
                if (EmitterItemObj.Value.w != value) {
                    EmitterItemObj.Value.w = value;
                    RaisePropertyChanged("WValueText");
                }
            }
        }

        #endregion

        #region -- Constructors --

        /// <summary>
        /// Initializes an instance of the <see cref="EmitterStackItemData"/> class with a referenced object.
        /// </summary>
        /// <param name="obj">The processor or evaluator that this item represents</param>
        /// <param name="isRoot">Is this item the emitter base?</param>
        /// <param name="pg">The property grid to be updated</param>
        public EmitterStackItemData(dynamic obj, bool isRoot, FrostyPropertyGrid pg)
        {
            propertyGrid = pg;
            EmitterItemObj = obj;

            XName = "XX";
            YName = "YY";
            ZName = "ZZ";
            WName = "WW";
        }

        #endregion
    }
}
