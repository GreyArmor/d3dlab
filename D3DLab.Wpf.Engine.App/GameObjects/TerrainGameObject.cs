﻿using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.Rendering.Strategies;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Common;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Components.Movements;
using D3DLab.Std.Engine.Core.Ext;
using D3DLab.Std.Engine.Core.Utilities;
using D3DLab.Wpf.Engine.App.D3D.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

namespace D3DLab.Wpf.Engine.App {

    public class SkyGameObject : SingleGameObject {
        /*
        * Sky
        * http://vterrain.org/Atmosphere/index.html
        * 
        * https://gamedev.ru/code/forum/?id=19689
        * http://www.philohome.com/skycollec/skycollec.htm
        * http://steps3d.narod.ru/tutorials/sky-tutorial.html
        */

        public static SkyGameObject Create(IEntityManager manager) {
            var resources = Path.Combine("../../../../D3DLab.Wpf.Engine.App/Resources/sky/");
            var tag = new ElementTag("SkyDome");
            var plane = new ElementTag("SkyPlane");

            //var geo = GenerateSphere(2, Vector3.Zero, 15f.ToRad());
            var file = new FileInfo(Path.Combine(resources, "sky.obj"));
            var points = new List<Vector3>();
            var indx = new List<int>();

            var index = 0;
            foreach (var line in File.ReadAllLines(file.FullName)) {
                var parts = line.Split(' ');
                points.Add(new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));

                indx.Add(index);
                index++;
            }


            var geo = new HittableGeometryComponent();
            geo.Positions = points.ToImmutableArray();
            geo.Indices = indx.ToImmutableArray();
            geo.Color = V4Colors.White;
            //geo.TextureCoordinates = geo.Positions.Select(x => new Vector2()).ToImmutableArray();

            manager.CreateEntity(tag)
                .AddComponents(new IGraphicComponent[] {
                    new D3DSkyRenderComponent() { },
                    geo,
                    new TransformComponent(),
                    new GradientMaterialComponent(),
                    new FollowUpCameraPositionComponent()
                });

            var planemesh = GeometryBuilder.BuildSkyPlane(SkyPlaneData.Default);
            manager.CreateEntity(plane)
                .AddComponents(new IGraphicComponent[] {
                    new D3DSkyPlaneRenderComponent(),
                    new D3DTexturedMaterialSamplerComponent(
                        new System.IO.FileInfo(Path.Combine(resources,"cloud001.bmp")),
                        new System.IO.FileInfo(Path.Combine(resources,"cloud002.bmp"))
                    ),
                    new SimpleGeometryComponent {
                        Positions = planemesh.Positions.ToImmutableArray(),
                        Indices = planemesh.Indices.ToImmutableArray(),
                        TextureCoordinates = planemesh.TextureCoordinates.ToImmutableArray(),
                    },
                    new TransformComponent(),
                    new FollowUpCameraPositionComponent(),
                    new SkyPlaneParallaxAnimationComponent()
                });

            return new SkyGameObject(tag);
        }

        static SimpleGeometryComponent GenerateSphere(float radius, Vector3 center, float stepDegree) {
            //{Minimum:X:-2 Y:-2 Z:-2 Maximum:X:2 Y:2 Z:2}
            var com = new SimpleGeometryComponent();

            for (var angle = stepDegree; angle < 360f; angle += stepDegree) {

            }

            return com;
        }

        public SkyGameObject(ElementTag tag) : base(tag, typeof(SkyGameObject).Name) {
        }
    }


    public class TerrainGameObject : SingleGameObject {
        /*
         * http://libnoise.sourceforge.net/ A portable, open-source, coherent noise-generating library for C++ 
         * https://github.com/JacekPrzemieniecki/VoxelTerrain
         * 
         * 
         * http://blog.wolfire.com/2010/12/Overgrowth-graphics-overview
         * http://archive.gamedev.net/archive/reference/programming/features/randomriver/page2.html
         * 
         * https://developer.nvidia.com/sites/default/files/akamai/gamedev/files/sdk/11/TerrainTessellation_WhitePaper.pdf
         * https://www.reddit.com/r/gamedev/comments/6i03ix/realistic_procedural_height_maps/
         * 
         * https://github.com/srajangarg/mesh-simplify
         * 
         * http://vterrain.org/LOD/Implementations/
         * 
         * https://apps.dtic.mil/dtic/tr/fulltext/u2/a439499.pdf
         */

        public TerrainGameObject(ElementTag tag) : base(tag, typeof(TerrainGameObject).Name) {
        }

        public static TerrainGameObject Create(IEntityManager manager) {
            var tag = new ElementTag("Terrain");

            var resources = Path.Combine("../../../../D3DLab.Wpf.Engine.App/Resources/terrain/");
            var grass = Path.Combine(resources, "1.jpg");
            var slope = Path.Combine(resources, "slope.bmp");
            var rock = Path.Combine(resources, "rock01d.bmp");
            var seafloor = Path.Combine(resources, "seafloor.bmp");
            var sand = Path.Combine(resources, "dirt04.bmp");
            var shore = Path.Combine(resources, "shore.jpg");
            var dirt = Path.Combine(resources, "dirt.bmp");
            var snow = Path.Combine(resources, "snow01n.bmp");


            var heigtmap = Path.Combine(resources, "test.bmp");

            manager.CreateEntity(tag)
                .AddComponents(new IGraphicComponent[] {
                    new D3DTerrainRenderComponent() {
                        CanRender = false
                    },
                    new Systems.TerrainConfigurationComponent {
                        Width = 256,
                        Height = 256, 
                    },
                    new D3DTexturedMaterialSamplerComponent(
                        new System.IO.FileInfo(seafloor),
                        new System.IO.FileInfo(shore),
                        new System.IO.FileInfo(sand),
                        new System.IO.FileInfo(grass),
                        new System.IO.FileInfo(dirt),
                        new System.IO.FileInfo(rock),
                        new System.IO.FileInfo(slope),

                        new System.IO.FileInfo(Path.Combine(resources, "rock01n.bmp")),
                        new System.IO.FileInfo(snow),
                        new System.IO.FileInfo(Path.Combine(resources,"distance01n.bmp"))
                        ),
                    new TransformComponent(),
                });

            return new TerrainGameObject(tag);
        }

        
        static Vector4[] LoadColourMap(string fullPath, int width, int height) {
            Bitmap colourBitMap = null;
            try {
                colourBitMap = new Bitmap(fullPath);
                // This check is optional.
                // Make sure the color map dimensions are the same as the terrain dimensions for easy 1 to 1 mapping.
                var btmW = colourBitMap.Width;
                var btmH = colourBitMap.Height;

                var colors = new Vector4[btmH * btmW];

                if ((btmW != width) || (btmH != height)) {
                    throw new Exception("Textures are not compatible.");
                }

                // Read the image data into the color map portion of the height map structure.
                int index;
                for (int j = 0; j < btmH; j++)
                    for (int i = 0; i < btmW; i++) {
                        index = (btmH * j) + i;
                        colors[index].X = colourBitMap.GetPixel(i, j).R / 255.0f; // 117.75f; //// 0.678431392
                        colors[index].Y = colourBitMap.GetPixel(i, j).G / 255.0f;  //117.75f; // 0.619607866
                        colors[index].Z = colourBitMap.GetPixel(i, j).B / 255.0f;  // 117.75f; // 0.549019635
                    }

                return colors;
            } finally {
                colourBitMap?.Dispose();
            }
        }
    }
}
