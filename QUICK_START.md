# 🚀 Guía de Inicio Rápido - Advanced Axis Marker Plugin

## ⚡ Instalación en 30 segundos

1. **Copia el plugin** a tu proyecto Unity:
   ```
   Tu_Proyecto/Assets/Plugins/ObjectAxesViewer/
   ```

2. **Espera** a que Unity compile los scripts automáticamente

3. **¡Listo!** Ya puedes usar el plugin

## 🎯 Uso Básico - 3 Pasos

### Método 1: Usando el Menú Contextual (Más Fácil)

1. **Selecciona tu modelo** en la jerarquía
2. **Click derecho** → `GameObject` → `Axis Plugin` → `Setup Model Axis Manager`
3. **¡Ya está!** Verás los ejes de todas las piezas

### Método 2: Usando la Ventana del Editor

1. **Abre la ventana**: `Window` → `Axis Viewer Manager`
2. **Arrastra tu modelo** al campo "Root Object"
3. **Click "Setup Manager"** → ¡Listo!

### Método 3: Por Código (Para Programadores)

```csharp
// Una sola línea para configurar todo
ModelAxisManager manager = AxisUtilities.SetupAxisMarkersForModel(tuModelo);
```

## 🎮 Controles Rápidos

### En el Editor
- **Espacio**: Toggle todos los ejes
- **Ctrl+Alt+T**: Toggle ejes en toda la escena
- **Ctrl+Alt+S**: Mostrar todos los ejes
- **Ctrl+Alt+H**: Ocultar todos los ejes

### En Runtime (Si usas RuntimeController)
- **Espacio**: Toggle todos los ejes
- **1-9**: Toggle piezas individuales
- **Tab**: Cambiar entre presets

## 🛠️ Configuración Rápida

### Cambiar Tamaño de Ejes
```csharp
manager.globalAxisLength = 1.0f;  // Más grande
manager.UpdateGlobalSettings();
```

### Mostrar Solo Eje Y (Útil para Gravedad)
```csharp
foreach(var piece in manager.ModelPieces)
{
    piece.axisMarker.SetAxisVisibility(false, true, false);
}
```

### Colores Personalizados
```csharp
AxisUtilities.SetCustomAxisColors(marker, Color.cyan, Color.yellow, Color.magenta);
```

## 🎨 Casos de Uso Comunes

### 🚗 Para Vehículos
```csharp
// Configurar automáticamente
var manager = AxisUtilities.SetupAxisMarkersForModel(miCoche);

// Solo ejes de rotación para ruedas
manager.SetPieceVisibility("Rueda", true);
var rueda = manager.GetPieceByName("Rueda");
rueda.axisMarker.SetAxisVisibility(false, false, true); // Solo Z
```

### 🏠 Para Arquitectura
```csharp
// Ejes pequeños para no interferir
manager.globalAxisLength = 0.2f;
manager.showLabels = false;
manager.UpdateGlobalSettings();
```

### 🎓 Para Educación
```csharp
// Ejes grandes y coloridos
var preset = AxisUtilities.Presets.Large;
foreach(var piece in manager.ModelPieces)
{
    preset.ApplyToMarker(piece.axisMarker);
}
```

## 🔧 Solución de Problemas Rápidos

### ❌ "No veo los ejes"
- Verifica que estés en Scene View (no Game View)
- Asegúrate que `showAxis = true`
- Ajusta `axisLength` (puede ser muy pequeño)

### ❌ "Muy lento"
- Reduce el número de piezas con filtros
- Desactiva `showLabels`
- Usa preset "Small"

### ❌ "No detecta las piezas"
- Activa `filterByRenderer = false`
- Activa `includeInactivePieces = true`
- Verifica que el objeto tenga hijos

## 📱 UI de Runtime

Para crear interfaz de usuario automáticamente:

```csharp
// Esto crea UI automáticamente
RuntimeAxisController controller = AxisUtilities.CreateRuntimeController(manager, true);
```

O manualmente:
1. Agrega `RuntimeAxisController` a cualquier GameObject
2. Asigna tu `ModelAxisManager`
3. Activa `createUIAutomatically`

## 🎯 Ejemplos Listos para Usar

Agrega el script `AxisPluginExample` a cualquier GameObject para ver ejemplos funcionando:

- Control por teclado
- Animaciones de ejes
- Configuraciones automáticas
- Y mucho más...

## 📚 ¿Necesitas Más?

- **Documentación completa**: Lee `README.md`
- **Configuración avanzada**: Usa `AxisPluginSettings`
- **Menus contextuales**: Click derecho en GameObjects
- **Ventana avanzada**: `Window → Axis Viewer Manager`

---

## ✨ Tip Pro

**¿Quieres configurar rápidamente un modelo complejo?**

1. Selecciona tu modelo
2. `GameObject` → `Axis Plugin` → `Setup Model Axis Manager`
3. `GameObject` → `Axis Plugin` → `Add Runtime Controller`
4. ¡Presiona Play y tendrás control completo en runtime!

**¡Disfruta visualizando tus ejes! 🎉**
