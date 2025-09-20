using UnityEngine;

[ExecuteAlways]
public class AxisMarkerImproved : MonoBehaviour
{
    [Header("Axis Settings")]
    public bool showAxis = true;        // Activar/desactivar eje individual
    public float axisLength = 0.5f;     // Tamano del eje
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
    }
}
