using System;
using StixGames;
using UnityEngine;

namespace StixGames.GrassShader
{
	[Serializable]
	public class GrassPainter
	{
		private const float Epsilon = 0.01f;

		public TextureTarget Target = TextureTarget.Density;
		public DensityChannel DensityChannel = DensityChannel.R;
		public ColorHeightChannel ColorHeightChannel = ColorHeightChannel.Color;
		public BrushMode Brush = BrushMode.Blend;
		public Color PaintColor = Color.white;
		public float PaintDensity = 1;
		public float Strength = 1;
		public float Size = 1;
		public float Softness = 0.5f;
		public float Spacing = 0.5f;
		public float Rotation = 0;
	
		/// <summary>
		/// The grass collider the brush will be applied to. Only set this when using ApplyBrush directly.
		/// </summary>
		public Collider[] GrassColliders = new Collider[0];
	
		/// <summary>
		/// The color height texture the brush will be applied to. Only set this when using ApplyBrush directly.
		/// </summary>
		public Texture2D ColorHeightTexture;
		
		/// <summary>
		/// The density texture the brush will be applied to. Only set this when using ApplyBrush directly.
		/// </summary>
		public Texture2D DensityTexture;

		public Texture2D CurrentTexture
		{
			get
			{
				switch (Target)
				{
					case TextureTarget.ColorHeight:
						return ColorHeightTexture;
					case TextureTarget.Density:
						return DensityTexture;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			set
			{
				switch (Target)
				{
					case TextureTarget.ColorHeight:
						ColorHeightTexture = value;
						break;
					case TextureTarget.Density:
						DensityTexture = value;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		
		public bool Draw(Ray ray)
		{
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				GrassColliders = new [] { hit.collider };
				ColorHeightTexture = hit.collider.GetComponent<Renderer>().material.GetTexture("_ColorMap") as Texture2D;
				DensityTexture = hit.collider.GetComponent<Renderer>().material.GetTexture("_Density") as Texture2D;
	
				ApplyBrush(hit.point);
				return true;
			}
	
			return false;
		}
	
		public void ApplyBrush(Vector3 hitPoint)
		{
			if (CurrentTexture == null)
			{
				Debug.Log("You haven't created a texture for the current mode yet.");
				return;
			}
			
			RaycastHit hit;
			Vector2 texForward, texRight;
			if (!GrassManipulationUtility.GetWorldToTextureSpaceMatrix(new Ray(hitPoint + Vector3.up * 1000, Vector3.down),
				Epsilon, GrassColliders, out hit, out texForward, out texRight))
			{
				return;
			}
	
			Vector2 texCoord = hit.textureCoord;
	
			//Convert the world space radius to a pixel radius in texture space. This requires square textures.
			int pixelRadius = (int)(Size * texForward.magnitude * CurrentTexture.width);
	
			//Calculate the pixel coordinates of the point where the raycast hit the texture.
			Vector2 mid = new Vector2(texCoord.x * CurrentTexture.width, texCoord.y * CurrentTexture.height);
	
			//Calculate the pixel area where the texture will be changed
			int targetStartX = (int)(mid.x - pixelRadius);
			int targetStartY = (int)(mid.y - pixelRadius);
			int startX = Mathf.Clamp(targetStartX, 0, CurrentTexture.width);
			int startY = Mathf.Clamp(targetStartY, 0, CurrentTexture.height);
			int width = Mathf.Min(targetStartX + pixelRadius * 2, CurrentTexture.width) - targetStartX;
			int height = Mathf.Min(targetStartY + pixelRadius * 2, CurrentTexture.height) - targetStartY;
	
			mid -= new Vector2(startX, startY);
	
			//Get pixels
			Color[] pixels = CurrentTexture.GetPixels(startX, startY, width, height);
	
			//Iterate trough all pixels
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int index = y * width + x;
					Vector2 uv = ((new Vector2(x, y) - mid) / pixelRadius) * 0.5f + new Vector2(0.5f, 0.5f);
					pixels[index] = ApplyBrushToPixel(pixels[index], uv);
				}
			}
	
			//Save pixels and apply them to the texture
			CurrentTexture.SetPixels(startX, startY, width, height, pixels);
			CurrentTexture.Apply();
		}
	
		private Color ApplyBrushToPixel(Color c, Vector2 v)
		{
			switch (Target)
			{
				case TextureTarget.ColorHeight:
					c = ColorHeightOperation(c, v);
					break;
				case TextureTarget.Density:
					c = DensityOperation(c, v);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return c;
		}

		private Color ColorHeightOperation(Color c, Vector2 v)
		{
			Color newColor;
			switch (ColorHeightChannel)
			{
				case ColorHeightChannel.Color:
					newColor = BrushOperation(c, v);
					newColor.a = c.a;
					return newColor;
				case ColorHeightChannel.Height:
					newColor = BrushOperation(c, v);
					c.a = newColor.a;
					return c;
				case ColorHeightChannel.Both:
					return BrushOperation(c, v);
				default:
					throw new InvalidOperationException("Channel invalid");
			}
		}

		private Color DensityOperation(Color c, Vector2 v)
		{
			switch (DensityChannel)
			{
				case DensityChannel.R:
					c.r = BrushOperation(c.r, v);
					break;
				case DensityChannel.G:
					c.g = BrushOperation(c.g, v);
					break;
				case DensityChannel.B:
					c.b = BrushOperation(c.b, v);
					break;
				case DensityChannel.A:
					c.a = BrushOperation(c.a, v);
					break;
				default:
					throw new InvalidOperationException("Channel invalid");
			}
			return c;
		}

		private Color BrushOperation(Color x, Vector2 v)
		{
			v -= new Vector2(0.5f, 0.5f);
			v *= 2;
			var distance = v.magnitude;
			
			//Calculate brush smoothness
			var value = SmoothStep(1, Mathf.Min(1 - Softness, 0.999f), distance);

			var result = x;
			switch (Brush)
			{
				case BrushMode.Add:
					result = x + PaintColor * value * Strength;
					break;
				case BrushMode.Subtract:
					result = x - PaintColor * value * Strength;
					break;
				case BrushMode.Blend:
					result = x * (1 - value * Strength) + PaintColor * Strength * value;
					break;
			}

			return new Color(
				Mathf.Clamp01(result.r), 
				Mathf.Clamp01(result.g), 
				Mathf.Clamp01(result.b), 
				Mathf.Clamp01(result.a));
		}
		
		private float BrushOperation(float x, Vector2 v)
		{
			v -= new Vector2(0.5f, 0.5f);
			v *= 2;
			var distance = v.magnitude;
			
			//Calculate brush smoothness
			var value = SmoothStep(1, Mathf.Min(1 - Softness, 0.999f), distance);

			var result = x;
			switch (Brush)
			{
				case BrushMode.Add:
					result = x + value * Strength;
					break;
	
				case BrushMode.Subtract:
					result = x - value * Strength;
					break;
	
				case BrushMode.Blend:
					result = x * (1 - value * Strength) + PaintDensity * Strength * value;
					break;
			}

			return Mathf.Clamp01(result);
		}
	
		//Taken from wikipedia, smoothstep
		private float SmoothStep(float edge0, float edge1, float x)
		{
			// Scale, bias and saturate x to 0..1 range
			x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
			// Evaluate polynomial
			return x * x * (3 - 2 * x);
		}

		public bool RaycastColliders(Ray ray, out RaycastHit hit)
		{
			hit = new RaycastHit();
			foreach (var collider in GrassColliders)
			{
				if (collider.Raycast(ray, out hit, Mathf.Infinity))
				{
					return true;
				}
			}
			return false;
		}
	}

	public enum TextureTarget
	{
		ColorHeight,
		Density
	}

	public enum BrushMode
	{
		Blend,
		Add,
		Subtract
	}
	
	public enum ColorHeightChannel
	{
		Color, 
		Height,
		Both
	}
	
	public enum DensityChannel
	{
		R,G,B,A
	}
}