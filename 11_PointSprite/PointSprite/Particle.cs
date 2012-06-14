using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.PointSprite
{
	/// <summary>
	/// 粒子
	/// </summary>
	public struct Particle
	{
		/// <summary>
		/// 粒子の中心座標
		/// </summary>
		public Vector3 X;

		/// <summary>
		/// 粒子の直径
		/// </summary>
		readonly float d;

		/// <summary>
		/// 粒子の表示色
		/// </summary>
		readonly Color4 color;

		/// <summary>
		/// 粒子を作成する
		/// </summary>
		/// <param name="_d">直径</param>
		/// <param name="_color">表示色</param>
		public Particle(float _d, Color4 _color)
		{
			// 各パラメーターを設定
			this.X = new Vector3();
			this.d = _d;
			this.color = _color;
		}		
	}
}