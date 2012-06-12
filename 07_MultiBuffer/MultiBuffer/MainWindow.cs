using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using CompositionTarget = System.Windows.Media.CompositionTarget;

using System.Drawing;

using SlimDX;
using SlimDX.Direct3D9;
namespace LWisteria.StudiesOfSlimDX.MouseControl
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow()
		{
			// コンポーネント初期化
			InitializeComponent();

			// ウインドウが読み込まれたら
			this.Loaded += (sender, e) =>
			{
				// Direct3D作成
				var direct3D = new SlimDX.Direct3D9.Direct3D();

				// デバイスを初期化
				Device device = null;

				// ティーポッドと文字列を初期化
				Mesh teapod = null;
				Mesh helloWorld = null;

				// バッファーウインドウ作成
				IntPtr handle = new HwndSource(0, 0, 0, 0, 0, "buffer", IntPtr.Zero).Handle;

				// デバイスを作成
				device = new Device(direct3D, 0,
					DeviceType.Hardware,
					handle,
					CreateFlags.HardwareVertexProcessing,
					new PresentParameters()
					{
						BackBufferFormat = Format.X8R8G8B8,
						BackBufferCount = 1,
						BackBufferWidth = (int)this.Width,
						BackBufferHeight = (int)this.Height,
						Multisample = MultisampleType.None,
						SwapEffect = SwapEffect.Discard,
						EnableAutoDepthStencil = true,
						AutoDepthStencilFormat = Format.D16,
						PresentFlags = PresentFlags.DiscardDepthStencil,
						PresentationInterval = PresentInterval.Default,
						Windowed = true,
						DeviceWindowHandle = handle
					});

				// ティーポッドと文字列を作成
				teapod = Mesh.CreateTeapot(device);
				helloWorld = Mesh.CreateText(device, new System.Drawing.Font("Arial", 10), "Hello, world!", 0.001f, 0.001f);

				// 光源描画を有効化
				device.SetRenderState(RenderState.Lighting, true);

				// 光源を設定
				device.SetLight(0, new Light()
				{
					Type = LightType.Directional,
					Diffuse = Color.White,
					Ambient = Color.GhostWhite,
					Direction = new Vector3(0.0f, -1.0f, 0.0f)
				});

				// 光源を有効化
				device.EnableLight(0, true);


				//射影変換を設定
				device.SetTransform(TransformState.Projection,
					Matrix.PerspectiveFovLH((float)(Math.PI / 4),
					(float)(this.Width / this.Height),
					0.1f, 20.0f));

				//ビューを設定
				device.SetTransform(TransformState.View,
					Matrix.LookAtLH(new Vector3(3.0f, 2.0f, -3.0f),
					Vector3.Zero,
					new Vector3(0.0f, 1.0f, 0.0f)));

				//マテリアル設定
				device.Material = new Material()
				{
					Diffuse = new Color4(Color.GhostWhite)
				};

				// 描画される時に描画処理を実行
				CompositionTarget.Rendering += (sender2, e2) =>
				{
					// 画像をロックしてバックバッファーに設定
					this.directXImage.Lock();
					this.directXImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, device.GetBackBuffer(0, 0).ComPointer);

					// フロントバッファーがあれば
					if (directXImage.IsFrontBufferAvailable)
					{
						// デバイスの現在の状態を確かめて失敗したら
						if (device.TestCooperativeLevel().IsFailure)
						{
							// 例外
							throw new Direct3D9Exception("デバイスが無効です");
						}

						// 全面灰色でクリア
						device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0.3f, 0.3f, 0.3f), 1.0f, 0);

						// シーン開始
						device.BeginScene();

						//ティーポットを描画
						device.SetTransform(TransformState.World,
							Matrix.Translation(-2, 0, 5));
						teapod.DrawSubset(0);

						//文字列を描画
						device.SetTransform(TransformState.World,
							Matrix.Translation(-4, 0, 0));
						helloWorld.DrawSubset(0);

						// シーン終了
						device.EndScene();

						// バッファー更新
						device.Present();
					}

					// バックバッファー領域を指定
					this.directXImage.AddDirtyRect(new Int32Rect(0, 0, directXImage.PixelWidth, directXImage.PixelHeight));

					// ロック解除
					this.directXImage.Unlock();
				};

				// イメージのバックバッファーを設定
				directXImage.Lock();
				directXImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, device.GetBackBuffer(0, 0).ComPointer);
				directXImage.Unlock();
			};
		}
	}
}