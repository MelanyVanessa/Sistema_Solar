using UnityEditor;
using UnityEditor.SceneManagement;

namespace SistemaSolar.Editor
{
    /// <summary>
    /// Al abrir el proyecto recién clonado (escena vacía "Untitled"), abre la escena del sistema solar.
    /// </summary>
    [InitializeOnLoad]
    static class AbrirEscenaAlIniciar
    {
        const string RutaEscena = "Assets/SistemaSolar/SistemaSolar.unity";
        const string Clave = "SistemaSolar.EscenaAbierta";

        static AbrirEscenaAlIniciar() => EditorApplication.delayCall += Abrir;

        static void Abrir()
        {
            if (SessionState.GetBool(Clave, false) || EditorApplication.isPlayingOrWillChangePlaymode) return;
            SessionState.SetBool(Clave, true);

            var activa = EditorSceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(activa.path) && !activa.isDirty &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(RutaEscena) != null)
                EditorSceneManager.OpenScene(RutaEscena);
        }
    }
}
