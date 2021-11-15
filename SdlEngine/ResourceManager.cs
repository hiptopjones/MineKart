using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class ResourceManager : IDisposable
	{
		private Dictionary<string, Texture> TextureMap { get; set; } = new Dictionary<string, Texture>();

		private Dictionary<string, Sound> SoundMap { get; set; } = new Dictionary<string, Sound>();

		~ResourceManager()
		{
			Dispose(isDisposing: false);
		}

		#region IDisposable
		private bool isAlreadyDisposed;

		protected virtual void Dispose(bool isDisposing)
		{
			if (!isAlreadyDisposed)
			{
				if (isDisposing)
				{
					TeardownManaged();
				}

				TeardownUnmanaged();
				isAlreadyDisposed = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool isDisposing)' method
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		private void TeardownManaged()
		{
			List<string> textureFilePaths = TextureMap.Keys.ToList();
			foreach (string textureFilePath in textureFilePaths)
			{
				Texture texture = TextureMap[textureFilePath];
				texture.Dispose();

				TextureMap.Remove(textureFilePath);
			}

			List<string> soundFilePaths = SoundMap.Keys.ToList();
			foreach (string soundFilePath in soundFilePaths)
			{
				Sound sound = SoundMap[soundFilePath];
				sound.Dispose();

				SoundMap.Remove(soundFilePath);
			}
		}

		private void TeardownUnmanaged()
		{
			// N/A
		}
		#endregion

		public Texture GetTexture(string textureFilePath)
		{
			Texture texture;

			if (false == TextureMap.TryGetValue(textureFilePath, out texture))
			{
				texture = Texture.LoadFromFile(textureFilePath);
				if (texture == null)
				{
					throw new Exception($"Unable to load texture: '{textureFilePath}'");
				}

				TextureMap[textureFilePath] = texture;
			}

			return texture;
		}

		public Sound GetSound(string soundFilePath)
		{
			Sound sound;

			if (false == SoundMap.TryGetValue(soundFilePath, out sound))
			{
				sound = Sound.LoadFromFile(soundFilePath);
				if (sound == null)
				{
					throw new Exception($"Unable to load sound: '{soundFilePath}'");
				}

				SoundMap[soundFilePath] = sound;
			}

			return sound;
		}
	}
}
