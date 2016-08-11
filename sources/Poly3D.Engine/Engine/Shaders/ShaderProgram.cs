using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Poly3D.Engine.Shaders
{
    public class ShaderProgram
    {
        private const string BasicTypes = @"bool|int|uint|float|double|[biud]?vec\d|mat\dx\d|mat\d";
        private readonly static Regex UniformsRegex = new Regex(@"uniform\s+(" + BasicTypes + @")\s+([\w_]+)\s*;", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly static Regex InputsRegex = new Regex(@"(?:in|attribute)\s+(" + BasicTypes + @")\s+([\w_]+)\s*;", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsSupported
        {
            get
            {
                return new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0) ? true : false;
            }
        }

        private Dictionary<int, ShaderParameter> _Parameters;
        private Shader _VertexShader;
        private Shader _FragmentShader;

        public Shader VertexShader
        {
            get { return _VertexShader; }
        }

        public Shader FragmentShader
        {
            get { return _FragmentShader; }
        }

        public IEnumerable<ShaderParameter> Parameters
        {
            get { return _Parameters.Values; }
        }

        public ShaderProgram()
        {
            _Parameters = new Dictionary<int, ShaderParameter>();
            _VertexShader = null;
            _FragmentShader = null;

        }

        private static string ReadPrefabCode(string resourceName)
        {
            using (var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var sr = new StreamReader(resStream))
                    return sr.ReadToEnd();
            }
        }

        public static string GetPrefabCode(string shaderName)
        {
            var resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var shaderPrefabs = resNames.Where(n => n.Contains("Prefabs.Shaders")).ToList();
            if (!shaderPrefabs.Any(n => n.EndsWith(shaderName + ".vert")))
                return null;
            var vertShaderRes = shaderPrefabs.FirstOrDefault(n => n.EndsWith(shaderName + ".vert"));

            return vertShaderRes != null ? ReadPrefabCode(vertShaderRes) : string.Empty;
        }

        public static void TestShader()
        {
            var shaderCode = GetPrefabCode("BasicShader");
            var matches = UniformsRegex.Matches(shaderCode);
        }
    }
}
