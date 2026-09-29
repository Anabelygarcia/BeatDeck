namespace BeatDeck.DataStructures
{
    /// <summary>
    /// Un nodo autorreferenciado: almacena un valor genérico y un enlace/puntero hacia el siguiente nodo de la lista.
    /// Es el bloque fundamental de construcción para la lista simplemente enlazada.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que almacenará el nodo (ej. la clase Track).</typeparam>
    public class Node<T>
    {
        /// <summary>El valor o datos contenidos en este nodo.</summary>
        public T Value { get; set; }

        /// <summary>Referencia al siguiente nodo de la lista, o null si es el último elemento.</summary>
        public Node<T>? Next { get; set; }

        /// <summary>
        /// Inicializa un nuevo nodo con el valor especificado y la referencia al siguiente nodo en null.
        /// </summary>
        /// <param name="value">Elemento a guardar dentro del nodo.</param>
        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }
}