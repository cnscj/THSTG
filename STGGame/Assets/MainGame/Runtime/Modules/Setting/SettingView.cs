using System.Collections.Generic;
using THGame;

namespace STGRuntime.UI
{
    public class SettingView : FViewWindow
    {
        FSlider soundVolumeSlider;
        FSlider musicVolumeSlider;
        FSlider effectVolumeSlider;

        FRichText soundVolumeText;
        FRichText musicVolumeText;
        FRichText effectVolumeText;

        FCheckbox soundMuteChBox;
        FCheckbox musicMuteChBox;
        FCheckbox effectMuteChBox;

        FButton applyBtn;

        public SettingView() : base("Setting", "SettingView")
        {
            
        }

        protected override void OnInitUI()
        {
            soundVolumeSlider = GetChild<FSlider>("soundVolumeSlider");
            musicVolumeSlider = GetChild<FSlider>("musicVolumeSlider");
            effectVolumeSlider = GetChild<FSlider>("effectVolumeSlider");

            soundVolumeText = GetChild<FRichText>("soundVolumeText");
            musicVolumeText = GetChild<FRichText>("musicVolumeText");
            effectVolumeText = GetChild<FRichText>("effectVolumeText");

            soundMuteChBox = GetChild<FCheckbox>("soundMuteChBox");
            musicMuteChBox = GetChild<FCheckbox>("musicMuteChBox");
            effectMuteChBox = GetChild<FCheckbox>("effectMuteChBox");

            soundVolumeSlider.OnChanged((context) =>
            {
                var val = soundVolumeSlider.GetValue();
                soundVolumeText.SetText(string.Format("{0}%", val));
                SoundManager.GetInstance().MaxSoundVolume = (int)val / 100f;
            });

            musicVolumeSlider.OnChanged((context) =>
            {
                var val = musicVolumeSlider.GetValue();
                musicVolumeText.SetText(string.Format("{0}%", val));
                SoundManager.GetInstance().MaxMusicVolume = (int)val / 100f;
            });

            effectVolumeSlider.OnChanged((context) =>
            {
                var val = effectVolumeSlider.GetValue();
                effectVolumeText.SetText(string.Format("{0}%", val));
                SoundManager.GetInstance().MaxEffectVolume = (int)val / 100f;
            });
            
        }

        protected override void OnInitEvent()
        {
            
        }

        protected override void OnEnter()
        {
            _updateLayer();
        }

        protected override void OnExit()
        {

        }

        private void _updateLayer()
        {
            soundVolumeSlider.SetValue(SoundManager.GetInstance().MaxSoundVolume * 100);
            musicVolumeSlider.SetValue(SoundManager.GetInstance().MaxMusicVolume * 100);
            effectVolumeSlider.SetValue(SoundManager.GetInstance().MaxEffectVolume * 100);

            soundVolumeText.SetText(string.Format("{0}%", soundVolumeSlider.GetValue()));
            musicVolumeText.SetText(string.Format("{0}%", musicVolumeSlider.GetValue()));
            effectVolumeText.SetText(string.Format("{0}%", effectVolumeSlider.GetValue()));
        }
    }
}


