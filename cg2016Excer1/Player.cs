using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace cg2016Excer1
{
    class Player 
    {
        public Texture2D _skin;
        public Vector2 position = new Vector2(0,0);
        public int score = 0;

        public Player(Texture2D _Skin, Vector2 Position, int Score)
        {
            _skin = _Skin;
            position = Position;
            score = Score;
        }
        public Player()
        {
            
        }


    }
}
