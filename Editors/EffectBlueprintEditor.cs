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
    public class EffectBlueprintEditor : FrostyAssetEditor
    {

        #region -- Part Names --

        private const string PART_AssetPropertyGrid = "PART_AssetPropertyGrid";
        private const string PART_EmitterStackPanel = "PART_EmitterStackPanel";
        private const string PART_EmitterStack = "PART_EmitterStack";
        private const string PART_EmitterStackColumn = "PART_EmitterStackColumn";

        #endregion

        #region -- Parts --

        private FrostyPropertyGrid pgAsset;
        private FrostyDockablePanel emitterStackPanel;
        private ItemsControl emitterStack;
        private ColumnDefinition emitterStackColumn;


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
            emitterStack = GetTemplateChild(PART_EmitterStack) as ItemsControl;
            emitterStack.Loaded += EmitterStack_Loaded;
            emitterStack.ItemsSource = EmitterStackItems;
            emitterStackColumn = GetTemplateChild(PART_EmitterStackColumn) as ColumnDefinition;
            emitterStackColumn.Width = new GridLength(2, GridUnitType.Star);

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
            int count = 0;
            foreach (dynamic component in obj.Object.Internal.Components) {
                if (((object)component.Internal).GetType().Name == "EmitterGraphEntityData") {
                    EmitterStackItems.Add(new EmitterStackItemData(component.Internal.EmitterGraphParams[0], pgAsset, $"[{count}] {component.Internal.__Id}"));
                    foreach (dynamic param in component.Internal.EmitterGraphParams) {
                        EmitterStackItems.Add(new EmitterStackItemData(param, pgAsset));
                    }
                }
                count++;
            }
        }

        #region -- Control Events --

        private void PgAsset_OnModified(object sender, ItemModifiedEventArgs e)
        {
            GetEmitterProcessors(asset.RootObject);
        }

        private void EmitterStack_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region -- Toolbar --

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
            editor = false;
            pgAsset.Object = asset.RootObject;
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
