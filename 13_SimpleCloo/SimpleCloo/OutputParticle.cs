using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// 表示用粒子
	/// </summary>
	public struct OutputParticle
	{
		/// <summary>
		/// 粒子の中心座標
		/// </summary>
		readonly Vector3 X;

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
		/// <param name="_x">中心座標</param>
		/// <param name="_d">直径</param>
		/// <param name="_color">表示色</param>
		public OutputParticle(Vector3 _x, float _d, Color4 _color)
		{
			// 各パラメーターを設定
			this.X = _x;
			this.d = _d;
			this.color = _color;
		}		
	}
}