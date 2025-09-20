using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ImprovedAxisManagerWindow : EditorWindow
{
    private Vector2 scrollPos;
    private ModelAxisManager currentManager;
    private GameObject selectedRootObject;
    private bool showGlobalSettings = true;
    private bool showPiecesList = true;
    private bool showFilters = false;
    
    // Configuracion global
    private float globalAxisLength =150f;
    private float globalAxisThickness = 2f;
    private bool globalShowLabels = true;
    
    // Filtros
    private string searchFilter = "";
    private bool filterByRenderer = true;
    private bool filterByCollider = false;
    private bool includeInactive = false;
    
    [MenuItem("Window/Axis Viewer Manager")]
    public static void ShowWindow()
    {
        GetWindow<ImprovedAxisManagerWindow>("Axis Manager");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Axis Viewer Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        DrawRootObjectSelection();
        EditorGUILayout.Space();
        
        if (currentManager != null)
        {
            DrawGlobalControls();
            EditorGUILayout.Space();
            
            DrawGlobalSettings();
            EditorGUILayout.Space();
            
            DrawFilters();
            EditorGUILayout.Space();
            
            DrawPiecesList();
        }
        else
        {
            EditorGUILayout.HelpBox("Selecciona un objeto raiz y haz clic en 'Setup Manager' para comenzar.", MessageType.Info);
        }
    }
    
    private void DrawRootObjectSelection()
    {
        EditorGUILayout.BeginHorizontal();
        selectedRootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", selectedRootObject, typeof(GameObject), true);
        
        if (GUILayout.Button("Setup Manager", GUILayout.Width(120)))
        {
            SetupManager();
        }
        EditorGUILayout.EndHorizontal();
        
        if (selectedRootObject != null && currentManager == null)
        {
            EditorGUILayout.HelpBox($"Objeto seleccionado: {selectedRootObject.name}. Haz clic en 'Setup Manager' para configurar.", MessageType.Warning);
        }
    }
    
    private void SetupManager()
    {
        if (selectedRootObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Por favor selecciona un objeto raiz primero.", "OK");
            return;
        }
        
        // Buscar si ya tiene un ModelAxisManager
        currentManager = selectedRootObject.GetComponent<ModelAxisManager>();
        
        if (currentManager == null)
        {
            // Crear uno nuevo
            currentManager = selectedRootObject.AddComponent<ModelAxisManager>();
            currentManager.rootModel = selectedRootObject;
            
            EditorUtility.SetDirty(selectedRootObject);
        }
        
        // Detectar piezas automaticamente
        currentManager.DetectModelPieces();
        
        // Sincronizar configuracion
        SyncGlobalSettings();
    }
    
    private void DrawGlobalControls()
    {
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Show All"))
        {
            currentManager.ShowAllAxes();
        }
        
        if (GUILayout.Button("Hide All"))
        {
            currentManager.HideAllAxes();
        }
        
        if (GUILayout.Button("Toggle All"))
        {
            currentManager.ToggleAllAxes();
        }
        
        if (GUILayout.Button("Refresh"))
        {
            currentManager.DetectModelPieces();
            SyncGlobalSettings();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Remove All Markers"))
        {
            if (EditorUtility.DisplayDialog("Confirmar", "Estas seguro de que quieres eliminar todos los marcadores de eje?", "Si", "No"))
            {
                currentManager.RemoveAllAxisMarkers();
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawGlobalSettings()
    {
        showGlobalSettings = EditorGUILayout.Foldout(showGlobalSettings, "Global Settings", true);
        
        if (showGlobalSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUI.BeginChangeCheck();
            
            globalAxisLength = EditorGUILayout.Slider("Axis Length", globalAxisLength, 0.1f, 150f);
            globalAxisThickness = EditorGUILayout.Slider("Axis Thickness", globalAxisThickness, 1f, 150f);
            globalShowLabels = EditorGUILayout.Toggle("Show Labels", globalShowLabels);
            
            if (EditorGUI.EndChangeCheck())
            {
                ApplyGlobalSettings();
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    private void DrawFilters()
    {
        showFilters = EditorGUILayout.Foldout(showFilters, "Filters & Detection", true);
        
        if (showFilters)
        {
            EditorGUI.indentLevel++;
            
            EditorGUI.BeginChangeCheck();
            
            filterByRenderer = EditorGUILayout.Toggle("Filter by Renderer", filterByRenderer);
            filterByCollider = EditorGUILayout.Toggle("Filter by Collider", filterByCollider);
            includeInactive = EditorGUILayout.Toggle("Include Inactive", includeInactive);
            
            if (EditorGUI.EndChangeCheck())
            {
                currentManager.filterByRenderer = filterByRenderer;
                currentManager.filterByCollider = filterByCollider;
                currentManager.includeInactivePieces = includeInactive;
                EditorUtility.SetDirty(currentManager);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Exclude Names:", EditorStyles.boldLabel);
            
            for (int i = 0; i < currentManager.excludeNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                currentManager.excludeNames[i] = EditorGUILayout.TextField(currentManager.excludeNames[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    currentManager.excludeNames.RemoveAt(i);
                    EditorUtility.SetDirty(currentManager);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Add Exclude Name"))
            {
                currentManager.excludeNames.Add("NewExclude");
                EditorUtility.SetDirty(currentManager);
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    private void DrawPiecesList()
    {
        var pieces = currentManager.ModelPieces;
        
        showPiecesList = EditorGUILayout.Foldout(showPiecesList, $"Model Pieces ({pieces.Count})", true);
        
        if (showPiecesList)
        {
            EditorGUI.indentLevel++;
            
            // Barra de busqueda
            searchFilter = EditorGUILayout.TextField("Search:", searchFilter);
            
            EditorGUILayout.Space();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
            
            var filteredPieces = string.IsNullOrEmpty(searchFilter) ? 
                pieces : 
                pieces.Where(p => p.displayName.ToLower().Contains(searchFilter.ToLower())).ToList();
            
            foreach (var piece in filteredPieces)
            {
                if (piece.gameObject == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                
                // Toggle de visibilidad
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
                
                // Nombre del objeto (clickeable para seleccionar)
                if (GUILayout.Button(piece.displayName, EditorStyles.label))
                {
                    Selection.activeGameObject = piece.gameObject;
                    EditorGUIUtility.PingObject(piece.gameObject);
                }
                
                // Controles individuales de ejes
                if (piece.axisMarker != null)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    GUI.color = Color.red;
                    bool showX = GUILayout.Toggle(piece.axisMarker.showXAxis, "X", GUILayout.Width(30));
                    
                    GUI.color = Color.green;
                    bool showY = GUILayout.Toggle(piece.axisMarker.showYAxis, "Y", GUILayout.Width(30));
                    
                    GUI.color = Color.blue;
                    bool showZ = GUILayout.Toggle(piece.axisMarker.showZAxis, "Z", GUILayout.Width(30));
                    
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
    
    private void SyncGlobalSettings()
    {
        if (currentManager != null)
        {
            globalAxisLength = currentManager.globalAxisLength;
            globalAxisThickness = currentManager.globalAxisThickness;
            globalShowLabels = currentManager.showLabels;
            filterByRenderer = currentManager.filterByRenderer;
            filterByCollider = currentManager.filterByCollider;
            includeInactive = currentManager.includeInactivePieces;
        }
    }
    
    private void ApplyGlobalSettings()
    {
        if (currentManager != null)
        {
            currentManager.globalAxisLength = globalAxisLength;
            currentManager.globalAxisThickness = globalAxisThickness;
            currentManager.showLabels = globalShowLabels;
            currentManager.UpdateGlobalSettings();
            EditorUtility.SetDirty(currentManager);
        }
    }
    
    private void OnSelectionChange()
    {
        // Auto-detectar si el objeto seleccionado tiene un ModelAxisManager
        if (Selection.activeGameObject != null)
        {
            var manager = Selection.activeGameObject.GetComponent<ModelAxisManager>();
            if (manager != null && manager != currentManager)
            {
                selectedRootObject = Selection.activeGameObject;
                currentManager = manager;
                SyncGlobalSettings();
                Repaint();
            }
        }
    }
}
