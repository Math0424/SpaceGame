using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{

    //TODO make this able to use 3D audio and attach to entities
    internal class SoundSystem : SystemComponent
    {
        private Dictionary<string, SoundEffect> _sounds;
        private List<SoundEffectInstance> _playingSounds;
        private Game _game;

        public SoundSystem(Game game)
        {
            _game = game;
            _playingSounds = new List<SoundEffectInstance>();
            _sounds = new Dictionary<string, SoundEffect>();
        }

        public void PlaySoundEffect(string effect)
        {
            if (!_sounds.ContainsKey(effect))
                _sounds[effect] = _game.Content.Load<SoundEffect>(effect);
            var sound = _sounds[effect].CreateInstance();
            sound.Play();
            _playingSounds.Add(sound);
        }

        public override void Close()
        {
            foreach (var y in _playingSounds)
                y.Stop();
        }


    }
}
