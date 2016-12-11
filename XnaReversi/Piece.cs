using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XnaReversi
{
    class Piece
    {
        //ピースの種類
        public static readonly int PIECE_NONE = 0;
        public static readonly int PIECE_BLACK = 1;
        public static readonly int PIECE_WHITE = 2;

        //ピースのテクスチャ。必ず最初に初期化する
        public static Texture2D[] textureSet;
        private Texture2D texture;
        private Vector2 size;
        private Vector2 position;
        private int type;
        public int Type
        {
            set
            {
                this.type = value;
                texture = textureSet[type];
            }
            get
            {
                return type;
            }
        }

        public Piece(Vector2 size, Vector2 pos)
        {
            this.size = size;
            this.position = pos;
        }

        public Texture2D getTexture()
        {
            return texture;
        }
        public Vector2 getPos()
        {
            return position;
        }
    }
}
