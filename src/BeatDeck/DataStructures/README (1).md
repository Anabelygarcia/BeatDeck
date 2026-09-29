# BeatDeck

Este es mi proyecto para la materia de Estructuras de Datos. Es una aplicación de Windows Forms que simula la cola de reproducción de un DJ (como el "próximo a sonar" de Spotify) y compara tres formas distintas de guardar esa cola:

1. Una **lista ligada simple hecha por mí desde cero** (con mis propias clases `Node<T>` y `SinglyLinkedList<T>`), usando solo referencias entre nodos.
2. La `LinkedList<T>` que ya trae .NET (lista doblemente ligada).
3. La `List<T>` normal de .NET (un arreglo dinámico).

Las mismas operaciones se pueden correr sobre las tres estructuras, y agregué un benchmark para medir con `Stopwatch` cuánto tarda cada una en la vida real. También le puse un reproductor de música de verdad, para que la cola no fuera solo una tabla con datos sino algo que en verdad se pudiera escuchar.

## Por qué hice esto

La idea me surgió pensando en cómo un programa como Rekordbox o Spotify puede meter una canción en "reproducir siguiente" sin que se trabe, aunque la cola tenga miles de canciones. Si usas un arreglo (`List<T>`), insertar algo casi al principio obliga a recorrer y mover todos los elementos siguientes (eso es O(n)). Si usas una lista ligada, solo cambias un par de referencias (eso es O(1)). Quería poder ver esa diferencia con números reales y no solo en teoría, por eso hice el benchmark.

## Qué hace la app

- Agregar una canción al final de la cola.
- Insertarla justo después de la que está sonando ("reproducir a continuación").
- Avanzar a la siguiente canción (quita la primera de la cola).
- Invertir la cola completa, solo cambiando las conexiones entre nodos, sin mover los datos.
- Ordenar las canciones por BPM (de menor a mayor).
- Quitar canciones duplicadas (mismo título), dejando solo la primera.
- Cambiar entre las tres estructuras con unos radio buttons, para probar lo mismo en cada una.
- Un reproductor real abajo de la ventana: play, pausa, stop, siguiente, barra para adelantar/atrasar y volumen.
- Se puede adjuntar un archivo de audio de verdad (mp3, wav, etc.) a cada canción con el botón "Examinar...".
- Si una canción no tiene archivo, el programa genera un sonido corto (un loop electrónico) según el BPM que le pusiste, para que igual se escuche algo.
- Un botón para correr la prueba de rendimiento (por default con 25,000 canciones) y comparar las tres estructuras.
- Se puede importar varias canciones a la vez, una carpeta completa, o simplemente arrastrar archivos a la ventana.

## Cómo está organizado el código

```
src/BeatDeck/
├── Program.cs                 -> aquí arranca el programa
├── Models/Track.cs            -> los datos de una canción
├── Audio/                     -> todo lo del reproductor y el generador de sonido
├── DataStructures/
│   ├── Node.cs                -> el nodo de mi lista ligada
│   └── SinglyLinkedList.cs    -> mi lista ligada hecha desde cero
└── UI/                        -> la ventana y toda la lógica de botones
```

La parte de `DataStructures` no depende de nada de Windows Forms ni del audio, la hice para que fuera reutilizable y solo se enfocara en la lógica de la lista.

## Mi lista ligada

La hice usando solo referencias `Next` entre nodos, sin arreglos ni listas de por medio. Estas son las operaciones que tiene:

- `AddLast` – agrega al final (guardo un puntero al último nodo para que esto sea rápido).
- `PlayNext` – inserta justo después de la cabeza.
- `Advance` – quita y regresa la cabeza de la lista.
- `Reverse` – invierte la lista completa moviendo solo los enlaces (usando los típicos punteros previous / current / next).
- `InsertSorted` – inserta manteniendo el orden.
- `RemoveDuplicates` – quita las canciones repetidas.

## Requisitos para correrlo

- Windows 10 u 11 (usa Windows Forms y `winmm.dll` para el audio).
- .NET SDK instalado.
- Visual Studio (opcional, también se puede correr desde la terminal).

## Cómo correrlo

Desde la carpeta del proyecto:

```
dotnet run --project src/BeatDeck/BeatDeck.csproj
```

O simplemente abrir `BeatDeck.sln` en Visual Studio y darle F5.

Para que el benchmark salga con números más reales conviene correrlo en modo Release:

```
dotnet run -c Release --project src/BeatDeck/BeatDeck.csproj
```

## Cómo se usa

1. Elegir con los radio buttons qué estructura quieres usar (mi lista, `LinkedList<T>` o `List<T>`).
2. Escribir el título y artista de la canción, el BPM y la duración.
3. Agregarla al final o mandarla a "reproducir a continuación".
4. Usar los botones de avanzar, invertir, ordenar o quitar duplicados para mover la cola.
5. Darle play para escuchar la cola desde el reproductor de abajo.
6. Si quieres, adjuntar un archivo de audio real antes de agregar la canción.

## Notas

- Las tres colas empiezan con las mismas tres canciones de ejemplo, pero después cada una es independiente: lo que le haces a una no afecta a las otras.
- El reproductor no abre ni depende de Windows Media Player ni de ningún otro programa, lo hice usando directamente la API de audio de Windows.
- La columna de "memoria/enlace" que se ve en la tabla es solo un identificador para poder seguir los nodos en pantalla, no es una dirección de memoria real.
