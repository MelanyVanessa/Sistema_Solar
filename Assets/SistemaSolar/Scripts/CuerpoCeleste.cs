using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace SistemaSolar
{
    public enum EstadoCuerpo { EnOrbita, Agarrado, Libre, Quemandose }

    /// <summary>
    /// Planeta o luna: orbita hasta que se agarra o golpea; luego es un objeto físico libre.
    /// Si se suelta con la mano sobre su propio anillo, vuelve a orbitar.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public class CuerpoCeleste : MonoBehaviour
    {
        [Header("Órbita")]
        public Transform centro;
        public float radioOrbita = 0.5f;
        public float periodoOrbital = 40f;   // segundos por vuelta
        public float anguloInicial;          // grados
        public AnilloOrbita anillo;

        [Header("Rotación propia")]
        public Transform modelo;             // hijo visual que gira sobre su eje
        public float velocidadRotacion = 30f; // grados por segundo

        [Header("Información")]
        public string nombre;
        [TextArea(4, 12)] public string informacion;

        [Header("Ajustes")]
        public float toleranciaAnillo = 0.06f;
        public float velocidadQuieto = 0.25f; // al soltarlo por debajo de esto queda quieto
        public float distanciaMaxima = 20f;

        public EstadoCuerpo Estado { get; private set; }
        public float Radio => transform.lossyScale.x * 0.5f;

        Rigidbody rb;
        XRGrabInteractable grab;
        float angulo;
        int framesTrasSoltar;
        Vector3 escalaInicial;
        Transform raizSistema;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(_ => AlAgarrar());
            grab.selectExited.AddListener(_ => AlSoltar());
            escalaInicial = transform.localScale;
            raizSistema = centro != null ? centro.root : transform.root;
        }

        void Start() => Reiniciar();

        public void Reiniciar()
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            transform.localScale = escalaInicial;
            grab.enabled = true;
            angulo = anguloInicial;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = transform.position = PosicionOrbital(angulo);
            transform.rotation = Quaternion.identity;
            EntrarEnOrbita();
        }

        void EntrarEnOrbita()
        {
            Estado = EstadoCuerpo.EnOrbita;
            rb.freezeRotation = true;
            transform.rotation = Quaternion.identity;
        }

        void PasarALibre()
        {
            Estado = EstadoCuerpo.Libre;
            rb.freezeRotation = false;
        }

        Vector3 PosicionOrbital(float grados)
        {
            float r = grados * Mathf.Deg2Rad;
            return centro.position + new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r)) * radioOrbita;
        }

        void FixedUpdate()
        {
            if (Estado == EstadoCuerpo.EnOrbita)
            {
                if (centro == null || !centro.gameObject.activeInHierarchy) { PasarALibre(); return; }
                angulo += 360f / periodoOrbital * Time.fixedDeltaTime;
                // Se mueve por velocidad (no kinematic) para que los choques se resuelvan con masa real.
                rb.linearVelocity = (PosicionOrbital(angulo) - rb.position) / Time.fixedDeltaTime;
            }
            else if (Estado == EstadoCuerpo.Libre && framesTrasSoltar > 0 && --framesTrasSoltar == 0)
            {
                if (rb.linearVelocity.magnitude < velocidadQuieto)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            if (Estado != EstadoCuerpo.Quemandose && raizSistema != null &&
                Vector3.Distance(transform.position, raizSistema.position) > distanciaMaxima)
                gameObject.SetActive(false);
        }

        void Update()
        {
            if (modelo != null && Estado == EstadoCuerpo.EnOrbita)
                modelo.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.Self);

            if (Estado == EstadoCuerpo.Agarrado && anillo != null)
                anillo.Resaltar(SobreSuAnillo());
        }

        bool SobreSuAnillo()
        {
            if (centro == null || !centro.gameObject.activeInHierarchy) return false;
            Vector3 d = transform.position - centro.position;
            float radial = new Vector2(d.x, d.z).magnitude;
            return Mathf.Abs(radial - radioOrbita) < toleranciaAnillo && Mathf.Abs(d.y) < toleranciaAnillo;
        }

        void AlAgarrar()
        {
            Estado = EstadoCuerpo.Agarrado;
            rb.freezeRotation = false;
            PanelInfo.Instancia?.Mostrar(this);
        }

        void AlSoltar()
        {
            if (Estado == EstadoCuerpo.Quemandose) return;
            PanelInfo.Instancia?.Ocultar(this);
            if (anillo != null) anillo.Resaltar(false);

            if (SobreSuAnillo())
            {
                Vector3 d = transform.position - centro.position;
                angulo = Mathf.Atan2(d.z, d.x) * Mathf.Rad2Deg;
                rb.angularVelocity = Vector3.zero;
                EntrarEnOrbita();
            }
            else
            {
                PasarALibre();
                framesTrasSoltar = 3; // esperar a que XRI aplique la velocidad de lanzamiento
            }
        }

        void OnCollisionEnter(Collision choque)
        {
            // Un cuerpo en órbita golpeado por otro que se mueve sale despedido (choques en cadena).
            if (Estado != EstadoCuerpo.EnOrbita) return;
            var otro = choque.rigidbody != null ? choque.rigidbody.GetComponent<CuerpoCeleste>() : null;
            if (otro != null && otro.Estado != EstadoCuerpo.EnOrbita)
                PasarALibre();
        }

        public void Quemar(ParticleSystem fuego)
        {
            if (Estado == EstadoCuerpo.Quemandose) return;
            if (grab.isSelected)
                grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
            PanelInfo.Instancia?.Ocultar(this);
            if (anillo != null) anillo.Resaltar(false);
            Estado = EstadoCuerpo.Quemandose;
            grab.enabled = false;
            StartCoroutine(Quemandose(fuego));
        }

        IEnumerator Quemandose(ParticleSystem fuego)
        {
            var renderers = GetComponentsInChildren<Renderer>();
            var bloque = new MaterialPropertyBlock();
            Vector3 escala = transform.localScale;
            Vector3 velocidad = rb.linearVelocity * 0.2f;
            rb.isKinematic = true;
            const float duracion = 1.5f;
            for (float t = 0; t < duracion; t += Time.deltaTime)
            {
                float k = t / duracion;
                transform.position += velocidad * Time.deltaTime;
                transform.localScale = escala * (1f - k);
                foreach (var r in renderers)
                {
                    r.GetPropertyBlock(bloque);
                    bloque.SetColor("_Fuego", new Color(1f, 0.4f, 0f, Mathf.Clamp01(k * 3f)));
                    r.SetPropertyBlock(bloque);
                }
                if (fuego != null)
                {
                    fuego.transform.position = transform.position;
                    fuego.Emit(2);
                }
                yield return null;
            }
            foreach (var r in renderers) r.SetPropertyBlock(null);
            gameObject.SetActive(false);
        }
    }
}
