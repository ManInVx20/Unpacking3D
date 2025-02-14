using System;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class PanelManager : PersistentMonoSingleton<PanelManager>
    {
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
                TryCloseCurrentPanel();
            }
        }
        
        private void TryCloseCurrentPanel()
        {
            
        }
    }
}