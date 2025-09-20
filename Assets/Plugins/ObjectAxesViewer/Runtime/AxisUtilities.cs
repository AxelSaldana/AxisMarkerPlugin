using UnityEngine;
using System.Collections.Generic;

public static class AxisUtilities
{
    /// <summary>
    /// Agrega automaticamente marcadores de eje a todos los hijos de un GameObject
    /// </summary>
    public static ModelAxisManager SetupAxisMarkersForModel(GameObject rootObject, bool includeInactive = false)
    {
        if (rootObject == null)
        {
            UnityEngine.Debug.LogError("Root object is null!");
            return null;
        }
        
        // Verificar si ya tiene un ModelAxisManager
        ModelAxisManager manager = rootObject.GetComponent<ModelAxisManager>();
        if (manager == null)
        {
            manager = rootObject.AddComponent<ModelAxisManager>();
        }
        
        manager.rootModel = rootObject;
        manager.includeInactivePieces = includeInactive;
        manager.DetectModelPieces();
        
        return manager;
    }
    
    /// <summary>
    /// Crea un controlador de runtime para un modelo
    /// </summary>
    public static RuntimeAxisController CreateRuntimeController(ModelAxisManager manager, bool createUI = true)
    {
        if (manager == null)
        {
            UnityEngine.Debug.LogError("ModelAxisManager is null!");
            return null;
        }
        
        GameObject controllerGO = new GameObject("RuntimeAxisController");
        RuntimeAxisController controller = controllerGO.AddComponent<RuntimeAxisController>();
        
        controller.modelManager = manager;
        controller.createUIAutomatically = createUI;
        
        return controller;
    }
    
    /// <summary>
    /// Encuentra todos los ModelAxisManager en la escena
    /// </summary>
    public static List<ModelAxisManager> FindAllAxisManagers()
    {
        ModelAxisManager[] managers = Object.FindObjectsOfType<ModelAxisManager>();
        return new List<ModelAxisManager>(managers);
    }
    
    /// <summary>
    /// Configura colores personalizados para los ejes
    /// </summary>
    public static void SetCustomAxisColors(AxisMarkerImproved marker, Color xColor, Color yColor, Color zColor)
    {
        if (marker == null) return;
        
        marker.xAxisColor = xColor;
        marker.yAxisColor = yColor;
        marker.zAxisColor = zColor;
    }
    
    /// <summary>
    /// Aplica configuracion de eje a multiples marcadores
    /// </summary>
    public static void ApplyAxisConfiguration(List<AxisMarkerImproved> markers, float length, float thickness, bool showLabels)
    {
        foreach (var marker in markers)
        {
            if (marker != null)
            {
                marker.axisLength = length;
                marker.axisThickness = thickness;
                marker.showLabels = showLabels;
            }
        }
    }
    
    /// <summary>
    /// Crea un preset de configuracion de ejes
    /// </summary>
    [System.Serializable]
    public class AxisPreset
    {
        public string name;
        public float axisLength = 0.5f;
        public float axisThickness = 2f;
        public bool showLabels = true;
        public Color xAxisColor = Color.red;
        public Color yAxisColor = Color.green;
        public Color zAxisColor = Color.blue;
        public bool showXAxis = true;
        public bool showYAxis = true;
        public bool showZAxis = true;
        
        public void ApplyToMarker(AxisMarkerImproved marker)
        {
            if (marker == null) return;
            
            marker.axisLength = axisLength;
            marker.axisThickness = axisThickness;
            marker.showLabels = showLabels;
            marker.xAxisColor = xAxisColor;
            marker.yAxisColor = yAxisColor;
            marker.zAxisColor = zAxisColor;
            marker.showXAxis = showXAxis;
            marker.showYAxis = showYAxis;
            marker.showZAxis = showZAxis;
        }
    }
    
    /// <summary>
    /// Presets predefinidos
    /// </summary>
    public static class Presets
    {
        public static AxisPreset Small => new AxisPreset
        {
            name = "Small",
            axisLength = 0.2f,
            axisThickness = 1f,
            showLabels = false
        };
        
        public static AxisPreset Medium => new AxisPreset
        {
            name = "Medium",
            axisLength = 0.5f,
            axisThickness = 2f,
            showLabels = true
        };
        
        public static AxisPreset Large => new AxisPreset
        {
            name = "Large",
            axisLength = 1f,
            axisThickness = 3f,
            showLabels = true
        };
        
        public static AxisPreset XOnly => new AxisPreset
        {
            name = "X Only",
            showXAxis = true,
            showYAxis = false,
            showZAxis = false
        };
        
        public static AxisPreset YOnly => new AxisPreset
        {
            name = "Y Only",
            showXAxis = false,
            showYAxis = true,
            showZAxis = false
        };
        
        public static AxisPreset ZOnly => new AxisPreset
        {
            name = "Z Only",
            showXAxis = false,
            showYAxis = false,
            showZAxis = true
        };
    }
    
    /// <summary>
    /// Herramientas de debugging
    /// </summary>
    public static class DebugTools
    {
        public static void LogAxisManagerInfo(ModelAxisManager manager)
        {
            if (manager == null)
            {
                UnityEngine.Debug.Log("ModelAxisManager is null");
                return;
            }
            
            UnityEngine.Debug.Log($"=== Axis Manager Info: {manager.name} ===");
            UnityEngine.Debug.Log($"Root Model: {(manager.rootModel ? manager.rootModel.name : "None")}");
            UnityEngine.Debug.Log($"Total Pieces: {manager.ModelPieces.Count}");
            UnityEngine.Debug.Log($"Visible Pieces: {manager.GetVisiblePieces().Count}");
            UnityEngine.Debug.Log($"Hidden Pieces: {manager.GetHiddenPieces().Count}");
            UnityEngine.Debug.Log($"Global Axis Length: {manager.globalAxisLength}");
            UnityEngine.Debug.Log($"Global Axis Thickness: {manager.globalAxisThickness}");
            UnityEngine.Debug.Log($"Show Labels: {manager.showLabels}");
        }
        
        public static void LogAllPieces(ModelAxisManager manager)
        {
            if (manager == null) return;
            
            UnityEngine.Debug.Log($"=== All Pieces in {manager.name} ===");
            for (int i = 0; i < manager.ModelPieces.Count; i++)
            {
                var piece = manager.ModelPieces[i];
                UnityEngine.Debug.Log($"{i}: {piece.displayName} - Visible: {piece.isVisible}");
            }
        }
    }
}
