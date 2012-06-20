using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.CpuDem
{
	/// <summary>
	/// 材質
	/// </summary>
	public class Material
	{
		/// <summary>
		/// この材質固有のID
		/// </summary>
		public readonly byte ID;

		/// <summary>
		/// 密度
		/// </summary>
		public readonly double Rho;

		/// <summary>
		/// ヤング係数
		/// </summary>
		public readonly double E;

		/// <summary>
		/// ポアソン比
		/// </summary>
		public readonly double Nu;

		/// <summary>
		/// 表示色
		/// </summary>
		public readonly Color4 Color;

		/// <summary>
		/// 材質を生成する
		/// </summary>
		/// <param name="id">固有番号</param>
		/// <param name="rho">密度</param>
		/// <param name="e">ヤング係数</param>
		/// <param name="nu">ポアソン比</param>
		/// <param name="color">表示色</param>
		public Material(byte id,
			double rho, double e, double nu,
			Color4 color)
		{
			// パラメーターを設定
			this.ID = id;
			this.Rho = rho;
			this.E = e;
			this.Nu = nu;
			this.Color = color;
		}
	}
}