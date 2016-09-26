using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cg2016Excer1
{
    class Collections
    {
        public Texture2D collectionTexture;
        public Vector2 curPosition;
        public Vector2 tarPosition;
        public int value;
        public bool visible;



        public Collections(Texture2D texture, Vector2 CurPosition,Vector2 TarPosition, int Value)
        {
            collectionTexture = texture;
            curPosition = CurPosition;
            value = Value;
            tarPosition = TarPosition;
            //visible = Visible;
        }



    }
}
