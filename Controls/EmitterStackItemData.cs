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

        public object EmitterItemObj;
        public object EvaluatorObj;

        private FrostyPropertyGrid propertyGrid;
        private string processorText;
        private string evaluatorText;

        #endregion

        #region -- Properties --

        public string ProcessorText
        {
            get
            {
                return processorText;
            }
            set
            {
                if (processorText != value)
                {
                    processorText = value;
                    RaisePropertyChanged("ProcessorText");
                }
            }
        }
        public string EvaluatorText
        {
            get
            {
                return evaluatorText;
            }
            set
            {
                if (evaluatorText != value)
                {
                    evaluatorText = value;
                    RaisePropertyChanged("EvaluatorText");
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
            isEmitterRoot = isRoot;
            ProcessorSelected = false;
            EvaluatorSelected = false;
            ProcessorText = "Processor";
            EvaluatorText = "Evaluator";
            EvaluatorVisible = true;

            ProcessorText = ((int)obj.PropertyId).ToString();

            EvaluatorText = ((int)obj.PropertyId).ToString();

        }

        #endregion

    }
}
