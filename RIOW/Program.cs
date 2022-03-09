using RIOW.HitObjects;
using RIOW.Materials;
using RIOW.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RIOW
{
    internal class Program
    {
        private const string FilePath = "image.bmp";

        private const int ImageWidth = 800;
        private const int ImageHeight = 800;
        private const float AspectRatio = ImageWidth / (float)ImageHeight;

        private const int ThreadCount = 12;

        private static int SamplesPerPixel = 100;
        private const int MaxDepth = 50;

        private static List<int> remainingLines;
        private static int previousCount;

        private static Vector3 RayColor(Ray originalRay, Ray ray, Vector3 backgroundColor, Vector3 ambientColor, HitObject world, int depth, Utils utils)
        {
            if (depth <= 0)
                return Vector3.Zero;

            HitRecord record = new HitRecord();
            if (!world.Hit(ray, 0.001f, Utils.Infinity, ref record, utils))
            {
                if (originalRay.Equals(ray))
                {
                return backgroundColor;
                }
                    return Vector3.Max(backgroundColor, ambientColor);
                Vector3 unitDirection = Vector3.Normalize(ray.Direction);
                var t = 0.5f * (unitDirection.Y + 1f);
                return (1f - t) * new Vector3(1, 1, 1) + t * new Vector3(0.5f, 0.7f, 1.0f);
            }

            Ray scattered;
            Vector3 attenuation;
            Vector3 emitted = record.material.Emitted(record.u, record.v, record.p);

            if (!record.material.Scatter(ray, record, out attenuation, out scattered, utils))
            {
                return emitted;
            }

            return emitted + attenuation * RayColor(originalRay, scattered, backgroundColor, ambientColor, world, depth - 1, utils);
        }

        private static void PrintLinesRemaining()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write($"Lines remaining: ");
            Console.CursorLeft = 17;
            Console.CursorTop = 1;
            for (int i = 0; i < previousCount.ToString().Length; i++)
            {
                Console.Write(" ");
            }
            Console.CursorLeft = 17;
            Console.Write($"{remainingLines.Count}");
            previousCount = remainingLines.Count;
            while (remainingLines.Count > 0)
            {
                if (remainingLines.Count != previousCount)
                {
                    Console.CursorLeft = 17;
                    Console.CursorTop = 1;
                    for (int i = 0; i < previousCount.ToString().Length; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.CursorLeft = 17;
                    Console.Write($"{remainingLines.Count}");
                    previousCount = remainingLines.Count;
                }
            }
        }

        private static void Process(byte[] buffer, int depth, HitObject world, Camera camera, Vector3 backgroundColor, Vector3 ambientColor)
        {
            Utils utils = new Utils();

            while (remainingLines.Count > 0)
            {
                int line = 0;
                lock (remainingLines)
                {
                    if (remainingLines.Count <= 0)
                        break;
                    line = remainingLines[0];
                    remainingLines.RemoveAt(0);
                }

                for (int i = 0; i < ImageWidth; i++)
                {
                    Vector3 pixelColor = Vector3.Zero;
                    for (int s = 0; s < SamplesPerPixel; s++)
                    {
                        float u = (float)(i + utils.random.NextDouble()) / (ImageWidth - 1);
                        float v = (float)(line + utils.random.NextDouble()) / (ImageHeight - 1);

                        Ray ray = camera.GetRay(u, v, utils);
                        pixelColor += RayColor(ray, ray, backgroundColor, ambientColor, world, MaxDepth, utils);
                    }

                    float scale = 1f / SamplesPerPixel;
                    pixelColor.X = MathF.Sqrt(pixelColor.X * scale);
                    pixelColor.Y = MathF.Sqrt(pixelColor.Y * scale);
                    pixelColor.Z = MathF.Sqrt(pixelColor.Z * scale);

                    Color color = pixelColor.ToColor();

                    int offset = (((ImageHeight - 1 - line) * ImageWidth) + i) * depth;

                    buffer[offset + 0] = color.R;
                    buffer[offset + 1] = color.G;
                    buffer[offset + 2] = color.B;
                    buffer[offset + 3] = color.A;
                }
            }
        }

        static HitObjectList RandomScene()
        {
            Utils utils = new Utils();

            HitObjectList world = new HitObjectList();

            world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(new CheckerTexture(Vector3.Zero, Vector3.One))));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    float chooseMat = utils.RandomFloat();
                    Vector3 center = new Vector3(a + 0.9f * utils.RandomFloat(), 0.2f, b + 0.9f * utils.RandomFloat());

                    if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9f)
                    {
                        Material material;

                        if (chooseMat < 0.8f)
                        {
                            Vector3 color = utils.RandomVector3() * utils.RandomVector3();
                            material = new Lambertian(color);
                            Vector3 center2 = center + new Vector3(0, utils.RandomFloat(0f, 0.5f), 0);
                            world.Add(new MovingSphere(center, center2, 0, 1, 0.2f, material));
                        }
                        else if (chooseMat < 0.95)
                        {
                            Vector3 color = utils.RandomVector3(0.5f, 1);
                            float fuzz = utils.RandomFloat(0, 0.5f);
                            material = new Metal(color, fuzz);
                            world.Add(new Sphere(center, 0.2f, material));
                        }
                        else
                        {
                            material = new Dielectric(1.5f);
                            world.Add(new Sphere(center, 0.2f, material));
                        }
                    }
                }
            }

            world.Add(new Sphere(new Vector3(0, 1, 0), 1, new Dielectric(1.5f)));

            world.Add(new Sphere(new Vector3(-4, 1, 0), 1, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));

            world.Add(new Sphere(new Vector3(4, 1, 0), 1, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)));

            return world;
        }

        static HitObjectList TwoSpheres()
        {
            HitObjectList world = new HitObjectList();

            Texture texture = new CheckerTexture(new Vector3(0.2f, 0.3f, 0.1f), new Vector3(0.9f, 0.9f, 0.9f));

            world.Add(new Sphere(new Vector3(0, -10, 0), 10, new Lambertian(texture)));
            world.Add(new Sphere(new Vector3(0, 10, 0), 10, new Lambertian(texture)));

            return world;
        }

        static HitObjectList TwoPerlinSpheres()
        {
            HitObjectList world = new HitObjectList();

            var perText = new NoiseTexture(4);

            world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(perText)));
            world.Add(new Sphere(new Vector3(0, 2, 0), 2, new Lambertian(perText)));

            return world;
        }

        static HitObjectList Earth()
        {
            HitObjectList world = new HitObjectList();

            world.Add(new Sphere(Vector3.Zero, 2, new Lambertian(new ImageTexture("earthmap.jpg"))));

            var light = new DiffuseLight(new Vector3(10));

            world.Add(new Sphere(new Vector3(20, 0, -10), 5, light));

            return world;
        }

        static HitObjectList SimpleLight()
        {
            HitObjectList world = new HitObjectList();

            var perText = new NoiseTexture(4);

            world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(perText)));
            world.Add(new Sphere(new Vector3(0, 2, 0), 2, new Lambertian(perText)));

            var diffLight = new DiffuseLight(new Vector3(4, 4, 4));
            world.Add(new XYRect(3, 5, 1, 3, -2, diffLight));
            //world.Add(new Sphere(new Vector3(0, 8, 0), 2, diffLight));

            return world;
        }

        static HitObjectList CornellBox()
        {
            HitObjectList world = new HitObjectList();

            var red = new Lambertian(new Vector3(0.65f, 0.05f, 0.05f));
            var white = new Lambertian(new Vector3(0.73f, 0.73f, 0.73f));
            var green = new Lambertian(new Vector3(0.12f, 0.45f, 0.15f));
            var light = new DiffuseLight(new Vector3(30));

            world.Add(new YZRect(0, 555, 0, 555, 555, green));
            world.Add(new YZRect(0, 555, 0, 555, 0, red));
            world.Add(new XZRect(213, 343, 227, 332, 554, light));
            world.Add(new XZRect(0, 555, 0, 555, 0, white));
            world.Add(new XZRect(0, 555, 0, 555, 555, white));
            world.Add(new XYRect(0, 555, 0, 555, 555, white));

            HitObject box1 = new Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
            box1 = new RotateY(box1, 15);
            box1 = new Translate(box1, new Vector3(265, 0, 295));

            HitObject box2 = new Box(new Vector3(0, 0, 0), new Vector3(165, 165, 165), white);
            box2 = new RotateY(box2, -18);
            box2 = new Translate(box2, new Vector3(130, 0, 65));

            world.Add(new ConstantMedium(box1, 0.01f, new Vector3()));
            world.Add(new ConstantMedium(box2, 0.01f, new Vector3(1, 1, 1)));

            return world;
        }

        static HitObjectList FinalScene()
        {
            Utils utils = new Utils();


            HitObjectList boxes1 = new HitObjectList();

            var ground = new Lambertian(new Vector3(0.48f, 0.83f, 0.53f));

            int boxesPerSide = 20;
            for (int i = 0; i < boxesPerSide; i++)
            {
                for (int j = 0; j < boxesPerSide; j++)
                {
                    float w = 100f;
                    float x0 = -1000f + i * w;
                    float z0 = -1000f + j * w;
                    float y0 = 0f;
                    float x1 = x0 + w;
                    float y1 = utils.RandomFloat(1, 101);
                    float z1 = z0 + w;

                    boxes1.Add(new Box(new Vector3(x0, y0, z0), new Vector3(x1, y1, z1), ground));
                }
            }
            HitObjectList world = new HitObjectList();

            world.Add(new BVHNode(boxes1, 0, 1));

            var light = new DiffuseLight(new Vector3(7, 7, 7));
            world.Add(new XZRect(123, 423, 147, 412, 554, light));

            var center1 = new Vector3(400, 400, 200);
            var center2 = center1 + new Vector3(30, 0, 0);
            var movingSphereMaterial = new Lambertian(new Vector3(0.7f, 0.3f, 0.1f));
            world.Add(new MovingSphere(center1, center2, 0, 1, 50, movingSphereMaterial));

            world.Add(new Sphere(new Vector3(260, 150, 45), 50, new Dielectric(1.5f)));
            world.Add(new Sphere(new Vector3(0, 150, 145), 50, new Metal(new Vector3(0.8f, 0.8f, 0.9f), 1)));

            var boundary = new Sphere(new Vector3(360, 150, 145), 70, new Dielectric(1.5f));
            world.Add(boundary);
            world.Add(new ConstantMedium(boundary, 0.2f, new Vector3(0.2f, 0.4f, 0.9f)));
            boundary = new Sphere(new Vector3(0, 0, 0), 5000, new Dielectric(1.5f));
            world.Add(new ConstantMedium(boundary, 0.0001f, Vector3.One));

            var emat = new Lambertian(new ImageTexture("earthmap.jpg"));
            world.Add(new Sphere(new Vector3(400, 200, 400), 100, emat));
            var perText = new NoiseTexture(0.1f);
            world.Add(new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(perText)));

            HitObjectList boxes2 = new HitObjectList();
            var white = new Lambertian(new Vector3(.73f));
            int ns = 1000;
            for (int i = 0; i < ns; i++)
            {
                boxes2.Add(new Sphere(utils.RandomVector3(0, 165), 10, white));
            }

            world.Add(new Translate(new RotateY(new BVHNode(boxes2, 0, 1), 15), new Vector3(-100, 270, 395)));

            return world;
        }

        private static void Main(string[] args)
        {
            remainingLines = new List<int>();
            for (int i = 0; i < ImageHeight; i++)
            {
                remainingLines.Add(i);
            }

            Vector3 lookfrom = Vector3.Zero;
            Vector3 lookat = Vector3.Zero;
            float vfov = 40;
            float aperture = 0f;

            HitObjectList world = new HitObjectList();

            Vector3 backgroundColor = Vector3.Zero;
            Vector3 ambientColor = Vector3.Zero;

            switch (4)
            {
                case 1:
                    world = RandomScene();
                    backgroundColor = new Vector3(.7f, .8f, 1f);
                    ambientColor = new Vector3(.7f, .8f, 1f);
                    lookfrom = new Vector3(13, 2, 3);
                    lookat = new Vector3(0, 0, 0);
                    vfov = 20;
                    aperture = 0.1f;
                    break;
                case 2:
                    world = TwoSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1f);
                    ambientColor = new Vector3(.7f, .8f, 1f);
                    lookfrom = new Vector3(13, 2, 3);
                    lookat = Vector3.Zero;
                    vfov = 20;
                    break;
                case 3:
                    world = TwoPerlinSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1f);
                    ambientColor = new Vector3(.7f, .8f, 1f);
                    lookfrom = new Vector3(13, 2, 3);
                    lookat = Vector3.Zero;
                    vfov = 20;
                    break;
                case 4:
                    world = Earth();
                    //backgroundColor = new Vector3(.7f, .8f, 1f);
                    SamplesPerPixel = 400;
                    backgroundColor = new Vector3(0f);
                    ambientColor= new Vector3(0.002f);
                    lookfrom = new Vector3(13, 2, 3);
                    lookat = Vector3.Zero;
                    vfov = 20;
                    break;
                case 5:
                    world = SimpleLight();
                    backgroundColor = new Vector3(0, 0, 0);
                    ambientColor = new Vector3(0, 0, 0);
                    SamplesPerPixel = 400;
                    lookfrom = new Vector3(26, 3, 6);
                    lookat = new Vector3(0, 2, 0);
                    vfov = 20f;
                    break;
                case 6:
                    world = CornellBox();
                    SamplesPerPixel = 400;
                    backgroundColor = new Vector3(0, 0, 0);
                    ambientColor = new Vector3(0, 0, 0);
                    //backgroundColor = new Vector3(.7f, .8f, 1f);
                    lookfrom = new Vector3(278, 278, -800);
                    lookat = new Vector3(278, 278, 0);
                    vfov = 40f;
                    break;
                default:
                case 7:
                    world = FinalScene();
                    SamplesPerPixel = 10000;
                    backgroundColor = new Vector3(0, 0, 0);
                    ambientColor = new Vector3(0, 0, 0);
                    //backgroundColor = new Vector3(.7f, .8f, 1f);
                    lookfrom = new Vector3(478, 278, -600);
                    lookat = new Vector3(278, 278, 0);
                    vfov = 40;
                    break;
            }

            Vector3 vup = Vector3.UnitY;
            float distToFocus = 10;
            Camera cam = new Camera(lookfrom, lookat, vup, vfov, AspectRatio, aperture, distToFocus, 0, 1);

            Bitmap bitmap = new Bitmap(ImageWidth, ImageHeight);

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;

            var buffer = new byte[data.Width * data.Height * depth];

            Action[] actions = new Action[ThreadCount + 1];

            for (int i = 0; i < ThreadCount; i++)
            {
                actions[i] = () => Process(buffer, depth, world, cam, backgroundColor, ambientColor);
            }

            actions[ThreadCount] = () => PrintLinesRemaining();


            DateTime started = DateTime.Now;

            Console.WriteLine("Started!");

            Parallel.Invoke(actions);

            DateTime finished = DateTime.Now;

            TimeSpan timeTaken = finished - started;

            Console.CursorLeft = 0;
            Console.CursorTop = 2;
            Console.WriteLine($"Took {timeTaken.TotalSeconds}");

            for (int i = 0; i < ImageWidth; i++)
            {
                for (int j = 0; j < ImageHeight; j++)
                {
                    int offset = ((j * ImageWidth) + i) * depth;

                    byte b = buffer[offset];

                    buffer[offset] = buffer[offset + 2];
                    buffer[offset + 2] = b;
                }
            }

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            bitmap.UnlockBits(data);

            bitmap.Save(FilePath);

            Process process = new Process();
            process.StartInfo.FileName = "explorer";
            process.StartInfo.Arguments = $"\"{FilePath}\"";
            process.Start();
        }
    }
}
