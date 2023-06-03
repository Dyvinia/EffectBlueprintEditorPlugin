﻿using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Frosty.Controls;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace ScalableEmitterEditorPlugin
{
    [TemplatePart(Name = PART_EmitterStackPanel, Type = typeof(FrostyDockablePanel))]
    [TemplatePart(Name = PART_EmitterStack, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_EmitterStackColumn, Type = typeof(ColumnDefinition))]
    [TemplatePart(Name = PART_Refresh, Type = typeof(Button))]
    [TemplatePart(Name = PART_AutoRefresh, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_ShowUnknown, Type = typeof(ToggleButton))]
    public class EffectBlueprintEditor : FrostyAssetEditor
    {

        #region -- Part Names --

        private const string PART_AssetPropertyGrid = "PART_AssetPropertyGrid";
        private const string PART_EmitterStackPanel = "PART_EmitterStackPanel";
        private const string PART_EmitterStack = "PART_EmitterStack";
        private const string PART_EmitterStackColumn = "PART_EmitterStackColumn";
        private const string PART_Refresh = "PART_Refresh";
        private const string PART_AutoRefresh = "PART_AutoRefresh";

        private const string PART_ShowEG = "PART_ShowEG";
        private const string PART_ShowLE = "PART_ShowLE";

        private const string PART_ShowUnknown = "PART_ShowUnknown";

        #endregion

        #region -- Parts --

        private FrostyPropertyGrid pgAsset;
        private FrostyDockablePanel emitterStackPanel;
        private ItemsControl emitterStack;
        private ColumnDefinition emitterStackColumn;
        private Button refreshButton;
        private ToggleButton autoRefreshButton;
        private ToggleButton showEGButton;
        private ToggleButton showLEButton;
        private ToggleButton showUnknownButton;


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

            refreshButton = GetTemplateChild(PART_Refresh) as Button;
            refreshButton.Click += RefreshButton_Click;

            autoRefreshButton = GetTemplateChild(PART_AutoRefresh) as ToggleButton;

            showEGButton = GetTemplateChild(PART_ShowEG) as ToggleButton;
            showEGButton.Click += RefreshButton_Click;
            showLEButton = GetTemplateChild(PART_ShowLE) as ToggleButton;
            showLEButton.Click += RefreshButton_Click;

            showUnknownButton = GetTemplateChild(PART_ShowUnknown) as ToggleButton;
            showUnknownButton.Click += RefreshButton_Click;

            Loaded += EmitterDocumentEditor_Loaded;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            GetEmitterProcessors(asset.RootObject);
        }

        private void EmitterDocumentEditor_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic obj = asset.RootObject;

            GetEmitterProcessors(obj);

            pgAsset.Object = obj.Object.Internal;

            emitterStackColumn.Width = new GridLength(2, GridUnitType.Star);

            if (EmitterStackItems.Count == 0)
                DisableEditor();
        }

        void GetEmitterProcessors(dynamic obj)
        {
            if (!editor) return;
            
            EmitterStackItems.Clear();
            int count = 0;
            foreach (dynamic component in obj.Object.Internal.Components) {
                if (component.Internal == null) continue;

                if (component.Internal.GetType().Name == "EmitterGraphEntityData" && showEGButton.IsChecked) {
                    dynamic reference = App.AssetManager.GetEbxEntry(component.Internal.EmitterGraph.External.FileGuid);

                    EmitterStackItems.Add(new EmitterStackItemData(-1, null, pgAsset, null, $"[{count}] {component.Internal.__Id} - {reference.DisplayName}"));

                    Dictionary<int, string[]> vsfParams = new Dictionary<int, string[]>();
                    try {
                        dynamic eGraph = App.AssetManager.GetEbx(reference).RootObject;

                        dynamic[] egParams = eGraph.EmitterGraphParams.ToArray();

                        string vsf = App.AssetManager.GetEbx(String.IsNullOrEmpty(eGraph.MeshVertexShaderFragmentAssetName) ? eGraph.VertexShaderFragmentAssetName : eGraph.MeshVertexShaderFragmentAssetName).RootObject.PipelineGeneratedSourceCode;

                        string[] lines = vsf.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> vsfParamLines = lines.Where(l => l.Contains("g_emitterGraphParams[") && l.Contains("].xyzw")).ToList();

                        for (int i = 0; i < vsfParamLines.Count; i++) {
                            List<string> param = new List<string>();
                            foreach (string vsfParamLine in vsfParamLines[i].Split()[3].Split('_')) {
                                param.Add(char.ToUpper(vsfParamLine[0]) + vsfParamLine.Substring(1));
                            }
                            vsfParams.Add(egParams[i].PropertyId, param.ToArray());
                        }
                    }
                    catch { }

                    foreach (dynamic param in component.Internal.EmitterGraphParams) {
                        if (vsfParams.TryGetValue(param.PropertyId, out string[] _) || showUnknownButton.IsChecked) {
                            EmitterStackItems.Add(new EmitterStackItemData(param.PropertyId, param.Value, pgAsset, vsfParams));
                        }
                    }
                }

                if (component.Internal.GetType().Name == "LightEffectEntityData" && showLEButton.IsChecked) {
                    EmitterStackItems.Add(new EmitterStackItemData(-1, null, pgAsset, null, $"[{count}] {component.Internal.__Id}"));

                    EmitterStackItems.Add(new EmitterStackItemData(-1, component.Internal.Light.Internal.Color, pgAsset, new Dictionary<int, string[]> { { -1, new string[] { "Color" } } }));
                }
                count++;
            }
            refreshButton.IsEnabled = false;
        }

        #region -- Control Events --

        private void PgAsset_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (autoRefreshButton.IsChecked == true)
                GetEmitterProcessors(asset.RootObject);
            else
                refreshButton.IsEnabled = true;
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
                new ToolbarItem("Editor", "Editor View", "", new RelayCommand((object state) => { EnableEditor(); })),
                new ToolbarItem("Standard", "Standard View", "", new RelayCommand((object state) => { DisableEditor(); }))
            };
        }

        private void DisableEditor()
        {
            editor = false;
            pgAsset.Object = asset.RootObject;
            EmitterStackItems.Clear();
            emitterStackColumn.Width = new GridLength(0);
        }

        private void EnableEditor()
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
