using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Windows.Media.Capture;
using Microsoft;
using System;
using System.Windows;
using Windows.Phone.Media;
using System.Drawing;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Devices.Geolocation;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace RabbitChasev1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Rabbit
    {
        Model model;
        Texture2D rabbitTex;
        public Matrix rotation;
        public void Initialize(ContentManager contentManager)
        {
            model = contentManager.Load<Model>("Rabbit");
            rabbitTex = contentManager.Load<Texture2D>("ForestRabbit.jpg");
        }

        public void Update(ContentManager contentManager, String theme)
        {
            rabbitTex = contentManager.Load<Texture2D>(theme+"Rabbit.jpg");
        }

        public void Draw(Vector3 cameraPosition, float aspectRatio)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = rabbitTex;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = GetWorldMatrix();
                    var cameraLookAtVector = Vector3.Zero;
                    var cameraUpVector = Vector3.UnitZ;

                    effect.View = Matrix.CreateLookAt(
                        cameraPosition, cameraLookAtVector, cameraUpVector);

                    float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                    float nearClipPlane = 1;
                    float farClipPlane = 200;

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        fieldOfView, aspectRatio, nearClipPlane, farClipPlane);


                }
                mesh.Draw();
            }
        }

        Matrix GetWorldMatrix()
        {
            const float circleRadius = 8;
            const float heightOffGround = 40;

            Matrix translationMatrix = Matrix.CreateTranslation(
                circleRadius, 0, heightOffGround);

            Matrix resize = Matrix.CreateScale((float).10);

            Matrix rotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(180));

            Matrix combined = translationMatrix * rotationMatrix * resize * rotation;

            return combined;
        }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        string theme = "Forest";

        //Music-Sound
        Song Background;
        List<SoundEffect> soundeffects;
        SoundEffectInstance stream, cricket, leaves;
        AudioListener ears;
        AudioEmitter streamposition, cricketposition, leavesposition;
        
        //X,Y,Z Coordinates based on Rotation matrix. (For Rabbit)
        OrientationSensor accelpos;
        Matrix rotationmatrix;

        //Rabbit
        Rabbit rabs;

        //Background
        Texture2D BG;
        Vector3 cameraPosition = new Vector3(0, 1, 40);
        VertexPositionTexture[] floorVerts;
        BasicEffect effect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            soundeffects = new List<SoundEffect>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            //testing floor
            floorVerts = new VertexPositionTexture[6];

            floorVerts[0].Position = new Vector3(-40, -20, 0);
            floorVerts[1].Position = new Vector3(-40, 20, 0);
            floorVerts[2].Position = new Vector3(40, -20, 0);
            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(40, 20, 0);
            floorVerts[5].Position = floorVerts[2].Position;
            effect = new BasicEffect(graphics.GraphicsDevice);
            floorVerts[0].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, 1);
            floorVerts[2].TextureCoordinate = new Vector2(1, 0);

            floorVerts[3].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[4].TextureCoordinate = new Vector2(1, 1);
            floorVerts[5].TextureCoordinate = floorVerts[2].TextureCoordinate;

            rabs = new Rabbit();

            //orientation sensor initialization
            rabs.Initialize(Content);
            accelpos = OrientationSensor.GetDefault();
            //SensorRotationMatrix rotate = accelpos.GetCurrentReading().RotationMatrix;
            //currentstart.M11 = rotate.M11;//right+ Left-
            //currentstart.M12 = rotate.M12;//right+ Left-
            //currentstart.M13 = rotate.M13;//right+ Left-
            //currentstart.M14 = 0;
            //currentstart.M21 = rotate.M21;//up+  down -
            //currentstart.M22 = rotate.M22;//up+  down -
            //currentstart.M23 = rotate.M23;//up+  down -
            //currentstart.M24 = 0;
            //currentstart.M31 = rotate.M31;//backward+  forward -
            //currentstart.M32 = rotate.M32;//backward+  forward -
            //currentstart.M33 = rotate.M33;//backward+  forward -
            //currentstart.M34 = 0;
            //currentstart.M41 = 0;
            //currentstart.M42 = 0;
            //currentstart.M43 = 0;
            //currentstart.M44 = 1;
            ScenarioEnable();
        }

        void DrawWall()
        {
            // The assignment of effect.View and effect.Projection
            // are nearly identical to the code in the Model drawing code.

            effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitZ);

            effect.TextureEnabled = true;
            effect.Texture = BG;

            float aspectRatio =
                graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);


            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, floorVerts, 0, 2);
            }
        }

        protected override void LoadContent()
        {
            BG = Content.Load<Texture2D>(theme+".jpg");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Background = Content.Load<Song>("01 - The End (T.E.I.N. Pt. 2)");
            soundeffects.Add(Content.Load<SoundEffect>("MonoStream"));
            soundeffects.Add(Content.Load<SoundEffect>("MonoCricket"));
            soundeffects.Add(Content.Load<SoundEffect>("MonoLeaves"));
            ears = new AudioListener();
            streamposition = new AudioEmitter();
            cricketposition = new AudioEmitter();
            leavesposition = new AudioEmitter();
            streamposition.Position = new Vector3(0, 0, 2);
            cricketposition.Position = new Vector3(3, 0, 0);
            leavesposition.Position = new Vector3(0, 3, 0);
        }

        public void musicstart()
        {
            //stream in 3d positioning.
            stream = soundeffects[0].CreateInstance();
            stream.IsLooped = true;
            stream.Apply3D(ears, streamposition);
            stream.Play();

            //Cricket in 3d positioning
            cricket = soundeffects[1].CreateInstance();
            cricket.IsLooped = true;
            cricket.Apply3D(ears, cricketposition);
            cricket.Play();

            //Leaves in 3d positioning
            leaves = soundeffects[2].CreateInstance();
            leaves.IsLooped = true;
            leaves.Apply3D(ears, leavesposition);
            leaves.Play();

            //MediaPlayer.Play(Background);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        int wait = 0;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            wait += 1;
            if(wait==100)
            {
                var app = App.Current as App;
                System.Diagnostics.Debug.WriteLine(app.theme);
                if(app.theme!=theme)
                {
                    theme = app.theme;
                    BG = Content.Load<Texture2D>(theme + ".jpg");
                    rabs.Update(Content, theme);
                }
                wait = 0;
            }
            //rotationx += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float aspectRatio =  graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            rabs.Draw(cameraPosition, aspectRatio);
            DrawWall();
            base.Draw(gameTime);
        }

        /// <summary>
        /// This here would be the sensor readings. 
        /// These can be taken out if the ROTY, ROTX, and ROTZ are moved
        /// </summary>
        private void ScenarioEnable()
        {
            if (accelpos != null)
            {
                accelpos.ReadingChanged += new TypedEventHandler<OrientationSensor, OrientationSensorReadingChangedEventArgs>(ReadingChanged);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Acceleromater Not found");
            }
        }

        private void ReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            OrientationSensorReading reading = e.Reading;
            SensorRotationMatrix rotate = reading.RotationMatrix;
            rotationmatrix.M11 = rotate.M11;//right+ Left-
            rotationmatrix.M12 = rotate.M12;//right+ Left-
            rotationmatrix.M13 = rotate.M13;//right+ Left-
            rotationmatrix.M14 = 0;
            rotationmatrix.M21 = rotate.M21;//up+  down -
            rotationmatrix.M22 = rotate.M22;//up+  down -
            rotationmatrix.M23 = rotate.M23;//up+  down -
            rotationmatrix.M24 = 0;
            rotationmatrix.M31 = rotate.M31;//backward+  forward -
            rotationmatrix.M32 = rotate.M32;//backward+  forward -
            rotationmatrix.M33 = rotate.M33;//backward+  forward -
            rotationmatrix.M34 = 0;
            rotationmatrix.M41 = 0;
            rotationmatrix.M42 = 0;
            rotationmatrix.M43 = 0;
            rotationmatrix.M44 = 1;
            Quaternion temp = Quaternion.CreateFromRotationMatrix(rotationmatrix);
            rabs.rotation = rotationmatrix;
        }
    }

}
