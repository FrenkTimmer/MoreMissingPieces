using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MoreMissingPieces
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	[BepInDependency(Jotunn.Main.ModGuid)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Major)]
	internal class MoreMissingPiecesPlugin : BaseUnityPlugin
	{
		public const string ModGuid = "org.bepinex.plugins.MoreMissingPieces";
		public const string ModName = "MoreMissingPieces";
		public const string ModVersion = "1.0.0";

		public static CustomLocalization CustomLoc => LocalizationManager.Instance.GetLocalization();

		[Serializable]
		public struct PieceEntry
		{
			public string Prefab;
			public PieceConfig Config;
		}

		private void Awake()
		{
			RegisterPieces();

			string locJson = AssetUtils.LoadTextFromResources("Localization.English.json", Assembly.GetExecutingAssembly());
			CustomLoc.AddJsonFile("English", locJson);
		}

		private List<PieceEntry> GetPieceEntries()
		{
			string configJson = AssetUtils.LoadTextFromResources("Configurations.json", Assembly.GetExecutingAssembly());
			try
			{
				return JsonConvert.DeserializeObject<List<PieceEntry>>(configJson);
			}
			catch (Exception ex)
			{
				Jotunn.Logger.LogError($"Failed to load piece configurations: {ex.Message}");
				return new List<PieceEntry>();
			}
		}

		private void RegisterPieces()
		{
			AssetBundle assetBundle = AssetUtils.LoadAssetBundleFromResources("vis_pieces", Assembly.GetExecutingAssembly());
			List<PieceEntry> pieceEntries = GetPieceEntries();

			foreach (PieceEntry entry in pieceEntries)
			{
				GameObject prefab = assetBundle.LoadAsset<GameObject>(entry.Prefab);
				if (prefab != null)
				{
					PieceManager.Instance.AddPiece(new CustomPiece(prefab, true, entry.Config));
				}
				else
				{
					Jotunn.Logger.LogWarning($"Prefab '{entry.Prefab}' not found in asset bundle.");
				}
			}
		}
	}
}