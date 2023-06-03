﻿using Frosty.Core.Controls;
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
    [TemplatePart(Name = PART_Refresh, Type = typeof(Button))]
    public class EmitterStackItem : UserControl
    {

        #region -- Part Names --

        private const string PART_StackCanvas = "PART_StackCanvas";
        private const string PART_Refresh = "PART_Refresh";

        #endregion

        #region -- Constructors --

        static EmitterStackItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmitterStackItem), new FrameworkPropertyMetadata(typeof(EmitterStackItem)));
        }

        public EmitterStackItem()
        {
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
}
