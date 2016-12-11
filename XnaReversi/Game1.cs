using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace XnaReversi
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Piece�̃T�C�Y
        /// </summary>
        public static readonly int PIECE_SIZE = 64;

        /// <summary>
        /// Piece�̌�(�ύX�s��)
        /// </summary>
        public static readonly int PIECE_ROW = 8;
        public static readonly int PIECE_COL = 8;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// ���
        /// </summary>
        Piece[,] pieces;

        /// <summary>
        /// �^�[��
        /// </summary>
        int turn;

        /// <summary>
        /// �X�L�b�v�������ǂ���
        /// </summary>
        bool skipped = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //window�T�C�Y��ύX
            graphics.PreferredBackBufferWidth = PIECE_SIZE * PIECE_COL;
            graphics.PreferredBackBufferHeight = PIECE_SIZE * PIECE_ROW;
            //�}�E�X�J�[�\���\��
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //�^�C�g���ύX
            Window.Title = "XNA-REVERSI";

            //piece������
            pieces = new Piece[8, 8];
            Piece.textureSet = new Texture2D[]{
                Content.Load<Texture2D>("none"),
                Content.Load<Texture2D>("black"),
                Content.Load<Texture2D>("white")
            };
            for (int i = 0; i < PIECE_ROW; i++)
            {
                for(int j = 0; j < PIECE_COL; j++)
                {
                    pieces[i, j] = new Piece(new Vector2(PIECE_SIZE, PIECE_SIZE), new Vector2(i * PIECE_SIZE, j * PIECE_SIZE));
                    pieces[i, j].Type = Piece.PIECE_NONE;
                }
            }

            base.Initialize();

            //���̃^�[����
            turn = Piece.PIECE_BLACK;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //��΂̏����z�u
            pieces[3, 3].Type = Piece.PIECE_BLACK;
            pieces[3, 4].Type = Piece.PIECE_WHITE;
            pieces[4, 3].Type = Piece.PIECE_WHITE;
            pieces[4, 4].Type = Piece.PIECE_BLACK;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //�}�E�X�̍��W�擾
            Vector2 mPos = changeVector(Mouse.GetState());
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //�}�E�X���N���b�N
                if (checkPut((int)mPos.X, (int)mPos.Y))
                {
                    //�u�������΂�u��
                    pieces[(int)mPos.X, (int)mPos.Y].Type = turn;

                    //�Ђ�����Ԃ�
                    int[] flip = upsetPieces((int)mPos.X, (int)mPos.Y);
                    int[] dl = { 1, 1, 0, -1, -1, -1, 0, 1 };
                    int[] dc = { 0, 1, 1, 1, 0, -1, -1, -1 };
                    for (int i = 0; i < flip.Length; i++)
                    {
                        int x = (int)mPos.X, y = (int)mPos.Y;
                        for (int j = 1; j <= flip[i]; j++)
                        {
                            x += dl[i];
                            y += dc[i];
                            pieces[x,y].Type = turn;
                        }
                    }
                    changeTurn();

                    //�X�L�b�v����
                    if(!checkPutPlayer())
                    {
                        if(skipped)
                        {
                            //�Q�[���I��
                            clear();
                        }
                        else
                        {
                            //�X�L�b�v
                            skipped = true;
                            changeTurn();
                        }
                    }
                    else
                    {
                        skipped = false;
                    }
                }
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            foreach(Piece piece in pieces)
            {
                spriteBatch.Draw(piece.getTexture(), piece.getPos(), Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// �}�E�X�̍��W��xy���W�ɕϊ�����
        /// </summary>
        /// <param name="mouseState"></param>
        /// <returns></returns>
        private Vector2 changeVector(MouseState mouseState)
        {
            return new Vector2((int)mouseState.X / PIECE_SIZE, (int)mouseState.Y / PIECE_SIZE);
        }

        /*
         * ���o�[�V�̃Q�[���֌W(�O��̃��o�[�V�Q��)
         */

        private void changeTurn()
        {
            if(turn == Piece.PIECE_BLACK)
            {
                turn = Piece.PIECE_WHITE;
            }
            else
            {
                turn = Piece.PIECE_BLACK;
            }
        }

        /// <summary>
        /// ���W�̂Ђ�����Ԃ����΂̐���Ԃ�
        /// </summary>
        /// <param name="line">x���W</param>
        /// <param name="col">y���W</param>
        /// <returns>8�����̂Ђ�����Ԃ����΂̐�</returns>
        private int[] upsetPieces(int line, int col)
        {
            //�E,�E��,��,����,��,����,��,�E��
            int[] flip = { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] dl = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dc = { 0, 1, 1, 1, 0, -1, -1, -1 };

            int rival = turn == Piece.PIECE_BLACK ? Piece.PIECE_WHITE : Piece.PIECE_BLACK;

            for (int i = 0; i < flip.Length; i++)
            {
                int x = line + dl[i], y = col + dc[i];
                while (x < PIECE_ROW && x >= 0 && y < PIECE_COL && y >= 0)
                {
                    if (pieces[x, y].Type == rival && x + dl[i] < PIECE_ROW && x + dl[i] >= 0 && y + dc[i] < PIECE_COL && y + dc[i] >= 0)
                    {
                        flip[i] += 1;
                    }
                    else if (pieces[x,y].Type == turn)
                    {
                        break;
                    }
                    else
                    {
                        flip[i] = 0;
                        break;
                    }
                    x += dl[i];
                    y += dc[i];
                }
            }

            return flip;
        }
        /// <summary>
        /// ���W�Ɍ�΂�u���邩�ǂ���
        /// </summary>
        /// <param name="line">x���W</param>
        /// <param name="col">y���W</param>
        /// <returns>true�ł���Βu����</returns>
        private bool checkPut(int line, int col)
        {
            int[] flip = upsetPieces(line, col);
            int flipNum = 0;
            for (int i = 0; i < flip.Length; i++)
            {
                flipNum += flip[i];
            }
            return pieces[line,col].Type == Piece.PIECE_NONE && flipNum > 0;
        }

        /// <summary>
        /// ���݂̃v���C���[���u���ꏊ�����邩�ǂ���
        /// </summary>
        /// <returns>true�ł���Βu����</returns>
        private bool checkPutPlayer()
        {
            for (int i = 0; i < PIECE_ROW; i++)
            {
                for (int j = 0; j < PIECE_COL; j++)
                {
                    if (checkPut(i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// �N���A���̏���
        /// </summary>
        private void clear()
        {
            //TODO
        }
    }
}
