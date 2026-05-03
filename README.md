# SleepSimulator

### Lab 7: Laboratorio 7: Scene Management

### Enlace del video: 

https://youtu.be/YFxhw6lGId8

### Niveles jugables:
- Bedroom
- RoadRush
- LucidDream

### Menus:
- MainMenu
- SceneMenu

### Boostrap Managers:

El proyecto utiliza un Bootstrap para inicializar objetos persistentes que se mantienen durante toda la ejecución del juego.

- GameManager
- AudioManager

Se implementó un sistema de Bootstrap para mantener managers persistentes como `GameManager` y `AudioManager`. Y por otro lado, estos managers también son singletowns.

El `GameManager` se encarga de:

- Controlar los cambios de escena.
- Cargar escenas de forma asincrónica.
- Mostrar la pantalla de carga.
- Redirigir al jugador entre niveles.
- Manejar el flujo después de perder o terminar un sueño.

El `AudioManager` se encarga de:

- Reproducir música de fondo.
- Cambiar la música dependiendo de la escena.
- Reproducir la música de la pantalla de carga.
- Mantener el audio activo entre escenas.



### Loading Screen:

Se implementó una escena llamada `LoadingScene`, la cual aparece durante los cambios de escena.

- LoadingScene
    Animations:
    - zzz
    - loading spin

### Additive Scene: 
- AddScene

Esta escena se carga mediante un trigger dentro de un nivel. Cuando el jugador entra al área indicada, se carga la escena adicional sin reemplazar completamente la escena actual.

## Manejo de escenas

El proyecto utiliza un `GameManager` persistente para controlar los cambios de escena. En lugar de cargar las escenas directamente desde cada script, se llama al `GameManager`, el cual se encarga de mostrar la pantalla de carga y realizar la transición de forma asincrónica.

Durante cada cambio de escena:

1. Se carga la escena `LoadingScene`.
2. Se reproduce la música de carga mediante el `AudioManager`.
3. Se inicia la carga asincrónica de la siguiente escena.
4. Se espera un tiempo mínimo para mostrar la animación de carga.
5. Se activa la nueva escena.
6. Se descarga la escena de carga.
7. Se cambia la música según la escena actual.



### Enlace del video: 

[https://youtu.be/5QcZM5y6SS8](https://youtu.be/VnwY8vEvdms)

# Demo de la primera version en Itch.io: 
https://angelargd8.itch.io/sleepsimulator?secret=gMhjAZNXUgFbp9xSiKY1KUWDim8


# Objetivo del juego:
Dormir procurando que el score no llegue tan bajo porque eso indica que no tuvo un descanso reparador.

# Limitaciones y reglas: 
El jugador no puede escoger qué tipo de sueño quiere tener o si quiere guardar el score porque eso depende de su memoria. 
Puede tener un sueño lúcido si tiene un score alto, ya que estos tienen un costo alto.

## Controles
- **W, A, S, D** – Movimiento
- **Space** – Salto
- **Mouse** – Mirar alrededor
- **E** - Abrir puertas/ encender tv/ encender ventilador / tomar o soltar watering cup
- **F** - regar plantas
- **Double Space (LucidDream)** – Volar


# Mini-game: RoadRush
**Mini game endless runner**

### Objetivo
Llegar lo más lejos posible mientras se evitan obstaculos y se obtienen recompensas

- **A / Left Arrow** – moverse a la izquierda
- **D / Right Arrow** – moverse a la derecha
- **Space** – Salto
- **shift** - boost

## Object Pooling
Se utilizó Object Pooling para mejorar el rendimiento, reutilizando objetos en lugar de crearlos y destruirlos constantemente durante el juego.
En este mini‑juego, el Object Pooling se usa principalmente en el sistema SegmentPool:

- Los segmentos de carretera se crean una sola vez al inicio.
- Los segmentos activos se mueven hacia atrás para simular que el jugador avanza. Cuando un segmento queda demasiado atrás del jugador, se mueve al frente en lugar de destruirse.

Las recompensas, como las ruedas, también se reutilizan junto con cada segmento. Cuando el jugador recoge una rueda, esta se desactiva en lugar de destruirse. Cuando el segmento de carretera se recicla, las ruedas dentro de ese segmento se reinician y se activan nuevamente.
Esto ayuda a mantener el juego eficiente.


Recompensas: 
- wheels

Obstaculos: 
- Tumbulos
- Carros




Assets: 
- https://assetstore.unity.com/packages/3d/props/electronics/old-ussr-lamp-110400
- https://assetstore.unity.com/packages/3d/props/basic-bedroom-starterpack-215986
- https://assetstore.unity.com/packages/3d/vehicles/mobile-optimize-free-low-poly-cars-327313
- https://ambientcg.com/view?id=NightSkyHDRI003
- https://assetstore.unity.com/packages/3d/environments/urban/city-voxel-pack-136141
- https://assetstore.unity.com/packages/3d/vegetation/trees/low-poly-tree-pack-57866
- https://assetstore.unity.com/packages/3d/environments/fantasy/idyllic-fantasy-nature-260042
- https://assetstore.unity.com/packages/3d/props/low-poly-3d-wheel-model-pack-247391
