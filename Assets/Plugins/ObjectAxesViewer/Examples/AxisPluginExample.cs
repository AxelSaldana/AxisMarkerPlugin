using UnityEngine;
using System.Collections;

/// <summary>
/// Script de ejemplo que demuestra como usar el Advanced Axis Marker Plugin
/// Agrega este script a cualquier GameObject para ver ejemplos de uso
/// </summary>
public class AxisPluginExample : MonoBehaviour
{
    [Header("Example Settings")]
    public GameObject targetModel;
    public bool setupOnStart = true;
    public bool createRuntimeUI = true;
    
    [Header("Keyboard Controls")]
    public KeyCode toggleAllKey = KeyCode.Space;
    public KeyCode showAllKey = KeyCode.Alpha1;
    public KeyCode hideAllKey = KeyCode.Alpha2;
    public KeyCode cyclePresetsKey = KeyCode.Tab;
    
    private ModelAxisManager axisManager;
    private RuntimeAxisController runtimeController;
    private int currentPresetIndex = 0;
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupAxisPlugin();
        }
        
        // Mostrar instrucciones en consola
        ShowInstructions();
    }
    
    private void Update()
    {
        HandleKeyboardInput();
    }
    
    /// <summary>
    /// Configura automaticamente el plugin para el modelo objetivo
    /// </summary>
    public void SetupAxisPlugin()
    {
        if (targetModel == null)
        {
            Debug.LogWarning("Target model no asignado! Usando este GameObject como modelo.");
            targetModel = gameObject;
        }
        
        // Configurar el ModelAxisManager
        axisManager = AxisUtilities.SetupAxisMarkersForModel(targetModel, true);
        
        if (axisManager != null)
        {
            Debug.Log($"Plugin configurado exitosamente para {targetModel.name}");
            Debug.Log($"Piezas detectadas: {axisManager.ModelPieces.Count}");
            
            // Configurar el controlador de runtime si se solicita
            if (createRuntimeUI)
            {
                runtimeController = AxisUtilities.CreateRuntimeController(axisManager, true);
                Debug.Log("Interfaz de runtime creada");
            }
        }
        else
        {
            Debug.LogError("Error al configurar el plugin");
        }
    }
    
    /// <summary>
    /// Maneja la entrada de teclado para controlar los ejes
    /// </summary>
    private void HandleKeyboardInput()
    {
        if (axisManager == null) return;
        
        // Toggle general
        if (Input.GetKeyDown(toggleAllKey))
        {
            axisManager.ToggleAllAxes();
            Debug.Log("Toggled all axes");
        }
        
        // Mostrar todos
        if (Input.GetKeyDown(showAllKey))
        {
            axisManager.ShowAllAxes();
            Debug.Log("Showing all axes");
        }
        
        // Ocultar todos
        if (Input.GetKeyDown(hideAllKey))
        {
            axisManager.HideAllAxes();
            Debug.Log("Hiding all axes");
        }
        
        // Cambiar presets
        if (Input.GetKeyDown(cyclePresetsKey))
        {
            CyclePresets();
        }
        
        // Control numerico de piezas individuales (1-9)
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                TogglePieceByIndex(i - 1);
            }
        }
    }
    
    /// <summary>
    /// Cambia entre diferentes presets de configuracion
    /// </summary>
    public void CyclePresets()
    {
        if (axisManager == null) return;
        
        var presets = new AxisUtilities.AxisPreset[]
        {
            AxisUtilities.Presets.Small,
            AxisUtilities.Presets.Medium,
            AxisUtilities.Presets.Large,
            AxisUtilities.Presets.XOnly,
            AxisUtilities.Presets.YOnly,
            AxisUtilities.Presets.ZOnly
        };
        
        currentPresetIndex = (currentPresetIndex + 1) % presets.Length;
        var preset = presets[currentPresetIndex];
        
        // Aplicar preset a todas las piezas
        foreach (var piece in axisManager.ModelPieces)
        {
            if (piece.axisMarker != null)
            {
                preset.ApplyToMarker(piece.axisMarker);
            }
        }
        
        Debug.Log($"Applied preset: {preset.name}");
    }
    
    /// <summary>
    /// Toggle de una pieza especifica por indice
    /// </summary>
    public void TogglePieceByIndex(int index)
    {
        if (axisManager == null || index >= axisManager.ModelPieces.Count) return;
        
        var piece = axisManager.ModelPieces[index];
        piece.isVisible = !piece.isVisible;
        
        if (piece.axisMarker != null)
        {
            piece.axisMarker.showAxis = piece.isVisible;
        }
        
        Debug.Log($"Toggled piece {index}: {piece.displayName} - {(piece.isVisible ? "Visible" : "Hidden")}");
    }
    
    /// <summary>
    /// Ejemplo de configuracion personalizada
    /// </summary>
    [ContextMenu("Apply Custom Configuration")]
    public void ApplyCustomConfiguration()
    {
        if (axisManager == null) return;
        
        // Configuracion personalizada
        axisManager.globalAxisLength = 150f;
        axisManager.globalAxisThickness = 3f;
        axisManager.showLabels = true;
        axisManager.UpdateGlobalSettings();
        
        // Colores personalizados para algunas piezas
        for (int i = 0; i < axisManager.ModelPieces.Count; i++)
        {
            var piece = axisManager.ModelPieces[i];
            if (piece.axisMarker != null)
            {
                // Alternar colores
                if (i % 2 == 0)
                {
                    AxisUtilities.SetCustomAxisColors(piece.axisMarker, 
                        Color.cyan, Color.yellow, Color.magenta);
                }
                else
                {
                    AxisUtilities.SetCustomAxisColors(piece.axisMarker, 
                        Color.red, Color.green, Color.blue);
                }
            }
        }
        
        Debug.Log("Applied custom configuration");
    }
    
    /// <summary>
    /// Ejemplo de animacion de ejes
    /// </summary>
    [ContextMenu("Animate Axes")]
    public void AnimateAxes()
    {
        if (axisManager != null)
        {
            StartCoroutine(AnimateAxesCoroutine());
        }
    }
    
    private IEnumerator AnimateAxesCoroutine()
    {
        Debug.Log("Starting axis animation...");
        
        float duration = 3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float size = Mathf.Lerp(0.2f, 1.5f, Mathf.PingPong(t * 2f, 1f));
            
            axisManager.globalAxisLength = size;
            axisManager.UpdateGlobalSettings();
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Restaurar tamano normal
        axisManager.globalAxisLength = 150f;
        axisManager.UpdateGlobalSettings();
        
        Debug.Log("Animation completed");
    }
    
    /// <summary>
    /// Ejemplo de configuracion por tipo de objeto
    /// </summary>
    [ContextMenu("Configure By Object Type")]
    public void ConfigureByObjectType()
    {
        if (axisManager == null) return;
        
        foreach (var piece in axisManager.ModelPieces)
        {
            if (piece.gameObject == null || piece.axisMarker == null) continue;
            
            // Configurar segun el tipo de componente
            if (piece.gameObject.GetComponent<Renderer>() != null)
            {
                // Objetos con renderer - ejes normales
                piece.axisMarker.axisLength = 150f;
                piece.axisMarker.SetAxisVisibility(true, true, true);
            }
            
            if (piece.gameObject.GetComponent<Collider>() != null)
            {
                // Objetos con collider - ejes mas gruesos
                piece.axisMarker.axisThickness = 4f;
            }
            
            if (piece.gameObject.GetComponent<Rigidbody>() != null)
            {
                // Objetos con rigidbody - solo eje Y (gravedad)
                piece.axisMarker.SetAxisVisibility(false, true, false);
                piece.axisMarker.yAxisColor = Color.yellow;
            }
            
            // Configurar por nombre
            if (piece.displayName.ToLower().Contains("wheel") || 
                piece.displayName.ToLower().Contains("rueda"))
            {
                // Ruedas - solo eje de rotacion
                piece.axisMarker.SetAxisVisibility(false, false, true);
                piece.axisMarker.zAxisColor = Color.cyan;
            }
        }
        
        Debug.Log("Configuration by object type applied");
    }
    
    /// <summary>
    /// Muestra las instrucciones de uso en la consola
    /// </summary>
    private void ShowInstructions()
    {
        Debug.Log("=== AXIS PLUGIN EXAMPLE - INSTRUCTIONS ===");
        Debug.Log($"Press {toggleAllKey} to toggle all axes");
        Debug.Log($"Press {showAllKey} to show all axes");
        Debug.Log($"Press {hideAllKey} to hide all axes");
        Debug.Log($"Press {cyclePresetsKey} to cycle through presets");
        Debug.Log("Press 1-9 to toggle individual pieces");
        Debug.Log("Right-click this script and use Context Menu for more examples");
        Debug.Log("==========================================");
    }
    
    /// <summary>
    /// Limpia todos los marcadores de eje
    /// </summary>
    [ContextMenu("Cleanup Axis Markers")]
    public void CleanupAxisMarkers()
    {
        if (axisManager != null)
        {
            axisManager.RemoveAllAxisMarkers();
            Debug.Log("All axis markers removed");
        }
    }
    
    // Metodos publicos para usar desde UI o otros scripts
    public void ShowAllAxes() => axisManager?.ShowAllAxes();
    public void HideAllAxes() => axisManager?.HideAllAxes();
    public void ToggleAllAxes() => axisManager?.ToggleAllAxes();
    public void ToggleUI() => runtimeController?.ToggleUI();
    
    // Propiedades publicas para inspeccion
    public ModelAxisManager AxisManager => axisManager;
    public RuntimeAxisController RuntimeController => runtimeController;
    public int PieceCount => axisManager?.ModelPieces.Count ?? 0;
    public int VisiblePieceCount => axisManager?.GetVisiblePieces().Count ?? 0;
}
