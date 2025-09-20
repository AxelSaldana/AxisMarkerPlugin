using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RuntimeAxisController : MonoBehaviour
{
    [Header("UI References")]
    public Canvas uiCanvas;
    public Transform pieceListParent;
    public GameObject pieceTogglePrefab;
    public Button showAllButton;
    public Button hideAllButton;
    public Button toggleAllButton;
    public Slider axisSizeSlider;
    public Toggle labelsToggle;
    
    [Header("Model Reference")]
    public ModelAxisManager modelManager;
    
    [Header("Auto Setup")]
    public bool createUIAutomatically = true;
    public bool showUIOnStart = true;
    
    private List<Toggle> pieceToggles = new List<Toggle>();
    private Dictionary<Toggle, ModelPiece> toggleToPiece = new Dictionary<Toggle, ModelPiece>();
    
    private void Start()
    {
        if (createUIAutomatically)
        {
            CreateRuntimeUI();
        }
        
        SetupUI();
        
        if (!showUIOnStart && uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
        }
    }
    
    private void CreateRuntimeUI()
    {
        // Crear Canvas si no existe
        if (uiCanvas == null)
        {
            GameObject canvasGO = new GameObject("AxisControllerUI");
            uiCanvas = canvasGO.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Crear panel principal
        GameObject mainPanel = CreateUIPanel("MainPanel", uiCanvas.transform);
        RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
        mainRect.anchorMin = new Vector2(0, 0.5f);
        mainRect.anchorMax = new Vector2(0.3f, 1f);
        mainRect.offsetMin = new Vector2(10, 10);
        mainRect.offsetMax = new Vector2(-10, -10);
        
        // Crear titulo
        CreateUIText("Axis Controller", mainPanel.transform, 16, TextAnchor.MiddleCenter);
        
        // Crear botones de control global
        GameObject buttonPanel = CreateUIPanel("ButtonPanel", mainPanel.transform);
        HorizontalLayoutGroup buttonLayout = buttonPanel.AddComponent<HorizontalLayoutGroup>();
        buttonLayout.spacing = 5;
        
        showAllButton = CreateUIButton("Show All", buttonPanel.transform);
        hideAllButton = CreateUIButton("Hide All", buttonPanel.transform);
        toggleAllButton = CreateUIButton("Toggle", buttonPanel.transform);
        
        // Crear controles de configuracion
        GameObject settingsPanel = CreateUIPanel("SettingsPanel", mainPanel.transform);
        VerticalLayoutGroup settingsLayout = settingsPanel.AddComponent<VerticalLayoutGroup>();
        settingsLayout.spacing = 5;
        
        CreateUIText("Axis Size:", settingsPanel.transform, 12);
        axisSizeSlider = CreateUISlider(settingsPanel.transform, 0.1f, 2f, 0.5f);
        
        labelsToggle = CreateUIToggle("Show Labels", settingsPanel.transform);
        
        // Crear area de scroll para la lista de piezas
        GameObject scrollArea = CreateScrollArea("PiecesList", mainPanel.transform);
        pieceListParent = scrollArea.transform.Find("Viewport/Content");
    }
    
    private void SetupUI()
    {
        if (modelManager == null)
        {
            Debug.LogWarning("ModelAxisManager no asignado!");
            return;
        }
        
        // Configurar botones
        if (showAllButton != null)
            showAllButton.onClick.AddListener(() => modelManager.ShowAllAxes());
        
        if (hideAllButton != null)
            hideAllButton.onClick.AddListener(() => modelManager.HideAllAxes());
        
        if (toggleAllButton != null)
            toggleAllButton.onClick.AddListener(() => modelManager.ToggleAllAxes());
        
        // Configurar slider de tamano
        if (axisSizeSlider != null)
        {
            axisSizeSlider.value = modelManager.globalAxisLength;
            axisSizeSlider.onValueChanged.AddListener(OnAxisSizeChanged);
        }
        
        // Configurar toggle de labels
        if (labelsToggle != null)
        {
            labelsToggle.isOn = modelManager.showLabels;
            labelsToggle.onValueChanged.AddListener(OnLabelsToggleChanged);
        }
        
        // Crear toggles para cada pieza
        CreatePieceToggles();
    }
    
    private void CreatePieceToggles()
    {
        if (pieceListParent == null) return;
        
        // Limpiar toggles existentes
        foreach (Transform child in pieceListParent)
        {
            Destroy(child.gameObject);
        }
        pieceToggles.Clear();
        toggleToPiece.Clear();
        
        // Crear toggle para cada pieza
        foreach (var piece in modelManager.ModelPieces)
        {
            GameObject toggleGO;
            
            if (pieceTogglePrefab != null)
            {
                toggleGO = Instantiate(pieceTogglePrefab, pieceListParent);
            }
            else
            {
                toggleGO = CreateUIToggle(piece.displayName, pieceListParent);
            }
            
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = piece.isVisible;
                toggle.onValueChanged.AddListener((value) => OnPieceToggleChanged(toggle, value));
                
                pieceToggles.Add(toggle);
                toggleToPiece[toggle] = piece;
                
                // Actualizar texto si es necesario
                Text toggleText = toggle.GetComponentInChildren<Text>();
                if (toggleText != null)
                {
                    toggleText.text = piece.displayName;
                }
            }
        }
    }
    
    private void OnAxisSizeChanged(float value)
    {
        if (modelManager != null)
        {
            modelManager.globalAxisLength = value;
            modelManager.UpdateGlobalSettings();
        }
    }
    
    private void OnLabelsToggleChanged(bool value)
    {
        if (modelManager != null)
        {
            modelManager.showLabels = value;
            modelManager.UpdateGlobalSettings();
        }
    }
    
    private void OnPieceToggleChanged(Toggle toggle, bool value)
    {
        if (toggleToPiece.ContainsKey(toggle))
        {
            var piece = toggleToPiece[toggle];
            piece.isVisible = value;
            if (piece.axisMarker != null)
            {
                piece.axisMarker.showAxis = value;
            }
        }
    }
    
    // Metodos publicos para control externo
    public void ToggleUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(!uiCanvas.gameObject.activeSelf);
        }
    }
    
    public void ShowUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(true);
        }
    }
    
    public void HideUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
        }
    }
    
    public void RefreshPieceList()
    {
        CreatePieceToggles();
    }
    
    // Metodos de utilidad para crear UI
    private GameObject CreateUIPanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 5;
        
        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        return panel;
    }
    
    private Text CreateUIText(string text, Transform parent, int fontSize = 14, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent, false);
        
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        
        return textComponent;
    }
    
    private Button CreateUIButton(string text, Transform parent)
    {
        GameObject buttonGO = new GameObject("Button");
        buttonGO.transform.SetParent(parent, false);
        
        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        Button button = buttonGO.AddComponent<Button>();
        
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 12;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private Toggle CreateUIToggle(string text, Transform parent)
    {
        GameObject toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(parent, false);
        
        Toggle toggle = toggleGO.AddComponent<Toggle>();
        
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(toggleGO.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(20, 20);
        bgRect.anchorMin = new Vector2(0, 0.5f);
        bgRect.anchorMax = new Vector2(0, 0.5f);
        bgRect.anchoredPosition = new Vector2(10, 0);
        
        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = Color.green;
        
        RectTransform checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;
        
        // Label
        GameObject label = new GameObject("Label");
        label.transform.SetParent(toggleGO.transform, false);
        Text labelText = label.AddComponent<Text>();
        labelText.text = text;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(25, 0);
        labelRect.offsetMax = Vector2.zero;
        
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        
        return toggle;
    }
    
    private Slider CreateUISlider(Transform parent, float minValue, float maxValue, float value)
    {
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(parent, false);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = value;
        
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderGO.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.blue;
        
        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderGO.transform, false);
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        
        return slider;
    }
    
    private GameObject CreateScrollArea(string name, Transform parent)
    {
        GameObject scrollGO = new GameObject(name);
        scrollGO.transform.SetParent(parent, false);
        
        ScrollRect scroll = scrollGO.AddComponent<ScrollRect>();
        
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGO.transform, false);
        viewport.AddComponent<Image>();
        viewport.AddComponent<Mask>();
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 2;
        
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = content.GetComponent<RectTransform>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.vertical = true;
        scroll.horizontal = false;
        
        return scrollGO;
    }
}
