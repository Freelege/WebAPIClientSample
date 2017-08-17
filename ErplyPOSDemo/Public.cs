using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using erplyAPI;

namespace Publicdefinition
{
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }

    public class WarehouseInfo
    {
        public int WarehouseID;
        public string WarehouseName;
        public string address;
        public List<KeyValuePair<int, string>> ListPosInfo;
        public WarehouseInfo()
        {
            WarehouseID = -1;
            WarehouseName = null;
            address = null;
            ListPosInfo = new List<KeyValuePair<int, string>>();
        }
    }

    public class SearchResult
    {
        public string description;
        public string quantity;
        public string price;
        public string total;
        public SearchResult()
        {
            description = " ";
            quantity = " ";
            price = " ";
            total = " ";
        }

        public SearchResult(string desc, string qty, string pri, string tot)
        {
            description = desc;
            quantity = qty;
            price = pri;
            total = tot;
        }
    }

    public static class GlobalVar
    {
        public static EAPI ErplyAPI;
        public static List<WarehouseInfo> ListWarehouseInfo;
        public static string UserName;
        public static DateTime LogonDateTime;
    }
}

