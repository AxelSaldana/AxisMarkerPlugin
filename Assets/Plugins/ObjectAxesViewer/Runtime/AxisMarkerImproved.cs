using UnityEngine;

[ExecuteAlways]
public class AxisMarkerImproved : MonoBehaviour
{
    [Header("Axis Settings")]
    public bool showAxis = true;        // Activar/desactivar eje individual
    public float axisLength = 0.5f;     // Tamano del eje
    public float axisThickness = 2f;    // Grosor de las lineas
    
    [Header("Individual Axis Control")]
    public bool showXAxis = true;       // Mostrar eje X (Rojo)
    public bool showYAxis = true;       // Mostrar eje Y (Verde)
    public bool showZAxis = true;       // Mostrar eje Z (Azul)
    
    [Header("Colors")]
    public Color xAxisColor = Color.red;
    public Color yAxisColor = Color.green;
    public Color zAxisColor = Color.blue;
    
    [Header("Labels")]
    public bool showLabels = true;
    public float labelOffset = 0.1f;
    
    private void OnDrawGizmos()
    {
        if (!showAxis) return;
        
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
        Vector3 position = transform.position;
        
        // Eje X (Rojo)
        if (showXAxis)
        {
            Gizmos.color = xAxisColor;
            Vector3 xDirection = transform.right * axisLength;
            DrawThickLine(position, position + xDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + xDirection + Vector3.right * labelOffset;
                DrawLabel(labelPos, "X");
            }
        }
        
        // Eje Y (Verde)
        if (showYAxis)
        {
            Gizmos.color = yAxisColor;
            Vector3 yDirection = transform.up * axisLength;
            DrawThickLine(position, position + yDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + yDirection + Vector3.up * labelOffset;
                DrawLabel(labelPos, "Y");
            }
        }
        
        // Eje Z (Azul)
        if (showZAxis)
        {
            Gizmos.color = zAxisColor;
            Vector3 zDirection = transform.forward * axisLength;
            DrawThickLine(position, position + zDirection);
            
            if (showLabels)
            {
                Vector3 labelPos = position + zDirection + Vector3.forward * labelOffset;
                DrawLabel(labelPos, "Z");
            }
        }
        
        // Dibujar punto central
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, axisLength * 0.05f);
    }
    
    private void DrawThickLine(Vector3 start, Vector3 end)
    {
        // Simular linea gruesa dibujando multiples lineas paralelas
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular1 = Vector3.Cross(direction, Vector3.up).normalized * (axisThickness * 0.001f);
        Vector3 perpendicular2 = Vector3.Cross(direction, perpendicular1).normalized * (axisThickness * 0.001f);
        
        // Linea principal
        Gizmos.DrawLine(start, end);
        
        // Lineas paralelas para simular grosor
        if (axisThickness > 1f)
        {
            Gizmos.DrawLine(start + perpendicular1, end + perpendicular1);
            Gizmos.DrawLine(start - perpendicular1, end - perpendicular1);
            Gizmos.DrawLine(start + perpendicular2, end + perpendicular2);
            Gizmos.DrawLine(start - perpendicular2, end - perpendicular2);
        }
        
        // Punta de flecha
        Vector3 arrowHead1 = end - direction * (axisLength * 0.1f) + perpendicular1 * (axisLength * 0.05f);
        Vector3 arrowHead2 = end - direction * (axisLength * 0.1f) - perpendicular1 * (axisLength * 0.05f);
        
        Gizmos.DrawLine(end, arrowHead1);
        Gizmos.DrawLine(end, arrowHead2);
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
    }
}
