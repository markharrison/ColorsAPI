using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ColorsAPI.Models;
using Microsoft.AspNetCore.Http;

namespace ColorsAPI.Services
{
    public class ColorsService
    {
        private List<ColorsItem> _listColors = new() {  };  // This will only work for a single instance of the service ... to be replaced by datastore

        private IConfiguration _config;

        public ColorsService(IConfiguration config)
        {
 
            _config = config;

            _ = Reset();

            return;
        }

        public async Task<List<ColorsItem>> GetAll()
        {
            await Task.Run(() => { });

            return _listColors;
        }


        public async Task<ColorsItem> GetById(int id)
        {
            ColorsItem _colorItem = _listColors.Find(x => x.Id == id);

            await Task.Run(() => { });

            return _colorItem;

        }

        public async Task<ColorsItem> GetByName(string pName)
        {
            await Task.Run(() => { });

            ColorsItem _colorItem = null;

            int idxName = _listColors.FindIndex(a => a.Name.ToLower() == pName.ToLower().Trim());
            if (idxName >= 0)
            {
                _colorItem = _listColors[idxName];
            }

            return _colorItem;

        }

        public async Task<ColorsItem> UpdateById(int id, ColorsItem colorsItemUpdate)
        {
            int idx = id;

            int idxName = _listColors.FindIndex(a => a.Name.ToLower() == colorsItemUpdate.Name.ToLower().Trim());
            if (idxName >= 0)
            {
                _listColors.RemoveAt(idxName);
            }

            if (idx > 0)
            {
                int idxId = _listColors.FindIndex(a => a.Id == idx);
                if (idxId >= 0)
                {
                    _listColors.RemoveAt(idxId);
                }
            }
            else
            {
                for (int i = 1; i <= 1000; i++)
                {
                    if (_listColors.Find(x => x.Id == i) == null)
                    {
                        idx = i;
                        break;
                    }
                }
            }

            colorsItemUpdate.Id = idx;
            colorsItemUpdate.Name = colorsItemUpdate.Name.ToLower().Trim();

            _listColors.Add(colorsItemUpdate);

            await Task.Run(() => { });

            return colorsItemUpdate;

        }

        public async Task<ColorsItem> DeleteById(int id)
        {
            int idxId = _listColors.FindIndex(a => a.Id == id);
            if (idxId >= 0)
            {
                _listColors.RemoveAt(idxId);
            }

            await Task.Run(() => { });

            return null;

        }

        public async Task<ColorsItem> DeleteAll()
        {
            _listColors.Clear();

            await Task.Run(() => { });

            return null;

        }

        public async Task<ColorsItem> Reset()
        {
            await DeleteAll();

            await UpdateById(1, new ColorsItem { Id = 1, Name = _config.GetValue<string>("Color1"), Data = null }); 
            await UpdateById(2, new ColorsItem { Id = 2, Name = _config.GetValue<string>("Color2"), Data = null });
            await UpdateById(3, new ColorsItem { Id = 2, Name = _config.GetValue<string>("Color3"), Data = null });

            return null;

        }

        public string GetAppConfigInfo(HttpContext context)
        {
            string EchoData(string key, string value)
            {
                return key + ": <span class='echodata'>" + value + "</span><br/>";
            }

            string EchoDataBull(string key, string value)
            {
                return EchoData("&nbsp;&bull;&nbsp;" + key, value);
            }

            string strHtml = "";
            strHtml += "<html><head>";
            strHtml += "<style>";
            strHtml += "body { font-family: \"Segoe UI\",Roboto,\"Helvetica Neue\",Arial;}";
            strHtml += ".echodata { color: blue }";
            strHtml += "</style>";
            strHtml += "</head><body>";
            strHtml += "<h3>ColorsAPI - AppConfigInfo</h3>";

            strHtml += EchoData("OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            strHtml += EchoData("Framework Description", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            strHtml += EchoData("BuildIdentifier", _config.GetValue<string>("BuildIdentifier"));

            if (_config.GetValue<string>("AdminPW") == context.Request.Query["pw"].ToString())
            {
                strHtml += EchoData("ASPNETCORE_ENVIRONMENT", _config.GetValue<string>("ASPNETCORE_ENVIRONMENT"));
                strHtml += EchoData("APPLICATIONINSIGHTS_CONNECTION_STRING", _config.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"));
                strHtml += EchoData("Default Colors", _config.GetValue<string>("Color1") + " | " + _config.GetValue<string>("Color2") + " | " + _config.GetValue<string>("Color3"));             
            }

            strHtml += "RequestInfo: <br/>";
            strHtml += EchoDataBull("host", context.Request.Host.ToString());
            strHtml += EchoDataBull("ishttps", context.Request.IsHttps.ToString());
            strHtml += EchoDataBull("method", context.Request.Method.ToString());
            strHtml += EchoDataBull("path", context.Request.Path.ToString());
            strHtml += EchoDataBull("pathbase", context.Request.PathBase.ToString());
            strHtml += EchoDataBull("pathbase", context.Request.Protocol.ToString());
            strHtml += EchoDataBull("pathbase", context.Request.QueryString.ToString());
            strHtml += EchoDataBull("scheme", context.Request.Scheme.ToString());

            strHtml += "Headers: <br/>";
            foreach (var key in context.Request.Headers.Keys)
            {
                strHtml += EchoDataBull(key, $"{context.Request.Headers[key]}");
            }

            strHtml += "Connection:<br/>";
            strHtml += EchoDataBull("localipaddress", context.Connection.LocalIpAddress.ToString());
            strHtml += EchoDataBull("localport", context.Connection.LocalPort.ToString());
            strHtml += EchoDataBull("remoteipaddress", context.Connection.RemoteIpAddress.ToString());
            strHtml += EchoDataBull("remoteport", context.Connection.RemotePort.ToString());

            strHtml += "<hr/>";
            strHtml += "<a href='/'>Home</a>" + "<br/>";
            strHtml += "</body></html>";

            return strHtml;

        }
    }

}
