using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace SistemaSolar
{
    /// <summary>Botón 3D: al presionarlo (tocar, apuntar o agarrar) reinicia el sistema.</summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class BotonReiniciar : MonoBehaviour
    {
        public ControlSistema sistema;
        public Transform tapa;
        public Renderer render;
        public Color colorNormal = new Color(0.8f, 0.1f, 0.1f);
        public Color colorEncima = new Color(1f, 0.4f, 0.3f);

        MaterialPropertyBlock bloque;
        bool ocupado;

        void Awake()
        {
            bloque = new MaterialPropertyBlock();
            var interactuable = GetComponent<XRSimpleInteractable>();
            interactuable.selectEntered.AddListener(_ => Presionar());
            interactuable.hoverEntered.AddListener(_ => Pintar(colorEncima));
            interactuable.hoverExited.AddListener(_ => Pintar(colorNormal));
            Pintar(colorNormal);
        }

        void Pintar(Color c)
        {
            if (render == null) return;
            render.GetPropertyBlock(bloque);
            bloque.SetColor("_BaseColor", c);
            render.SetPropertyBlock(bloque);
        }

        void Presionar()
        {
            if (ocupado) return;
            sistema.Reiniciar();
            StartCoroutine(Hundir());
        }

        IEnumerator Hundir()
        {
            ocupado = true;
            Vector3 inicio = tapa.localPosition;
            tapa.localPosition = inicio + Vector3.down * 0.01f;
            yield return new WaitForSeconds(0.3f);
            tapa.localPosition = inicio;
            ocupado = false;
        }
    }
}
