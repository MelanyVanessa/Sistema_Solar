using TMPro;
using UnityEngine;

namespace SistemaSolar
{
    /// <summary>Panel flotante que muestra la información del planeta agarrado.</summary>
    public class PanelInfo : MonoBehaviour
    {
        public static PanelInfo Instancia { get; private set; }

        public TMP_Text titulo;
        public TMP_Text texto;
        public float separacion = 0.08f;

        CuerpoCeleste actual;
        Transform camara;

        void Awake()
        {
            Instancia = this;
            gameObject.SetActive(false);
        }

        public void Mostrar(CuerpoCeleste cuerpo)
        {
            actual = cuerpo;
            titulo.text = cuerpo.nombre;
            texto.text = cuerpo.informacion;
            gameObject.SetActive(true);
            LateUpdate();
        }

        public void Ocultar(CuerpoCeleste cuerpo)
        {
            if (actual != cuerpo) return;
            actual = null;
            gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (actual == null || !actual.gameObject.activeInHierarchy) { gameObject.SetActive(false); return; }
            if (camara == null && Camera.main != null) camara = Camera.main.transform;
            if (camara == null) return;
            Vector3 haciaCamara = (camara.position - actual.transform.position).normalized;
            Vector3 lado = Vector3.Cross(Vector3.up, haciaCamara).normalized;
            transform.position = actual.transform.position + Vector3.up * (actual.Radio + separacion) - lado * (actual.Radio + 0.12f);
            transform.rotation = Quaternion.LookRotation(transform.position - camara.position, Vector3.up);
        }
    }
}
