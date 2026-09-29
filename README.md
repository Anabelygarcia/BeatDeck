# 🎧 BeatDeck GUI

### DJ Queue & Transition Manager — Singly Linked List vs. Native .NET Collections

> **TecNM Campus Monclova — Computer Engineering**
> **Course:** Data Structures (3rd semester) · **Unit 2:** Linear Data Structures
> **Version:** 2.0.0

A Windows Forms application that models a DJ play queue and compares three ways to store it:

1. A **singly linked list built from scratch** (`SinglyLinkedList<T>` and `Node<T>`), using only node references.
2. **.NET `LinkedList<T>`** (doubly linked list from the standard library).
3. **.NET `List<T>`** (dynamic array).

The same queue operations run on all three, a built-in **benchmark** measures the real cost of each with `Stopwatch`, and a **music player** at the bottom of the window actually plays the queue.

---

## Why this project?

Professional DJ software (Rekordbox, Serato, Spotify) must let a user insert a song as "play next" in a queue of thousands of tracks without any lag. With a dynamic array, inserting at position 1 shifts every following element (`Array.Copy`, **O(n)**). With a linked list, only two references change (**O(1)**). This project lets you see that difference with your own eyes.

---

## Features

| Feature | Description |
| :-- | :-- |
| **Agregar al final** | Adds a track at the end of the queue. |
| **Reproducir a continuación** | Inserts a track right after the head (position 2). |
| **Avanzar canción** | Removes the head and shows it as "Now playing". |
| **Reverse List (In-Place)** | Reverses the queue by redirecting links only. |
| **Ordenar por curva de BPM** | Orders tracks from lowest to highest BPM (stable). |
| **Eliminar duplicados** | Removes repeated titles (case-insensitive), keeping the first one. |
| **Structure switch** | Radio buttons change which structure the buttons operate on; the grid refreshes instantly. |
| **Memory / Link column** | Shows each node's identity tag and the tag of its `Next` node (custom list mode). |
| **Built-in music player** | Bottom bar with Play/Pause, Stop, Next, a seek bar, elapsed/total time and volume. When a track ends, the next one in the queue starts automatically. |
| **Audio file per track** | Optional **Examinar...** button to attach an MP3 / WAV / WMA file. The track duration is read from the file automatically. |
| **Synthesized beat** | Tracks without a file still make sound: the app generates a short electronic loop at the track's BPM (kick, hi-hat, bass, arpeggio). |
| **Benchmark** | Stress test with a configurable number of tracks (1,000 – 100,000; default 25,000), run on a background thread so the window never freezes. |
| **Input validation** | Empty title or artist is rejected with a friendly message. |
| **Safe empty-queue handling** | Advancing an empty queue shows a message box instead of crashing. |

---

## The music player

The player is part of the application itself. It does **not** open, embed or depend on Windows Media Player, the Windows "Music" app or any other program.

- **Engine:** it uses the multimedia API that ships with Windows (`winmm.dll`, MCI) through P/Invoke in `Audio/AudioPlayer.cs`. No NuGet packages are needed.
- **Controls:** Play / Pause, Stop, Next, seek bar (drag to jump), volume slider.
- **Queue integration:** *Advance Track*, *Next* and the end of a song all take the head of the queue that is currently selected, show it in "Now playing" and start playing it. Pressing **Play** with nothing loaded also starts the queue.
- **Tracks without a file:** `Audio/ToneGenerator.cs` builds a WAV loop from scratch (maximum 45 seconds, cached in a temporary folder that is deleted when the app closes). Each track gets its own key and pattern, so they sound different.
- **Supported formats:** whatever Windows can decode through MCI. **MP3 and WAV** are the safest choice; WMA usually works too.

---

## Requirements

| Item | Specification |
| :-- | :-- |
| OS | Windows 10 / 11 (Windows Forms + `winmm.dll` audio) |
| Audio | Any working sound output device |
| SDK | [.NET 10 SDK](https://dotnet.microsoft.com/download) |
| Language | C# 14 |
| IDE (optional) | Visual Studio 2026 or later, or any editor + `dotnet` CLI |
| Dependencies | None. Only the .NET Base Class Library is used. |

---

## Build and run

From the repository root:

```bash
# Run in Debug
dotnet run --project src/BeatDeck/BeatDeck.csproj

# Recommended for benchmarking: Release mode
dotnet run -c Release --project src/BeatDeck/BeatDeck.csproj
```

Or open `BeatDeck.sln` in Visual Studio and press **F5**.

> 💡 Benchmark numbers are more meaningful in **Release** mode, because Debug builds add overhead that hides real differences.

---

## Project structure

```text
BeatDeck/
├── BeatDeck.sln
├── README.md
└── src/
    └── BeatDeck/
        ├── BeatDeck.csproj
        ├── Program.cs                    // Entry point
        ├── Models/
        │   └── Track.cs                  // Immutable record with track metadata (+ optional audio file)
        ├── Audio/
        │   ├── AudioPlayer.cs            // Built-in player (MCI wrapper: play, pause, seek, volume)
        │   ├── AudioFormats.cs           // Known audio extensions + folder scanning
        │   ├── AudioImporter.cs          // Reads duration / title / artist of an audio file to import it
        │   └── ToneGenerator.cs          // Synthesizes a beat for tracks without an audio file
        ├── DataStructures/
        │   ├── Node.cs                   // Generic self-referencing node
        │   └── SinglyLinkedList.cs       // Linked list algorithms, from scratch
        └── UI/
            ├── MainForm.cs               // UI logic, queue actions, benchmark
            └── MainForm.Designer.cs      // WinForms layout
```

**Layering:** `DataStructures` and `Models` know nothing about Windows Forms or audio, so the linked list is fully reusable. Only `UI` depends on the other layers.

---

## The custom linked list

`SinglyLinkedList<T>` works exclusively with `Node<T>.Next` references. It uses **no internal arrays and no `List<T>`**.

| Method | Purpose | Time | Extra space |
| :-- | :-- | :-- | :-- |
| `AddLast(T)` | Append at the end (uses a tail reference) | O(1) | O(1) |
| `PlayNext(T)` | Insert right after the head ("Up Next") | O(1) | O(1) |
| `Advance()` | Remove and return the head | O(1) | O(1) |
| `Reverse()` | Reverse in place with `previous` / `current` / `next` | O(n) | O(1) |
| `InsertSorted(T, cmp)` | Insert keeping sorted order (stable) | O(n) | O(1) |
| `SortInPlace(cmp)` | Sort by relinking existing nodes (extra method) | O(n²) | O(1) |
| `RemoveDuplicates(eq)` | Remove repeated items, keep the first | O(n²) | O(1) |
| `Clear()` | Empty the list | O(1) | O(1) |

### How `Reverse()` works

```csharp
Tail = Head;                      // old head becomes the tail
Node<T>? previous = null;
Node<T>? current = Head;
Node<T>? next = null;

while (current != null)
{
    next = current.Next;          // 1. remember the rest of the list
    current.Next = previous;      // 2. flip the arrow
    previous = current;           // 3. advance 'previous'
    current = next;               // 4. advance 'current'
}
Head = previous;                  // 5. the last node visited is the new head
```

No new nodes, no auxiliary lists, and no swapped values — only links change.

---

## How to use the app

1. **Pick a structure** with the radio buttons (custom list, `LinkedList<T>` or `List<T>`).
2. **Type a title and artist**, choose the BPM (60–220) and duration in seconds.
3. Press **+ Agregar al final** or **Reproducir a continuación**.
4. Use **Avanzar canción**, **Invertir lista**, **Ordenar por curva de BPM** or **Eliminar duplicados** to manipulate the queue.
5. Press **Avanzar canción** (or **Play** in the player bar) to start playing the queue. Use **Pause**, **Stop**, **Next**, the seek bar and the volume slider as in any music player.
6. Optionally press **Examinar...** before enqueueing to attach a real audio file to a track.
7. **Import songs** in bulk: **Importar canciones...** (select several files), **Importar carpeta...** (includes subfolders) or simply **drag files / folders** onto the window. Each readable song is added at the end of the selected structure; title and artist come from the file name (`Artist - Title.mp3`), the duration from the file and the BPM from the form.
8. Press **Ejecutar prueba de rendimiento** to compare all three structures.

> 🎵 **Supported formats:** MP3, WAV, WMA, AAC, M4A, FLAC, OGG, OPUS, AIFF, MIDI and more. Playback goes through Windows (MCI / DirectShow), so a format only works if Windows has its codec (e.g. OGG / OPUS may need an extra codec pack). Files Windows cannot open are skipped during import and reported.

> ℹ️ The three queues start with the same three demo tracks but are **independent afterwards**: an action only affects the structure currently selected. Switch the radio button to see the other queues.

---

## Benchmark

The stress test measures two scenarios for each structure:

- **Middle insertion** (right after the head) — where linked lists should win.
- **Insertion at the end** — where all three are expected to perform well.

Before measuring, the app runs a small warm-up so JIT compilation is not counted. Results appear in the text box at the bottom, together with a short technical conclusion.

Illustrative output (your numbers will differ):

```text
=== STRESS TEST RESULTS (25,000 TRACKS) ===

Middle insertion (right after the head, "Up Next"):
  Custom singly linked list :      1.10 ms   [O(1) each: relink 2 references]
  .NET LinkedList<T>        :      1.60 ms   [O(1) each: AddAfter on a node]
  .NET List<T> (array)      :     45.00 ms   [O(n) each: Array.Copy shifts the tail]
```

---

## Manual test cases

| ID | Scenario | Steps | Expected result |
| :-- | :-- | :-- | :-- |
| **CP-01** | Insert at end | Enter *"Track 1"* (120 BPM) → **Agregar al final** | Appears in the last row; other rows unchanged. |
| **CP-02** | Up Next | Enter *"Track VIP"* → **Reproducir a continuación** | Appears in row 2, right behind the head. |
| **CP-03** | Advance | Press **Avanzar canción** | "Now playing" label updates; first row disappears. |
| **CP-04** | In-place reverse | Queue with BPMs `[100, 110, 120]` → **Invertir lista** | Grid shows `[120, 110, 100]`. |
| **CP-05** | BPM curve | Insert `128, 115, 140, 120` → **Ordenar por curva de BPM** | Grid shows `115 → 120 → 128 → 140`. |
| **CP-06** | Purge duplicates | Two tracks titled *"Strobe"* → **Eliminar duplicados** | First one kept, replica removed, links intact. |
| **CP-07** | Empty queue | **Avanzar canción** on an empty queue | Informational message box; no crash. |
| **CP-08** | Play from queue | Press **Avanzar canción** with the demo queue | The head track starts playing; the player bar shows its name and running time. |
| **CP-09** | Pause / Resume | Press **Pause**, wait, then **Play** | Time freezes while paused and continues from the same point. |
| **CP-10** | Auto-advance | Let a short track finish | The next track in the queue starts automatically. At the end: "End of setlist". |
| **CP-11** | Real audio file | **Examinar...** → pick an MP3 → **Agregar al final** → play it | The duration is filled in from the file and the song plays inside the app. |
| **CP-12** | Import several songs | **Importar canciones...** → select 3+ files | All readable songs appear at the end of the active queue with real durations; a summary message shows the count. |
| **CP-13** | Import a folder | **Importar carpeta...** → pick a folder with subfolders | Every audio file found (including subfolders) is added. |
| **CP-14** | Drag & drop | Drag songs or a folder from Explorer onto the window | Same result as CP-12 / CP-13. |
| **CP-15** | Unreadable file | Import a `.txt` file renamed to `.mp3` | It is skipped and the summary reports it; no crash. |

---

## Design notes

- **Tail reference:** the custom list keeps a `Tail` pointer so `AddLast` is O(1). Every operation that can change the last node (`PlayNext`, `Advance`, `Reverse`, sorting, purging) updates it.
- **Fair comparison:** sorting with `LinkedList<T>` and `List<T>` uses the stable `OrderBy`, so tracks with equal BPM stay in the same order as in the custom list. Duplicate purging is case-insensitive in all three structures.
- **In-place reverse for `LinkedList<T>`:** `LinkedList<T>` has no `Reverse()`, so the app moves each following node to the front, without any temporary collection.
- **No external player:** playback goes through Windows' low-level MCI audio API, not through the Media Player control or the Music app. Using a library such as NAudio would be an alternative, but it would add a NuGet dependency.
- **"Memory / Link" column:** the tag shown for each node is the object's identity hash code. It identifies nodes consistently, but it is **not** a real memory address.

---

## Authors

| Name | TecNM control number |
| :-- | :-- |
| _Your name here_ | _XXXXXXXX_ |
| _Teammate name here_ | _XXXXXXXX_ |

> Remember to replace the placeholder names and control numbers in the header comment of every source file.

## License

Educational project for TecNM Campus Monclova. Add your preferred license here if you publish the repository.
