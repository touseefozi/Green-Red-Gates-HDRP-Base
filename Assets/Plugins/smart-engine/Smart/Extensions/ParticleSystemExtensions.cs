using UnityEngine;

namespace Smart.Extensions
{
	public static class ParticleSystemExtensions
	{
		public static void SetStartColor(this ParticleSystem particleSystem, ParticleSystem.MinMaxGradient color)
		{
			var mainModule = particleSystem.main;
			mainModule.startColor = color;
		}
		
		public static void SetColorOverLifetime(this ParticleSystem particleSystem, ParticleSystem.MinMaxGradient color)
		{
			var colorOverLifetime = particleSystem.colorOverLifetime;
			colorOverLifetime.color = color;
		}
	}
}