using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace LWisteria.StudiesOfOpenTK.MultiProgram
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
			int[] program = { 0, 0 };

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

				// ビュー（カメラ）を作成
				Matrix4 view =
					Matrix4.LookAt(cameraPosition,
						new Vector3(0, 0, 0),
						new Vector3(0, 0, 1)
					);

				// 立方体プログラムでビューと光源を割り当て
				GL.UseProgram(program[0]);
				GL.UniformMatrix4(GL.GetUniformLocation(program[0], "view"), false, ref view);
				GL.Uniform3(GL.GetUniformLocation(program[0], "light"), -cameraPosition);

				// 三角錐プログラムでビューと光源を割り当て
				GL.UseProgram(program[1]);
				GL.UniformMatrix4(GL.GetUniformLocation(program[1], "view"), false, ref view);
				GL.Uniform3(GL.GetUniformLocation(program[1], "light"), -cameraPosition);

				// 使用するプログラムを無効化
				GL.UseProgram(0);

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
					int[] vertexShaderStatus = { 0, 0 };
					int[] geometryShaderStatus = { 0, 0 };
					int[] flagmentShaderStatus = { 0, 0 };

					// 頂点・ジオメトリ・フラグメントリシェーダーを作成
					int[] vertexShader = { GL.CreateShader(ShaderType.VertexShader), GL.CreateShader(ShaderType.VertexShader) };
					int[] geometryShader = { GL.CreateShader(ShaderType.GeometryShaderExt), GL.CreateShader(ShaderType.GeometryShaderExt) };
					int[] flagmentShader = { GL.CreateShader(ShaderType.FragmentShader), GL.CreateShader(ShaderType.FragmentShader) };

					// ソースから読み込み
					GL.ShaderSource(vertexShader[0], Properties.Resources.cube_vertex);
					GL.ShaderSource(geometryShader[0], Properties.Resources.cube_geometry);
					GL.ShaderSource(flagmentShader[0], Properties.Resources.cube_fragment);
					GL.ShaderSource(vertexShader[1], Properties.Resources.pyramid_vertex);
					GL.ShaderSource(geometryShader[1], Properties.Resources.pyramid_geometry);
					GL.ShaderSource(flagmentShader[1], Properties.Resources.pyramid_fragment);

					// コンパイル
					GL.CompileShader(vertexShader[0]);
					GL.CompileShader(geometryShader[0]);
					GL.CompileShader(flagmentShader[0]);
					GL.CompileShader(vertexShader[1]);
					GL.CompileShader(geometryShader[1]);
					GL.CompileShader(flagmentShader[1]);

					// シェーダーを読み込み
					GL.GetShader(vertexShader[0], ShaderParameter.CompileStatus, out vertexShaderStatus[0]);
					GL.GetShader(geometryShader[0], ShaderParameter.CompileStatus, out geometryShaderStatus[0]);
					GL.GetShader(flagmentShader[0], ShaderParameter.CompileStatus, out flagmentShaderStatus[0]);
					GL.GetShader(vertexShader[1], ShaderParameter.CompileStatus, out vertexShaderStatus[1]);
					GL.GetShader(geometryShader[1], ShaderParameter.CompileStatus, out geometryShaderStatus[1]);
					GL.GetShader(flagmentShader[1], ShaderParameter.CompileStatus, out flagmentShaderStatus[1]);

					// ログを取得
					string[] vertexLog = { GL.GetShaderInfoLog(vertexShader[0]), GL.GetShaderInfoLog(vertexShader[1]) };
					string[] geometryLog = { GL.GetShaderInfoLog(geometryShader[0]), GL.GetShaderInfoLog(geometryShader[1]) };
					string[] flagmentLog = { GL.GetShaderInfoLog(flagmentShader[0]), GL.GetShaderInfoLog(flagmentShader[1]) };

					// コンパイルに失敗していたら
					if(vertexLog[0] + geometryLog[0] + flagmentLog[0] != "")
					{
						// 例外
						throw new ApplicationException(String.Format(
							"Vertex:{0}" + System.Environment.NewLine +
							"Geometry:{1}" + System.Environment.NewLine +
							"Flagment:{2}" + System.Environment.NewLine,
							vertexLog[0], geometryLog[0], flagmentLog[0]));
					}
					else if(vertexLog[1] + geometryLog[1] + flagmentLog[1] != "")
					{
						// 例外
						throw new ApplicationException(String.Format(
							"Vertex:{0}" + System.Environment.NewLine +
							"Geometry:{1}" + System.Environment.NewLine +
							"Flagment:{2}" + System.Environment.NewLine,
							vertexLog[1], geometryLog[1], flagmentLog[1]));
					}

					// 立方体プログラムを作成して、シェーダーを設定
					program[0] = GL.CreateProgram();
					GL.AttachShader(program[0], vertexShader[0]);
					GL.AttachShader(program[0], geometryShader[0]);
					GL.AttachShader(program[0], flagmentShader[0]);

					// 三角錐プログラムを作成して、シェーダーを設定
					program[1] = GL.CreateProgram();
					GL.AttachShader(program[1], vertexShader[1]);
					GL.AttachShader(program[1], geometryShader[1]);
					GL.AttachShader(program[1], flagmentShader[1]);

					// プログラムをリンク
					GL.LinkProgram(program[0]);
					GL.LinkProgram(program[1]);

					// 不要になったシェーダーを削除
					GL.DeleteShader(vertexShader[0]);
					GL.DeleteShader(geometryShader[0]);
					GL.DeleteShader(flagmentShader[0]);
					GL.DeleteShader(vertexShader[1]);
					GL.DeleteShader(geometryShader[1]);
					GL.DeleteShader(flagmentShader[1]);
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
					int[] positionLocation = { GL.GetAttribLocation(program[0], "cubePosition"), GL.GetAttribLocation(program[1], "cubePosition") };
					int[] sizeLocation = { GL.GetAttribLocation(program[0], "cubeSize"), GL.GetAttribLocation(program[1], "cubeSize") };
					int[] colorLocation = { GL.GetAttribLocation(program[0], "cubeColor"), GL.GetAttribLocation(program[1], "cubeColor") };

					// 頂点配列バッファー(VAO)を作成
					GL.GenVertexArrays(vao.Length, vao);


					// 明るい立方体のVAOを有効化
					GL.BindVertexArray(vao[0]);

					// 明るい立方体のVAOに、明るい立方体のVBOを割り当て
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[0]);

					// 立方体プログラムで位置を有効化
					GL.EnableVertexAttribArray(positionLocation[0]);
					GL.EnableVertexAttribArray(sizeLocation[0]);
					GL.EnableVertexAttribArray(colorLocation[0]);

					// 各パラメーターのオフセットを設定
					GL.VertexAttribPointer(
						positionLocation[0],
						3,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						0);
					GL.VertexAttribPointer(
						sizeLocation[0],
						1,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes);
					GL.VertexAttribPointer(
						colorLocation[0],
						4,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes + sizeof(float));


					// 暗い立方体のVAOを有効化
					GL.BindVertexArray(vao[1]);

					// 暗い立方体のVAOに、暗い立方体のVBOを割り当て
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[1]);

					// 三角錐プログラムで位置を有効化
					GL.EnableVertexAttribArray(positionLocation[1]);
					GL.EnableVertexAttribArray(sizeLocation[1]);
					GL.EnableVertexAttribArray(colorLocation[1]);

					// 各パラメーターのオフセットを設定
					GL.VertexAttribPointer(
						positionLocation[1],
						3,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						0);
					GL.VertexAttribPointer(
						sizeLocation[1],
						1,
						VertexAttribPointerType.Float,
						false,
						Cube.SizeInByte,
						Vector3.SizeInBytes);
					GL.VertexAttribPointer(
						colorLocation[1],
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

					// 投影法を作成
					Matrix4 projection = Matrix4.CreateOrthographic(
						(float)(this.glControl.Width / r),
						(float)(this.glControl.Height / r),
						-20, 20);

					// 立方体プログラムに投影法と環境光強度を割り当て
					GL.UseProgram(program[0]);
					GL.UniformMatrix4(GL.GetUniformLocation(program[0], "projection"), false, ref projection);
					GL.Uniform1(GL.GetUniformLocation(program[0], "ambient"), 0.3f);

					// 三角錐プログラムに投影法と環境光強度を割り当て
					GL.UseProgram(program[1]);
					GL.UniformMatrix4(GL.GetUniformLocation(program[1], "projection"), false, ref projection);
					GL.Uniform1(GL.GetUniformLocation(program[1], "ambient"), 0.3f);

					// 使用プログラムの無効化
					GL.UseProgram(0);

					// 再描画
					redraw();
				}

				// 一番最初にコントロールにフォーカスを当てておく
				this.glControl.Focus();
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

				// 投影法を作成
				Matrix4 projection = Matrix4.CreateOrthographic(
					(float)(this.glControl.Width / r),
					(float)(this.glControl.Height / r),
					-20, 20);

				// 立方体プログラムに投影法を割り当て
				GL.UseProgram(program[0]);
				GL.UniformMatrix4(GL.GetUniformLocation(program[0], "projection"), false, ref projection);

				// 三角錐プログラムに投影法を割り当て
				GL.UseProgram(program[1]);
				GL.UniformMatrix4(GL.GetUniformLocation(program[1], "projection"), false, ref projection);

				// 使用するプログラムの指定
				GL.UseProgram(0);

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


				// 立方体プログラムを有効化
				GL.UseProgram(program[0]);

				// 明るい立方体のVAOとEBOを有効化
				GL.BindVertexArray(vao[0]);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[0]);

				// 明るい立方体を描画
				GL.DrawElements(BeginMode.Points, indexCount[0], DrawElementsType.UnsignedInt, 0);


				// 三角錐プログラムをゆこうか
				GL.UseProgram(program[1]);

				// 暗い立方体のVAOとEBOを有効化 
				GL.BindVertexArray(vao[1]);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo[1]);

				// 暗い立方体を描画
				GL.DrawElements(BeginMode.Points, indexCount[1], DrawElementsType.UnsignedInt, 0);


				// 使用するプログラムの無効化
				GL.UseProgram(0);


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
				GL.DeleteProgram(program[0]);
				GL.DeleteProgram(program[1]);
			};
		}
	}
}