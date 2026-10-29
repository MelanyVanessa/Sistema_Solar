# Sistema Solar VR (Meta Quest 2)

Sistema solar interactivo en realidad virtual: agarra los planetas, lánzalos, haz que choquen,
quémalos en el Sol y devuélvelos a su órbita soltándolos sobre su anillo blanco.

## Requisitos
- **Unity 6000.6.0f1** (instalar desde Unity Hub) con el módulo **Android Build Support**
  (incluye OpenJDK y Android SDK & NDK).
- **Git LFS** (las texturas y modelos están en LFS): https://git-lfs.com

## Bajar el proyecto en otro computador
```bash
git lfs install
git clone https://github.com/MelanyVanessa/Sistema_Solar.git
```
1. Unity Hub → **Add → Add project from disk** → elegir la carpeta `Sistema_Solar`.
2. Abrirlo con Unity 6000.6.0f1 (la primera vez tarda porque genera la carpeta `Library`).
3. Abrir la escena `Assets/SistemaSolar/SistemaSolar.unity`.

## Probar
- **En el editor:** Play (usa el XR Device Simulator de la plantilla).
- **En las Quest 2:** activar el modo desarrollador en la app Meta Horizon, conectar por USB-C,
  **File → Build Profiles → Android → Switch Platform**, y luego **Build And Run**.

## Controles
| Acción | Cómo |
|---|---|
| Moverse | Joystick izquierdo |
| Girar | Joystick derecho |
| Teletransportarse | Joystick derecho hacia adelante, apuntar y soltar |
| Agarrar / lanzar | Botón de agarre (grip) o la mano |
| Devolver a la órbita | Soltar el planeta sobre su anillo blanco (se ilumina) |
| Reiniciar | Presionar el botón rojo **REINICIAR** |

## Regenerar la escena
Menú **Sistema Solar → Construir escena** vuelve a crear la escena y los materiales.
Los tamaños, distancias y textos se editan en `Assets/SistemaSolar/Editor/ConstructorSistemaSolar.cs`
(`Escala` cambia el tamaño de todo el sistema).

## Créditos
Texturas de planetas: [Solar System Scope](https://www.solarsystemscope.com/textures/) — CC BY 4.0.
