using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScalableEmitterEditorPlugin
{
    [TemplatePart(Name = PART_StackCanvas, Type = typeof(Canvas))]
    public class EffectStackItem : UserControl
    {

        #region -- Part Names --

        private const string PART_StackCanvas = "PART_StackCanvas";

        #endregion

        #region -- Constructors --

        static EffectStackItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EffectStackItem), new FrameworkPropertyMetadata(typeof(EffectStackItem)));
        }

        public EffectStackItem()
        {
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
}
