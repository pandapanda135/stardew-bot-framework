using System.Text;
using StardewBotFramework.Debug;

namespace BotTesting;

public static class CsvWriter
{
	public static void Write(Dictionary<string,List<string>> values)
	{
		var currentTime = DateTime.Now;
		int amount = values[values.Keys.ToList()[0]].Count;
		string path = $"performance/pathfind_perf_test_{currentTime.Year}_{currentTime.Month}_{currentTime.Day}_{currentTime.Hour}_{currentTime.Minute}_{currentTime.Second}_{amount}.csv";

		if (!Directory.Exists("performance"))
		{
			Directory.CreateDirectory("performance");
		}
		// either create or clear file
		File.WriteAllText(path, "");
		
		StringBuilder builder = new();
		string keys = "";
		foreach (var key in values.Keys.Where(key => values[key].Count == 0)) values.Remove(key);
		
		for (int i = 0; i < values.Keys.Count; i++)
		{
			string key = $"{values.Keys.ToList()[i]},";
			// remove unused keys
			if (values.Keys.ToList()[i].Contains(','))
			{
				key = $"\"{key}\"";
			}
			keys += key;
			if (i == values.Keys.Count)
			{
				keys += ",";
			}
		}

		builder.AppendLine(keys);
		
		for (var column = 0; column < values.Values.ToArray()[0].Count; column++)
		{
			string line = $"";
			for (var row = 0; row < values.Keys.Count; row++)
			{
				line += $"{values[values.Keys.ToList()[row]][column]},";
				if (row == values.Keys.Count)
				{
					line += $",";
				}
			}
			
			builder.AppendLine(line);
		}
		
		Logger.Info($"Writing: {path}");
		File.AppendAllText(path, builder.ToString());
	}
}