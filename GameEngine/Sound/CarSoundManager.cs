using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using GameEngine.Cameras;
using GameEngine.Physics;
using GameEngine.Physics.BepuPhysicsAdapter;

public class CarSoundManager : DrawableGameComponent
{
    //Set the sound effects to use
    AudioEngine audioEngine;
    WaveBank waveBank;
    SoundBank soundBank;
    Cue[] cueArray;
    Cue cameraCue;
    CameraController camera_;
    CameraState state_;
    AudioCategory musicCategory;
    AudioCategory defaultCategory;

    protected AudioListener listener_;

    protected CarActor car_;


    public CarSoundManager(CarActor car, CameraController camera, Game game)
        : base(game)
    {
        cueArray = new Cue[CarSoundEmitters.AudioEmitters.Count];
        car_ = car;
        listener_ = camera.Listener;
        camera_ = camera;
        state_ = camera.CurrentCamera.State;
    }

    protected override void LoadContent()
    {
        audioEngine = new AudioEngine("Content\\Audio\\Audio.xgs");
        waveBank = new WaveBank(audioEngine, "Content\\Audio\\myWaveBank.xwb");
        soundBank = new SoundBank(audioEngine, "Content\\Audio\\mySoundBank.xsb");
        musicCategory = audioEngine.GetCategory("Music");
        defaultCategory = audioEngine.GetCategory("Default");

        for (int i = 0; i < cueArray.Length; i++)
        {
            cueArray[i] = soundBank.GetCue("engine");
            cueArray[i].Apply3D(listener_, CarSoundEmitters.AudioEmitters[i]);
            cueArray[i].Play();
        }


        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (state_ != camera_.CurrentCamera.State)
        {
            state_ = camera_.CurrentCamera.State;
            defaultCategory.SetVolume(50);
            switch (state_)
            {
                case CameraState.Chase:
                    cameraCue = soundBank.GetCue("chase");
                    cameraCue.Play();
                    break;
                case CameraState.Reverse:
                    cameraCue = soundBank.GetCue("reverse");
                    cameraCue.Play();
                    break;
                case CameraState.Dynamic:
                    cameraCue = soundBank.GetCue("dynamic");
                    cameraCue.Play();
                    break;
                case CameraState.TopDown:
                    cameraCue = soundBank.GetCue("top_down");
                    cameraCue.Play();
                    break;
            }
        }

        for (int i = 0; i < cueArray.Length; i++)
            cueArray[i].Apply3D(listener_, CarSoundEmitters.AudioEmitters[i]);

        if (car_.LinearVelocity.LengthSquared() <= 400)
            musicCategory.SetVolume(car_.LinearVelocity.LengthSquared() / 16);
        else if (car_.LinearVelocity.LengthSquared() > 1650)
            musicCategory.SetVolume(0);
        else
            musicCategory.SetVolume(25);

        base.Update(gameTime);
    }
}

