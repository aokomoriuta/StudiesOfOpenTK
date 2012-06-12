using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace LWisteria.StudiesOfOpenTK.MultiBuffer
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


			// プログラムを初期化
			int program = 0;

			// バッファーオブジェクトを初期化
			int[] vbo = { 0, 0 };
			int[] vao = { 0, 0 };
			int[] ebo = { 0, 0 };

			// 頂点数はゼロに初期化
			int[] indexCount = { 0, 0 };

			// カメラパラメーター
			double r = 100;
			double theta = 0;
			double phi = 0;

			// 再描画処理
			Action redraw = () =>
			{
				// カメラの位置を計算
				var cameraPosition = new Vector3(
							(float)(Math.Cos(theta) * Math.Cos(phi)),
							(float)(Math.Sin(theta) * Math.Cos(phi)),
							(float)(Math.Sin(phi)));

				// ビュー（カメラ）を作成して割り当て
				Matrix4 view =
					Matrix4.LookAt(cameraPosition,
						new Vector3(0, 0, 0),
						new Vector3(0, 0, 1)
					);
				GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);

				// 光源の方向を計算
				GL.Uniform3(GL.GetUniformLocation(program, "light"), -cameraPosition);

				// 再描画
				this.glControl.Invalidate();
			};

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// シェーダーとプログラムの作成
				{
					// コントロールを有効化
					glControl.MakeCurrent();

					// シェーダーの状態
					int vertexShaderStatus;
					int geometryShaderStatus;
					int flagmentShaderStatus;

					// 頂点・ジオメトリ・フラグメントリシェーダーを作成
					int vertexShader = GL.CreateShader(ShaderType.VertexShader);
					int geometryShader = GL.CreateShader(ShaderType.GeometryShaderExt);
					int flagmentShader = GL.CreateShader(ShaderType.FragmentShader);

					// ソースから読み込み
					GL.ShaderSource(vertexShader, Properties.Resources.vertex);
					GL.ShaderSource(geometryShader, Properties.Resources.geometry);
					GL.ShaderSource(flagmentShader, Properties.Resources.fragment);

					// コンパイル
					GL.CompileShader(vertexShader);
					GL.CompileShader(geometryShader);
					GL.CompileShader(flagmentShader);

					// シェーダーを読み込み
					GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out vertexShaderStatus);
					GL.GetShader(geometryShader, ShaderParameter.CompileStatus, out geometryShaderStatus);
					GL.GetShader(flagmentShader, ShaderParameter.CompileStatus, out flagmentShaderStatus);

					// ログを取得
					string vertexLog = GL.GetShaderInfoLog(vertexShader);
					string geometryLog = GL.GetShaderInfoLog(geometryShader);
					string flagmentLog = GL.GetShaderInfoLog(flagmentShader);

					// コンパイルに失敗していたら
					if(vertexLog + geometryLog + flagmentLog != "")
					{
						// 例外
						throw new ApplicationException(String.Format(
							"Vertex:{0}" + System.Environment.NewLine +
							"Geometry:{1}" + System.Environment.NewLine +
							"Flagment:{2}" + System.Environment.NewLine,
							vertexLog, geometryLog, flagmentLog));
					}

					// プログラムを作成して、シェーダーを設定
					program = GL.CreateProgram();
					GL.AttachShader(program, vertexShader);
					GL.AttachShader(program, geometryShader);
					GL.AttachShader(program, flagmentShader);

					// プログラムをリンク
					GL.LinkProgram(program);

					// 不要になったシェーダーを削除
					GL.DeleteShader(vertexShader);
					GL.DeleteShader(geometryShader);
					GL.DeleteShader(flagmentShader);

					// 使用するプログラムの指定
					GL.UseProgram(program);
				}

				// 頂点・頂点インデックスの作成
				{
					// 明るい立方体データの作成
					var lightCubes = new Cube[]
					{
						new Cube(new Vector3(0, 0, 0), 0.2f, new Color4(255,   0,   0, 255)),
						new Cube(new Vector3(2, 0, 0), 0.4f, new Color4(  0, 255,   0, 255)),
						new Cube(new Vector3(4, 0, 0), 0.6f, new Color4(  0,   0, 255, 255)),
						new Cube(new Vector3(0, 2, 0), 0.8f, new Color4(255, 255,   0, 255)),
						new Cube(new Vector3(0, 4, 0), 1.0f, new Color4(  0, 255, 255, 255)),
						new Cube(new Vector3(0, 6, 0), 1.2f, new Color4(255,   0, 255, 255)),
					};
					var lightCubesIndices = new uint[]
					{
						0,
						1,
						2,
						3,
						4,
						5
					};

					// 暗い立方体データの作成
					var darkCubes = new Cube[]
					{
						new Cube(new Vector3( 0,  0, 2), 0.2f, new Color4(100,   0,   0, 255)),
						new Cube(new Vector3(-2,  0, 2), 0.4f, new Color4(  0, 100,   0, 255)),
						new Cube(new Vector3(-4,  0, 2), 0.6f, new Color4(  0,   0, 100, 255)),
						new Cube(new Vector3( 0, -2, 2), 0.8f, new Color4(100, 100,   0, 255)),
						new Cube(new Vector3( 0, -4, 2), 1.0f, new Color4(  0, 100, 100, 255)),
						new Cube(new Vector3( 0, -6, 2), 1.2f, new Color4(100,   0, 100, 255)),
					};
					var darkCubesIndices = new uint[]
					{
						0,
						1,
						2,
						3,
						4,
						5
					};

					// インデックス数設定
					indexCount[0] = lightCubesIndices.Length;
					indexCount[1] = darkCubesIndices.Length;


					// 頂点バッファー(VBO)を作成
					GL.GenBuffers(vbo.Length, vbo);

					// 明るい立方体のVBOを有効化（バインド）
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[0]);

					// 明るい立方体のVBOを確保してデータを割り当て
					GL.BufferData<Cube>(BufferTarget.ArrayBuffer,
						new IntPtr(Cube.SizeInByte * lightCubes.Length),
						lightCubes,
						BufferUsageHint.StaticDraw);

					// 暗い立方体のVBOを有効化（バインド）
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[1]);

					// 暗い立方体のVBOを確保してデータ割り当て
					GL.BufferData<Cube>(BufferTarget.ArrayBuffer,
						new IntPtr(Cube.SizeInByte * darkCubes.Length),
						darkCubes,
						BufferUsageHint.StaticDraw);

					// VBOを無効化
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


					// 頂点インデックスバッファー(EBO)を作成
					GL.GenBuffers(ebo.Length, ebo);

					// 明るい立方体のEBOを有効化（バインド）
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[0]);

					// 明るい立方体のEBOを確保してデータ割り当て
					GL.BufferData<uint>(
						BufferTarget.ElementArrayBuffer,
						new IntPtr(sizeof(uint) * indexCount[0]),
						lightCubesIndices,
						BufferUsageHint.StaticDraw);

					// 暗い立方体のEBOを有効化（バインド）
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[1]);

					// 暗い立方体のEBOを確保してデータ割り当て
					GL.BufferData<uint>(
						BufferTarget.ElementArrayBuffer,
						new IntPtr(sizeof(uint) * indexCount[1]),
						darkCubesIndices,
						BufferUsageHint.StaticDraw);

					// EBOを無効化
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				}

				// 頂点データとシェーダーの関連付け
				{
					// シェーダー内の位置の場所を取得
					int positionLocation = GL.GetAttribLocation(program, "cubePosition");
					int sizeLocation = GL.GetAttribLocation(program, "cubeSize");
					int colorLocation = GL.GetAttribLocation(program, "cubeColor");

					// 頂点配列バッファー(VAO)を作成
					GL.GenVertexArrays(vao.Length, vao);


					// 明るい立方体のVAOを有効化
					GL.BindVertexArray(vao[0]);

					// 明るい立方体のVAOに、明るい立方体のVBOを割り当て
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[0]);

					// 位置を有効化
					GL.EnableVertexAttribArray(positionLocation);
					GL.EnableVertexAttribArray(sizeLocation);
					GL.EnableVertexAttribArray(colorLocation);

					// 各パラメーターのオフセットを設定
					GL.VertexAttribPointer(
						positionLocation,
						3,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						0);
					GL.VertexAttribPointer(
						sizeLocation,
						1,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes);
					GL.VertexAttribPointer(
						colorLocation,
						4,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes + sizeof(float));


					// 暗い立方体のVAOを有効化
					GL.BindVertexArray(vao[1]);

					// 暗い立方体のVAOに、暗い立方体のVBOを割り当て
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[1]);

					// 位置を有効化
					GL.EnableVertexAttribArray(positionLocation);
					GL.EnableVertexAttribArray(sizeLocation);
					GL.EnableVertexAttribArray(colorLocation);

					// 各パラメーターのオフセットを設定
					GL.VertexAttribPointer(
						positionLocation,
						3,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						0);
					GL.VertexAttribPointer(
						sizeLocation,
						1,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes);
					GL.VertexAttribPointer(
						colorLocation,
						4,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes + sizeof(float));


					// VAOとVBOを無効化
					GL.BindVertexArray(0);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				}

				// カメラや光源などその他の設定
				{
					// 深度を有効化
					GL.Enable(EnableCap.DepthTest);

					// シェーダー側で点の大きさを変更可能に
					GL.Enable(EnableCap.ProgramPointSize);

					// 画面全体を表示
					GL.Viewport(0, 0, this.glControl.Width, this.glControl.Height);

					// 投影法を作成して割り当て
					Matrix4 projection = Matrix4.CreateOrthographic(
						(float)(this.glControl.Width / r),
						(float)(this.glControl.Height / r),
						-20, 20);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);

					// 環境光強度を割り当て
					GL.Uniform1(GL.GetUniformLocation(program, "ambient"), 0.3f);

					// 再描画
					redraw();
				}
			};

			// 以前のマウス位置
			Vector2? oldMouseLocation = null;

			// マウスが動いたら
			this.glControl.MouseMove += (sender, e) =>
			{
				// 左ボタンが押されていたら
				if(e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					// マウス位置を取得
					var thisMouseLocation = new Vector2(e.Location.X, e.Location.Y);

					// 一番最初でなければ
					if(oldMouseLocation != null)
					{
						// 移動量を計算
						var delta = thisMouseLocation - oldMouseLocation.Value;

						// 角度を変更
						theta += -2 * Math.PI * delta.X / this.glControl.Width;
						phi += Math.PI * delta.Y / this.glControl.Height;

						// 仰角は-π/2からπ/2まで
						phi = Math.Max(-Math.PI / 2, phi);
						phi = Math.Min(phi, Math.PI / 2);

						// 水平角は0から2πまで
						theta = (theta >= 0) ? theta : 2 * Math.PI + theta;
						theta = (theta <= 2 * Math.PI) ? theta : theta - 2 * Math.PI;
					}

					// 前の位置を覚えておく
					oldMouseLocation = thisMouseLocation;

					// 再描画
					redraw();
				}
				// それ以外の場合
				else
				{
					// マウス位置を初期化
					oldMouseLocation = null;
				}
			};

			// マウスホイールされたら
			this.glControl.MouseWheel += (sender, e) =>
			{
				// 距離を変更
				r *= Math.Pow(1.5, Math.Sign(e.Delta));

				// 投影法を作成して割り当て
				Matrix4 projection = Matrix4.CreateOrthographic(
					(float)(this.glControl.Width / r),
					(float)(this.glControl.Height / r),
					-20, 20);
				GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);

				// 再描画
				this.glControl.Invalidate();
			};

			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// コントロールを有効化
				glControl.MakeCurrent();

				// 画面をクリア
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


				// 明るい立方体のVAOとEBOを有効化
				GL.BindVertexArray(vao[0]);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[0]);

				// 明るい立方体を描画
				GL.DrawElements(BeginMode.Points, indexCount[0], DrawElementsType.UnsignedInt, 0);


				// 暗い立方体のVAOとEBOを有効化 
				GL.BindVertexArray(vao[1]);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[1]);

				// 暗い立方体を描画
				GL.DrawElements(BeginMode.Points, indexCount[1], DrawElementsType.UnsignedInt, 0);


				// VAOとEBOを無効化
				GL.BindVertexArray(0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


				// 画面に表示
				glControl.SwapBuffers();
			};

			// 終了時に
			this.Closed += (sender, e) =>
			{
				// バッファー等を削除
				GL.DeleteBuffers(vbo.Length, vbo);
				GL.DeleteBuffers(ebo.Length, ebo);
				GL.DeleteVertexArrays(vao.Length, vao);

				// プログラムを削除
				GL.DeleteProgram(program);
			};
		}
	}
}