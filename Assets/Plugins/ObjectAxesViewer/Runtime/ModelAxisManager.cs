using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ModelPiece
{
    public GameObject gameObject;
    public AxisMarkerImproved axisMarker;
    public bool isVisible = true;
    public string displayName;
    
    public ModelPiece(GameObject go)
    {
        gameObject = go;
        displayName = go.name;
        axisMarker = go.GetComponent<AxisMarkerImproved>();
        if (axisMarker == null)
        {
            axisMarker = go.AddComponent<AxisMarkerImproved>();
        }
    }
}

public class ModelAxisManager : MonoBehaviour
{
    [Header("Model Settings")]
    public GameObject rootModel;
    public bool autoDetectPieces = true;
    public bool includeInactivePieces = false;
    
    [Header("Axis Display Settings")]
    public float globalAxisLength = 0.5f;
    [Range(1f, 150f)]
    public float globalAxisThickness = 2f;
    public bool showLabels = true;
    
    [Header("Filter Settings")]
    public bool filterByRenderer = true;
    public bool filterByCollider = false;
    public List<string> excludeNames = new List<string>();
    
    [SerializeField]
    private List<ModelPiece> modelPieces = new List<ModelPiece>();
    
    public List<ModelPiece> ModelPieces => modelPieces;
    
    private void Start()
    {
        if (autoDetectPieces && rootModel != null)
        {
            DetectModelPieces();
        }
    }
    
    [ContextMenu("Detect Model Pieces")]
    public void DetectModelPieces()
    {
        if (rootModel == null)
        {
            Debug.LogWarning("Root model no asignado!");
            return;
        }
        
        modelPieces.Clear();
        
        // Obtener todos los GameObjects hijos
        Transform[] allTransforms = rootModel.GetComponentsInChildren<Transform>(includeInactivePieces);
        
        foreach (Transform t in allTransforms)
        {
            if (t == rootModel.transform) continue; // Saltar el objeto raiz
            
            // Aplicar filtros
            if (ShouldIncludePiece(t.gameObject))
            {
                ModelPiece piece = new ModelPiece(t.gameObject);
                modelPieces.Add(piece);
                
                // Configurar el marcador de eje
                ConfigureAxisMarker(piece.axisMarker);
            }
        }
        
        Debug.Log($"Detectadas {modelPieces.Count} piezas en el modelo {rootModel.name}");
    }
    
    private bool ShouldIncludePiece(GameObject go)
    {
        // Verificar si esta en la lista de exclusion
        if (excludeNames.Contains(go.name))
            return false;
        
        // Filtrar por Renderer si esta activado
        if (filterByRenderer && go.GetComponent<Renderer>() == null)
            return false;
        
        // Filtrar por Collider si esta activado
        if (filterByCollider && go.GetComponent<Collider>() == null)
            return false;
        
        return true;
    }
    
    private void ConfigureAxisMarker(AxisMarkerImproved marker)
    {
        marker.axisLength = globalAxisLength;
        marker.axisThickness = globalAxisThickness;
        marker.showLabels = showLabels;
    }
    
    public void ShowAllAxes()
    {
        foreach (var piece in modelPieces)
        {
            if (piece.axisMarker != null)
            {
                piece.axisMarker.showAxis = true;
                piece.isVisible = true;
            }
        }
    }
    
    public void HideAllAxes()
    {
        foreach (var piece in modelPieces)
        {
            if (piece.axisMarker != null)
            {
                piece.axisMarker.showAxis = false;
                piece.isVisible = false;
            }
        }
    }
    
    public void ToggleAllAxes()
    {
        bool anyVisible = modelPieces.Any(p => p.isVisible);
        
        if (anyVisible)
            HideAllAxes();
        else
            ShowAllAxes();
    }
    
    public void SetPieceVisibility(int index, bool visible)
    {
        if (index >= 0 && index < modelPieces.Count)
        {
            modelPieces[index].isVisible = visible;
            if (modelPieces[index].axisMarker != null)
            {
                modelPieces[index].axisMarker.showAxis = visible;
            }
        }
    }
    
    public void SetPieceVisibility(string pieceName, bool visible)
    {
        var piece = modelPieces.FirstOrDefault(p => p.displayName == pieceName);
        if (piece != null)
        {
            piece.isVisible = visible;
            if (piece.axisMarker != null)
            {
                piece.axisMarker.showAxis = visible;
            }
        }
    }
    
    public void UpdateGlobalSettings()
    {
        foreach (var piece in modelPieces)
        {
            if (piece.axisMarker != null)
            {
                ConfigureAxisMarker(piece.axisMarker);
            }
        }
    }
    
    public void RemoveAllAxisMarkers()
    {
        foreach (var piece in modelPieces)
        {
            if (piece.axisMarker != null)
            {
                if (Application.isPlaying)
                    Destroy(piece.axisMarker);
                else
                    DestroyImmediate(piece.axisMarker);
            }
        }
        modelPieces.Clear();
    }
    
    public void AddExcludeName(string name)
    {
        if (!excludeNames.Contains(name))
        {
            excludeNames.Add(name);
        }
    }
    
    public void RemoveExcludeName(string name)
    {
        excludeNames.Remove(name);
    }
    
    // Metodos para uso en runtime
    public ModelPiece GetPieceByName(string name)
    {
        return modelPieces.FirstOrDefault(p => p.displayName == name);
    }
    
    public List<ModelPiece> GetVisiblePieces()
    {
        return modelPieces.Where(p => p.isVisible).ToList();
    }
    
    public List<ModelPiece> GetHiddenPieces()
    {
        return modelPieces.Where(p => !p.isVisible).ToList();
    }
}
