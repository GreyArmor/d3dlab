﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using D3DLab.Std.Engine.Core.Shaders;

namespace D3DLab.SDX.Engine.Shader {
    public class CombinedShadersLoader {
        const string entry = "main";

        public IShaderInfo[] Load(string resource, string keyname) {
            string text;
            using (var srt = this.GetType().Assembly.GetManifestResourceStream(resource)) {
                var reader = new StreamReader(srt);
                text = reader.ReadToEnd();
            }

            var res = new List<IShaderInfo>();
            var parts = text.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < parts.Length; ++i){
                if(CreateInfo(res, keyname, parts, ShaderStages.Vertex.ToString().ToLower(), ref i)) { continue; }
                if(CreateInfo(res, keyname, parts, ShaderStages.Fragment.ToString().ToLower(), ref i)) { continue; }
            }
            return res.ToArray();
        }
        bool CreateInfo(List<IShaderInfo> infos, string keyname, string[] parts, string partName, ref int i) {
            var part = parts[i].Trim();
            if (part.Length == partName.Length && string.Compare(part, partName, true) == 0) {
                i++;
                var info = new ShaderInMemoryInfo($"{keyname}_{partName}Shader", parts[i].Trim(), null,
                    partName, entry);               
                infos.Add(info);
                return true;
            }
            return false;
        }
    }
}
