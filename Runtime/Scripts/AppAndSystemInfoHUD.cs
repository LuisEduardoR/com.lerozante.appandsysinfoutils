/*
* Author: Luís Eduardo Rozante
*/

using UnityEngine;

using TMPro;

namespace Lerozante.AppAndSysInfoUtils {

    /// <summary>
    /// Controls the HUD panel containing software and hardware statistics.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class AppAndSystemInfoHUD : MonoBehaviour {

        [Header("Component References")]

        [Tooltip("Text to display the info at.")]
        [SerializeField] TMP_Text infoText = null;

        [Header("Customization")]

        [Tooltip("Number of frames to average for the FPS.")]
        [SerializeField] int FPSSampleCount = 30;

        [Tooltip("Delay between updates (in seconds).")]
        [SerializeField] float updateFPSDelay = 0.5f;

        [Tooltip("Show FPS be colored based on how high it is?")]
        [SerializeField] bool colorCodeFPS = true;

        [Tooltip("Only show this in the Unity Editor.")]
        [SerializeField] bool showOnlyInEditor = false;

        // Canvas used to draw the HUD
        Canvas _canvas = null;

        // Used to calculate and store the FPS
        float[] _frameTimeSamples;
        float _sampleSum;
        int _currentSample;

        string _fpsText;
        float _lastUpdateTime;

        // Info about API and OS version along with hardware config.
        string _hardwareInfo;

        /// <summary>
        /// Sets if the statistics panel should be enabled.
        /// </summary>
        /// <param "time"=Should the panel be enabled?</param>
        public void SetEnabled(bool enabled) {
            _canvas.enabled = enabled;
        }

        void Awake() {
            _canvas = GetComponent<Canvas>();

            // Hides if not in Editor
            if (showOnlyInEditor && !Application.isEditor)
                SetEnabled(false);
        }

        void Start() {

            // Creates and initializes the FPS sample buffer and sample sum.
            _frameTimeSamples = new float[FPSSampleCount];
            for (int i = 0; i < FPSSampleCount; i++)
                _frameTimeSamples[i] = Time.unscaledDeltaTime;
            _sampleSum = FPSSampleCount * Time.unscaledDeltaTime;
            _currentSample = 0;

            _lastUpdateTime = Time.time;

            // We just need to get hardware info at start because it's supposed to not change.
            _hardwareInfo = GetFormatedHardwareInfo();
        }

        void Update() {

            // Calculates and updates the FPS:

            SampleFPS();

            if (Time.time - _lastUpdateTime >= updateFPSDelay) {
                float fps = CalculateFPSFromSamples();
                _fpsText = GetFormatedFPS(fps, colorCodeFPS);
                _lastUpdateTime = Time.time;
            }

            // Gathers the app and system information that can change:

            // Creates a string with screen resolution info.
            var resolutionText = "<b>Resolution:</b> " + 
                $"{Screen.width}x{Screen.height} ({AspectRatioUtils.ResolutionToAspectRatioString(Screen.currentResolution)})";

            // Creates a string with window info.
            var windowText = "<b>Window Mode:</b> ";
            if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) windowText += "FullScreen";
            else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) windowText += "Windowed FullScreen";
            else windowText += "Windowed";

            // Creates a string with VSync info.
            var vSyncText = $"<b>VSync:</b> {(QualitySettings.vSyncCount > 0 ? "On" : "Off")}";

            // Creates a string with quality level info.
            var qualityLevelText = $"<b>Quality Level:</b> {QualitySettings.names[QualitySettings.GetQualityLevel()]}";

            // Puts the all the information on the screen:
            infoText.text = $"{_fpsText}<br>{_hardwareInfo}<br>{resolutionText}<br>{windowText}<br>{vSyncText}<br>{qualityLevelText}";

        }

        /// <summary>
        /// Gathers system hardware information into a formated string.
        /// </summary>
        /// <returns> The hardware info string. </returns>
        string GetFormatedHardwareInfo() {

            string version = Application.version;
            string graphicsAPIInfo = SystemInfo.graphicsDeviceType.ToString();
            string OSInfo = SystemInfo.operatingSystem;
            string processorInfo = SystemInfo.processorType;
            int systemMemoryInfo = SystemInfo.systemMemorySize;
            string graphicsCardInfo = SystemInfo.graphicsDeviceName;
            int graphicsMemoryInfo = SystemInfo.graphicsMemorySize;

            // Creates the hardware info string.
            return $"<b>Version:</b> {version}<br>" +
                    $"<b>API:</b> {graphicsAPIInfo}<br>" +
                    $"<b>OS:</b> {OSInfo}<br>" +
                    $"<b>CPU:</b> {processorInfo}<br>" +
                    $"<b>RAM:</b> {systemMemoryInfo}MB<br>" +
                    $"<b>GPU:</b> {graphicsCardInfo}<br>" +
                    $"<b>VRAM:</b> {graphicsMemoryInfo}MB";

        }

        /// <summary>
        /// Gets a FPS sample (replaces the oldest one).
        /// </summary>
        void SampleFPS() {

            // Removes the oldest sample from the average and adds a new one.
            _sampleSum -= _frameTimeSamples[_currentSample];
            _frameTimeSamples[_currentSample] = Time.unscaledDeltaTime;
            _sampleSum += _frameTimeSamples[_currentSample];

            // Changes currentSample to the next one.
            _currentSample = (_currentSample + 1) % FPSSampleCount;

        }

        /// <summary>
        /// Calculates the FPS from the samples collected.
        /// </summary>
        float CalculateFPSFromSamples() {
            // For 1 samples we would use 1 / Time.unscaledDeltaTime, so for more samples we use:
            // (number of samples) / (sum of the Time.unscaledDeltaTime from every sample)
            return FPSSampleCount / _sampleSum;
        }

        /// <summary>
        /// Creates the FPS string in the correct format.
        /// </summary>
        /// <param "fps"=The FPS to formated.</param>
        /// <returns> The formated string. </returns>
        string GetFormatedFPS(float fps, bool colorCounter) {

            // Returns FPS without a color.
            if (!colorCounter)
                return $"<b>FPS:</b> {Mathf.Floor(fps)}";

            // Color codes the FPS.
            string hexColor;
            if (fps < 30) hexColor = "#ff0000";
            else if (fps < 60) hexColor = "#ffff00";
            else if (fps < 120) hexColor = "#00ff00";
            else if (fps < 240) hexColor = "#00ffff";
            else hexColor = "#ff00ff";

            return $"<b>FPS:</b> <color={hexColor}>{Mathf.Floor(fps)}</color>";

        }

    }

}
