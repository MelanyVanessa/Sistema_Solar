using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.EventSystems;

namespace SistemaSolar.Editor
{
    /// <summary>Menú "Sistema Solar > Construir escena": genera materiales, objetos y la escena completa.</summary>
    public static class ConstructorSistemaSolar
    {
        const string Raiz = "Assets/SistemaSolar";
        const string Gen = Raiz + "/Generado";
        const string Tex = "Assets/AHProxy/PlanetShader/DemoScene/Textures/";
        const string TexPropias = Raiz + "/Texturas/";
        const string RutaEscena = Raiz + "/SistemaSolar.unity";
        const string RutaXROrigin = "Assets/VRTemplateAssets/Prefabs/Setup/Complete XR Origin Set Up Variant.prefab";
        const string RutaSkybox = "Assets/AHProxy/PlanetShader/DemoScene/Materials/DemoSceneSkybox.mat";

        // Tamaño del sistema: multiplica diámetros y distancias.
        const float Escala = 2f;
        static readonly Vector3 PosicionSistema = new Vector3(0f, 1.1f, 3.6f);

        struct Datos
        {
            public string nombre, textura, info;
            public float diametro, radioOrbita, periodo, giro, inclinacion, angulo;
            public Color tinte;
            public bool anillos;
        }

        static Datos D(string nombre, string textura, float diametro, float radio, float periodo, float giro,
            float inclinacion, float angulo, string info, bool anillos = false, Color? tinte = null) => new Datos
        {
            nombre = nombre, textura = textura, diametro = diametro * Escala, radioOrbita = radio * Escala, periodo = periodo,
            giro = giro, inclinacion = inclinacion, angulo = angulo, info = info, anillos = anillos,
            tinte = tinte ?? Color.white
        };

        static string Info(string tipo, string diametro, string distancia, string dia, string anio, string temp, string lunas, string dato) =>
            $"<b>Tipo:</b> {tipo}\n<b>Diámetro:</b> {diametro}\n<b>Distancia al Sol:</b> {distancia}\n<b>Día:</b> {dia}\n" +
            $"<b>Año:</b> {anio}\n<b>Temperatura:</b> {temp}\n<b>Lunas:</b> {lunas}\n\n<i>¿Sabías que...?</i> {dato}";

        static readonly Datos[] Planetas =
        {
            D("Mercurio", Tex + "Mercury/4k_mercury.png", 0.05f, 0.25f, 20f, 5f, 0.03f, 30f,
                Info("Planeta rocoso", "4.879 km", "58 millones de km", "59 días terrestres", "88 días", "-173 °C a 427 °C", "0",
                    "Es el planeta más pequeño y el más cercano al Sol.")),
            D("Venus", Tex + "Venus/4k_venus_atmosphere.png", 0.07f, 0.33f, 31f, 3f, 177f, 140f,
                Info("Planeta rocoso", "12.104 km", "108 millones de km", "243 días (gira al revés)", "225 días", "464 °C", "0",
                    "Es el planeta más caliente: su atmósfera de dióxido de carbono atrapa el calor.")),
            D("Tierra", Tex + "Earth/4k_earth_daymap.png", 0.075f, 0.47f, 40f, 36f, 23.4f, 250f,
                Info("Planeta rocoso", "12.742 km", "150 millones de km", "24 horas", "365 días", "15 °C en promedio", "1",
                    "Es el único planeta conocido con vida y con agua líquida en su superficie.")),
            D("Marte", Tex + "Mars/4k_mars.png", 0.06f, 0.61f, 55f, 35f, 25.2f, 60f,
                Info("Planeta rocoso", "6.779 km", "228 millones de km", "24 horas y 37 minutos", "687 días", "-63 °C en promedio", "2 (Fobos y Deimos)",
                    "Tiene el volcán más alto del sistema solar: el Monte Olimpo.")),
            D("Júpiter", Tex + "Jupiter/4k_jupiter.png", 0.16f, 0.82f, 138f, 57f, 3.1f, 190f,
                Info("Gigante gaseoso", "139.820 km", "778 millones de km", "10 horas", "12 años", "-110 °C", "Más de 90",
                    "Su Gran Mancha Roja es una tormenta más grande que la Tierra.")),
            D("Saturno", Tex + "Saturn/4k_saturn.png", 0.14f, 1.05f, 217f, 54f, 26.7f, 320f,
                Info("Gigante gaseoso", "116.460 km", "1.430 millones de km", "10 horas y 40 minutos", "29 años", "-140 °C", "Más de 200",
                    "Es tan poco denso que flotaría en el agua."), anillos: true),
            D("Urano", Tex + "Uranus/2k_uranus.png", 0.10f, 1.25f, 367f, 44f, 97.8f, 100f,
                Info("Gigante helado", "50.724 km", "2.870 millones de km", "17 horas", "84 años", "-195 °C", "Más de 25",
                    "Gira acostado: su eje está inclinado casi 98 grados.")),
            D("Neptuno", Tex + "Neptune/2k_neptune.png", 0.10f, 1.42f, 513f, 43f, 28.3f, 220f,
                Info("Gigante helado", "49.244 km", "4.500 millones de km", "16 horas", "165 años", "-200 °C", "16",
                    "Tiene los vientos más rápidos del sistema solar: más de 2.000 km/h.")),
            D("Plutón", TexPropias + "2k_moon.jpg", 0.05f, 1.58f, 630f, 9f, 122.5f, 10f,
                Info("Planeta enano", "2.377 km", "5.900 millones de km", "6 días y 9 horas", "248 años", "-230 °C", "5",
                    "Desde 2006 es un planeta enano. Tiene un enorme glaciar con forma de corazón."),
                tinte: new Color(0.95f, 0.8f, 0.65f)),
        };

        static readonly Datos Luna = D("Luna", TexPropias + "2k_moon.jpg", 0.03f, 0.075f, 12f, 30f, 6.7f, 0f,
            "<b>Tipo:</b> Satélite natural\n<b>Diámetro:</b> 3.474 km\n<b>Distancia a la Tierra:</b> 384.400 km\n" +
            "<b>Día:</b> 27 días\n<b>Vuelta a la Tierra:</b> 27 días\n<b>Temperatura:</b> -173 °C a 127 °C\n\n" +
            "<i>¿Sabías que...?</i> Siempre vemos la misma cara de la Luna.");

        [MenuItem("Sistema Solar/Construir escena")]
        public static void Construir()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Directory.CreateDirectory(Gen);

            var shader = Shader.Find("SistemaSolar/Planeta");
            var fisica = CrearFisica();
            var esfera = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");

            var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigurarEntorno();
            CrearXR();

            var sistema = new GameObject("Sistema Solar");
            sistema.transform.position = PosicionSistema;
            var control = sistema.AddComponent<ControlSistema>();

            // Sol
            var texSol = Textura(TexPropias + "2k_sun.jpg", 2048);
            var matSol = CrearMaterial("Sol", shader, texSol, Color.white);
            matSol.SetFloat("_Emisivo", 1f);
            var sol = Modelo("Sol", esfera, matSol, sistema.transform);
            sol.transform.localPosition = Vector3.zero;
            sol.transform.localScale = Vector3.one * 0.3f * Escala;
            sol.AddComponent<SphereCollider>().isTrigger = true;
            var compSol = sol.AddComponent<Sol>();
            compSol.fuego = CrearFuego(sistema.transform);

            var matAnillo = MaterialTransparente("Anillo Orbita", "Universal Render Pipeline/Particles/Unlit", null, false);
            var cuerpos = new CuerpoCeleste[Planetas.Length + 1];
            CuerpoCeleste tierra = null;
            for (int i = 0; i < Planetas.Length; i++)
            {
                cuerpos[i] = CrearCuerpo(Planetas[i], sol.transform, sistema.transform, esfera, shader, fisica, matAnillo);
                if (Planetas[i].nombre == "Tierra") tierra = cuerpos[i];
            }
            var luna = CrearCuerpo(Luna, tierra.transform, sistema.transform, esfera, shader, fisica, matAnillo);
            luna.toleranciaAnillo = 0.03f * Escala;
            cuerpos[Planetas.Length] = luna;
            control.cuerpos = cuerpos;

            CrearPanel();
            CrearBoton(control);

            EditorSceneManager.SaveScene(escena, RutaEscena);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(RutaEscena, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Sistema Solar: escena construida en " + RutaEscena);
        }

        // ---------- Entorno ----------

        static void ConfigurarEntorno()
        {
            RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(RutaSkybox);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.fog = false;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;

            // Suelo invisible para que la locomoción no caiga al vacío.
            var suelo = new GameObject("Suelo invisible");
            suelo.transform.position = new Vector3(0f, -0.05f, 0f);
            suelo.AddComponent<BoxCollider>().size = new Vector3(60f, 0.1f, 60f);
            // Teletransporte a cualquier punto del suelo (además del joystick de la plantilla).
            suelo.AddComponent<TeleportationArea>().interactionLayers = InteractionLayerMask.GetMask("Teleport");
        }

        static void CrearXR()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RutaXROrigin);
            if (prefab != null) PrefabUtility.InstantiatePrefab(prefab);
            else Debug.LogError("No se encontró el XR Origin en " + RutaXROrigin);

            if (UnityEngine.Object.FindAnyObjectByType<XRInteractionManager>() == null)
                new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<XRUIInputModule>();
            }
        }

        // ---------- Cuerpos ----------

        static CuerpoCeleste CrearCuerpo(Datos d, Transform centro, Transform padre, Mesh esfera, Shader shader,
            PhysicsMaterial fisica, Material matAnillo)
        {
            var raiz = new GameObject(d.nombre);
            raiz.transform.SetParent(padre, false);
            raiz.transform.localScale = Vector3.one * d.diametro;
            var col = raiz.AddComponent<SphereCollider>();
            col.radius = 0.5f;
            col.sharedMaterial = fisica;

            var rb = raiz.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.mass = Mathf.Max(0.1f, Mathf.Pow(d.diametro / 0.075f, 3f));
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var grab = raiz.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.throwOnDetach = true;
            grab.useDynamicAttach = true;

            var eje = new GameObject("Eje").transform;
            eje.SetParent(raiz.transform, false);
            eje.localRotation = Quaternion.Euler(0f, 0f, d.inclinacion);

            var tex = Textura(d.textura, 2048);
            var modelo = Modelo("Modelo", esfera, CrearMaterial(d.nombre, shader, tex, d.tinte), eje);
            if (d.anillos) CrearAnillosSaturno(modelo.transform);

            var anillo = new GameObject("Órbita " + d.nombre);
            anillo.transform.SetParent(padre, false);
            var linea = anillo.AddComponent<LineRenderer>();
            linea.sharedMaterial = matAnillo;
            linea.shadowCastingMode = ShadowCastingMode.Off;
            linea.receiveShadows = false;
            var compAnillo = anillo.AddComponent<AnilloOrbita>();
            compAnillo.centro = centro;
            compAnillo.radio = d.radioOrbita;
            compAnillo.grosor = 0.003f * Escala;

            var cuerpo = raiz.AddComponent<CuerpoCeleste>();
            cuerpo.centro = centro;
            cuerpo.radioOrbita = d.radioOrbita;
            cuerpo.toleranciaAnillo = 0.06f * Escala;
            cuerpo.distanciaMaxima = 40f;
            cuerpo.periodoOrbital = d.periodo;
            cuerpo.anguloInicial = d.angulo;
            cuerpo.anillo = compAnillo;
            cuerpo.modelo = modelo.transform;
            cuerpo.velocidadRotacion = d.giro;
            cuerpo.nombre = d.nombre;
            cuerpo.informacion = d.info;

            float a = d.angulo * Mathf.Deg2Rad;
            raiz.transform.position = centro.position + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * d.radioOrbita;
            return cuerpo;
        }

        static GameObject Modelo(string nombre, Mesh malla, Material mat, Transform padre)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(padre, false);
            go.AddComponent<MeshFilter>().sharedMesh = malla;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
            return go;
        }

        static void CrearAnillosSaturno(Transform modelo)
        {
            var tex = Textura(Tex + "Saturn/2k_saturn_ring_alpha.png", 1024, true);
            var mat = MaterialTransparente("Anillos Saturno", "Universal Render Pipeline/Unlit", tex, false);
            mat.SetFloat("_Cull", 0f);

            const int seg = 128;
            const float interior = 0.62f, exterior = 1.15f;
            var v = new Vector3[seg * 2];
            var uv = new Vector2[seg * 2];
            var tri = new int[seg * 6];
            for (int i = 0; i < seg; i++)
            {
                float ang = i * Mathf.PI * 2f / seg;
                var dir = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
                v[i * 2] = dir * interior;
                v[i * 2 + 1] = dir * exterior;
                uv[i * 2] = new Vector2(0f, 0.5f);
                uv[i * 2 + 1] = new Vector2(1f, 0.5f);
                int s = (i + 1) % seg;
                int t = i * 6;
                tri[t] = i * 2; tri[t + 1] = s * 2; tri[t + 2] = i * 2 + 1;
                tri[t + 3] = s * 2; tri[t + 4] = s * 2 + 1; tri[t + 5] = i * 2 + 1;
            }
            var malla = new Mesh { name = "AnillosSaturno", vertices = v, uv = uv, triangles = tri };
            malla.RecalculateNormals();
            malla.RecalculateBounds();
            Guardar(malla, Gen + "/AnillosSaturno.asset");

            var go = Modelo("Anillos", malla, mat, modelo);
            go.transform.localRotation = Quaternion.identity;
        }

        static ParticleSystem CrearFuego(Transform padre)
        {
            var go = new GameObject("Fuego");
            go.transform.SetParent(padre, false);
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.02f * Escala, 0.05f * Escala);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.3f, 0f), new Color(1f, 0.85f, 0.2f));
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 500;

            var emision = ps.emission;
            emision.rateOverTime = 0f;
            var forma = ps.shape;
            forma.shapeType = ParticleSystemShapeType.Sphere;
            forma.radius = 0.03f * Escala;

            var color = ps.colorOverLifetime;
            color.enabled = true;
            var g = new Gradient();
            g.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(1f, 0.2f, 0f), 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            color.color = g;

            var tam = ps.sizeOverLifetime;
            tam.enabled = true;
            tam.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

            var r = go.GetComponent<ParticleSystemRenderer>();
            r.sharedMaterial = MaterialTransparente("Fuego", "Universal Render Pipeline/Particles/Unlit", TexturaParticula(), true);
            return ps;
        }

        // ---------- Interfaz ----------

        static void CrearPanel()
        {
            var go = new GameObject("Panel Info", typeof(RectTransform));
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(340f, 260f);
            rt.localScale = Vector3.one * 0.0008f;

            var fondo = new GameObject("Fondo", typeof(RectTransform)).AddComponent<Image>();
            fondo.transform.SetParent(go.transform, false);
            fondo.color = new Color(0.02f, 0.03f, 0.1f, 0.85f);
            Estirar(fondo.rectTransform, 0f);

            var titulo = Texto("Título", go.transform, 30f, FontStyles.Bold);
            titulo.alignment = TextAlignmentOptions.Top;
            Estirar(titulo.rectTransform, 12f);

            var texto = Texto("Texto", go.transform, 15f, FontStyles.Normal);
            texto.alignment = TextAlignmentOptions.TopLeft;
            Estirar(texto.rectTransform, 16f);
            texto.rectTransform.offsetMax = new Vector2(-16f, -52f);

            var panel = go.AddComponent<PanelInfo>();
            panel.titulo = titulo;
            panel.texto = texto;
        }

        static TextMeshProUGUI Texto(string nombre, Transform padre, float tam, FontStyles estilo)
        {
            var t = new GameObject(nombre, typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
            t.transform.SetParent(padre, false);
            t.fontSize = tam;
            t.fontStyle = estilo;
            t.color = Color.white;
            t.raycastTarget = false;
            return t;
        }

        static void Estirar(RectTransform rt, float margen)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(margen, margen);
            rt.offsetMax = new Vector2(-margen, -margen);
        }

        static void CrearBoton(ControlSistema control)
        {
            var raiz = new GameObject("Botón Reiniciar");
            raiz.transform.position = new Vector3(0.4f, 0.8f, 0.05f);
            var col = raiz.AddComponent<BoxCollider>();
            col.size = new Vector3(0.12f, 0.05f, 0.12f);
            col.center = new Vector3(0f, 0.02f, 0f);

            var unlit = Shader.Find("Universal Render Pipeline/Unlit");
            var matBase = new Material(unlit) { name = "Botón Base" };
            matBase.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.28f));
            Guardar(matBase, Gen + "/Boton Base.mat");
            var matTapa = new Material(unlit) { name = "Botón Tapa" };
            Guardar(matTapa, Gen + "/Boton Tapa.mat");

            var cilindro = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            var baseGo = Modelo("Base", cilindro, matBase, raiz.transform);
            baseGo.transform.localScale = new Vector3(0.12f, 0.01f, 0.12f);
            var tapa = Modelo("Tapa", cilindro, matTapa, raiz.transform);
            tapa.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            tapa.transform.localScale = new Vector3(0.085f, 0.01f, 0.085f);

            var etiqueta = new GameObject("Etiqueta").AddComponent<TextMeshPro>();
            etiqueta.transform.SetParent(raiz.transform, false);
            etiqueta.transform.localPosition = new Vector3(0f, 0.09f, 0f);
            etiqueta.text = "REINICIAR";
            etiqueta.fontSize = 0.35f;
            etiqueta.alignment = TextAlignmentOptions.Center;
            etiqueta.rectTransform.sizeDelta = new Vector2(0.3f, 0.06f);

            raiz.AddComponent<XRSimpleInteractable>();
            var poke = raiz.AddComponent<XRPokeFilter>();
            ConfigurarPoke(poke);

            var boton = raiz.AddComponent<BotonReiniciar>();
            boton.sistema = control;
            boton.tapa = tapa.transform;
            boton.render = tapa.GetComponent<MeshRenderer>();
        }

        // El botón se presiona hacia abajo.
        static void ConfigurarPoke(XRPokeFilter poke)
        {
            var so = new SerializedObject(poke);
            var it = so.GetIterator();
            while (it.Next(true))
            {
                if (it.name == "m_PokeDirection" && it.propertyType == SerializedPropertyType.Enum)
                {
                    int i = Array.IndexOf(it.enumNames, "NegativeY");
                    if (i >= 0) it.enumValueIndex = i;
                }
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---------- Assets ----------

        static Texture2D Textura(string ruta, int tamMax, bool alfa = false)
        {
            if (AssetImporter.GetAtPath(ruta) is TextureImporter imp)
            {
                imp.maxTextureSize = tamMax;
                imp.mipmapEnabled = true;
                imp.alphaIsTransparency = alfa;
                imp.wrapModeU = TextureWrapMode.Repeat;
                imp.wrapModeV = TextureWrapMode.Clamp;
                imp.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    name = "Android", overridden = true, maxTextureSize = tamMax,
                    format = alfa ? TextureImporterFormat.ASTC_4x4 : TextureImporterFormat.ASTC_6x6
                });
                imp.SaveAndReimport();
            }
            else Debug.LogWarning("Textura no encontrada: " + ruta);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(ruta);
        }

        static Texture2D TexturaParticula()
        {
            string ruta = TexPropias + "particula.png";
            if (!File.Exists(ruta))
            {
                const int n = 64;
                var t = new Texture2D(n, n, TextureFormat.RGBA32, false);
                for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                    float a = Mathf.Clamp01(1f - d);
                    t.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
                }
                File.WriteAllBytes(ruta, t.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(t);
                AssetDatabase.ImportAsset(ruta);
            }
            return Textura(ruta, 64, true);
        }

        static Material CrearMaterial(string nombre, Shader shader, Texture2D tex, Color tinte)
        {
            var m = new Material(shader) { name = nombre };
            m.SetTexture("_BaseMap", tex);
            m.SetColor("_Tinte", tinte);
            m.enableInstancing = true;
            return Guardar(m, $"{Gen}/{nombre}.mat");
        }

        static Material MaterialTransparente(string nombre, string shader, Texture2D tex, bool aditivo)
        {
            var m = new Material(Shader.Find(shader)) { name = nombre };
            if (tex != null) m.SetTexture("_BaseMap", tex);
            m.SetFloat("_Surface", 1f);
            m.SetFloat("_Blend", aditivo ? 2f : 0f);
            m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            m.SetFloat("_DstBlend", (float)(aditivo ? BlendMode.One : BlendMode.OneMinusSrcAlpha));
            m.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            m.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
            m.SetFloat("_ZWrite", 0f);
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.renderQueue = (int)RenderQueue.Transparent;
            return Guardar(m, $"{Gen}/{nombre}.mat");
        }

        static PhysicsMaterial CrearFisica()
        {
            var f = new PhysicsMaterial("Planeta")
            {
                bounciness = 0.9f, dynamicFriction = 0f, staticFriction = 0f,
                bounceCombine = PhysicsMaterialCombine.Maximum, frictionCombine = PhysicsMaterialCombine.Minimum
            };
            return Guardar(f, Gen + "/Planeta.physicMaterial");
        }

        static T Guardar<T>(T asset, string ruta) where T : UnityEngine.Object
        {
            AssetDatabase.DeleteAsset(ruta);
            AssetDatabase.CreateAsset(asset, ruta);
            return asset;
        }
    }
}
