using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Frosty.Controls;
using System.Windows.Media;
using System.Dynamic;

namespace ScalableEmitterEditorPlugin
{
    [TemplatePart(Name = PART_EmitterStackPanel, Type = typeof(FrostyDockablePanel))]
    [TemplatePart(Name = PART_EmitterStack, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_EmitterStackColumn, Type = typeof(ColumnDefinition))]
    [TemplatePart(Name = PART_EmitterQualityLowText, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_EmitterQualityLow, Type = typeof(RadioButton))]
    [TemplatePart(Name = PART_EmitterQualityMedium, Type = typeof(RadioButton))]
    [TemplatePart(Name = PART_EmitterQualityHigh, Type = typeof(RadioButton))]
    [TemplatePart(Name = PART_EmitterQualityUltra, Type = typeof(RadioButton))]
    public class EffectBlueprintEditor : FrostyAssetEditor
    {

        #region -- Part Names --

        private const string PART_AssetPropertyGrid = "PART_AssetPropertyGrid";
        private const string PART_EmitterStackPanel = "PART_EmitterStackPanel";
        private const string PART_EmitterStack = "PART_EmitterStack";
        private const string PART_EmitterStackColumn = "PART_EmitterStackColumn";

        private const string PART_EmitterQualityLowText = "PART_EmitterQualityLowText";
        private const string PART_EmitterQualityLow = "PART_EmitterQualityLow";
        private const string PART_EmitterQualityMedium = "PART_EmitterQualityMedium";
        private const string PART_EmitterQualityHigh = "PART_EmitterQualityHigh";
        private const string PART_EmitterQualityUltra = "PART_EmitterQualityUltra";

        #endregion

        #region -- Parts --

        private FrostyPropertyGrid pgAsset;
        private FrostyDockablePanel emitterStackPanel;
        private ItemsControl emitterStack;
        private ColumnDefinition emitterStackColumn;

        private TextBlock emitterQualityLowText;
        private RadioButton emitterQualityLow;

        private bool editor = true;

        #endregion

        public ObservableCollection<EmitterStackItemData> EmitterStackItems { get; set; }

        #region -- Constructors --

        static EffectBlueprintEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EffectBlueprintEditor), new FrameworkPropertyMetadata(typeof(EffectBlueprintEditor)));
        }

        public EffectBlueprintEditor()
            : base(null)
        {
            EmitterStackItems = new ObservableCollection<EmitterStackItemData>();
        }

        public EffectBlueprintEditor(ILogger inLogger)
            : base(inLogger)
        {
            EmitterStackItems = new ObservableCollection<EmitterStackItemData>();
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            pgAsset = GetTemplateChild(PART_AssetPropertyGrid) as FrostyPropertyGrid;
            pgAsset.OnModified += PgAsset_OnModified;
            emitterStackPanel = GetTemplateChild(PART_EmitterStackPanel) as FrostyDockablePanel;
            //emitterStackPanel.MouseLeftButtonDown += EmitterStackPanel_MouseLeftButtonDown;
            emitterStack = GetTemplateChild(PART_EmitterStack) as ItemsControl;
            emitterStack.Loaded += EmitterStack_Loaded;
            emitterStack.MouseLeftButtonDown += EmitterStack_MouseLeftButtonDown;
            emitterStack.ItemsSource = EmitterStackItems;
            emitterStackColumn = GetTemplateChild(PART_EmitterStackColumn) as ColumnDefinition;
            emitterStackColumn.Width = new GridLength(2, GridUnitType.Star);

            UpdateToolbar();

            Loaded += EmitterDocumentEditor_Loaded;
            
        }

        private void EmitterDocumentEditor_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic obj = asset.RootObject;

            GetEmitterProcessors(obj);

            pgAsset.Object = obj.Object.Internal;

            emitterStackColumn.Width = new GridLength(2, GridUnitType.Star);
        }

        void GetEmitterProcessors(dynamic obj)
        {
            if (!editor) return;
            
            EmitterStackItems.Clear();
            foreach (dynamic component in obj.Object.Internal.Components) {
                if (((object)component.Internal).GetType().Name == "EmitterGraphEntityData") {
                    EmitterStackItems.Add(new EmitterStackItemData(component.Internal.EmitterGraphParams[0], pgAsset, component.Internal.__Id));
                    foreach (dynamic param in component.Internal.EmitterGraphParams) {
                        EmitterStackItems.Add(new EmitterStackItemData(param, pgAsset));
                    }
                }
            }
        }

        #region -- Control Events --

        private void PgAsset_OnModified(object sender, ItemModifiedEventArgs e)
        {
            GetEmitterProcessors(asset.RootObject);
        }

        private void UpdateToolbar()
        {
            dynamic obj = asset.RootObject;
        }

        /*
        private void EmitterStackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject visualHit = VisualTreeHelper.HitTest(emitterStackPanel, e.GetPosition(emitterStackPanel)).VisualHit;
            while (visualHit != emitterStack && visualHit != null)
            {
                visualHit = VisualTreeHelper.GetParent(visualHit);
            }

            if (visualHit == emitterStack)
            {
                pgAsset.Object = 0;
                pgAsset.Object = null;
                //logger.Log("Clicked emitter stack");

                for (int i = 0; i < emitterStack.Items.Count; i++)
                {
                    // go down the visual tree into the UniformGrid
                    UIElement stackItemParent = (UIElement)emitterStack.ItemContainerGenerator.ContainerFromIndex(i);
                    for (int k = 0; k < 4; k++)
                    {
                        stackItemParent = VisualTreeHelper.GetChild(stackItemParent, 0) as UIElement;
                    }

                    // go down the visual tree into the processor block
                    UIElement proc = stackItemParent;
                    for (int k = 0; k < 2; k++)
                    {
                        proc = VisualTreeHelper.GetChild(proc, 1 - k) as UIElement;
                    }

                    // go down the visual tree into the evaluator block
                    UIElement eval = stackItemParent;
                    for (int k = 0; k < 2; k++)
                    {
                        eval = VisualTreeHelper.GetChild(eval, 0) as UIElement;
                    }

                    if (proc != null)
                    {
                        if (proc.IsMouseOver)
                        {
                            EmitterStackItems[i].ProcessorSelected = true;
                            pgAsset.SetClass(EmitterStackItems[i].EmitterItemObj);
                            //logger.Log("Processor selected");
                        }
                        else
                        {
                            EmitterStackItems[i].ProcessorSelected = false;
                            //logger.Log("Processor deselected");
                        }
                    }
                    if (eval != null)
                    {
                        if (eval.IsMouseOver)
                        {
                            EmitterStackItems[i].EvaluatorSelected = true;
                            pgAsset.SetClass(EmitterStackItems[i].EvaluatorObj);
                            //logger.Log("Evaluator selected");
                        }
                        else
                        {
                            EmitterStackItems[i].EvaluatorSelected = false;
                            //logger.Log("Evaluator deselected");
                        }
                    }
                }
            }
            else
            {
                // stop the property grid from being cleared if there isn't a stack being displayed
                if (EmitterStackItems.Count > 0)
                {
                    foreach (EmitterStackItemData item in EmitterStackItems)
                    {
                        item.ProcessorSelected = false;
                        item.EvaluatorSelected = false;
                    }
                    pgAsset.SetClass(EmitterStackItems[0].EmitterItemObj);
                }
            }
        }*/

        private void EmitterStack_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void EmitterStack_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ///logger.Log("Clicked emitter stack");
            ///for (int i = 0; i < emitterStack.Items.Count; i++)
            ///{
            ///    //ContentPresenter c = (ContentPresenter)emitterStack.ItemContainerGenerator.ContainerFromItem(emitterStack.Items[i]);
            ///    //Border proc = c.ContentTemplate.FindName("PART_ProcessorBox", c) as Border;
            ///    Border proc = ItemsControlHelpers.findElementInItemsControlItemAtIndex<Border>(emitterStack, i, "PART_ProcessorBox") as Border;
            ///    Border eval = ItemsControlHelpers.findElementInItemsControlItemAtIndex<Border>(emitterStack, i, "PART_EvaluatorBox") as Border;
            ///    //Border eval = c.ContentTemplate.FindName("PART_EvaluatorBox", c) as Border;
            ///
            ///    if (proc != null)
            ///    {
            ///        if (proc.IsMouseOver)
            ///        {
            ///            EmitterStackItems[i].ProcessorSelected = true;
            ///            logger.Log("Processor selected");
            ///        }
            ///        else
            ///        {
            ///            EmitterStackItems[i].ProcessorSelected = false;
            ///            logger.Log("Processor deselected");
            ///        }
            ///    }
            ///    if (eval != null)
            ///    {
            ///        if (eval.IsMouseOver)
            ///        {
            ///            EmitterStackItems[i].EvaluatorSelected = true;
            ///            logger.Log("Evaluator selected");
            ///        }
            ///        else
            ///        {
            ///            EmitterStackItems[i].EvaluatorSelected = false;
            ///            logger.Log("Evaluator deselected");
            ///        }
            ///    }
            ///
            ///}
            ///EmitterStackItem stackItem = emitterStack.InputHitTest(e.GetPosition(emitterStack)) as EmitterStackItem;
            ///if (stackItem != null)
            ///{
            ///    emitterStack.Items.Contains(stackItem);
            ///    logger.Log("passed hit test");
            ///}
        }

        #endregion

        #region -- Toolbar --

        bool[] activeQualityLevel = { true, false, false, false };

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new ToolbarItem("Disable Editor", "", "", new RelayCommand((object state) => { DisableEditor(this); })),
                new ToolbarItem("Show Editor", "", "", new RelayCommand((object state) => { EnableEditor(this); }))
            };
        }

        private void DisableEditor(object sender)
        {
            pgAsset.Object = asset.RootObject;
            editor = false;
            EmitterStackItems.Clear();
            emitterStackColumn.Width = new GridLength(0);
        }

        private void EnableEditor(object sender)
        {
            editor = true;
            dynamic obj = asset.RootObject;
            pgAsset.Object = obj.Object.Internal;
            GetEmitterProcessors(obj);
            emitterStackColumn.Width = new GridLength(2, GridUnitType.Star);
        }

        #endregion

    }
}
