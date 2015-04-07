using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TextConverter
{
	private static string SelectRandomWord(string random_list)
	{
		List<string> randomStringList = new List<string>();
		Match match = Regex.Match(random_list.Substring (1, random_list.Length - 2),
		            "{[^}]*}");

		while(match.Success)
		{
			randomStringList.Add(match.Value);
			match = match.NextMatch();
		}

		string selected = randomStringList[Random.Range(0, randomStringList.Count)];
		return selected.Substring(1, selected.Length - 2);
	}
	public static string GetTextFromFormatText(string format_text, params string[] param)
	{
		string output = format_text;

		//Regex regex = new Regex("%[[%]^]*%]");

		Match match = Regex.Match(format_text, "\\[[^\\]]*\\]");

		while(match.Success)
		{
			output = output.Replace(match.Value, SelectRandomWord(match.Value));
			match = match.NextMatch();
		}

		for(int i=0; i<param.Length; i++)
		{
			output = output.Replace("#"+i, param[i]);;
		}

		return output;
	}

    public static string TranslateDescData(string descData)
    {
        string output = "";
        Match match = Regex.Match(descData,
                    "\"[^\"]*\"");

        while (match.Success)
        {
            output += match.Value.Substring(1, match.Value.Length-2) + "\n";
            match = match.NextMatch();
        }

        return output;
    }
}

