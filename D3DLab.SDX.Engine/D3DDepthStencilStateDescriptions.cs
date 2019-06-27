using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.Direct3D11;

namespace D3DLab.SDX.Engine {
    public static class D3DBlendStateDescriptions {
        public static BlendStateDescription BlendStateDisabled {
            get {
                var blendStateDesc = new BlendStateDescription();
                blendStateDesc.RenderTarget[0].IsBlendEnabled = false;
                blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
                blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                return blendStateDesc;
            }
        }

    }
    public static class D3DDepthStencilStateDescriptions {
        // Now create a second depth stencil state which turns off the Z buffer for 2D rendering.
        // The difference is that DepthEnable is set to false.
        // All other parameters are the same as the other depth stencil state.
        /// <summary>
        /// Correct overlap objects based on depth 
        /// </summary>
        public static DepthStencilStateDescription DepthDisabled = new DepthStencilStateDescription {
            IsDepthEnabled = false, 
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = true,
            StencilReadMask = 0xFF,
            StencilWriteMask = 0xFF,
            // Stencil operation if pixel front-facing.
            FrontFace = new DepthStencilOperationDescription() {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Increment,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            },
            // Stencil operation if pixel is back-facing.
            BackFace = new DepthStencilOperationDescription() {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Decrement,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            }
        };

        /// <summary>
        /// Overlap based on rendering order
        /// </summary>
        public static DepthStencilStateDescription DepthEnabled = new DepthStencilStateDescription {
            // true - correct overlap objects based on depth 
            // false - overlap based on rendering order
            IsDepthEnabled = true,

            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = true,
            StencilReadMask = 0xFF,
            StencilWriteMask = 0xFF,
            // Stencil operation if pixel front-facing.
            FrontFace = new DepthStencilOperationDescription() {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Increment,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            },
            // Stencil operation if pixel is back-facing.
            BackFace = new DepthStencilOperationDescription() {
                FailOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Decrement,
                PassOperation = StencilOperation.Keep,
                Comparison = Comparison.Always
            }
        };
    }
}
