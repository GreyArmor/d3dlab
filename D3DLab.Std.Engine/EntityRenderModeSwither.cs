using D3DLab.Std.Engine;
using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Components;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Std.Engine.Render;
using D3DLab.Std.Engine.Shaders;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine {
    public class Particles {

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Particle {
            public Vector3 Position;
            public Vector3 Velocity;
            public const int SizeInBytes = 4 * (3 + 3);
        }

        public class ParticleShaderSpecification : ShaderSpecification<Particle> {
            public ParticleShaderSpecification(ShaderTechniquePass[] passes) : base(passes) { }

            public override VertexLayoutDescription[] GetVertexDescription() {
                return new[] {
                    new VertexLayoutDescription(
                            new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                            new VertexElementDescription("Velocity", VertexElementSemantic.Position, VertexElementFormat.Float3))
                };
            }
            public override ResourceLayoutDescription GetResourceDescription() {
                return new ResourceLayoutDescription(
                       new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("Particles", ResourceKind.StructuredBufferReadWrite, 
                        ShaderStages.Vertex | ShaderStages.Fragment));
            }
            public override uint GetVertexSizeInBytes() {
                return Particle.SizeInBytes;
            }

            protected override Particle[] ConvertVertexToShaderStructure(Geometry3D geo) {
                var res = new List<Particle>();

                for (int i = 0; i < geo.Positions.Count; i++) {
                    var pos = geo.Positions[i];
                    res.Add(new Particle() {
                        Position = new Vector3(pos.X, pos.Y, pos.Z)
                    });
                }

                return res.ToArray();
            }
        }
        public class ParticleDeviceBufferesUpdater : DeviceBufferesUpdater {
            public DeviceBuffer StructuredBuffer;
            public const int PARTICLES_COUNT = 1000000;
            Particle[] particles;

            public ParticleDeviceBufferesUpdater(ParticleShaderSpecification shader) : base(shader) {
                particles = new Particle[PARTICLES_COUNT];
                var random = new System.Random();
                for (int i = 0; i < PARTICLES_COUNT; i++) {
                    particles[i].Position = random.NextVector3(new Vector3(-30f, -30f, -30f), new Vector3(30f, 30f, 30f));
                }
            }
            public override void UpdateVertex(Geometry3D geo) {
                //var vertices = shader.ConvertVertexToShaderStructure<Particle>(geo);
                //factory.CreateIfNullBuffer(ref vertexBuffer, new BufferDescription((uint)(shader.GetVertexSizeInBytes() * vertices.Length),
                //   BufferUsage.VertexBuffer));
                //cmd.UpdateBuffer(vertexBuffer, 0, vertices);

                //factory.CreateIfNullBuffer(ref indexBuffer, new BufferDescription(sizeof(ushort) * (uint)PARTICLES_COUNT,
                // BufferUsage.IndexBuffer));
                //cmd.UpdateBuffer(indexBuffer, 0, PARTICLES_COUNT);

                factory.CreateIfNullBuffer(ref StructuredBuffer, 
                    new BufferDescription((uint)(Particle.SizeInBytes * particles.Length), 
                    BufferUsage.StructuredBufferReadWrite, Particle.SizeInBytes));
                cmd.UpdateBuffer(StructuredBuffer, 0, particles);
            }
        }

        #region shader

        const string ShaderStructures =
@"
struct Particle
{
    float3 Position;
    float3 Velocity;
};
StructuredBuffer<Particle> Particles : register(t0);
struct VertexInput
{
    uint VertexID : SV_VertexID;
};
struct PixelInput
{
	float4 Position : SV_POSITION;
	float2 UV : TEXCOORD0;
	float3 PositionTWS : TEXCOORD1;
};
";

        const string VertexShaderText =
@"
cbuffer ProjectionBuffer : register(b0)
{
	float4x4 Projection;
}
cbuffer ViewBuffer : register(b1)
{
	float4x4 View;
}
PixelInput TriangleVS(VertexInput input)
{

	PixelInput output = (PixelInput)0;

	Particle particle = Particles[input.VertexID];

	float4 worldPosition = float4(particle.Position, 1);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	return output;
}
";
        const string GeometryShaderText =
@"
PixelInput _offsetNprojected(PixelInput data, float2 offset, float2 uv)
{
	data.Position.xy += offset;
	data.Position = mul(data.Position, Projection);
	data.UV = uv;

	return data;
}

[maxvertexcount(4)]
void TriangleGS( point PixelInput input[1], inout TriangleStream<PixelInput> stream )
{
	PixelInput pointOut = input[0];
	
	float size = 0.1f;

	stream.Append( _offsetNprojected(pointOut, float2(-1,-1) * size, float2(0, 0)) );
	stream.Append( _offsetNprojected(pointOut, float2(-1, 1) * size, float2(0, 1)) );
	stream.Append( _offsetNprojected(pointOut, float2( 1,-1) * size, float2(1, 0)) );
	stream.Append( _offsetNprojected(pointOut, float2( 1, 1) * size, float2(1, 1)) );

	stream.RestartStrip();
}
";
        const string PixelShaderText =
@"
struct PixelOutput
{
    float4 Color : SV_TARGET0;
};
PixelOutput TrianglePS(PixelInput input)
{
    PixelOutput output = (PixelOutput)0;
	output.Color = float4((float3)0.1, 1);
	return output;
}
";




        #endregion

        public class ParticleRenderableComponent : ShaderComponent, IRenderableComponent {
            ParticleDeviceBufferesUpdater deviceBufferes;
            public ParticleRenderableComponent(IVeldridShaderSpecification shader, ParticleDeviceBufferesUpdater deviceBufferes) : base(shader, deviceBufferes) {
                this.deviceBufferes = deviceBufferes;
            }

            public void Update(VeldridRenderState state) {
                var cmd = state.Commands;
                var factory = state.Factory;
                var viewport = state.Viewport;

                Bufferes.Update(factory, cmd);
                Resources.Update(factory, cmd);

                Shader.UpdateShaders(factory);

                Bufferes.UpdateWorld();
                Bufferes.UpdateVertex(null);
//                Bufferes.UpdateIndex();

                Resources.UpdateResourceLayout();
                Resources.UpdateResourceSet(new ResourceSetDescription(
                           Resources.Layout,
                           viewport.ProjectionBuffer,
                           viewport.ViewBuffer,
                           deviceBufferes.StructuredBuffer));
            }

            public void Render(VeldridRenderState state) {
                var cmd = state.Commands;
                var factory = state.Factory;
                var gd = state.GrDevice;
                //Additive Blend State
                var _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                        BlendStateDescription.SingleAdditiveBlend,
                        DepthStencilStateDescription.Disabled,
                        RasterizerStateDescription.CullNone,

                        //BlendStateDescription.SingleOverrideBlend,
                        //DepthStencilStateDescription.Disabled,
                        //RasterizerStateDescription.CullNone,

                        PrimitiveTopology.PointList,
                        Shader.passes.First().Description,
                        new[] { Resources.Layout },
                        gd.SwapchainFramebuffer.OutputDescription));
                
                cmd.SetPipeline(_pipeline);
                cmd.SetGraphicsResourceSet(0, Resources.Set);
                //  cmd.SetVertexBuffer(0, Bufferes.Vertex);
                //   cmd.SetIndexBuffer(Bufferes.Index, IndexFormat.UInt16);
                cmd.Draw(ParticleDeviceBufferesUpdater.PARTICLES_COUNT);
                //cmd.DrawIndexed((uint)ParticleDeviceBufferesUpdater.PARTICLES_COUNT, 1, 0, 0, 0);
            }
        }

        public static ParticleRenderableComponent GetParticlesCom() {
            var name = "Particle";

            var compiler = new D3DShaderCompilator(null);

            var vertex = new ShaderInMemoryInfo(name + "Vertex", ShaderStructures + VertexShaderText,
                compiler.Compile(ShaderStructures + VertexShaderText,
                    "TriangleVS", ShaderStages.Vertex.ToString()),
                ShaderStages.Vertex.ToString(), "TriangleVS");

            //var geometry = new ShaderInMemoryInfo(name + "Geometry", ShaderStructures + GeometryShaderText,
            //    compiler.Compile(ShaderStructures + GeometryShaderText,
            //        "TriangleGS", ShaderStages.Geometry.ToString()),
            //    ShaderStages.Geometry.ToString(), "TriangleGS");

            var pixel = new ShaderInMemoryInfo(name + "Pixel", ShaderStructures + PixelShaderText,
               compiler.Compile(ShaderStructures + PixelShaderText,
                   "TrianglePS", ShaderStages.Fragment.ToString()),
               ShaderStages.Fragment.ToString(), "TrianglePS");

            var pass = new ShaderTechniquePass(new IShaderInfo[] { vertex, /*geometry,*/ pixel });

            var shader = new ParticleShaderSpecification(new[] { pass });

            return new ParticleRenderableComponent(shader, new ParticleDeviceBufferesUpdater(shader));
        }
    }

    public class EntityRenderModeSwither {

        #region shader

        const string VertexShaderText =
@"
struct VSInputLS
{
	float4 pos	: POSITION0;
	float4 c	: COLOR0;

	//float4 mr0	: TEXCOORD1;
	//float4 mr1	: TEXCOORD2;
	//float4 mr2	: TEXCOORD3;
	//float4 mr3	: TEXCOORD4;
};

struct GSInputLS
{
	float4 pos	: SV_Position;
	//float4 wp   : POSITION0;
	float4 c	: COLOR0;
};

cbuffer ProjectionBuffer : register(b0)
{
	float4x4 Projection;
}
cbuffer ViewBuffer : register(b1)
{
	float4x4 View;
}
cbuffer WorldBuffer : register(b2)
{
	float4x4 World;
}

GSInputLS VShaderBoundingBox(VSInputLS input)
{
	GSInputLS output;// = (GSInputLS)0;
	
	float4 inputp = input.pos;

	output.pos = mul(World, inputp);
	//output.wp = output.p;
	output.pos = mul(View, output.pos);
	output.pos = mul(Projection, output.pos);
	
	output.c = input.c;

	return output;
}";
        const string PixelShaderText =
@"
struct PSInputLS
{
	float4 pos	: SV_Position;
	//float4 wp   : POSITION0;
	//noperspective
	//	float3 t	: TEXCOORD;
	float4 c	: COLOR;
};

float4 vLineParams = float4(4, 1, 0, 0);

float4 PShaderBoundingBox(PSInputLS input) : SV_Target
{
	return input.c;
}";
        const string GeometryShaderText =
@"
struct GSInputLS
{
	float4 pos	: POSITION;
	//float4 wp   : POSITION1;
	float4 c	: COLOR;
};
struct PSInputLS
{
	float4 pos	: SV_POSITION;
//	float4 wp   : POSITION1;
//	noperspective
//		float3 t	: TEXCOORD;
	float4 c	: COLOR;
};

[maxvertexcount(1)]
void GShaderBoundingBox(point GSInputLS points[1], inout PointStream<PSInputLS> output)
{
PSInputLS v;
v.pos = points[0].pos;
v.c = points[0].c;

output.Append(v);

//float3 dd = points[0].p + float3(0, 0, 1) * 300;
//v.p = float4(dd,1);
//v.c = points[0].c;
//output.Append(v);
output.RestartStrip();

/*
float4 p0 = points[0].p;
float4 p1 = points[1].p;

float w0 = p0.w;
float w1 = p1.w;

p0.xyz = p0.xyz / p0.w;
p1.xyz = p1.xyz / p1.w;

float3 line01 = p1 - p0;
float3 dir = normalize(line01);
// scale to correct window aspect ratio
float3 ratio = float3(960, 540,0);// float3(RenderTargetSize.y, RenderTargetSize.x, 0);
ratio = normalize(ratio);

float3 unit_z = normalize(float3(0, 0, -1));
float3 normal = normalize(cross(unit_z, dir) * ratio);

float width = 10;

PSInputLS v[4];

float3 dir_offset = dir * ratio * width;
float3 normal_scaled = normal * ratio * width;

float3 p0_ex = p0 - dir_offset;
float3 p1_ex = p1 + dir_offset;

v[0].p = float4(p0_ex - normal_scaled, 1) * w0;
//v[0].t = float2(0,0);
v[0].c = points[0].c;

v[1].p = float4(p0_ex + normal_scaled, 1) * w0;
//v[1].t = float2(0,0);
v[1].c = points[0].c;

v[2].p = float4(p1_ex + normal_scaled, 1) * w1;
//v[2].t = float2(0,0);
v[2].c = points[0].c;

v[3].p = float4(p1_ex - normal_scaled, 1) * w1;
//v[3].t = float2(0,0);
v[3].c = points[0].c;

output.Append(v[2]);
output.Append(v[1]);
output.Append(v[0]);

output.RestartStrip();

output.Append(v[3]);
output.Append(v[2]);
output.Append(v[0]);

output.RestartStrip();
*/
}";
        #endregion

        #region box com 

        public class LineShaderSpecification : ShaderSpecification<LineShaderSpecification.LinesVertex> {
            public LineShaderSpecification(ShaderTechniquePass[] passes) : base(passes) { }

            public override VertexLayoutDescription[] GetVertexDescription() {
                return new[] {
                    new VertexLayoutDescription(
                            new VertexElementDescription("pos", VertexElementSemantic.Position, VertexElementFormat.Float4),
                            new VertexElementDescription("c", VertexElementSemantic.Color, VertexElementFormat.Float4))
                };
            }
            public override ResourceLayoutDescription GetResourceDescription() {
                return new ResourceLayoutDescription(
                       new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex));
            }
            public override uint GetVertexSizeInBytes() {
                return LinesVertex.SizeInBytes;
            }

            protected override LinesVertex[] ConvertVertexToShaderStructure(Geometry3D geo) {
                var res = new List<LinesVertex>();

                for (int i = 0; i < geo.Positions.Count; i++) {
                    var pos = geo.Positions[i];
                    res.Add(new LinesVertex() { Position = new Vector4(pos.X, pos.Y, pos.Z, 1), Color = RgbaFloat.Yellow.ToVector4() });
                }

                return res.ToArray();
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct LinesVertex {
                public Vector4 Position;
                public Vector4 Color;
                public const int SizeInBytes = 4 * (4 + 4);
            }
        }
        public class LineDeviceBufferesUpdater : DeviceBufferesUpdater {
            public LineDeviceBufferesUpdater(LineShaderSpecification shader) : base(shader) {

            }
            public override void UpdateVertex(Geometry3D geo) {
                var vertices = shader.ConvertVertexToShaderStructure<LineShaderSpecification.LinesVertex>(geo);
                factory.CreateIfNullBuffer(ref vertexBuffer, new BufferDescription((uint)(shader.GetVertexSizeInBytes() * vertices.Length),
                   BufferUsage.VertexBuffer));
                cmd.UpdateBuffer(vertexBuffer, 0, vertices);
            }
        }

        static BoundingBoxRenderableComponent GetBoundingBoxCom(BoundingBox box) {
            var name = "BoundingBoxShader";

            var compiler = new D3DShaderCompilator(null);

            var vertex = new ShaderInMemoryInfo(name + "Vertex", VertexShaderText,
                compiler.Compile(VertexShaderText,
                    "VShaderBoundingBox", ShaderStages.Vertex.ToString()),
                ShaderStages.Vertex.ToString(), "VShaderBoundingBox");

            var pixel = new ShaderInMemoryInfo(name + "Pixel", PixelShaderText,
                compiler.Compile(PixelShaderText,
                    "PShaderBoundingBox", ShaderStages.Fragment.ToString()),
                ShaderStages.Fragment.ToString(), "PShaderBoundingBox");

            var geometry = new ShaderInMemoryInfo(name + "Geometry", GeometryShaderText,
                compiler.Compile(GeometryShaderText,
                    "GShaderBoundingBox", ShaderStages.Geometry.ToString()),
                ShaderStages.Geometry.ToString(), "GShaderBoundingBox");

            var pass = new ShaderTechniquePass(new IShaderInfo[] { vertex, geometry, pixel, });

            var shader = new LineShaderSpecification(new[] { pass });

            return new BoundingBoxRenderableComponent(shader, new LineDeviceBufferesUpdater(shader), box);
        }

        #endregion
               
        public enum Modes {
            BoundingBox,
            PointsCloud,
        }

        readonly GraphicEntity entity;

        public EntityRenderModeSwither(GraphicEntity entity) {
            this.entity = entity;
        }

        public void TurnOn(Modes mode) {
            var geo = entity.GetComponent<IGeometryComponent>();

            entity.AddComponent(GetBoundingBoxCom(geo.Geometry.Bounds));

           // entity.AddComponent(Particles.GetParticlesCom());

        }

        public void TurnOff(Modes mode) {
         //   entity.RemoveComponentsOfType<BoundingBoxRenderableComponent>();
        }
    }
}
