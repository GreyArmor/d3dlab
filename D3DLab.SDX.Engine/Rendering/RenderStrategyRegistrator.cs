using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.Rendering.Strategies;
using D3DLab.SDX.Engine.Shader;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace D3DLab.SDX.Engine.Rendering {
    /*
    class FilterByType<TCom1>
        where TCom1 : IGraphicComponent {
        public virtual IEnumerable<GraphicEntity> Filter(IEnumerable<GraphicEntity> entities) {
            return entities.Where(x => x.Has<TCom1>());
        }

    }
    class FilterByType<TCom1, TCom2> : FilterByType<TCom1>
        where TCom1 : IGraphicComponent 
        where TCom2 : IGraphicComponent {
        public override IEnumerable<GraphicEntity> Filter(IEnumerable<GraphicEntity> entities) {
            return base.Filter(entities).Where(x => x.Has<TCom2>());
        }
    }

    class EntityHasSet<TCom1>
        where TCom1 : IGraphicComponent {

        public virtual bool Has(GraphicEntity entity) {
            return entity.Has<TCom1>();
        }

    }
    class EntityHasSet<TCom1, TCom2> : EntityHasSet<TCom1>
        where TCom1 : IGraphicComponent
        where TCom2 : IGraphicComponent {

        public override bool Has(GraphicEntity entity) {
            return base.Has(entity) && entity.Has<TCom2>();
        }

    }

    */


    
}
