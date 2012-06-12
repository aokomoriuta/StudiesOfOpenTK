using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.SizeVariableCube
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
			int vbo = 0;
			int vao = 0;
			int ebo = 0;

			// 頂点数はゼロに初期化
			int indexCount = 0;

			// カメラパラメーター
			double r = 100;
			double theta = 0;
			double phi = 0;

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

					// 最大頂点数を指定
					int maxVerticesCount;
					GL.GetInteger((GetPName)ExtGeometryShader4.MaxGeometryOutputVerticesExt, out maxVerticesCount);
					GL.Ext.ProgramParameter(program, ExtGeometryShader4.GeometryVerticesOutExt, maxVerticesCount);

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
					// 立方体データの作成
					var cubes = new Cube[]
					{
						new Cube(new Vector3(0, 0, 0), 0.2f),
						new Cube(new Vector3(2, 0, 0), 0.4f),
						new Cube(new Vector3(4, 0, 0), 0.8f),
						new Cube(new Vector3(0, 2, 0), 1.0f),
						new Cube(new Vector3(0, 4, 0), 1.2f),
						new Cube(new Vector3(0, 6, 0), 1.4f),
					};

					// 頂点インデックスデータの作成
					var indices = new uint[]
					{
						0,
						1,
						2,
						3,
						4,
						5
					};

					// 頂点インデックス数設定
					indexCount = indices.Length;

					// 頂点バッファー(VBO)を作成して割り当て
					GL.GenBuffers(1, out vbo);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

					// 頂点インデックスバッファー(EBO)を作成して割り当て
					GL.GenBuffers(1, out ebo);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

					// データを書き込み
					GL.BufferData<Cube>(BufferTarget.ArrayBuffer,
						new IntPtr(Cube.SizeInByte * cubes.Length),
						cubes,
						BufferUsageHint.StaticDraw);
					GL.BufferData<uint>(
						BufferTarget.ElementArrayBuffer,
						new IntPtr(sizeof(uint) * indexCount),
						indices,
						BufferUsageHint.StaticDraw);
				}

				// 頂点データとシェーダーの関連付け
				{
					// 頂点配列バッファー(VAO)を作成して割り当て
					GL.GenVertexArrays(1, out vao);
					GL.BindVertexArray(vao);

					// シェーダー内の位置の場所を取得
					int positionLocation = GL.GetAttribLocation(program, "cubePosition");
					int sizeLocation = GL.GetAttribLocation(program, "cubeSize");

					// 位置を有効化
					GL.EnableVertexAttribArray(positionLocation);
					GL.EnableVertexAttribArray(sizeLocation);

					// 位置のサイズとオフセットを設定
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
						true,
						Cube.SizeInByte,
						Vector3.SizeInBytes);

					// 作成した頂点および頂点配列を設定
					GL.BindVertexArray(vao);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				}

				// カメラや光源などその他の設定
				{
					// 深度を有効化
					GL.Enable(EnableCap.DepthTest);

					// シェーダー側で点の大きさを変更可能に
					GL.Enable(EnableCap.ProgramPointSize);

					// 画面全体を表示
					GL.Viewport(0, 0, this.glControl.Width, this.glControl.Height);

					// ビュー（カメラ）を作成して割り当て
					Matrix4 view =
						Matrix4.LookAt(
							new Vector3(
								(float)(Math.Cos(theta) * Math.Cos(phi)),
								(float)(Math.Sin(theta) * Math.Cos(phi)),
								(float)(Math.Sin(phi))),
							new Vector3(0, 0, 0),
							new Vector3(0, 0, 1)
						);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);

					// 投影法を作成して割り当て
					Matrix4 projection = Matrix4.CreateOrthographic(
						(float)(this.glControl.Width / r),
						(float)(this.glControl.Height / r),
						-20, 20);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);
				}
			};

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
					}

					// 前の位置を覚えておく
					oldMouseLocation = thisMouseLocation;

					// ビュー（カメラ）を作成して割り当て
					Matrix4 view =
						Matrix4.LookAt(
							new Vector3(
								(float)(Math.Cos(theta) * Math.Cos(phi)),
								(float)(Math.Sin(theta) * Math.Cos(phi)),
								(float)(Math.Sin(phi))),
							new Vector3(0, 0, 0),
							new Vector3(0, 0, (float)Math.Cos(phi))
						);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);


					// 再描画
					this.glControl.Invalidate();
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

				// 描画
				GL.DrawElements(BeginMode.Points, indexCount, DrawElementsType.UnsignedInt, 0);

				// バッファーを交換
				glControl.SwapBuffers();
			};

			// 終了時に
			this.Closed += (sender, e) =>
			{
				// バッファー等を削除
				GL.DeleteBuffers(1, ref vbo);
				GL.DeleteBuffers(1, ref ebo);
				GL.DeleteVertexArrays(1, ref vao);

				// プログラムを削除
				GL.DeleteProgram(program);
			};
		}
	}
}