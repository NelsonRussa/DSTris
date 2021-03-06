﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SFML.Graphics;
using SFML.System;

namespace DSTris.Model
{
    class Config
    {
        // Guardar as configurações gerais do jogo
        public class GameConfig
        {
            public class BackgroundConfig
            {
                public string TextureName { get; set; }
                public Vector2f GameCoords { get; set; }
                public Vector2f NextBlockCoords { get; set; }
                public Vector2f StatsCoords { get; set; }
                public float StatsMaxY { get; set; }
            }

            public string AssetsFolder { get; set; }
            public string FontName { get; set; }
            public Vector2i GridSize { get; set; }
            public Vector2i PlayableSize { get; set; }
            public BackgroundConfig Background;

            public GameConfig()
            {
                Background = new BackgroundConfig();
            }

        }
        public class MenuConfig
        {
            public class BackgroundConfig
            {
                public string TextureName { get; set; }
            }
            public BackgroundConfig Background;
        }
        public class BlockConfig
        {
            public int ID { get; set; }
            public string TextureName { get; set; }
            public List<Vector2i> Parts { get; set; }
            public Vector2f Size { get; set; }
            public Color StatsColor { get; set; }
        }
        //
        public GameConfig Game;
        public MenuConfig Menu;
        public List<BlockConfig> Blocks;

        // 
        public void Load()
        {
            // Ler ficheiro de configuração, caso não consiga, gera uma exceção
            var xmlDoc = new XmlDocument();
            if (!File.Exists("config.xml"))
                throw new FileNotFoundException("Ficheiro de configuração não encontrado!", "config.xml");
            xmlDoc.Load("config.xml");
            var xmlDocElem = xmlDoc.DocumentElement;


            // Ler configurações do jogo

            // Config/Game
            Game = new GameConfig();
            var nodeGame = xmlDocElem.SelectSingleNode("/config/game");
            Game.AssetsFolder = nodeGame.Attributes["assetsFolder"].Value;
            Game.FontName = GetAssetFullName(nodeGame.Attributes["fontName"].Value);
            if (!File.Exists(Game.FontName))
                throw new FileNotFoundException("Ficheiro da fonte não encontrado!", Game.FontName);
            Game.GridSize = new Vector2i(
                Convert.ToInt32(nodeGame.Attributes["gridWidth"].Value),
                Convert.ToInt32(nodeGame.Attributes["gridHeight"].Value));
            Game.PlayableSize = new Vector2i(
                Convert.ToInt32(nodeGame.Attributes["playableWidth"].Value),
                Convert.ToInt32(nodeGame.Attributes["playableHeight"].Value));
            // - Background
            var nodeGameBackground = nodeGame.SelectSingleNode("background");
            Game.Background = new GameConfig.BackgroundConfig();
            Game.Background.TextureName = GetAssetFullName(nodeGameBackground.Attributes["textureName"].Value);
            if (!File.Exists(Game.Background.TextureName))
                throw new FileNotFoundException("Ficheiro de textura não encontrado!", Game.Background.TextureName);
            Game.Background.GameCoords = new Vector2f(
                Convert.ToSingle(nodeGameBackground.Attributes["gameCoordX"].Value),
                Convert.ToSingle(nodeGameBackground.Attributes["gameCoordY"].Value));
            Game.Background.NextBlockCoords = new Vector2f(
                Convert.ToSingle(nodeGameBackground.Attributes["nextBlockCoordX"].Value),
                Convert.ToSingle(nodeGameBackground.Attributes["nextBlockCoordY"].Value));
            Game.Background.StatsCoords = new Vector2f(
                Convert.ToSingle(nodeGameBackground.Attributes["statsX"].Value),
                Convert.ToSingle(nodeGameBackground.Attributes["statsY"].Value));
            Game.Background.StatsMaxY = Convert.ToSingle(nodeGameBackground.Attributes["statsMaxY"].Value);

            // Config/Menu
            Menu = new MenuConfig();
            var nodeMenu = xmlDocElem.SelectSingleNode("/config/menu");
            // - Background
            var nodeMenuBackground = nodeMenu.SelectSingleNode("background");
            Menu.Background = new MenuConfig.BackgroundConfig();
            Menu.Background.TextureName = GetAssetFullName(nodeMenuBackground.Attributes["textureName"].Value);
            if (!File.Exists(Menu.Background.TextureName))
                throw new FileNotFoundException("Ficheiro de textura não encontrado!", Menu.Background.TextureName);

            // Blocks
            var nodeBlocks = xmlDocElem.SelectNodes("/config/blocks/block");
            Blocks = new List<BlockConfig>();
            int nextID = 1;
            foreach (XmlNode nodeBlock in nodeBlocks)
            {
                var block = new BlockConfig();
                block.ID = nextID;
                block.TextureName = nodeBlock.Attributes["textureName"].Value;
                string[] blockLines = nodeBlock.InnerText.Trim().Split('\n');
                block.Size = new Vector2f(blockLines[0].Trim().Length, blockLines.Length);
                block.Parts = new List<Vector2i>();
                for (int l = 0; l < block.Size.Y; l++)
                {
                    for (int c = 0; c < block.Size.X; c++)
                    {
                        if (blockLines[l].Trim().Substring(c, 1) == "1")
                            block.Parts.Add(new Vector2i(c, l));
                    }
                }
                string[] color = nodeBlock.Attributes["statsColor"].Value.Split(',');
                block.StatsColor = new Color(Convert.ToByte(color[0]), Convert.ToByte(color[1]), 
                    Convert.ToByte(color[2]));
                Blocks.Add(block);
                nextID++;
            }
        }

        //
        public string GetAssetFullName(string name)
        {
            return $"{Game.AssetsFolder}\\{name}";
        }
    }
}
