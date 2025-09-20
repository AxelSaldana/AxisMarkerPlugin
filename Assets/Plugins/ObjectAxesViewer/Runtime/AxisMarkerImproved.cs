using UnityEngine;

[ExecuteAlways]
public class AxisMarkerImproved : MonoBehaviour
{
    [Header("Axis Settings")]
    public bool showAxis = true;
    [Range(1f, 150f)]// Activar/desactivar eje individual
    public float axisLength = 150f;     // Tamano del eje
    [Range(1f, 150f)]
    public float axisThickness = 2f;    // Grosor de las lineas (1-150)
    
    [Header("Individual Axis Control")]
    public bool showXAxis = true;       // Mostrar eje X (Rojo)
    public bool showYAxis = true;       // Mostrar eje Y (Verde)
    public bool showZAxis = true;       // Mostrar eje Z (Azul)
    
    [Header("Colors")]
    public Color xAxisColor = Color.red;
    public Color yAxisColor = Color.green;
    public Color zAxisColor = Color.blue;
    
    [Header("Labels")   ]
    public bool showLabels = true;
    public float labelOffset = 0.1f;
    
    [Header("Positioning")]
    public bool useObjectCenter = true;      // Usar centro del objeto (bounds) en lugar de transform.position
    
    [Header("Runtime Rendering")]
    public bool useRuntimeRendering = true;  // Usar LineRenderers en runtime
    public bool showInPlayMode = true;       // Mostrar en modo Play
    public Material axisMaterial;            // Material para los LineRenderers
    
    // Variables privadas para runtime rendering
    private LineRenderer xAxisLineRenderer;
    private LineRenderer yAxisLineRenderer;
    private LineRenderer zAxisLineRenderer;
    private GameObject axisContainer;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool runtimeRenderersCreated = false;
    
    private void Start()
    {
        if (useRuntimeRendering && Application.isPlaying)
        {
            CreateRuntimeRenderers();
        }
    }
    
    private void Update()
    {
        if (useRuntimeRendering && Application.isPlaying && showInPlayMode)
        {
            // Verificar si necesitamos crear los renderers
            if (!runtimeRenderersCreated)
            {
                CreateRuntimeRenderers();
            }
            
            // Actualizar solo si la posición o rotación cambió
            if (transform.position != lastPosition || transform.rotation != lastRotation)
            {
                UpdateRuntimeRenderers();
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
        }
        else if (runtimeRenderersCreated && (!showInPlayMode || !showAxis))
        {
            // Ocultar renderers si no deberían mostrarse
            SetRuntimeRenderersVisibility(false);
        }
        else if (runtimeRenderersCreated && showInPlayMode && showAxis)
        {
            // Mostrar renderers si deberían mostrarse
            SetRuntimeRenderersVisibility(true);
        }
    }
    
    private void OnDestroy()
    {
        CleanupRuntimeRenderers();
    }
    
    private void OnDrawGizmos()
    {
        if (!showAxis) return;
        
        if (debugMode)
        {
            // Dibujar informaciÃ³n de debug
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.2f, 
                $"{gameObject.name}\nPos: {transform.position:F2}");
            #endif
        }
        
        DrawAxes();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showAxis) return;
        
        // Dibujar con mayor intensidad cuando esta seleccionado
        Color originalX = xAxisColor;
        Color originalY = yAxisColor;
        Color originalZ = zAxisColor;
        
        xAxisColor = Color.Lerp(xAxisColor, Color.white, 0.3f);
        yAxisColor = Color.Lerp(yAxisColor, Color.white, 0.3f);
        zAxisColor = Color.Lerp(zAxisColor, Color.white, 0.3f);
        
        DrawAxes();
        
        // Restaurar colores originales
        xAxisColor = originalX;
        yAxisColor = originalY;
        zAxisColor = originalZ;
    }
    
    private void DrawAxes()
    {
        // Usar el centro visual del objeto (igual que en runtime)
        Vector3 position = GetObjectCenter();
        
        // Eje X (Rojo) - Usar la rotaciÃ³n del objeto para los ejes locales
        if (showXAxis)
        {
            Gizmos.color = xAxisColor;
            Vector3 xDirection = transform.right * axisLength;
            DrawThickLine(position, position + xDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + xDirection + transform.right * labelOffset;
                DrawLabel(labelPos, "X");
            }
        }
        
        // Eje Y (Verde) - Usar la rotaciÃ³n del objeto para los ejes locales
        if (showYAxis)
        {
            Gizmos.color = yAxisColor;
            Vector3 yDirection = transform.up * axisLength;
            DrawThickLine(position, position + yDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + yDirection + transform.up * labelOffset;
                DrawLabel(labelPos, "Y");
            }
        }
        
        // Eje Z (Azul) - Usar la rotaciÃ³n del objeto para los ejes locales
        if (showZAxis)
        {
            Gizmos.color = zAxisColor;
            Vector3 zDirection = transform.forward * axisLength;
            DrawThickLine(position, position + zDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + zDirection + transform.forward * labelOffset;
                DrawLabel(labelPos, "Z");
            }
        }
        
        // Dibujar punto central en la posiciÃ³n del objeto
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, axisLength * 0.05f);
    }
    
    private void DrawThickLine(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        
        // Calcular vectores perpendiculares para el grosor
        Vector3 perpendicular1, perpendicular2;
        
        // Elegir el mejor vector perpendicular basado en la direccion
        if (Mathf.Abs(Vector3.Dot(direction, Vector3.up)) < 0.9f)
        {
            perpendicular1 = Vector3.Cross(direction, Vector3.up).normalized;
        }
        else
        {
            perpendicular1 = Vector3.Cross(direction, Vector3.right).normalized;
        }
        
        perpendicular2 = Vector3.Cross(direction, perpendicular1).normalized;
        
        // Escalar el grosor basado en el valor de axisThickness
        float thickness = axisThickness * 0.002f; // Escalado mejorado
        
        // Linea principal
        Gizmos.DrawLine(start, end);
        
        // Sistema de lineas multiples para grosor variable optimizado
        if (axisThickness > 1f)
        {
            // Optimizar el numero de lineas basado en el grosor
            int lineCount;
            if (axisThickness <= 10f)
                lineCount = Mathf.RoundToInt(axisThickness);
            else if (axisThickness <= 50f)
                lineCount = 10 + Mathf.RoundToInt((axisThickness - 10f) * 0.5f); // Menos densidad
            else
                lineCount = 30 + Mathf.RoundToInt((axisThickness - 50f) * 0.2f); // Aun menos densidad
            
            lineCount = Mathf.Clamp(lineCount, 2, 50); // Maximo 50 lineas para rendimiento
            
            for (int i = 1; i <= lineCount; i++)
            {
                float offset = (thickness * i) / lineCount;
                
                // Lineas en cruz para mejor cobertura
                Gizmos.DrawLine(start + perpendicular1 * offset, end + perpendicular1 * offset);
                Gizmos.DrawLine(start - perpendicular1 * offset, end - perpendicular1 * offset);
                Gizmos.DrawLine(start + perpendicular2 * offset, end + perpendicular2 * offset);
                Gizmos.DrawLine(start - perpendicular2 * offset, end - perpendicular2 * offset);
                
                // Lineas diagonales para mayor densidad en grosores altos
                if (axisThickness > 5f)
                {
                    Vector3 diagonal1 = (perpendicular1 + perpendicular2).normalized * offset;
                    Vector3 diagonal2 = (perpendicular1 - perpendicular2).normalized * offset;
                    
                    Gizmos.DrawLine(start + diagonal1, end + diagonal1);
                    Gizmos.DrawLine(start - diagonal1, end - diagonal1);
                    Gizmos.DrawLine(start + diagonal2, end + diagonal2);
                    Gizmos.DrawLine(start - diagonal2, end - diagonal2);
                }
                
                // Lineas adicionales para grosores extremos
                if (axisThickness > 20f && i % 2 == 0)
                {
                    Vector3 extraOffset1 = perpendicular1 * offset * 0.5f + perpendicular2 * offset * 0.5f;
                    Vector3 extraOffset2 = perpendicular1 * offset * 0.5f - perpendicular2 * offset * 0.5f;
                    
                    Gizmos.DrawLine(start + extraOffset1, end + extraOffset1);
                    Gizmos.DrawLine(start - extraOffset1, end - extraOffset1);
                    Gizmos.DrawLine(start + extraOffset2, end + extraOffset2);
                    Gizmos.DrawLine(start - extraOffset2, end - extraOffset2);
                }
            }
        }
        
        // Punta de flecha mejorada y escalada
        float arrowSize = axisLength * 0.15f;
        float arrowWidth = thickness * 10f + axisLength * 0.03f;
        
        Vector3 arrowBase = end - direction * arrowSize;
        Vector3 arrowHead1 = arrowBase + perpendicular1 * arrowWidth;
        Vector3 arrowHead2 = arrowBase - perpendicular1 * arrowWidth;
        Vector3 arrowHead3 = arrowBase + perpendicular2 * arrowWidth;
        Vector3 arrowHead4 = arrowBase - perpendicular2 * arrowWidth;
        
        // Dibujar punta de flecha
        Gizmos.DrawLine(end, arrowHead1);
        Gizmos.DrawLine(end, arrowHead2);
        Gizmos.DrawLine(end, arrowHead3);
        Gizmos.DrawLine(end, arrowHead4);
        
        // Lineas adicionales para puntas de flecha mas gruesas
        if (axisThickness > 3f)
        {
            Gizmos.DrawLine(arrowHead1, arrowHead2);
            Gizmos.DrawLine(arrowHead3, arrowHead4);
        }
    }
    
    private void DrawLabel(Vector3 position, string text)
    {
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
        #endif
    }
    
    // Metodos publicos para control programatico
    public void ToggleAxis()
    {
        showAxis = !showAxis;
    }
    
    public void ToggleXAxis()
    {
        showXAxis = !showXAxis;
    }
    
    public void ToggleYAxis()
    {
        showYAxis = !showYAxis;
    }
    
    public void ToggleZAxis()
    {
        showZAxis = !showZAxis;
    }
    
    public void SetAxisVisibility(bool x, bool y, bool z)
    {
        showXAxis = x;
        showYAxis = y;
        showZAxis = z;
        
        // Recrear renderers si están activos para reflejar los cambios
        if (runtimeRenderersCreated && Application.isPlaying)
        {
            RecreateRuntimeRenderers();
        }
    }
    
    // MÃ©todo para debug - verificar la posiciÃ³n actual
    public void DebugPosition()
    {
        Debug.Log($"AxisMarker en objeto '{gameObject.name}':");
        Debug.Log($"  - Transform Position: {transform.position:F3}");
        Debug.Log($"  - Object Center: {GetObjectCenter():F3}");
        Debug.Log($"  - Use Object Center: {useObjectCenter}");
        Debug.Log($"  - Posición local: {transform.localPosition:F3}");
        Debug.Log($"  - Padre: {(transform.parent ? transform.parent.name : "None")}");
        
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log($"  - Renderer Bounds Center: {renderer.bounds.center:F3}");
            Debug.Log($"  - Renderer Bounds Size: {renderer.bounds.size:F3}");
        }
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log($"  - Collider Bounds Center: {collider.bounds.center:F3}");
            Debug.Log($"  - Collider Bounds Size: {collider.bounds.size:F3}");
        }
    }

    // Método para forzar el debug en OnDrawGizmos
    [System.NonSerialized]
    public bool debugMode = false;

    #region Runtime Rendering Methods
    
    private Vector3 GetObjectCenter()
    {
        // Si no queremos usar el centro del objeto, usar transform.position
        if (!useObjectCenter)
        {
            return transform.position;
        }
        
        // Intentar obtener el centro visual del objeto
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Usar el centro del bounds del renderer (como Unity hace con los gizmos)
            return renderer.bounds.center;
        }
        
        // Si no hay renderer, intentar con el collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds.center;
        }
        
        // Como último recurso, usar la posición del transform
        return transform.position;
    }
    
    private void CreateRuntimeRenderers()
    {
        if (runtimeRenderersCreated) return;

        // Crear contenedor para los ejes
        axisContainer = new GameObject($"{gameObject.name}_AxisRenderers");
        axisContainer.transform.SetParent(transform, false);

        // Crear material por defecto si no se asignó uno
        if (axisMaterial == null)
        {
            axisMaterial = CreateDefaultAxisMaterial();
        }

        // Crear LineRenderer para eje X
        if (showXAxis)
        {
            xAxisLineRenderer = CreateAxisLineRenderer("X_Axis", xAxisColor);
        }

        // Crear LineRenderer para eje Y
        if (showYAxis)
        {
            yAxisLineRenderer = CreateAxisLineRenderer("Y_Axis", yAxisColor);
        }

        // Crear LineRenderer para eje Z
        if (showZAxis)
        {
            zAxisLineRenderer = CreateAxisLineRenderer("Z_Axis", zAxisColor);
        }

        runtimeRenderersCreated = true;
        UpdateRuntimeRenderers();
    }

    private LineRenderer CreateAxisLineRenderer(string name, Color color)
    {
        GameObject axisGO = new GameObject(name);
        axisGO.transform.SetParent(axisContainer.transform, false);

        LineRenderer lr = axisGO.AddComponent<LineRenderer>();
        lr.material = axisMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = axisThickness * 0.001f;
        lr.endWidth = axisThickness * 0.001f;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.sortingOrder = 1000; // Renderizar encima de otros objetos

        return lr;
    }

    private Material CreateDefaultAxisMaterial()
    {
        // Intentar usar el mejor shader disponible para líneas
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Legacy Shaders/Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");
        
        Material mat = new Material(shader);
        mat.name = "DefaultAxisMaterial";
        mat.color = Color.white;
        
        // Configurar para que sea visible a través de objetos si es posible
        if (mat.HasProperty("_ZTest"))
        {
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }
        
        return mat;
    }

    private void UpdateRuntimeRenderers()
    {
        if (!runtimeRenderersCreated) return;

        // Obtener la posición central del objeto (como Unity lo hace)
        Vector3 position = GetObjectCenter();

        // Actualizar eje X
        if (xAxisLineRenderer != null && showXAxis)
        {
            Vector3 xDirection = transform.right * axisLength;
            xAxisLineRenderer.SetPosition(0, position);
            xAxisLineRenderer.SetPosition(1, position + xDirection);
            xAxisLineRenderer.startWidth = axisThickness * 0.001f;
            xAxisLineRenderer.endWidth = axisThickness * 0.001f;
            xAxisLineRenderer.startColor = xAxisColor;
            xAxisLineRenderer.endColor = xAxisColor;
        }

        // Actualizar eje Y
        if (yAxisLineRenderer != null && showYAxis)
        {
            Vector3 yDirection = transform.up * axisLength;
            yAxisLineRenderer.SetPosition(0, position);
            yAxisLineRenderer.SetPosition(1, position + yDirection);
            yAxisLineRenderer.startWidth = axisThickness * 0.001f;
            yAxisLineRenderer.endWidth = axisThickness * 0.001f;
            yAxisLineRenderer.startColor = yAxisColor;
            yAxisLineRenderer.endColor = yAxisColor;
        }

        // Actualizar eje Z
        if (zAxisLineRenderer != null && showZAxis)
        {
            Vector3 zDirection = transform.forward * axisLength;
            zAxisLineRenderer.SetPosition(0, position);
            zAxisLineRenderer.SetPosition(1, position + zDirection);
            zAxisLineRenderer.startWidth = axisThickness * 0.001f;
            zAxisLineRenderer.endWidth = axisThickness * 0.001f;
            zAxisLineRenderer.startColor = zAxisColor;
            zAxisLineRenderer.endColor = zAxisColor;
        }
    }

    private void SetRuntimeRenderersVisibility(bool visible)
    {
        if (axisContainer != null)
        {
            axisContainer.SetActive(visible);
        }
    }

    private void CleanupRuntimeRenderers()
    {
        if (axisContainer != null)
        {
            if (Application.isPlaying)
                Destroy(axisContainer);
            else
                DestroyImmediate(axisContainer);
        }

        xAxisLineRenderer = null;
        yAxisLineRenderer = null;
        zAxisLineRenderer = null;
        axisContainer = null;
        runtimeRenderersCreated = false;
    }

    // Método público para recrear los renderers (útil cuando se cambian configuraciones)
    public void RecreateRuntimeRenderers()
    {
        CleanupRuntimeRenderers();
        if (useRuntimeRendering && Application.isPlaying)
        {
            CreateRuntimeRenderers();
        }
    }

    // Método público para forzar actualización
    public void ForceUpdateRuntimeRenderers()
    {
        if (runtimeRenderersCreated)
        {
            UpdateRuntimeRenderers();
        }
    }

    // Context Menu methods para facilitar el uso
    [ContextMenu("Toggle Runtime Rendering")]
    public void ToggleRuntimeRendering()
    {
        useRuntimeRendering = !useRuntimeRendering;
        
        if (useRuntimeRendering && Application.isPlaying)
        {
            CreateRuntimeRenderers();
        }
        else if (!useRuntimeRendering)
        {
            CleanupRuntimeRenderers();
        }
    }
    
    [ContextMenu("Recreate Runtime Renderers")]
    public void ContextMenuRecreateRenderers()
    {
        RecreateRuntimeRenderers();
    }
    
    [ContextMenu("Force Update Renderers")]
    public void ContextMenuForceUpdate()
    {
        ForceUpdateRuntimeRenderers();
    }
    
    #endregion
}
