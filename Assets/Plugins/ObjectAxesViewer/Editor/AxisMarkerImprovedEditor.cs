using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AxisMarkerImproved))]
public class AxisMarkerImprovedEditor : Editor
{
    private AxisMarkerImproved marker;
    
    private void OnEnable()
    {
        marker = (AxisMarkerImproved)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Axis Marker Improved", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Control principal
        EditorGUI.BeginChangeCheck();
        marker.showAxis = EditorGUILayout.Toggle("Show Axis", marker.showAxis);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(marker);
        }
        
        if (marker.showAxis)
        {
            EditorGUI.indentLevel++;
            
            // Configuracion de tamano
            EditorGUILayout.LabelField("Size Settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            marker.axisLength = EditorGUILayout.Slider("Axis Length", marker.axisLength, 0.1f, 150f);
            marker.axisThickness = EditorGUILayout.Slider("Axis Thickness", marker.axisThickness, 1f, 150f);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            
            EditorGUILayout.Space();
            
            // Control individual de ejes
            EditorGUILayout.LabelField("Individual Axis Control", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            GUI.color = marker.showXAxis ? Color.white : Color.gray;
            EditorGUI.BeginChangeCheck();
            marker.showXAxis = EditorGUILayout.ToggleLeft("X Axis", marker.showXAxis);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = marker.xAxisColor;
            EditorGUI.BeginChangeCheck();
            marker.xAxisColor = EditorGUILayout.ColorField(marker.xAxisColor, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUI.color = marker.showYAxis ? Color.white : Color.gray;
            EditorGUI.BeginChangeCheck();
            marker.showYAxis = EditorGUILayout.ToggleLeft("Y Axis", marker.showYAxis);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = marker.yAxisColor;
            EditorGUI.BeginChangeCheck();
            marker.yAxisColor = EditorGUILayout.ColorField(marker.yAxisColor, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUI.color = marker.showZAxis ? Color.white : Color.gray;
            EditorGUI.BeginChangeCheck();
            marker.showZAxis = EditorGUILayout.ToggleLeft("Z Axis", marker.showZAxis);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = marker.zAxisColor;
            EditorGUI.BeginChangeCheck();
            marker.zAxisColor = EditorGUILayout.ColorField(marker.zAxisColor, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Configuracion de etiquetas
            EditorGUILayout.LabelField("Label Settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            marker.showLabels = EditorGUILayout.Toggle("Show Labels", marker.showLabels);
            if (marker.showLabels)
            {
                marker.labelOffset = EditorGUILayout.Slider("Label Offset", marker.labelOffset, 0f, 0.5f);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(marker);
            }
            
            EditorGUILayout.Space();
            
            // Botones de control rapido
            EditorGUILayout.LabelField("Quick Controls", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All Axes"))
            {
                marker.SetAxisVisibility(true, true, true);
                EditorUtility.SetDirty(marker);
            }
            if (GUILayout.Button("X Only"))
            {
                marker.SetAxisVisibility(true, false, false);
                EditorUtility.SetDirty(marker);
            }
            if (GUILayout.Button("Y Only"))
            {
                marker.SetAxisVisibility(false, true, false);
                EditorUtility.SetDirty(marker);
            }
            if (GUILayout.Button("Z Only"))
            {
                marker.SetAxisVisibility(false, false, true);
                EditorUtility.SetDirty(marker);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Colors"))
            {
                marker.xAxisColor = Color.red;
                marker.yAxisColor = Color.green;
                marker.zAxisColor = Color.blue;
                EditorUtility.SetDirty(marker);
            }
            if (GUILayout.Button("Reset Size"))
            {
                marker.axisLength = 150f;
                marker.axisThickness = 2f;
                EditorUtility.SetDirty(marker);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Informacion adicional
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Position: {marker.transform.position}");
        EditorGUILayout.LabelField($"Rotation: {marker.transform.eulerAngles}");
        EditorGUILayout.LabelField($"Scale: {marker.transform.localScale}");
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnSceneGUI()
    {
        if (!marker.showAxis) return;
        
        // Mostrar handles interactivos en la scene view
        Vector3 position = marker.transform.position;
        
        // Handle para ajustar el tamano del eje
        Handles.color = Color.white;
        EditorGUI.BeginChangeCheck();
        float newLength = Handles.RadiusHandle(Quaternion.identity, position, marker.axisLength);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(marker, "Change Axis Length");
            marker.axisLength = newLength;
            EditorUtility.SetDirty(marker);
        }
        
        // Etiqueta con informacion
        Handles.Label(position + Vector3.up * (marker.axisLength + 0.2f), 
                     $"{marker.name}\nLength: {marker.axisLength:F2}");
    }
}
