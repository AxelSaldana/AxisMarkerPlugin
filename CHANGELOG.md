# 📝 Changelog - Advanced Axis Marker Plugin

## [2.0.0] - 2024-01-20

### 🎉 Nueva Versión Completa

Esta es una reescritura completa del plugin original con muchas nuevas características y mejoras.

### ✨ Nuevas Características

#### 🔧 Componentes Principales
- **`AxisMarkerImproved`**: Versión mejorada del marcador original
  - Control individual de ejes X, Y, Z
  - Colores personalizables para cada eje
  - Etiquetas opcionales con offset configurable
  - Grosor de línea ajustable
  - Puntas de flecha mejoradas

- **`ModelAxisManager`**: Sistema de gestión completo
  - Detección automática de piezas del modelo
  - Filtros inteligentes (Renderer, Collider, nombres)
  - Configuración global aplicable a todas las piezas
  - API completa para control programático

- **`RuntimeAxisController`**: Interfaz de usuario para runtime
  - Creación automática de UI Canvas
  - Controles interactivos para el usuario final
  - Sliders para ajuste en tiempo real
  - Lista de piezas con toggles individuales

#### 🎨 Herramientas de Editor
- **`ImprovedAxisManagerWindow`**: Ventana de editor avanzada
  - Lista completa de piezas con búsqueda
  - Configuración global en tiempo real
  - Herramientas de filtrado y selección
  - Controles de ejes individuales (X, Y, Z)

- **`ModelAxisManagerEditor`**: Inspector personalizado
  - Controles rápidos y estadísticas
  - Herramientas de selección avanzadas
  - Presets de configuración rápida
  - Información en Scene View

- **`AxisMarkerImprovedEditor`**: Editor mejorado para marcadores
  - Controles visuales mejorados
  - Handles interactivos en Scene View
  - Configuración de colores por eje
  - Botones de configuración rápida

#### 🛠️ Utilidades y Configuración
- **`AxisUtilities`**: Biblioteca de utilidades
  - Métodos de configuración automática
  - Presets predefinidos (Small, Medium, Large, etc.)
  - Herramientas de debugging
  - Funciones de búsqueda y filtrado

- **`AxisPluginSettings`**: Configuración global
  - ScriptableObject para guardar configuraciones
  - Valores por defecto personalizables
  - Aplicación automática a nuevos marcadores
  - Validación de rangos

- **`AxisPluginMenus`**: Menús contextuales
  - Integración completa con menús de Unity
  - Shortcuts de teclado
  - Validaciones automáticas
  - Herramientas de limpieza

#### 📚 Documentación y Ejemplos
- **`AxisPluginExample`**: Script de ejemplo completo
  - Configuración automática
  - Control por teclado
  - Animaciones de ejes
  - Casos de uso comunes

### 🚀 Mejoras de Rendimiento
- Assembly Definitions para mejor organización
- Batching de operaciones cuando es posible
- Filtros para reducir marcadores innecesarios
- Configuración de intervalos de actualización

### 🎯 Funcionalidades Principales

#### Editor
- ✅ Detección automática de piezas
- ✅ Control individual por pieza y por eje
- ✅ Configuración global con aplicación en lote
- ✅ Filtros inteligentes de detección
- ✅ Búsqueda y selección de piezas
- ✅ Presets de configuración rápida
- ✅ Menús contextuales integrados
- ✅ Shortcuts de teclado

#### Runtime
- ✅ Interfaz de usuario automática
- ✅ Control programático completo
- ✅ Configuración dinámica
- ✅ Eventos y callbacks
- ✅ Optimización de rendimiento

#### Visualización
- ✅ Ejes X (rojo), Y (verde), Z (azul)
- ✅ Colores personalizables
- ✅ Etiquetas opcionales
- ✅ Grosor ajustable
- ✅ Puntas de flecha
- ✅ Punto central
- ✅ Resaltado en selección

### 🔧 API Mejorada

#### Configuración Básica
```csharp
// Una línea para configurar todo
ModelAxisManager manager = AxisUtilities.SetupAxisMarkersForModel(modelo);

// Control básico
manager.ShowAllAxes();
manager.HideAllAxes();
manager.ToggleAllAxes();
```

#### Control Avanzado
```csharp
// Por pieza específica
manager.SetPieceVisibility("NombrePieza", true);

// Por eje individual
marker.SetAxisVisibility(true, false, true); // Solo X y Z

// Configuración global
manager.globalAxisLength = 1.0f;
manager.UpdateGlobalSettings();
```

#### Runtime UI
```csharp
// Crear interfaz automáticamente
RuntimeAxisController controller = AxisUtilities.CreateRuntimeController(manager, true);
```

### 📁 Estructura del Proyecto

```
Assets/Plugins/ObjectAxesViewer/
├── Runtime/
│   ├── AxisMarker.cs (original)
│   ├── AxisMarkerImproved.cs (nuevo)
│   ├── ModelAxisManager.cs (nuevo)
│   ├── RuntimeAxisController.cs (nuevo)
│   ├── AxisUtilities.cs (nuevo)
│   ├── AxisPluginSettings.cs (nuevo)
│   └── ObjectAxesViewer.Runtime.asmdef
├── Editor/
│   ├── AxisMarkerEditor.cs (original)
│   ├── AxisManagerWindow.cs (original)
│   ├── AxisMarkerImprovedEditor.cs (nuevo)
│   ├── ImprovedAxisManagerWindow.cs (nuevo)
│   ├── ModelAxisManagerEditor.cs (nuevo)
│   ├── AxisPluginMenus.cs (nuevo)
│   └── ObjectAxesViewer.Editor.asmdef
└── Examples/
    └── AxisPluginExample.cs (nuevo)
```

### 🎮 Controles de Teclado

#### Editor
- `Ctrl+Alt+T`: Toggle todos los ejes en la escena
- `Ctrl+Alt+S`: Mostrar todos los ejes en la escena
- `Ctrl+Alt+H`: Ocultar todos los ejes en la escena

#### Runtime (con AxisPluginExample)
- `Espacio`: Toggle todos los ejes
- `1`: Mostrar todos los ejes
- `2`: Ocultar todos los ejes
- `Tab`: Cambiar entre presets
- `1-9`: Toggle piezas individuales

### 🛠️ Menús Contextuales

#### GameObject Menu
- `GameObject → Axis Plugin → Add Axis Marker`
- `GameObject → Axis Plugin → Setup Model Axis Manager`
- `GameObject → Axis Plugin → Add Runtime Controller`
- `GameObject → Axis Plugin → Remove All Axis Markers`

#### Tools Menu
- `Tools → Axis Plugin → Open Axis Manager Window`
- `Tools → Axis Plugin → Create Settings Asset`
- `Tools → Axis Plugin → Find All Axis Managers`
- `Tools → Axis Plugin → Cleanup All Axis Markers in Scene`

### 🔄 Compatibilidad

- ✅ Compatible con Unity 2020.3 LTS y superior
- ✅ Funciona en Editor y Runtime
- ✅ Compatible con todos los render pipelines
- ✅ Soporte para prefabs
- ✅ Mantiene compatibilidad con versión original

### 📚 Documentación

- `README.md`: Documentación completa
- `QUICK_START.md`: Guía de inicio rápido
- `CHANGELOG.md`: Este archivo
- Comentarios extensivos en el código
- Ejemplos de uso incluidos

### 🎯 Casos de Uso Soportados

- ✅ Debugging de orientación de objetos
- ✅ Visualización de sistemas de coordenadas
- ✅ Educación y demostración de conceptos 3D
- ✅ Herramientas de desarrollo para equipos
- ✅ Análisis de modelos complejos
- ✅ Vehículos y maquinaria
- ✅ Arquitectura y construcción
- ✅ Animaciones y rigging

---

## [1.0.0] - Versión Original

### Características Básicas
- `AxisMarker`: Componente básico para mostrar ejes
- `AxisMarkerEditor`: Editor simple
- `AxisManagerWindow`: Ventana básica de gestión
- Visualización simple de ejes X, Y, Z

---

**¡Gracias por usar el Advanced Axis Marker Plugin! 🎉**
