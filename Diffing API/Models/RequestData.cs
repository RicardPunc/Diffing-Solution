using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Diffing_API.Models
{
    public class RequestData
    {
        public String data { get; set; }

        //Helper structure used for storing all diff offsets and lengths
        public struct Diff
        {
            public int offset { get; set; }
            public int length { get; set; }

        }

        //Method for decoding Base64 encoded strings
        public string Decode()
        {
            byte[] dataBytes = Convert.FromBase64String(data); 
            return Encoding.UTF8.GetString(dataBytes);
        }

        //Method for comparing two decoded strings
        public JsonResult Compare(RequestData data)
        {
            String left = this.Decode();
            String right = data.Decode();

            if (left.Equals(right))
            {
                return new JsonResult(new { diffResultType = "Equals" });
            }

            if (left.Length != right.Length)
            {
                return new JsonResult(new { diffResultType = "SizeDoNotMatch" });
            }

            List<Diff> diffs = new List<Diff>();

            //Finds all diffs, their offsets and lengths
            for (int i=0; i<left.Length; i++)
            {
                if (!left.ElementAt(i).Equals(right.ElementAt(i)))
                {
                    int j;
                    for (j=i+1; j<left.Length; j++)
                    {
                        if (left.ElementAt(i).Equals(right.ElementAt(j)))
                            break;
                    }

                    diffs.Add(new Diff { offset = i, length = j - i});

                    i = j;
                }
            }

            return new JsonResult(new { diffResultType = "ContentDoNotMatch",  diffs });

        }
    }
}
