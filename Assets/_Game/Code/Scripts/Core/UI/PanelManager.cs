using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

namespace VinhLB
{
    public class PanelManager : PersistentMonoSingleton<PanelManager>
    {
        [SerializeField]
        private Transform _panelHolderTF;
        
        private readonly List<Panel> _panelList = new();
        
        public Type CurrentPanelType => _panelList.Count > 0 ? _panelList[^1].GetType() : null;
        
        public T GetPanel<T>(string panelName) where T : Panel
        {
            return (T)_panelList.Find(panel => panel.PanelName.Equals(panelName));
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCurrentPanel();
            }
        }
        
        public async UniTask<Panel> CreatePanel<T>(string panelName, bool canBack, Action<T> onSetup = null, bool autoOpen = true)
            where T : Panel
        {
            // Create panel async
            float startFrameCount = Time.frameCount;
            Stopwatch stopwatch = Stopwatch.StartNew();
            T panel = (await Addressables.InstantiateAsync(panelName, _panelHolderTF)).GetComponent<T>();
            stopwatch.Stop();
            Debug.Log($"[{nameof(PanelManager)}] Created {panelName} in {stopwatch.ElapsedMilliseconds}ms " +
                      $"(frame: {Time.frameCount - startFrameCount})");
            
            // Setup panel
            panel.Initialize(panelName, canBack);
            _panelList.Add(panel);
            
            // Invoke setup callback
            onSetup?.Invoke(panel);
            
            // Play open animation
            if (autoOpen)
            {
                panel.Open();
            }
            
            return panel;
        }
        
        public async UniTask OpenPanel(string panelName, bool waitOpenCompleted = false)
        {
            // Find panel of type by name
            Panel panel = _panelList.Find(panel => panel.PanelName.Equals(panelName));
            if (panel == null)
            {
                Debug.LogWarning($"[{nameof(PanelManager)}] Cannot find panel {panelName}");
                return;
            }

            // Play open animation
            panel.Open();
            
            // Wait until close completed
            if (waitOpenCompleted)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(panel.OpenAnimDuration));
            }
        }
        
        public async UniTask ClosePanel(string panelName, bool immediately = false, bool waitCloseCompleted = false)
        {
            // Find panel of type by name
            Panel panel = _panelList.Find(panel => panel.PanelName.Equals(panelName));
            if (panel == null)
            {
                Debug.LogWarning($"[{nameof(PanelManager)}] Cannot find panel {panelName}");
                return;
            }
            
            // Play close animation (if not immediately)
            if (immediately)
            {
                panel.CloseImmediately();
            }
            else
            {
                panel.Close();
            }
            
            // Wait until close completed
            if (waitCloseCompleted)
            {
                await UniTask.WaitUntil(() => panel == null);
            }
        }
        
        public void ReleasePanel(Panel releasedPanel)
        {
            Debug.Log($"[{nameof(PanelManager)}] Released {releasedPanel.PanelName}");
            _panelList.Remove(releasedPanel);
        }
        
        private void CloseCurrentPanel()
        {
            if (_panelList.Count == 0)
            {
                Debug.LogWarning($"[{nameof(PanelManager)} Stack is empty]");
                return;
            }
            
            if (!_panelList.Last().CanBack)
            {
                Debug.LogWarning($"[{nameof(PanelManager)} Cannot back]");
                return;
            }
            
            Debug.Log($"[{nameof(PanelManager)}] Close {CurrentPanelType.Name}");
            Panel panelInTop = _panelList.Last();
            _panelList.Remove(panelInTop);
            
            panelInTop.OnCloseButton();
        }
    }
}