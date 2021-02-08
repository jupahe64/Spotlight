using BYAML;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlIterator;

namespace Spotlight
{
    public class ObjectCamera
    {
        Dictionary<string, dynamic> properties = new Dictionary<string, dynamic>();
        string idSuffix = string.Empty;

        public ObjectCamera(ArrayEntry bymlEntry, out string objID)
        {
            objID = string.Empty;

            foreach (var parameter in bymlEntry.IterDictionary())
            {
                if (parameter.Key == "Id")
                {
                    string id = parameter.Parse();

                    string[] idParts = id.Split('_');

                    if (idParts.Length > 1)
                        idSuffix = idParts[1];

                    objID = idParts[0];
                }
                else
                {
                    dynamic data = parameter.Parse();

                    if(data is Dictionary<string, dynamic> dict && dict.Count==3 && dict.ContainsKey("X") && dict.ContainsKey("Y") && dict.ContainsKey("Z"))
                    {
                        data = new Vector3(
                            dict["X"] / 100,
                            dict["Y"] / 100,
                            dict["Z"] / 100
                            );
                    }

                    properties.Add(parameter.Key, data);
                }
            }
        }

        public dynamic this[string key] => properties[key];
    }
}
