using UnityEngine;

/// <summary>
/// Configuracion global del plugin de ejes
/// Permite guardar y cargar configuraciones predeterminadas
/// </summary>
[CreateAssetMenu(fileName = "AxisPluginSettings", menuName = "Axis Plugin/Settings")]
public class AxisPluginSettings : ScriptableObject
{
    [Header("Default Axis Settings")]
    [Range(1f, 150f)]
    public float defaultAxisLength = 150f;
    [Range(1f, 150f)]
    public float defaultAxisThickness = 2f;
    public bool defaultShowLabels = true;
    public float defaultLabelOffset = 0.1f;
    
    [Header("Default Colors")]
    public Color defaultXAxisColor = Color.red;
    public Color defaultYAxisColor = Color.green;
    public Color defaultZAxisColor = Color.blue;
    
    [Header("Default Visibility")]
    public bool defaultShowXAxis = true;
    public bool defaultShowYAxis = true;
    public bool defaultShowZAxis = true;
    public bool defaultShowAxis = true;
    
    [Header("Detection Settings")]
    public bool defaultFilterByRenderer = true;
    public bool defaultFilterByCollider = false;
    public bool defaultIncludeInactive = false;
    public bool defaultAutoDetectOnStart = true;
    
    [Header("Runtime UI Settings")]
    public bool defaultCreateRuntimeUI = true;
    public bool defaultShowUIOnStart = true;
    
    [Header("Performance Settings")]
    public int maxMarkersPerModel = 100;
    public bool enableBatching = true;
    public float updateInterval = 0.1f;
    
    private static AxisPluginSettings _instance;
    
    /// <summary>
    /// Instancia singleton de la configuracion
    /// </summary>
    public static AxisPluginSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AxisPluginSettings>("AxisPluginSettings");
                if (_instance == null)
                {
                    _instance = CreateInstance<AxisPluginSettings>();
                    Debug.LogWarning("No se encontro AxisPluginSettings en Resources. Usando configuracion por defecto.");
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Aplica la configuracion por defecto a un AxisMarkerImproved
    /// </summary>
    public void ApplyToMarker(AxisMarkerImproved marker)
    {
        if (marker == null) return;
        
        marker.axisLength = defaultAxisLength;
        marker.axisThickness = defaultAxisThickness;
        marker.showLabels = defaultShowLabels;
        marker.labelOffset = defaultLabelOffset;
        
        marker.xAxisColor = defaultXAxisColor;
        marker.yAxisColor = defaultYAxisColor;
        marker.zAxisColor = defaultZAxisColor;
        
        marker.showXAxis = defaultShowXAxis;
        marker.showYAxis = defaultShowYAxis;
        marker.showZAxis = defaultShowZAxis;
        marker.showAxis = defaultShowAxis;
    }
    
    /// <summary>
    /// Aplica la configuracion por defecto a un ModelAxisManager
    /// </summary>
    public void ApplyToManager(ModelAxisManager manager)
    {
        if (manager == null) return;
        
        manager.globalAxisLength = defaultAxisLength;
        manager.globalAxisThickness = defaultAxisThickness;
        manager.showLabels = defaultShowLabels;
        
        manager.filterByRenderer = defaultFilterByRenderer;
        manager.filterByCollider = defaultFilterByCollider;
        manager.includeInactivePieces = defaultIncludeInactive;
        manager.autoDetectPieces = defaultAutoDetectOnStart;
    }
    
    /// <summary>
    /// Crea un preset personalizado basado en la configuracion actual
    /// </summary>
    public AxisUtilities.AxisPreset CreatePreset(string name = "Custom")
    {
        return new AxisUtilities.AxisPreset
        {
            name = name,
            axisLength = defaultAxisLength,
            axisThickness = defaultAxisThickness,
            showLabels = defaultShowLabels,
            xAxisColor = defaultXAxisColor,
            yAxisColor = defaultYAxisColor,
            zAxisColor = defaultZAxisColor,
            showXAxis = defaultShowXAxis,
            showYAxis = defaultShowYAxis,
            showZAxis = defaultShowZAxis
        };
    }
    
    /// <summary>
    /// Valida que la configuracion este en rangos aceptables
    /// </summary>
    private void OnValidate()
    {
        defaultAxisLength = Mathf.Clamp(defaultAxisLength, 0.1f, 150f);
        defaultAxisThickness = Mathf.Clamp(defaultAxisThickness, 1f, 150f);
        defaultLabelOffset = Mathf.Clamp(defaultLabelOffset, 0f, 1f);
        maxMarkersPerModel = Mathf.Clamp(maxMarkersPerModel, 1, 1000);
        updateInterval = Mathf.Clamp(updateInterval, 0.01f, 1f);
    }
    
    /// <summary>
    /// Resetea la configuracion a valores por defecto
    /// </summary>
    [ContextMenu("Reset to Defaults")]
    public void ResetToDefaults()
    {
        defaultAxisLength = 0.5f;
        defaultAxisThickness = 2f;
        defaultShowLabels = true;
        defaultLabelOffset = 0.1f;
        
        defaultXAxisColor = Color.red;
        defaultYAxisColor = Color.green;
        defaultZAxisColor = Color.blue;
        
        defaultShowXAxis = true;
        defaultShowYAxis = true;
        defaultShowZAxis = true;
        defaultShowAxis = true;
        
        defaultFilterByRenderer = true;
        defaultFilterByCollider = false;
        defaultIncludeInactive = false;
        defaultAutoDetectOnStart = true;
        
        defaultCreateRuntimeUI = true;
        defaultShowUIOnStart = true;
        
        maxMarkersPerModel = 100;
        enableBatching = true;
        updateInterval = 0.1f;
    }
}
