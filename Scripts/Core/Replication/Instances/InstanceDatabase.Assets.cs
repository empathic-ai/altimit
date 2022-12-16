using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Altimit.Serialization;
#if UNITY_64
using UnityEngine;
#endif

namespace Altimit
{
    public partial class Database
    {
        static Database()
        {
            foreach (var extensionByType in extensionsByType)
            {
                assetTypes.Add(extensionByType.Key);
            }
        }

        Dictionary<AID, AssetMetadata> assetMetadatas = new Dictionary<AID, AssetMetadata>();

        public Action<AID> onAssetRemoved;
        static HashSet<Type> assetTypes = new HashSet<Type>();
        static Dictionary<Type, string> extensionsByType = new Dictionary<Type, string>
        {
#if UNITY_64
            { typeof(Sprite), ".jpg" },
            { typeof(Texture2D), ".jpg" }
#endif
        };

        public bool IsAssetType(Type type)
        {
            return assetTypes.Contains(type);
        }

        public async Task RemoveAsset(AID instanceID)
        {
            TryRemoveInstance(instanceID);
            File.Delete(GetAssetPath(instanceID));
            assetMetadatas.Remove(instanceID);
        }

        public async Task AddAsset(AssetMetadata assetOptions, byte[] bytes)
        {
            await AddAsset(AID.New(), assetOptions, bytes);
        }

        // Saves asset data to a file
        public async Task AddAsset(AID instanceID, AssetMetadata assetOptions, byte[] bytes)
        {
            assetMetadatas.Add(instanceID, assetOptions);
            await WriteBytes(GetAssetMetadataPath(instanceID), await Serializer.SerializeAsync(assetOptions));
            await WriteBytes(GetAssetPath(instanceID), bytes);
            object instance;
            if (TryGetInstance(instanceID, out instance))
            {

            }
            onAssetAdded?.Invoke(instanceID, bytes);
        }

        public async Task<byte[]> ReadBytes(string filePath)
        {
            byte[] bytes;
            using (FileStream SourceStream = File.Open(filePath, FileMode.Open))
            {
                bytes = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(bytes, 0, (int)SourceStream.Length);
            }
            return bytes;
        }

        public async Task WriteBytes(string filePath, byte[] bytes)
        {
            using (FileStream SourceStream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                SourceStream.Seek(0, SeekOrigin.End);
                await SourceStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public async Task<bool> TryAddAsset(AID instanceID, AssetMetadata assetMetadata, byte[] bytes)
        {
            if (HasAsset(instanceID))
                return false;
            await AddAsset(instanceID, assetMetadata, bytes);
            return true;
        }

        public async Task<bool> TryAddAsset(AID instanceID)
        {
            if (HasAsset(instanceID))
                return false;
            await AddAsset(instanceID);
            return true;
        }

        // Loads asset data from a file
        public async Task AddAsset(AID instanceID)
        {
            var assetMetadata = Serializer.Deserialize<AssetMetadata>(await ReadBytes(GetAssetMetadataPath(instanceID)));
            assetMetadatas.Add(instanceID, assetMetadata);

            var assetBytes = await GetAssetBytes(instanceID);
            onAssetAdded?.Invoke(instanceID, assetBytes);
        }

        public async Task<byte[]> GetAssetBytes(AID instanceID)
        {
            return await ReadBytes(GetAssetPath(instanceID));
        }

        public async Task<T> AddAsset<T>(T instance)
        {
            return (T)(await AddAsset(typeof(T), instance));
        }

        public async Task<object> AddAsset(Type type, object instance)
        {
            var bytes = await Serializer.SerializeAsync(instance);
            AID instanceID;
            TryAddInstance(instance, out instanceID);
            await AddAsset(instanceID, new AssetMetadata(type.GetNativeTypeName(), extensionsByType[type]), bytes);
            return instance;
        }

        public async Task UpdateAsset(AID instanceID, object instance)
        {
            var bytes = await Serializer.SerializeAsync(instance);
            await UpdateAsset(instanceID, bytes);
        }

        public async Task UpdateAsset(AID instanceID, byte[] bytes)
        {
            await WriteBytes(GetAssetPath(instanceID), bytes);
            if (!File.Exists(GetAssetMetadataPath(instanceID)))
                await WriteBytes(GetAssetMetadataPath(instanceID), await Serializer.SerializeAsync(GetAssetMetadata(instanceID)));
            var fileExtension = assetMetadatas[instanceID];
            object instance;
            if (TryGetInstance(instanceID, out instance))
                await Serializer.DeserializeAsync(instance.GetType(), GetInstance(instanceID), bytes);
            onAssetUpdated?.Invoke(instanceID, bytes);
        }

        public string GetAssetExtension(object instance)
        {
            return GetAssetExtension(GetInstanceID(instance));
        }

        public string GetAssetExtension(Guid instanceID)
        {
            return Path.GetExtension(GetAssetPath(instanceID));
        }

        public string GetAssetPath(object instance)
        {
            return GetAssetPath(GetInstanceID(instance));
        }

        public string GetAssetPath(AID instanceID)
        {
            var assetMetadata = assetMetadatas[instanceID];
            return Path.Combine(OS.Settings.AssetsPath, instanceID.ToString() + assetMetadata.Extension);
        }

        public AssetMetadata GetAssetMetadata(AID instanceID)
        {
            return assetMetadatas[instanceID];
        }

        public string GetAssetMetadataPath(AID instanceID)
        {
            return Path.Combine(OS.Settings.AssetsPath, instanceID.ToString() + ".meta");
        }

        public bool HasAsset(object instance)
        {
            return HasAsset(GetInstanceID(instance));
        }

        public bool HasAsset(AID instanceID)
        {
            return assetMetadatas.TryGetValue(instanceID, out _);
        }

        // TODO: Rewrite to import asset for persistent rooms
        /*
        public async Task ImportAsync(string fileName)
        {
            var assetPath = AssetPath;
            Directory.CreateDirectory(assetPath);
            foreach (var filePath in Directory.GetFiles(assetPath))
                File.Delete(filePath);
            //string directoryPath = Path.GetFullPath(Path.GetDirectoryName(targetFilePath));
            ZipUtil.Unzip(Path.Combine(ProjectPath, fileName), assetPath);
            foreach (var filePath in Directory.GetFiles(assetPath))
            {
                var globalFileName = Path.GetFileName(filePath);
                if (globalFileName != assetFileName)
                {
                    globalFileName = globalFileName.Replace(Path.GetExtension(filePath), null);
                    var tex = new Texture2D(0, 0);
                    //tex.LoadFromPath(filePath);
                    tex.LoadImage(File.ReadAllBytes(filePath));
                    globe.AddInstance(Guid.Parse(globalFileName), tex);
                }
            }
            var locals = await Serializer.DeserializeAsync<LocalInstanceReference[]>(File.ReadAllBytes(Path.Combine(assetPath, assetFileName)));
            //Debug.Log("Importing locals :" + GlobeExtensions.GlobalToString(locals, false));
            globe.AddLocals(locals);
            //Debug.Log("Imported globe :" + globe.ToString());

            foreach (var filePath in Directory.GetFiles(assetPath))
                File.Delete(filePath);
            Directory.Delete(assetPath);
        }*/

        /*

        public static async Task ExportAsync(string fileName, Globe globe)
        {
            //var globe = new Globe();

            //AssetExtensions.ExtendObjects(go, globe);
            string assetPath = AssetPath;
            Directory.CreateDirectory(assetPath);
            foreach (var filePath in Directory.GetFiles(assetPath))
                File.Delete(filePath);
            //Debug.Log("Exporting asset :" + globe.ToString());
            var locals = globe.GetLocals();
            //Debug.Log("Exporting locals :" + GlobeExtensions.GlobalToString(locals, false));

            var bytes = await Serializer.SerializeAsync(locals);
            using (FileStream SourceStream = File.Open(Path.Combine(assetPath, assetFileName), FileMode.OpenOrCreate))
            {
                SourceStream.Seek(0, SeekOrigin.End);
                await SourceStream.WriteAsync(bytes, 0, bytes.Length);
            }
            foreach (var globalByID in globe.GlobalsByID.GetFirst())
            {
                string globalFilePath;
                if (TryGetFilePath(globalByID.Value, out globalFilePath))
                {
                    string targetGlobalFilePath = Path.Combine(assetPath, globalByID.Key.ToString()+Path.GetExtension(globalFilePath));
                    if (Path.GetExtension(globalFilePath) == ".psd")
                    {
                        var tex = (Texture2D)globalByID.Value;
                        bytes = Texture2DInfo.GetReadableCopy(tex).EncodeToPNG();
                        using (FileStream SourceStream = File.Open(Path.ChangeExtension(targetGlobalFilePath, ".png"), FileMode.OpenOrCreate))
                        {
                            SourceStream.Seek(0, SeekOrigin.End);
                            await SourceStream.WriteAsync(bytes, 0, bytes.Length);
                        }
                    } else
                    {
                        File.Copy(globalFilePath, targetGlobalFilePath);
                    }
                }
            }
            await Task.Run(() => ZipUtil.Zip(Path.Combine(ProjectPath, fileName), Directory.GetFiles(assetPath)));
            //Debug.Log(directoryPath.TrimEnd('\\') + ".zip");

            foreach (var filePath in Directory.GetFiles(assetPath))
                File.Delete(filePath);
            Directory.Delete(assetPath);

            //AssetExtensions.UnextendObjects(go, globe);
        }
        */
    }
}
