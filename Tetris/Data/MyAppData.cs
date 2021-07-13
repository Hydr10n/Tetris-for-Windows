using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Tetris.Data
{
    static class MyAppData
    {
        private static readonly string GameSaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Saved Games\Hydr10n\Tetris\GameSave.xml");
        private static readonly XDocument xDocument;

        static MyAppData()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(GameSaveFilePath));
            xDocument = File.Exists(GameSaveFilePath) ? XDocument.Load(GameSaveFilePath) : new XDocument();
            xDocument.Declaration = new XDeclaration(null, "utf-8", null);
            if (xDocument.Root == null)
                xDocument.Add(new XElement("Data"));
        }

        public static void Save<T>(string key, T data)
        {
            xDocument.Root.Add(key == null ? (object)data : new XElement(key, data));
            xDocument.Save(GameSaveFilePath);
        }

        public static IEnumerable<XElement> Load() => xDocument.Root.Elements();
    }
}