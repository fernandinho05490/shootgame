using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shootgame
{
    public enum EstadoJogo
    {
        Menu,
        Jogando,
        Opcoes,
        Creditos
    }

    public enum Dificuldade
    {
        Facil,
        Medio,
        CascaGrossa
    }

    public class Menu
    {
        private List<string> itensMenu = new List<string>() { "PLAY", "OPCOES", "CREDITOS" };
        public List<string> opcoesDificuldade = new List<string>() { "FACIL", "MEDIO", "CASCA GROSSA" };
        public int selectedItem = 0;
        public int selectedDificuldade = 0;

        private double elapsed = 0; // Adicionado
        private double delay = 200; // Adicionado

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, EstadoJogo estadoAtual)
        {
            List<string> itensAtuais = estadoAtual == EstadoJogo.Opcoes ? opcoesDificuldade : itensMenu;
            int itemSelecionado = estadoAtual == EstadoJogo.Opcoes ? selectedDificuldade : selectedItem;

            for (int i = 0; i < itensAtuais.Count; i++)
            {
                Color color = (i == itemSelecionado) ? Color.Red : Color.White;
                spriteBatch.DrawString(font, itensAtuais[i], new Vector2(100, 100 + i * 30), color);
            }
        }

        public void Update(EstadoJogo estadoAtual, GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    if (estadoAtual == EstadoJogo.Opcoes)
                    {
                        selectedDificuldade--;
                        if (selectedDificuldade < 0)
                            selectedDificuldade = opcoesDificuldade.Count - 1;
                    }
                    else
                    {
                        selectedItem--;
                        if (selectedItem < 0)
                            selectedItem = itensMenu.Count - 1;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    if (estadoAtual == EstadoJogo.Opcoes)
                    {
                        selectedDificuldade++;
                        if (selectedDificuldade >= opcoesDificuldade.Count)
                            selectedDificuldade = 0;
                    }
                    else
                    {
                        selectedItem++;
                        if (selectedItem >= itensMenu.Count)
                            selectedItem = 0;
                    }
                }

                elapsed = 0; // Reset the elapsed time
            }
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Menu menu;
        private EstadoJogo estadoAtual = EstadoJogo.Menu;
        private Dificuldade dificuldadeAtual = Dificuldade.Medio;

        Texture2D targetSprite;
        Texture2D crosshairsSprite;
        Texture2D backgroundSprite;
        SpriteFont gameFont;

        Vector2 targetPosition = new Vector2(300, 300);
        const int targetRadius = 45;

        MouseState mState;
        bool mReleased = true;
        int score = 0;

        float timer = 0;
        float interval = 2000;

        float gameTimer = 30; // Adicionado
        bool gameEnded = false; // Adicionado

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            menu = new Menu();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            targetSprite = Content.Load<Texture2D>("target");
            crosshairsSprite = Content.Load<Texture2D>("crosshairs");
            backgroundSprite = Content.Load<Texture2D>("sky");
            gameFont = Content.Load<SpriteFont>("galleryFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mState = Mouse.GetState();

            if (estadoAtual == EstadoJogo.Jogando && !gameEnded)
            {
                gameTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (gameTimer <= 0)
                {
                    gameEnded = true;
                    gameTimer = 0;
                }

                if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                {
                    float mouseTargetDist = Vector2.Distance(targetPosition, mState.Position.ToVector2());
                    if (mouseTargetDist < targetRadius)
                    {
                        score++;

                        Random rand = new Random();

                        targetPosition.X = rand.Next(0, _graphics.PreferredBackBufferWidth);
                        targetPosition.Y = rand.Next(0, _graphics.PreferredBackBufferHeight);
                    }
                    mReleased = false;
                }

                if (mState.LeftButton == ButtonState.Released)
                {
                    mReleased = true;
                }

                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timer > interval)
                {
                    Random rand = new Random();

                    targetPosition.X = rand.Next(0, _graphics.PreferredBackBufferWidth);
                    targetPosition.Y = rand.Next(0, _graphics.PreferredBackBufferHeight);

                    // Redefina o temporizador
                    timer = 0;
                }
            }

            menu.Update(estadoAtual, gameTime); // Passamos gameTime para o método Update do menu

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                switch (estadoAtual)
                {
                    case EstadoJogo.Menu:
                        switch (menu.selectedItem)
                        {
                            case 0: // PLAY
                                estadoAtual = EstadoJogo.Jogando;
                                break;
                            case 1: // OPCOES
                                estadoAtual = EstadoJogo.Opcoes;
                                break;
                            case 2: // CREDITOS
                                estadoAtual = EstadoJogo.Creditos;
                                break;
                        }
                        break;
                    case EstadoJogo.Opcoes:
                        switch (menu.selectedDificuldade)
                        {
                            case 0: // FACIL
                                dificuldadeAtual = Dificuldade.Facil;
                                interval = 3000;
                                estadoAtual = EstadoJogo.Menu; // Volta para o menu após selecionar a dificuldade
                                break;
                            case 1: // MEDIO
                                dificuldadeAtual = Dificuldade.Medio;
                                interval = 2000;
                                estadoAtual = EstadoJogo.Menu; // Volta para o menu após selecionar a dificuldade
                                break;
                            case 2: // CASCA GROSSA
                                dificuldadeAtual = Dificuldade.CascaGrossa;
                                interval = 500;
                                estadoAtual = EstadoJogo.Menu; // Volta para o menu após selecionar a dificuldade
                                break;
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (estadoAtual == EstadoJogo.Menu || estadoAtual == EstadoJogo.Opcoes)
            {
                menu.Draw(_spriteBatch, gameFont, estadoAtual);
            }
            else if (estadoAtual == EstadoJogo.Jogando)
            {
                _spriteBatch.Draw(backgroundSprite, new Vector2(0, 0), Color.White);
                _spriteBatch.DrawString(gameFont, score.ToString(), new Vector2(400, 50), Color.White);
                _spriteBatch.DrawString(gameFont, "Tempo restante: " + Math.Round(gameTimer).ToString(), new Vector2(400, 80), Color.White); // Adicionado
                _spriteBatch.Draw(targetSprite, new Vector2(targetPosition.X - targetRadius, targetPosition.Y - targetRadius), Color.White);
                _spriteBatch.Draw(crosshairsSprite, new Vector2(mState.X - crosshairsSprite.Width / 2, mState.Y - crosshairsSprite.Height / 2), Color.White);
            }
            else if (gameEnded) // Adicionado
            {
                _spriteBatch.DrawString(gameFont, "Jogo terminado! Sua pontuação final é: " + score.ToString(), new Vector2(400, 50), Color.White);
            }
            else if (estadoAtual == EstadoJogo.Creditos)
            {
                string[] creditos = new string[]
            {
                "Desenvolvido por:",
                "  Fernando Patrício 01530576",
                " Lucca Lima DAlbuquerque 01534204",
                " Gabriel Batista da Silva 01527525",
                " Pedro Henrique Marques De Luna Calado 01518089",
                " Arthur Manuel 01531055 ",
                " Edleson duarte batista junior 01516994",
                " Gabriel Bandeira de Melo Oliveira da Silva - 01531782"
            };

                Vector2 position = new Vector2(100, 100);
                _spriteBatch.DrawString(gameFont, creditos[i], position, Color.White);
                
                _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
