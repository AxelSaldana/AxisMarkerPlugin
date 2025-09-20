using UnityEngine;
using UnityEditor;

/// <summary>
/// Menus contextuales y de herramientas para el plugin de ejes
/// </summary>
public static class AxisPluginMenus
{
    private const string MENU_ROOT = "GameObject/Axis Plugin/";
    private const string TOOLS_ROOT = "Tools/Axis Plugin/";
    
    // Menus del GameObject
    [MenuItem(MENU_ROOT + "Add Axis Marker", false, 0)]
    private static void AddAxisMarker()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var marker = selected.GetComponent<AxisMarkerImproved>();
            if (marker == null)
            {
                marker = selected.AddComponent<AxisMarkerImproved>();
                AxisPluginSettings.Instance.ApplyToMarker(marker);
                EditorUtility.SetDirty(selected);
                Debug.Log($"Axis marker agregado a {selected.name}");
            }
            else
            {
                Debug.LogWarning($"{selected.name} ya tiene un AxisMarkerImproved");
            }
        }
        else
        {
            Debug.LogWarning("Selecciona un GameObject primero");
        }
    }
    
    [MenuItem(MENU_ROOT + "Setup Model Axis Manager", false, 1)]
    private static void SetupModelAxisManager()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var manager = AxisUtilities.SetupAxisMarkersForModel(selected, true);
            if (manager != null)
            {
                AxisPluginSettings.Instance.ApplyToManager(manager);
                Selection.activeGameObject = selected;
                Debug.Log($"ModelAxisManager configurado para {selected.name} con {manager.ModelPieces.Count} piezas");
            }
        }
        else
        {
            Debug.LogWarning("Selecciona un GameObject primero");
        }
    }
    
    [MenuItem(MENU_ROOT + "Add Runtime Controller", false, 2)]
    private static void AddRuntimeController()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var manager = selected.GetComponent<ModelAxisManager>();
            if (manager != null)
            {
                var controller = AxisUtilities.CreateRuntimeController(manager, true);
                if (controller != null)
                {
                    Debug.Log($"RuntimeAxisController agregado para {selected.name}");
                }
            }
            else
            {
                Debug.LogWarning($"{selected.name} necesita un ModelAxisManager primero");
            }
        }
        else
        {
            Debug.LogWarning("Selecciona un GameObject con ModelAxisManager primero");
        }
    }
    
    [MenuItem(MENU_ROOT + "Remove All Axis Markers", false, 10)]
    private static void RemoveAllAxisMarkers()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var manager = selected.GetComponent<ModelAxisManager>();
            if (manager != null)
            {
                if (EditorUtility.DisplayDialog("Confirmar", 
                    $"¿Eliminar todos los marcadores de eje de {selected.name}?", 
                    "Si", "No"))
                {
                    manager.RemoveAllAxisMarkers();
                    Debug.Log($"Marcadores de eje eliminados de {selected.name}");
                }
            }
            else
            {
                // Buscar y eliminar marcadores individuales
                var markers = selected.GetComponentsInChildren<AxisMarkerImproved>();
                if (markers.Length > 0)
                {
                    if (EditorUtility.DisplayDialog("Confirmar", 
                        $"¿Eliminar {markers.Length} marcadores de eje encontrados?", 
                        "Si", "No"))
                    {
                        foreach (var marker in markers)
                        {
                            UnityEditor.EditorApplication.delayCall += () => {
                                if (marker != null)
                                    UnityEngine.Object.DestroyImmediate(marker);
                            };
                        }
                        Debug.Log($"{markers.Length} marcadores eliminados");
                    }
                }
                else
                {
                    Debug.LogWarning("No se encontraron marcadores de eje");
                }
            }
        }
    }
    
    // Menus de herramientas
    [MenuItem(TOOLS_ROOT + "Open Axis Manager Window", false, 0)]
    private static void OpenAxisManagerWindow()
    {
        ImprovedAxisManagerWindow.ShowWindow();
    }
    
    [MenuItem(TOOLS_ROOT + "Create Settings Asset", false, 1)]
    private static void CreateSettingsAsset()
    {
        var settings = ScriptableObject.CreateInstance<AxisPluginSettings>();
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Guardar Configuracion del Plugin",
            "AxisPluginSettings",
            "asset",
            "Selecciona donde guardar la configuracion");
        
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = settings;
            Debug.Log($"Configuracion creada en: {path}");
        }
    }
    
    [MenuItem(TOOLS_ROOT + "Find All Axis Managers", false, 2)]
    private static void FindAllAxisManagers()
    {
        var managers = AxisUtilities.FindAllAxisManagers();
        
        Debug.Log($"=== AXIS MANAGERS ENCONTRADOS ({managers.Count}) ===");
        
        foreach (var manager in managers)
        {
            AxisUtilities.DebugTools.LogAxisManagerInfo(manager);
        }
        
        if (managers.Count > 0)
        {
            // Seleccionar todos los managers encontrados
            Selection.objects = managers.ConvertAll(m => m.gameObject).ToArray();
        }
    }
    
    [MenuItem(TOOLS_ROOT + "Cleanup All Axis Markers in Scene", false, 10)]
    private static void CleanupAllAxisMarkersInScene()
    {
        if (EditorUtility.DisplayDialog("Confirmar Limpieza", 
            "¿Eliminar TODOS los marcadores de eje de la escena actual?", 
            "Si", "No"))
        {
            var allMarkers = Object.FindObjectsOfType<AxisMarkerImproved>();
            var allManagers = Object.FindObjectsOfType<ModelAxisManager>();
            
            // Eliminar a traves de managers primero
            foreach (var manager in allManagers)
            {
                manager.RemoveAllAxisMarkers();
            }
            
            // Eliminar marcadores sueltos
            foreach (var marker in allMarkers)
            {
                if (marker != null)
                {
                    UnityEngine.Object.DestroyImmediate(marker);
                }
            }
            
            Debug.Log($"Limpieza completada: {allMarkers.Length} marcadores y {allManagers.Length} managers procesados");
        }
    }
    
    // Validaciones de menu
    [MenuItem(MENU_ROOT + "Add Axis Marker", true)]
    private static bool ValidateAddAxisMarker()
    {
        return Selection.activeGameObject != null;
    }
    
    [MenuItem(MENU_ROOT + "Setup Model Axis Manager", true)]
    private static bool ValidateSetupModelAxisManager()
    {
        return Selection.activeGameObject != null;
    }
    
    [MenuItem(MENU_ROOT + "Add Runtime Controller", true)]
    private static bool ValidateAddRuntimeController()
    {
        var selected = Selection.activeGameObject;
        return selected != null && selected.GetComponent<ModelAxisManager>() != null;
    }
    
    [MenuItem(MENU_ROOT + "Remove All Axis Markers", true)]
    private static bool ValidateRemoveAllAxisMarkers()
    {
        var selected = Selection.activeGameObject;
        if (selected == null) return false;
        
        var manager = selected.GetComponent<ModelAxisManager>();
        if (manager != null) return true;
        
        var markers = selected.GetComponentsInChildren<AxisMarkerImproved>();
        return markers.Length > 0;
    }
    
    // Shortcuts de teclado
    [MenuItem(TOOLS_ROOT + "Toggle All Axes in Scene %&t", false, 20)]
    private static void ToggleAllAxesInScene()
    {
        var managers = AxisUtilities.FindAllAxisManagers();
        foreach (var manager in managers)
        {
            manager.ToggleAllAxes();
        }
        
        if (managers.Count > 0)
        {
            Debug.Log($"Toggled axes for {managers.Count} managers");
        }
        else
        {
            Debug.LogWarning("No se encontraron ModelAxisManager en la escena");
        }
    }
    
    [MenuItem(TOOLS_ROOT + "Show All Axes in Scene %&s", false, 21)]
    private static void ShowAllAxesInScene()
    {
        var managers = AxisUtilities.FindAllAxisManagers();
        foreach (var manager in managers)
        {
            manager.ShowAllAxes();
        }
        
        Debug.Log($"Showing axes for {managers.Count} managers");
    }
    
    [MenuItem(TOOLS_ROOT + "Hide All Axes in Scene %&h", false, 22)]
    private static void HideAllAxesInScene()
    {
        var managers = AxisUtilities.FindAllAxisManagers();
        foreach (var manager in managers)
        {
            manager.HideAllAxes();
        }
        
        Debug.Log($"Hiding axes for {managers.Count} managers");
    }
}
