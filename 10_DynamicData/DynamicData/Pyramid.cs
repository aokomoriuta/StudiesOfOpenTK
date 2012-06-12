using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.DynamicData
{
	/// <summary>
	/// 四角錐
	/// </summary>
	public struct Pyramid
	{
		/// <summary>
		/// 四角錐の中心座標
		/// </summary>
		readonly Vector3 position;

		/// <summary>
		/// 四角錐の1辺の長さ
		/// </summary>
		readonly float size;

		/// <summary>
		/// 四角錐の表示色
		/// </summary>
		readonly Color4 color;

		/// <summary>
		/// 四角錐を作成する
		/// </summary>
		/// <param name="_position">中心座標</param>
		/// <param name="_size">1辺の長さ</param>
		/// <param name="_color">表示色</param>
		public Pyramid(Vector3 _position, float _size, Color4 _color)
		{
			// 各パラメーターを設定
			this.position = _position;
			this.size = _size;
			this.color = _color;
		}
		
	}
}