using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK
{
	/// <summary>
	/// 3次元カメラ操作付きビューポート
	/// </summary>
	public class Viewport3D : Viewport
	{
		/// <summary>
		/// カメラの距離
		/// </summary>
		public double R { set; get; }

		/// <summary>
		/// 水平角
		/// </summary>
		public double Theta { set; get; }

		/// <summary>
		/// 仰角
		/// </summary>
		public double Phi { set; get; }

		/// <summary>
		/// コントロールを作成する
		/// </summary>
		public Viewport3D()
		{
			// コントロール上でマウスホイールされたら
			this.glControl.MouseWheel += (sender2, e2) =>
			{
				// 距離を変更
				this.R *= Math.Pow(1.5, Math.Sign(e2.Delta));

				// 再描画
				this.Invalidate();
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
						this.Theta += -2 * Math.PI * delta.X / this.glControl.Width;
						this.Phi += Math.PI * delta.Y / this.glControl.Height;

						// 仰角は-π/2からπ/2まで
						this.Phi = Math.Max(-Math.PI / 2, this.Phi);
						this.Phi = Math.Min(this.Phi, Math.PI / 2);

						// 水平角は0から2πまで
						this.Theta = (this.Theta >= 0) ? this.Theta : 2 * Math.PI + this.Theta;
						this.Theta = (this.Theta <= 2 * Math.PI) ? this.Theta : this.Theta - 2 * Math.PI;
					}

					// 前の位置を覚えておく
					oldMouseLocation = thisMouseLocation;

					// 再描画
					this.Invalidate();
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
		/// 再描画する
		/// </summary>
		public void Invalidate()
		{
			// カメラの位置を計算
			var cameraPosition = new Vector3(
						(float)(Math.Cos(this.Theta) * Math.Cos(this.Phi)),
						(float)(Math.Sin(this.Theta) * Math.Cos(this.Phi)),
						(float)(Math.Sin(this.Phi)));

			// ビュー（カメラ）を作成して割り当て
			base.Program.SetUniform("view",
				Matrix4.LookAt(cameraPosition,
					new Vector3(0, 0, 0),
					new Vector3(0, 0, 1)
				));

			// 投影法を作成して割り当て
			base.Program.SetUniform("projection",
				Matrix4.CreateOrthographic(
				(float)(this.ActualWidth / this.R),
				(float)(this.ActualHeight / this.R),
				-20, 20)
			);

			// 光源の方向を計算
			base.Program.SetUniform("light", -cameraPosition);

			// コントロールを再描画
			this.glControl.Invalidate();
		}
	}
}