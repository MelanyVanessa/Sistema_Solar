using UnityEngine;

namespace SistemaSolar
{
    /// <summary>Anillo blanco que dibuja la órbita y se resalta cuando el planeta está encima.</summary>
    [RequireComponent(typeof(LineRenderer))]
    public class AnilloOrbita : MonoBehaviour
    {
        public Transform centro;
        public float radio = 0.5f;
        public int segmentos = 96;
        public float grosor = 0.003f;
        public Color colorNormal = new Color(1f, 1f, 1f, 0.35f);
        public Color colorResaltado = new Color(0.4f, 1f, 1f, 1f);

        LineRenderer linea;

        void Awake()
        {
            linea = GetComponent<LineRenderer>();
            linea.loop = true;
            linea.useWorldSpace = false;
            linea.positionCount = segmentos;
            for (int i = 0; i < segmentos; i++)
            {
                float a = i * Mathf.PI * 2f / segmentos;
                linea.SetPosition(i, new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radio);
            }
            Resaltar(false);
        }

        void LateUpdate()
        {
            if (centro == null) return;
            linea.enabled = centro.gameObject.activeInHierarchy;
            transform.SetPositionAndRotation(centro.position, Quaternion.identity);
        }

        public void Resaltar(bool activo)
        {
            linea.startColor = linea.endColor = activo ? colorResaltado : colorNormal;
            linea.widthMultiplier = activo ? grosor * 3f : grosor;
        }
    }
}
