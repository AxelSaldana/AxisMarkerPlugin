# Advanced Axis Marker Plugin para Unity

Un plugin completo para visualizar los ejes de coordenadas (X, Y, Z) de todos los objetos en un modelo 3D, tanto en el editor como en runtime.

## Características Principales

### ✨ Funcionalidades
- **Visualización de Ejes**: Muestra ejes X (rojo), Y (verde), Z (azul) con flechas y etiquetas
- **Control Individual**: Activa/desactiva ejes por pieza y por eje individual
- **Detección Automática**: Encuentra automáticamente todas las piezas de un modelo
- **Editor y Runtime**: Funciona tanto en el editor de Unity como durante la ejecución
- **Interfaz Completa**: Ventana de editor avanzada y UI de runtime
- **Filtros Inteligentes**: Filtra objetos por Renderer, Collider, nombres, etc.
- **Presets**: Configuraciones predefinidas para diferentes casos de uso

### 🎯 Casos de Uso
- Debugging de orientación de objetos
- Visualización de sistemas de coordenadas
- Educación y demostración de conceptos 3D
- Herramientas de desarrollo para equipos
- Análisis de modelos complejos

## Instalación

1. Copia la carpeta `Assets/Plugins/ObjectAxesViewer` a tu proyecto
2. Unity compilará automáticamente los scripts
3. Accede al plugin desde `Window > Axis Viewer Manager`

## Uso Rápido

### En el Editor

1. **Abrir la Ventana del Manager**:
   - Ve a `Window > Axis Viewer Manager`

2. **Configurar un Modelo**:
   - Selecciona tu objeto raíz en "Root Object"
   - Haz clic en "Setup Manager"
   - El plugin detectará automáticamente todas las piezas

3. **Controlar la Visualización**:
   - Usa "Show All", "Hide All", "Toggle All" para control global
   - Usa los toggles individuales para cada pieza
   - Controla ejes X, Y, Z individualmente

### En Runtime

1. **Configuración Automática**:
   ```csharp
   // Agregar a cualquier GameObject
   ModelAxisManager manager = AxisUtilities.SetupAxisMarkersForModel(miModelo);
   RuntimeAxisController controller = AxisUtilities.CreateRuntimeController(manager, true);
   ```

2. **Control Programático**:
   ```csharp
   // Mostrar/ocultar todos los ejes
   manager.ShowAllAxes();
   manager.HideAllAxes();
   
   // Control por pieza específica
   manager.SetPieceVisibility("NombrePieza", true);
   
   // Configuración global
   manager.globalAxisLength = 1.0f;
   manager.UpdateGlobalSettings();
   ```

## Componentes del Sistema

### 🔧 Componentes Principales

#### `AxisMarkerImproved`
- Componente base que dibuja los ejes en un GameObject
- Control individual de ejes X, Y, Z
- Configuración de colores, tamaño y etiquetas
- Funciona con `OnDrawGizmos` para visualización en editor

#### `ModelAxisManager`
- Gestiona todos los marcadores de eje de un modelo
- Detección automática de piezas
- Configuración global y filtros
- API completa para control programático

#### `RuntimeAxisController`
- Interfaz de usuario para runtime
- Crea automáticamente UI Canvas
- Controles interactivos para el usuario final
- Integración completa con ModelAxisManager

### 🎨 Herramientas de Editor

#### `ImprovedAxisManagerWindow`
- Ventana de editor avanzada (`Window > Axis Viewer Manager`)
- Lista completa de piezas con búsqueda
- Configuración global en tiempo real
- Herramientas de filtrado y selección

#### `ModelAxisManagerEditor`
- Inspector personalizado para ModelAxisManager
- Controles rápidos y estadísticas
- Herramientas de selección avanzadas
- Presets de configuración rápida

## Configuración Avanzada

### Filtros de Detección

```csharp
ModelAxisManager manager = GetComponent<ModelAxisManager>();

// Configurar filtros
manager.filterByRenderer = true;      // Solo objetos con Renderer
manager.filterByCollider = false;     // Incluir objetos sin Collider
manager.includeInactivePieces = true; // Incluir objetos inactivos

// Excluir objetos por nombre
manager.AddExcludeName("Helper");
manager.AddExcludeName("Temp");

// Aplicar cambios
manager.DetectModelPieces();
```

### Presets Personalizados

```csharp
// Usar presets predefinidos
var preset = AxisUtilities.Presets.Large;
preset.ApplyToMarker(axisMarker);

// Crear preset personalizado
var customPreset = new AxisUtilities.AxisPreset
{
    name = "Mi Preset",
    axisLength = 0.8f,
    axisThickness = 4f,
    showLabels = true,
    xAxisColor = Color.magenta
};
```

### Configuración de Colores

```csharp
// Cambiar colores globalmente
foreach(var piece in manager.ModelPieces)
{
    AxisUtilities.SetCustomAxisColors(
        piece.axisMarker, 
        Color.cyan,    // X
        Color.yellow,  // Y  
        Color.magenta  // Z
    );
}
```

## API de Runtime

### Control Básico

```csharp
// Obtener el manager
ModelAxisManager manager = FindObjectOfType<ModelAxisManager>();

// Mostrar/ocultar todo
manager.ShowAllAxes();
manager.HideAllAxes();
manager.ToggleAllAxes();

// Control por pieza
manager.SetPieceVisibility("Rueda1", true);
manager.SetPieceVisibility("Motor", false);

// Buscar piezas
ModelPiece pieza = manager.GetPieceByName("Chasis");
List<ModelPiece> visibles = manager.GetVisiblePieces();
```

### Configuración Dinámica

```csharp
// Cambiar configuración global
manager.globalAxisLength = 1.5f;
manager.globalAxisThickness = 3f;
manager.showLabels = false;
manager.UpdateGlobalSettings();

// Configurar pieza específica
var marker = manager.GetPieceByName("Pieza1").axisMarker;
marker.SetAxisVisibility(true, false, true); // Solo X y Z
marker.axisLength = 0.3f;
```

### Eventos y Callbacks

```csharp
// Detectar cambios (implementar según necesidades)
public class AxisEventHandler : MonoBehaviour
{
    private ModelAxisManager manager;
    
    void Start()
    {
        manager = GetComponent<ModelAxisManager>();
    }
    
    void Update()
    {
        // Ejemplo: Toggle con tecla
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.ToggleAllAxes();
        }
        
        // Ejemplo: Control por número
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) && i <= manager.ModelPieces.Count)
            {
                var piece = manager.ModelPieces[i-1];
                piece.isVisible = !piece.isVisible;
                piece.axisMarker.showAxis = piece.isVisible;
            }
        }
    }
}
```

## Herramientas de Debugging

```csharp
// Información del manager
AxisUtilities.DebugTools.LogAxisManagerInfo(manager);

// Lista de todas las piezas
AxisUtilities.DebugTools.LogAllPieces(manager);

// Encontrar todos los managers en la escena
var allManagers = AxisUtilities.FindAllAxisManagers();
foreach(var mgr in allManagers)
{
    Debug.Log($"Manager encontrado: {mgr.name}");
}
```

## Mejores Prácticas

### 🎯 Rendimiento
- Usa filtros para reducir el número de marcadores innecesarios
- Desactiva etiquetas si no las necesitas
- Considera usar presets "Small" para modelos complejos

### 🎨 Visualización
- Ajusta `axisLength` según el tamaño de tu modelo
- Usa colores contrastantes para mejor visibilidad
- Activa solo los ejes que necesites ver

### 🔧 Desarrollo
- Usa `ModelAxisManager` como componente principal
- Implementa `RuntimeAxisController` para interfaces de usuario
- Aprovecha los presets para configuraciones rápidas

## Solución de Problemas

### Los ejes no se ven
- Verifica que `showAxis` esté activado
- Comprueba que el objeto tenga un `AxisMarkerImproved`
- Asegúrate de que `axisLength` sea apropiado para tu escala

### Rendimiento lento
- Reduce el número de marcadores activos
- Desactiva etiquetas si no las necesitas
- Usa filtros más restrictivos

### UI de runtime no aparece
- Verifica que `RuntimeAxisController` esté configurado
- Comprueba que `createUIAutomatically` esté activado
- Asegúrate de que hay un `EventSystem` en la escena

## Extensiones y Personalización

El plugin está diseñado para ser extensible. Puedes:

- Crear nuevos tipos de marcadores heredando de `AxisMarkerImproved`
- Implementar sistemas de guardado/carga de configuraciones
- Agregar nuevos filtros de detección
- Crear interfaces de usuario personalizadas
- Integrar con otros sistemas de visualización

## Licencia

Este plugin es de uso libre para proyectos personales y comerciales.

---

**¿Necesitas ayuda?** Revisa los scripts de ejemplo en la carpeta `Runtime` o consulta los comentarios en el código fuente.
