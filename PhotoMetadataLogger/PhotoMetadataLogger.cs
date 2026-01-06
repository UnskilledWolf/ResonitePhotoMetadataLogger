using Elements.Core;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

using System.Text.Json;

namespace PhotoMetadataLogger;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class PhotoMetadataLogger : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
	public override string Name => "Resonite Photo Metadata Logger";
	public override string Author => "AxiomWolf";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/resonite-modding-group/ExampleMod/";

	public override void OnEngineInit() {
		Harmony harmony = new("dev.axiomwolf.PhotoMetadataLogger");
		harmony.PatchAll();
	}

	// Append exporting of Metadata to the PhotoMetadata's Save Screenshot context menu option
	[HarmonyPatch(typeof(PhotoMetadata), "NotifyOfScreenshot")]
	class PhotoMetadata_NotifyOfScreenshot_ExportMetadata {
		static void Postfix(PhotoMetadata __instance) {
			Msg("Exporting photo summary.");

			PhotoMetadataSummary summary = new(__instance);

			var options = new JsonSerializerOptions {
				IncludeFields = true,
			};
			string jsonString = JsonSerializer.Serialize(summary, options);
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Resonite.jsonl");
			File.AppendAllText(path, jsonString + Environment.NewLine);
		}
	}

	public class PhotoMetadataSummary(PhotoMetadata photo) {
		public readonly string CameraManufacturer = photo.CameraManufacturer.Value ?? "N/A";
		public readonly string CameraModel = photo.CameraModel.Value ?? "N/A";
		public readonly float CameraFOV = photo.CameraFOV.Value;
		public readonly bool Is360 = photo.Is360.Value;
		public readonly bool IsStereo = photo.StereoLayout.Value != StereoLayout.None;
		public readonly string LocationName = photo.LocationName.Value ?? "Unknown";
		public readonly string LocationURL = photo.LocationURL.Value != null ? photo.LocationURL.Value.AbsoluteUri : "";
		public readonly UserSummary LocationHost = new(photo.LocationHost);
		public readonly string LocationAccessLevel = photo.LocationAccessLevel.ToString();
		public readonly bool LocationHiddenFromListing = photo.LocationHiddenFromListing.Value ?? false;
		public readonly DateTime TimeTaken = photo.TimeTaken.Value;
		public readonly UserSummary TakenBy = new(photo.TakenBy);
		public readonly float[] TakenGlobalPosition = FlattenFloat3(photo.TakenGlobalPosition.Value);
		public readonly float[] TakenGlobalRotation = FlattenFloat3(photo.TakenGlobalRotation.Value.EulerAngles);
		public readonly float[] TakenGlobalScale = FlattenFloat3(photo.TakenGlobalScale.Value);
		public readonly string AppVersion = photo.AppVersion.Value;
		public readonly WorldUserSummary[] UserInfos = ProcessUserInfos(photo.UserInfos);

		// Predict filename based on the time the photo was taken
		// TODO still figure out a way to get the correct extension
		public readonly string filename = photo.TimeTaken.Value.ToString("yyyy-MM-dd HH.mm.ss") + ".jpg";
	}

	public static float[] FlattenFloat3(float3 input) {
		return [input[0], input[1], input[2]];
	}

	public static WorldUserSummary[] ProcessUserInfos(SyncList<AssetMetadata.UserInfo> usersList) {
		var items = usersList.Select(user => new WorldUserSummary(user));
		return [.. items];
	}

	public class WorldUserSummary(AssetMetadata.UserInfo userInfo) {
		public readonly UserSummary User = new(userInfo.User);
		public readonly bool IsInVR = userInfo.IsInVR;
		public readonly bool IsPresent = userInfo.IsPresent;
		public readonly float[] HeadPosition = FlattenFloat3(userInfo.HeadPosition.Value);
		public readonly float[] HeadOrientation = FlattenFloat3(userInfo.HeadOrientation.Value.EulerAngles);
		public readonly DateTime SessionJoinTimestamp = userInfo.SessionJoinTimestamp;
	}

	public class UserSummary(UserRef user) {
		public readonly string Username = user.User != null ? user.Target.UserName : "";
		public readonly string UserId = user.LinkedCloudId;
	}
}
