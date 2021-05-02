using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonConverter
{
   public static string ToJson(object obj, bool pretty = false)
   {
      return JsonConvert.SerializeObject(obj, pretty ? Formatting.Indented : Formatting.None);
   }

   public static T FromJson<T>(string json)
   {
      return JsonConvert.DeserializeObject<T>(json);
   }
}

