using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using Poly3D.Engine.Utilities;


namespace Poly3D.Engine.OpenGL
{
    public class Texture : IDisposable
    {

        private TextureTarget _Target;
        private int _Id;
        private TextureMinFilter _MinificationFilter;
        private TextureMagFilter _MagnificationFilter;
        private TextureCompareMode _CompareMode;
        private bool isDirty;

        /// <summary>
        /// Gets the texture id (name).
        /// </summary>
        public int Id
        {
            get { return _Id; }
        }

        public TextureTarget Target
        {
            get { return _Target; }
        }
        
        /// <summary>
        /// Gets or sets the texture magnification filter.
        /// The texture magnification function is used whenever the level-of-detail function used when sampling from the texture determines that the texture should be magified. 
        /// </summary>
        [DefaultValue(TextureMagFilter.Linear)]
        public TextureMagFilter MagnificationFilter
        {
            get { return _MagnificationFilter; }
            set
            {
                if (_MagnificationFilter == value)
                    return;
                _MagnificationFilter = value;
                isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the texture minification filter.
        /// The texture minifying function is used whenever the level-of-detail function used when sampling from the texture determines that the texture should be minified.
        /// </summary>
        [DefaultValue(TextureMinFilter.NearestMipmapLinear)]
        public TextureMinFilter MinificationFilter
        {
            get { return _MinificationFilter; }
            set
            {
                if (_MinificationFilter == value)
                    return;
                _MinificationFilter = value;
                isDirty = true;
            }
        }
        
        /// <summary>
        /// Specifies the texture comparison mode for currently bound depth textures.
        /// </summary>
        [DefaultValue(TextureCompareMode.None)]
        public TextureCompareMode CompareMode
        {
            get { return _CompareMode; }
            set
            {
                if (_CompareMode == value)
                    return;
                _CompareMode = value;
                isDirty = true;
            }
        }

        public Texture(int id)
        {
            _Id = id;
            _MinificationFilter = TextureMinFilter.NearestMipmapLinear;
            _MagnificationFilter = TextureMagFilter.Linear;
            _CompareMode = TextureCompareMode.None;
        }

        public Texture(TextureTarget target, int id)
        {
            _Target = target;
            _Id = id;
            _MinificationFilter = TextureMinFilter.NearestMipmapLinear;
            _MagnificationFilter = TextureMagFilter.Linear;
            _CompareMode = TextureCompareMode.None;
        }

        //private TextureTarget FindTarget()
        //{
        //    GL.GetInteger(GetPName.Texture2D,
        //    return TextureTarget.Texture2D;
        //}

        public void RefreshProperties()
        {
            using (var tmpBind = new TempAssign<TextureUnit>(
                GetActiveTextureUnit(),
                () => GL.BindTexture(Target, Id),
                (t) => GL.ActiveTexture(t)))
            {
                _MinificationFilter = GetTexParameter<TextureMinFilter>(GetTextureParameter.TextureMinFilter);
                _MagnificationFilter = GetTexParameter<TextureMagFilter>(GetTextureParameter.TextureMagFilter);
                _CompareMode = GetTexParameter<TextureCompareMode>(GetTextureParameter.TextureCompareMode);
            }
            //isDirty = false;
        }

        public void UpdateProperties()
        {
            using (var tmpBind = new TempAssign<TextureUnit>(
                GetActiveTextureUnit(), 
                () => GL.BindTexture(Target, Id), 
                (t) => GL.ActiveTexture(t)))
            {
                GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)MinificationFilter);
                GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)MagnificationFilter);
                GL.TexParameter(Target, TextureParameterName.TextureCompareMode, (int)CompareMode);
            }
            isDirty = false;
        }

        public void Bind()
        {
            if (!GL.IsEnabled(EnableCap.Texture2D))
                GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(Target, Id);
            if (isDirty)
                UpdateProperties();
        }

        public void AssignImage(Bitmap textureImage)
        {
            if (Target != TextureTarget.Texture2D)
                return;
            
            using (var tmpBind = new TempAssign<TextureUnit>(
                GetActiveTextureUnit(),
                () => GL.BindTexture(Target, Id),
                (t) => GL.ActiveTexture(t)))
            {
                BitmapData bmp_data = textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, textureImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                textureImage.UnlockBits(bmp_data);
            }
        }

        public static Texture FromImage(string imagePath)
        {
            using (var bmp = (Bitmap)Bitmap.FromFile(imagePath))
            {
                return FromImage(bmp);
            }
        }

        public static Texture FromImage(Bitmap textureImage)
        {
            var tex = Create();
            tex.AssignImage(textureImage);
            return tex;
        }

        public static Texture Create()
        {
            return Create(TextureTarget.Texture2D);
        }

        public static Texture Create(TextureTarget target)
        {
            var tex = new Texture(target, GL.GenTexture());
            GL.BindTexture(target, tex.Id);
            return tex;
        }

        private T GetTexParameter<T>(GetTextureParameter pname)
        {
            int outVal;
            GL.GetTexParameter(Target, pname, out outVal);
            return (T)Convert.ChangeType(outVal, typeof(T));
        }

        #region Static properties

        private static int _MaxTextureUnits;
        private static int _MaxTextureImageUnits;
        private static int _MaxCombinedTextureImageUnits;

        public static int MaxTextureUnits
        {
            get
            {
                if (_MaxTextureUnits == 0 && GLGet.InContext)
                {
                    _MaxTextureUnits = GLGet.Integer(GetPName.MaxTextureUnits);
                }
                return _MaxTextureUnits;
            }
        }

        public static int MaxTextureImageUnits
        {
            get
            {
                if (_MaxTextureImageUnits == 0 && GLGet.InContext)
                {
                    _MaxTextureImageUnits = GLGet.Integer(GetPName.MaxTextureImageUnits);
                }
                return _MaxTextureImageUnits;
            }
        }

        public static int MaxCombinedTextureImageUnits
        {
            get
            {
                if (_MaxCombinedTextureImageUnits == 0 && GLGet.InContext)
                {
                    _MaxCombinedTextureImageUnits = GLGet.Integer(GetPName.MaxCombinedTextureImageUnits);
                }
                return _MaxCombinedTextureImageUnits;
            }
        }

        #endregion

        public static bool TextureExists(int texture)
        {
            return GL.IsTexture(texture);
        }

        public static Texture GetTexture(int texture)
        {
            if (TextureExists(texture))
                return new Texture(texture);
            return null;
        }

        public static TextureUnit GetActiveTextureUnit()
        {
            if (!GLGet.InContext)
                return TextureUnit.Texture0;
            return (TextureUnit)GLGet.Integer(GetPName.ActiveTexture);
        }

        ~Texture()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Id != 0)
            {
                GL.DeleteTexture(Id);
                _Id = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}
