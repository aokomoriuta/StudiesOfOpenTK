using OpenTK;
using OpenTK.Graphics;
using LWisteria.StudiesOfOpenTK.Math;
namespace LWisteria.StudiesOfOpenTK.CpuDem
{
	/// <summary>
	/// 粒子の種類
	/// </summary>
	public enum ParticleType
	{
		// 自由に移動可能
		FreeMovable,

		// 固定
		Fixed,
	}

	/// <summary>
	/// 粒子
	/// </summary>
	public class Particle
	{
		/// <summary>
		/// 中心座標
		/// </summary>
		public Vector X;

		/// <summary>
		/// 速度
		/// </summary>
		public Vector U;

		/// <summary>
		/// 加速度
		/// </summary>
		public Vector A;


		/// <summary>
		/// 粒子の直径
		/// </summary>
		public readonly double D;

		/// <summary>
		/// 材質
		/// </summary>
		public readonly Material Material;

		/// <summary>
		/// 種類
		/// </summary>
		public readonly ParticleType Type;

		/// <summary>
		/// 固有番号
		/// </summary>
		public readonly ulong ID;

		/// <summary>
		/// 粒子を作成する
		/// </summary>
		/// <param name="id">他の粒子と識別するための固有番号</param>
		/// <param name="d">直径</param>
		/// <param name="material">材質</param>
		/// <param name="type">粒子の種類</param>
		public Particle(ulong id, double d, Material material, ParticleType type)
		{
			// 各物性値を初期化
			this.X = new Vector();
			this.U = new Vector();
			this.A = new Vector();

			// 各パラメーターを設定
			this.ID = id;
			this.D = d;
			this.Material = material;
			this.Type = type;
		}

		/// <summary>
		/// 体積を取得する
		/// </summary>
		public double V
		{
			get
			{
				// πd^3/6
				return System.Math.PI * D * D * D / 6;
			}
		}

		/// <summary>
		/// 質量を取得する
		/// </summary>
		public double M
		{
			get
			{
				// ρV
				return this.Material.Rho * V;
			}
		}
	}
}