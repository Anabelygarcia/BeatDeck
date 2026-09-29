using System.Collections;

namespace BeatDeck.DataStructures
{
    /// <summary>
    /// Lista simplemente enlazada genérica implementada únicamente con referencias de tipo <see cref="Node{T}"/>.
    /// No utiliza arreglos internos, ni colecciones de System.Collections.Generic, ni estructuras auxiliares.
    /// Mantiene una referencia al último nodo (Tail) para que la inserción al final sea de complejidad O(1).
    /// </summary>
    /// <typeparam name="T">Tipo de elemento que almacena la lista.</typeparam>
    public class SinglyLinkedList<T> : IEnumerable<T>
    {
        /// <summary>Referencia al primer nodo de la lista (cabeza).</summary>
        public Node<T>? Head { get; private set; }

        /// <summary>Referencia al último nodo de la lista (cola).</summary>
        public Node<T>? Tail { get; private set; }

        /// <summary>Obtiene la cantidad total de elementos en la lista.</summary>
        public int Count { get; private set; }

        /// <summary>Indica si la lista está vacía (true si Head es null).</summary>
        public bool IsEmpty => Head is null;

        // =========================================================================
        // 1. Agregar al final: O(1) gracias a la referencia al último nodo (Tail).
        // =========================================================================
        public void AddLast(T value)
        {
            var node = new Node<T>(value);
            if (Tail is null)
            {
                // Si la lista está vacía, el nuevo nodo es tanto el inicio como el fin.
                Head = Tail = node;
            }
            else
            {
                // Enlaza el nuevo nodo después del Tail actual y actualiza la referencia de Tail.
                Tail.Next = node;
                Tail = node;
            }
            Count++;
        }

        // =========================================================================
        // 2. "A continuación" (PlayNext): inserta justo después de la cabeza: O(1).
        // =========================================================================
        public void PlayNext(T value)
        {
            var node = new Node<T>(value);
            if (Head is null)
            {
                Head = Tail = node;
            }
            else
            {
                // Inserta el nuevo nodo entre Head y Head.Next.
                node.Next = Head.Next;
                Head.Next = node;

                // Si la lista solo tenía un nodo, el nuevo nodo insertado pasa a ser el último (Tail).
                if (ReferenceEquals(Tail, Head)) Tail = node;
            }
            Count++;
        }

        // =========================================================================
        // 3. Avanzar la cola (remover la cabeza): O(1).
        // =========================================================================
        public T Advance()
        {
            if (Head is null)
                throw new InvalidOperationException("La cola de reproducción está vacía.");

            T value = Head.Value;
            Head = Head.Next; // La cabeza pasa a ser el siguiente nodo.

            if (Head is null) Tail = null; // Si no quedan elementos, Tail también es null.
            Count--;
            return value;
        }

        // =========================================================================
        // 4. Inversión en el sitio (In-place reversal): Tiempo O(n), Espacio O(1).
        // Solo reorienta los enlaces 'Next': sin nodos nuevos, sin listas auxiliares ni intercambio de valores.
        // =========================================================================
        public void Reverse()
        {
            Tail = Head; // La antigua cabeza se convierte en la nueva cola.

            Node<T>? previous = null;
            Node<T>? current = Head;
            Node<T>? next = null;

            while (current != null)
            {
                next = current.Next;     // 1. Guarda la referencia al resto de la lista.
                current.Next = previous; // 2. Invierte la flecha del enlace.
                previous = current;      // 3. Avanza la referencia del nodo previo.
                current = next;          // 4. Avanza al siguiente nodo a procesar.
            }

            Head = previous;             // 5. El último nodo visitado pasa a ser la nueva cabeza.
        }

        // =========================================================================
        // 5. Inserción ordenada: O(n). Mantiene el orden de llegada para elementos iguales (estable).
        // =========================================================================
        public void InsertSorted(T value, Comparison<T> comparison)
        {
            LinkSorted(new Node<T>(value), comparison);
            Count++;
        }

        /// <summary>
        /// Ordena toda la lista reordenando sus nodos EXISTENTES mediante ordenamiento por inserción (Insertion Sort).
        /// Complejidad temporal O(n²), complejidad espacial O(1).
        /// </summary>
        public void SortInPlace(Comparison<T> comparison)
        {
            Node<T>? remaining = Head;
            Head = null;
            Tail = null;

            while (remaining != null)
            {
                Node<T>? next = remaining.Next; // Guarda la referencia antes de que LinkSorted sobrescriba remaining.Next.
                LinkSorted(remaining, comparison);
                remaining = next;
            }
        }

        // =========================================================================
        // 6. Eliminar duplicados sin estructuras externas: Tiempo O(n²), Espacio O(1).
        // Conserva la primera aparición del elemento. Devuelve la cantidad de nodos eliminados.
        // =========================================================================
        public int RemoveDuplicates(Func<T, T, bool> areEqual)
        {
            int removed = 0;
            Node<T>? current = Head;

            while (current != null)
            {
                Node<T> runner = current;
                while (runner.Next != null)
                {
                    if (areEqual(current.Value, runner.Next.Value))
                    {
                        // Si el duplicado es la cola actual, ajusta Tail al nodo anterior.
                        if (ReferenceEquals(runner.Next, Tail)) Tail = runner;

                        runner.Next = runner.Next.Next; // Omite (desenlaza) el nodo duplicado.
                        Count--;
                        removed++;
                    }
                    else
                    {
                        runner = runner.Next;
                    }
                }
                current = current.Next;
            }

            return removed;
        }

        /// <summary>Limpia la lista eliminando todas las referencias de inicio y fin.</summary>
        public void Clear()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        /// <summary>
        /// Reubica un nodo ya existente dentro de su posición ordenada correspondiente.
        /// No modifica la propiedad Count.
        /// </summary>
        private void LinkSorted(Node<T> node, Comparison<T> comparison)
        {
            if (Head is null)
            {
                node.Next = null;
                Head = Tail = node;
                return;
            }

            // Si debe ir antes del primer elemento.
            if (comparison(node.Value, Head.Value) < 0)
            {
                node.Next = Head;
                Head = node;
                return;
            }

            // Busca el punto de inserción adecuado.
            Node<T> current = Head;
            while (current.Next != null && comparison(node.Value, current.Next.Value) >= 0)
                current = current.Next;

            node.Next = current.Next;
            current.Next = node;
            if (node.Next is null) Tail = node;
        }

        // =========================================================================
        // Implementación de IEnumerable para permitir bucles foreach y DataBinding
        // =========================================================================
        public IEnumerator<T> GetEnumerator()
        {
            Node<T>? current = Head;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}