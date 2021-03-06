﻿using BlackHoleRaytracer.Hitable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace BlackHoleRaytracer
{
    class Program
    {
        static void Main(string[] args)
        {

            // Set up some default parameters, which can be overridden by command line args.
            var cameraPos = new Vector3(0, 3, -20);
            var lookAt = new Vector3(0, 0, 0);
            var up = new Vector3(-0.3f, 1, 0);
            float fov = 55f;
            float curvatureCoeff = -1.5f;
            float angularMomentum = 0;
            string fileName = "image.png";


            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-camera") && i < args.Length - 3)
                {
                    cameraPos = new Vector3(float.Parse(args[i + 1]), float.Parse(args[i + 2]), float.Parse(args[i + 3]));
                }
                else if (args[i].Equals("-lookat") && i < args.Length - 3)
                {
                    lookAt = new Vector3(float.Parse(args[i + 1]), float.Parse(args[i + 2]), float.Parse(args[i + 3]));
                }
                else if (args[i].Equals("-up") && i < args.Length - 3)
                {
                    up = new Vector3(float.Parse(args[i + 1]), float.Parse(args[i + 2]), float.Parse(args[i + 3]));
                }
                else if (args[i].Equals("-fov") && i < args.Length - 1)
                {
                    fov = float.Parse(args[i + 1]);
                }
                else if (args[i].Equals("-curvature") && i < args.Length - 1)
                {
                    curvatureCoeff = float.Parse(args[i + 1]);
                }
                else if (args[i].Equals("-angularmomentum") && i < args.Length - 1)
                {
                    angularMomentum = float.Parse(args[i + 1]);
                }
                else if (args[i].Equals("-o") && i < args.Length - 1)
                {
                    fileName = args[i + 1];
                }
            }
            

            var hitables = new List<IHitable>
            {
                //new CheckeredDisk(2.6, 14.0, Color.BlueViolet, Color.MediumBlue, Color.ForestGreen, Color.DarkGreen),
                new TexturedDisk(2.6, 12.0, new Bitmap("disk_textured.png")),
                //new CheckeredDisk(equation.Rmstable, 20.0, Color.BlueViolet, Color.MediumBlue, Color.ForestGreen, Color.LightSeaGreen),
                //new TexturedDisk(2, 20.0, new Bitmap("disk.jpg")),
                new Horizon(null, false),
                new Sky(new Bitmap("sky8k.jpg"), 30).SetTextureOffset(Math.PI / 2),

                //new CheckeredSphere(2, 2, -14, 1, Color.RoyalBlue, Color.DarkBlue),

                new TexturedSphere(2, 2, -10, 1, new Bitmap("earth1k.jpg")).SetTextureOffset(Math.PI),
                new TexturedSphere(-2, -2, -8, 1, new Bitmap("mars1k.jpg")),
                new ReflectiveSphere(-1, 2, -10, 1),
                new ReflectiveSphere(3, -3, -7, 1),
                new ReflectiveSphere(3, 5, 5, 1),
                new ReflectiveSphere(-3.7, 2, -7, 1),

                //new TexturedSphere(24, 0, 2, 1, new Bitmap("earthmap1k.jpg")),
                //new TexturedSphere(16, 0, 4, 1, new Bitmap("gstar.jpg")),
                //new TexturedSphere(-10, -10, -10, 1, new Bitmap("gstar.jpg")),
                //new CheckeredSphere(-10, -10, -10, 1, Color.RoyalBlue, Color.DarkBlue)
            };

            int numRandomSpheres = 0;
            var starTexture = new Bitmap("sun2k.jpg");
            var starBitmap = Util.getNativeTextureBitmap(starTexture);
            var random = new Random();
            double tempR = 0, tempTheta = 0, tempPhi = 0;
            double tempX = 0, tempY = 0, tempZ = 0;
            for (int i = 0; i < numRandomSpheres; i++)
            {
                tempR = 6.5 + random.NextDouble() * 6.0;
                tempTheta = random.NextDouble() * Math.PI * 2;
                Util.ToCartesian(tempR, tempTheta, 0, ref tempX, ref tempY, ref tempZ);
                hitables.Add(new TexturedSphere(tempX, tempY, tempZ, 0.05f + (float)random.NextDouble() * 0.2f, starBitmap, starTexture.Width, starTexture.Height)
                    .SetTextureOffset(random.NextDouble() * Math.PI * 2));
            }




            var scene = new Scene(cameraPos, lookAt, up, fov, hitables, curvatureCoeff, angularMomentum);

            //new KerrRayProcessor(400, 200, scene, fileName).Process();
            new SchwarzschildRayProcessor(1920, 1080, scene, fileName).Process();



            /*
            int numFrames = 16;
            double angleIncrement = (Math.PI * 2) / numFrames;
            double curvatureMultiplier = 0;
            var rotationMatrix = Matrix4x4.CreateRotationY((float)angleIncrement);
            tempR = 20; tempTheta = 0; tempPhi = 0;

            Directory.CreateDirectory("anim");

            for (int i = 0; i < numFrames; i++)
            {
                fileName = Path.Combine("anim", "frame" + i + ".png");



                tempTheta += angleIncrement;
                tempPhi = Math.Sin(tempTheta) * (Math.PI / 6);

                var rotation = Matrix4x4.CreateRotationY((float)tempTheta);
                var tempCamPos = cameraPos;
                tempCamPos = Vector3.Transform(tempCamPos, rotation);
                rotation = Matrix4x4.CreateRotationX((float)tempPhi);
                tempCamPos = Vector3.Transform(tempCamPos, rotation);
                
                var scene = new Scene(tempCamPos, lookAt, up, fov, hitables, curvatureCoeff, angularMomentum);

                //new KerrRayProcessor(400, 200, scene, fileName).Process();
                new SchwarzschildRayProcessor(1280, 720, scene, fileName).Process();


                //curvatureMultiplier += angleIncrement;
            }
            */


            /*
            int numFrames = 16;
            double angleIncrement = (Math.PI * 2) / numFrames;
            double curvatureMultiplier = 0;
            var rotationMatrix = Matrix4x4.CreateRotationY((float)angleIncrement);

            Directory.CreateDirectory("anim");

            for (int i = 0; i < numFrames; i++)
            {
                fileName = Path.Combine("anim", "frame" + i + ".png");
                
                var scene = new Scene(cameraPos, lookAt, up, fov, hitables, curvatureCoeff * (float)(1.0 - Math.Cos(curvatureMultiplier) * Math.Cos(curvatureMultiplier)), angularMomentum);

                //new KerrRayProcessor(400, 200, scene, fileName).Process();
                new SchwarzschildRayProcessor(1200, 720, scene, fileName).Process();
                
                cameraPos = Vector3.Transform(cameraPos, rotationMatrix);
                //curvatureMultiplier += angleIncrement;
            }
            */

        }
    }
}
