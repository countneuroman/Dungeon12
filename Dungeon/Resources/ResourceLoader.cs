﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;
using Dungeon.Scenes.Manager;
using LiteDB;

namespace Dungeon.Resources
{
    public static class ResourceLoader
    {
        /// <summary>
        /// Флаг позволяющий не освобождать ресуры
        /// полезно для дебага
        /// </summary>
        public static bool NotDisposingResources = false;

        /// <summary>
        /// Флаг кэширования изображений и их масок
        /// </summary>
        public static bool CacheImagesAndMasks = true;

        public static bool Exists(string resource) => LoadResource(resource) != default;

        private static LiteDatabase liteDatabase;
        private static LiteDatabase LiteDatabase
        {
            get
            {
                if (liteDatabase == default)
                {
                    liteDatabase = new LiteDatabase(ResourceCompiler.CompilePath);
                    DungeonGlobal.Exit += () => liteDatabase.Dispose();
                }
                return liteDatabase;
            }
        }

        private static Resource LoadResource(string resource)
        {
            if (RuntimeCache.ContainsKey(resource))
            {
                return RuntimeCache[resource];
            }

            var db = LiteDatabase.GetCollection<Resource>();

            var res = db.Find(x => x.Path == resource).FirstOrDefault();

            var x = db.Find(x => x.Path.ToLowerInvariant().Contains("shader")).ToArray();

            if (res != default)
            {
                RuntimeCache.Add(resource, res);
            }

            return res;
        }

        public static Resource Load(string resource, bool caching = false)
        {
            var res = LoadResource(resource);

            if (res == default)
            {
                throw new KeyNotFoundException($"Ресурс {resource} не найден!");
            }

            bool addToScene = !caching;
            if (NotDisposingResources)
            {
                addToScene = !SceneManager.Preapering?.Resources.Any(r => r.Path == res.Path) ?? false;
            }

            if (addToScene)
            {
                SceneManager.Preapering?.Resources.Add(res);
            }

            return res;
        }

        private static Dictionary<string, Resource> RuntimeCache = new Dictionary<string, Resource>();
        public static void SaveStream(byte[] bytes, string image)
        {
            throw new NotImplementedException("А нефиг оставлять TODOшки");
            //RuntimeCache[image] = bytes;
        }

        public static Type LoadType(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return default;

            var type = TryGetFromAssembly(className, DungeonGlobal.GameAssembly);
            if (type == default)
            {
                foreach (var asm in DungeonGlobal.Assemblies)
                {
                    type = TryGetFromAssembly(className, asm);
                    if (type != default)
                    {
                        break;
                    }
                }
            }

            if (type == default)
            {
                type = Type.GetType(className);
            }

            if (type == default)
            {
                throw new DllNotFoundException($"Тип {className} не найден ни в одной из загруженных сборок!");
            }

            return type;
        }

        private static Type TryGetFromAssembly(string className, Assembly assembly)
        {
            if (assembly == default)
                return default;

            var type = assembly.GetType(className);
            if (type == default)
            {
                type = assembly.GetTypes().FirstOrDefault(x => x.Name == className);
            }

            return type;
        }
    }
}