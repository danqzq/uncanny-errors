// Made by @danqzq
// Official Website: https://www.danqzq.games
// GitHub: https://github.com/danqzq
// YouTube: https://www.youtube.com/@danqzq
// Twitter: https://twitter.com/danqzq
// Discord Server: https://discord.gg/cyfJUjBGgK

using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Dan.UncannyErrors.Scripts
{
    public class UncannyErrorsManager : EditorWindow
    {
        private static UncannyErrorsPhaseSettings _phaseSettings;

        private static bool _justReloaded;
        private static bool _isSettingsMenu;
        
        private static bool _openOnError = EditorPrefs.GetBool(OpenOnErrorKey, true);
        private const string OpenOnErrorKey = "UncannyErrors_OpenOnError";
        
        private static bool _openOnRecompilation = EditorPrefs.GetBool(OpenOnRecompilationKey, true);
        private const string OpenOnRecompilationKey = "UncannyErrors_OpenOnRecompilation";
        
        private static string _windowTitle = EditorPrefs.GetString(WindowTitleKey, "Mr. Incredible");
        private const string WindowTitleKey = "UncannyErrors_WindowTitle";

        private static bool _isAudioEnabled = EditorPrefs.GetBool(IsAudioEnabledKey, true);
        private const string IsAudioEnabledKey = "UncannyErrors_IsAudioEnabled";
        
        private static float _volume = EditorPrefs.GetFloat(VolumeKey, 1.0f);
        private const string VolumeKey = "UncannyErrors_Volume";
        
        private const string SettingsFileName = "UncannyErrorsSettings";
        private const string SettingsDefaultPath = "Assets/UncannyErrors/Resources/MrIncredibleSettings.asset";

        private static Phase GetPhaseAtErrorCount => _phaseSettings.GetPhaseAtErrorCount();

        private static void Init(bool settingsMenu)
        {
            _isSettingsMenu = settingsMenu;
            
            _openOnError = EditorPrefs.GetBool(OpenOnErrorKey, true);
            _openOnRecompilation = EditorPrefs.GetBool(OpenOnRecompilationKey, true);
            _windowTitle = EditorPrefs.GetString(WindowTitleKey, "Mr. Incredible");
            _isAudioEnabled = EditorPrefs.GetBool(IsAudioEnabledKey, true);
            _volume = EditorPrefs.GetFloat(VolumeKey, 1.0f);

            _phaseSettings = Resources.Load<UncannyErrorsPhaseSettings>(SettingsFileName);
            if (_phaseSettings == null)
            {
                _phaseSettings = CreateInstance<UncannyErrorsPhaseSettings>();
                AssetDatabase.CreateAsset(_phaseSettings, SettingsDefaultPath);
            }

            GetWindow<UncannyErrorsManager>(_windowTitle, true);
            
            _justReloaded = true;
        }

        [MenuItem("Window/Uncanny Errors/Main Window")]
        public static void OnOpenMain() => Init(settingsMenu: false);

        [MenuItem("Window/Uncanny Errors/Settings")]
        public static void OnOpenSettings() => Init(settingsMenu: true);

        internal static void OnError()
        {
            _justReloaded = true;
            if (_openOnError)
                Init(settingsMenu: false);
        }
        
        internal static void OnRecompile()
        {
            _justReloaded = true;
            if (_openOnRecompilation)
                Init(settingsMenu: false);
        }

        private void OnGUI()
        {
            GUILayout.Label($"Error Count: {UncannyErrorsLogListener.errorList?.Count}");
            
            if (_isSettingsMenu)
            {
                OnSettingsGUI();
                return;
            }

            if (_phaseSettings == null)
            {
                _phaseSettings = Resources.Load<UncannyErrorsPhaseSettings>(SettingsFileName);
            }

            var phase = GetPhaseAtErrorCount;
            var image = phase.texture;
            var width = position.width;
            var height = position.height;
            GUILayout.Label(image, GUILayout.Width(width), GUILayout.Height(height));

            if (!_justReloaded) return;
            
            _justReloaded = false;
            PlayClipAtPoint(phase.clip);
        }
        
        private static void OnSettingsGUI()
        {
            EditorGUILayout.ObjectField("Settings", _phaseSettings, typeof(UncannyErrorsPhaseSettings), false);
            
            _openOnError = EditorGUILayout.Toggle("Open on Error", _openOnError);
            _openOnRecompilation = EditorGUILayout.Toggle("Open on Recompilation", _openOnRecompilation);
            _windowTitle = EditorGUILayout.TextField("Window Title", _windowTitle);
            _isAudioEnabled = EditorGUILayout.Toggle("Enable Audio", _isAudioEnabled);
            if (_isAudioEnabled) _volume = EditorGUILayout.Slider("Audio Volume", _volume, 0.0f, 1.0f);

            if (GUILayout.Button("Save Settings"))
            {
                EditorPrefs.SetBool(OpenOnErrorKey, _openOnError);
                EditorPrefs.SetBool(OpenOnRecompilationKey, _openOnRecompilation);
                EditorPrefs.SetString(WindowTitleKey, _windowTitle);
                EditorPrefs.SetBool(IsAudioEnabledKey, _isAudioEnabled);
                EditorPrefs.SetFloat(VolumeKey, _volume);
                
                GetWindow<UncannyErrorsManager>(_windowTitle, true).titleContent.text = _windowTitle;
            }
            
            if (GUILayout.Button("Back to Main")) 
                OnOpenMain();

            if (GUILayout.Button("Made by @danqzq", new GUIStyle{alignment = TextAnchor.LowerRight}))
                Application.OpenURL("https://www.danqzq.games");
        }

        private static void PlayClipAtPoint(AudioClip clip)
        {
            if (!_isAudioEnabled) return;
            
            var gameObject = new GameObject("[Uncanny Errors Audio]") {transform = {position = Vector3.zero}};
            var audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = _volume;
            audioSource.Play();
            
            EditorApplication.delayCall += () => 
                DestroyObj(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
        }
        
        private static async void DestroyObj(Object obj, float delay = 0f)
        {
            await Task.Delay((int) delay * 1000);
            DestroyImmediate(obj);
        }
    }
}
