using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// 3次元カメラ操作付きビューポート
	/// </summary>
	public class Viewport3D : Viewport
	{
		// カメラ
		public readonly Camera Camera;

		/// <summary>
		/// コントロールを作成する
		/// </summary>
		public Viewport3D()
		{
			// カメラを作成
			this.Camera = new Camera();

			// コントロール上でマウスホイールされたら
			this.glControl.MouseWheel += (sender2, e2) =>
			{
				// 距離を変更
				this.Camera.R *= Math.Pow(1.5, Math.Sign(e2.Delta));
			};

			// 以前のマウス位置
			Vector2? oldMouseLocation = null;

			// マウスが動いたら
			this.glControl.MouseMove += (sender2, e2) =>
			{
				// 左ボタンが押されていたら
				if(e2.Button == System.Windows.Forms.MouseButtons.Left)
				{
					// マウス位置を取得
					var thisMouseLocation = new Vector2(e2.X, e2.Y);

					// 一番最初でなければ
					if(oldMouseLocation != null)
					{
						// 移動量を計算
						var delta = thisMouseLocation - oldMouseLocation.Value;

						// 角度を変更
						this.Camera.Theta += -2 * Math.PI * delta.X / this.glControl.Width;
						this.Camera.Phi += Math.PI * delta.Y / this.glControl.Height;
					}

					// 前の位置を覚えておく
					oldMouseLocation = thisMouseLocation;
				}
				// それ以外の場合
				else
				{
					// マウス位置を初期化
					oldMouseLocation = null;
				}
			};
		}

		/// <summary>
		/// ビューを取得する
		/// </summary>
		public Matrix4 View
		{
			get
			{
				// カメラの位置から計算して作成
				return Matrix4.LookAt(this.Camera.Position,
					new Vector3(0, 0, 0),
					new Vector3(0, 0, 1));
			}
		}

		/// <summary>
		/// 投影法を取得する
		/// </summary>
		public Matrix4 Projection
		{
			get
			{
				// カメラの距離から計算して作成
				return Matrix4.CreateOrthographic(
					(float)(this.ActualWidth / this.Camera.R),
					(float)(this.ActualHeight / this.Camera.R),
					-20, 20);
			}
		}
	}
}