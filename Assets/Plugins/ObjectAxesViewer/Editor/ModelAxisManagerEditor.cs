using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ModelAxisManager))]
public class ModelAxisManagerEditor : Editor
{
    private ModelAxisManager manager;
    private bool showPiecesList = true;
    private bool showSettings = true;
    private Vector2 scrollPosition;
    
    private void OnEnable()
    {
        manager = (ModelAxisManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Model Axis Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Configuracion basica
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        // Botones de control
        DrawControlButtons();
        
        EditorGUILayout.Space();
        
        // Configuracion avanzada
        DrawAdvancedSettings();
        
        EditorGUILayout.Space();
        
        // Lista de piezas
        DrawPiecesList();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawControlButtons()
    {
        EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Detect Pieces"))
        {
            manager.DetectModelPieces();
        }
        
        if (GUILayout.Button("Show All"))
        {
            manager.ShowAllAxes();
        }
        
        if (GUILayout.Button("Hide All"))
        {
            manager.HideAllAxes();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Toggle All"))
        {
            manager.ToggleAllAxes();
        }
        
        if (GUILayout.Button("Update Settings"))
        {
            manager.UpdateGlobalSettings();
        }
        
        EditorGUILayout.EndHorizontal();
        
        if (manager.ModelPieces.Count > 0)
        {
            if (GUILayout.Button("Remove All Markers", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to remove all axis markers?", "Yes", "No"))
                {
                    manager.RemoveAllAxisMarkers();
                }
            }
        }
    }
    
    private void DrawAdvancedSettings()
    {
        showSettings = EditorGUILayout.Foldout(showSettings, "Advanced Settings", true);
        
        if (showSettings)
        {
            EditorGUI.indentLevel++;
            
            // Estadisticas
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Pieces: {manager.ModelPieces.Count}");
            EditorGUILayout.LabelField($"Visible Pieces: {manager.GetVisiblePieces().Count}");
            EditorGUILayout.LabelField($"Hidden Pieces: {manager.GetHiddenPieces().Count}");
            
            EditorGUILayout.Space();
            
            // Herramientas de seleccion
            EditorGUILayout.LabelField("Selection Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Visible"))
            {
                var visibleObjects = manager.GetVisiblePieces().Select(p => p.gameObject).ToArray();
                Selection.objects = visibleObjects;
            }
            
            if (GUILayout.Button("Select Hidden"))
            {
                var hiddenObjects = manager.GetHiddenPieces().Select(p => p.gameObject).ToArray();
                Selection.objects = hiddenObjects;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Herramientas de configuracion rapida
            EditorGUILayout.LabelField("Quick Config", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X Only"))
            {
                SetAllAxesVisibility(true, false, false);
            }
            if (GUILayout.Button("Y Only"))
            {
                SetAllAxesVisibility(false, true, false);
            }
            if (GUILayout.Button("Z Only"))
            {
                SetAllAxesVisibility(false, false, true);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("XY Only"))
            {
                SetAllAxesVisibility(true, true, false);
            }
            if (GUILayout.Button("XZ Only"))
            {
                SetAllAxesVisibility(true, false, true);
            }
            if (GUILayout.Button("YZ Only"))
            {
                SetAllAxesVisibility(false, true, true);
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("All Axes"))
            {
                SetAllAxesVisibility(true, true, true);
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    private void DrawPiecesList()
    {
        var pieces = manager.ModelPieces;
        
        showPiecesList = EditorGUILayout.Foldout(showPiecesList, $"Model Pieces ({pieces.Count})", true);
        
        if (showPiecesList && pieces.Count > 0)
        {
            EditorGUI.indentLevel++;
            
            // Barra de busqueda simple
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(50));
            string searchTerm = EditorGUILayout.TextField("", GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            var filteredPieces = string.IsNullOrEmpty(searchTerm) ? 
                pieces : 
                pieces.Where(p => p.displayName.ToLower().Contains(searchTerm.ToLower())).ToList();
            
            foreach (var piece in filteredPieces)
            {
                if (piece.gameObject == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                
                // Toggle principal
                EditorGUI.BeginChangeCheck();
                bool newVisibility = EditorGUILayout.Toggle(piece.isVisible, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    piece.isVisible = newVisibility;
                    if (piece.axisMarker != null)
                    {
                        piece.axisMarker.showAxis = newVisibility;
                    }
                }
                
                // Nombre (clickeable)
                if (GUILayout.Button(piece.displayName, EditorStyles.label))
                {
                    Selection.activeGameObject = piece.gameObject;
                    EditorGUIUtility.PingObject(piece.gameObject);
                }
                
                // Controles de ejes individuales
                if (piece.axisMarker != null)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    GUI.color = Color.red;
                    bool showX = GUILayout.Toggle(piece.axisMarker.showXAxis, "X", GUILayout.Width(25));
                    
                    GUI.color = Color.green;
                    bool showY = GUILayout.Toggle(piece.axisMarker.showYAxis, "Y", GUILayout.Width(25));
                    
                    GUI.color = Color.blue;
                    bool showZ = GUILayout.Toggle(piece.axisMarker.showZAxis, "Z", GUILayout.Width(25));
                    
                    GUI.color = Color.white;
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        piece.axisMarker.SetAxisVisibility(showX, showY, showZ);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUI.indentLevel--;
        }
    }
    
    private void SetAllAxesVisibility(bool x, bool y, bool z)
    {
        foreach (var piece in manager.ModelPieces)
        {
            if (piece.axisMarker != null)
            {
                piece.axisMarker.SetAxisVisibility(x, y, z);
            }
        }
    }
    
    // Dibujar gizmos en la scene view
    private void OnSceneGUI()
    {
        if (manager == null || manager.ModelPieces == null) return;
        
        // Mostrar informacion adicional en la scene view
        Handles.BeginGUI();
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label($"Axis Manager: {manager.name}", EditorStyles.boldLabel);
        GUILayout.Label($"Pieces: {manager.ModelPieces.Count}");
        GUILayout.Label($"Visible: {manager.GetVisiblePieces().Count}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        Handles.EndGUI();
    }
}
