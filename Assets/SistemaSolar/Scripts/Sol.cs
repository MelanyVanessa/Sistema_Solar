using UnityEngine;

namespace SistemaSolar
{
    /// <summary>Quema cualquier cuerpo celeste que lo toque y envía su posición a los shaders.</summary>
    public class Sol : MonoBehaviour
    {
        static readonly int PosicionLuz = Shader.PropertyToID("_Main_Space_Light_Position");

        public ParticleSystem fuego;
        public float velocidadRotacion = 5f;

        void Update()
        {
            Shader.SetGlobalVector(PosicionLuz, transform.position);
            transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);
        }

        void OnTriggerEnter(Collider otro)
        {
            var cuerpo = otro.attachedRigidbody != null ? otro.attachedRigidbody.GetComponent<CuerpoCeleste>() : null;
            if (cuerpo != null) cuerpo.Quemar(fuego);
        }
    }
}
