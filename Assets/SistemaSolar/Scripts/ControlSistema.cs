using UnityEngine;

namespace SistemaSolar
{
    /// <summary>Devuelve todos los cuerpos a su órbita original.</summary>
    public class ControlSistema : MonoBehaviour
    {
        [Tooltip("En orden: primero los planetas, después las lunas.")]
        public CuerpoCeleste[] cuerpos;

        public void Reiniciar()
        {
            foreach (var c in cuerpos)
                if (c != null) c.Reiniciar();
        }
    }
}
