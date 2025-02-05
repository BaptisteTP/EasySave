using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
	public class Settings
	{
		public string? ActiveLanguage { get; set; }
		public string? FallBackLanguage { get; set; }
		public string? DailyLogPath { get; set; }
		public string? RealTimeLogPath { get; set; }

		public static Settings CreateBaseSettings()
		{
			Settings baseSettings = new Settings()
			{
				ActiveLanguage = "",
				FallBackLanguage = "en-US",
				DailyLogPath = "",
				RealTimeLogPath = ""
			};

			var options = new JsonSerializerOptions { WriteIndented = true };
			string baseSttingsJson = JsonSerializer.Serialize<Settings>(baseSettings, options);

			using (StreamWriter sw = File.CreateText("appsettings.json"))
			{
				sw.WriteLine(baseSttingsJson);
			}

			return baseSettings;
		}

		public static Settings GetBaseSettings()
		{
			string jsonAppSettings = File.ReadAllText("appsettings.json");
			return JsonSerializer.Deserialize<Settings>(jsonAppSettings);
		}
	}
}
