using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Publicdefinition;

namespace ErplyPOSDemo
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void button_login_Click(object sender, EventArgs e)
        {
            //用户验证
            try
            {
                this.Cursor = Cursors.WaitCursor;

                GlobalVar.ErplyAPI.clientCode = textBox_CustomerCode.Text;
                GlobalVar.ErplyAPI.username = textBox_Username.Text;
                GlobalVar.ErplyAPI.password = textBox_Password.Text;

                GlobalVar.ErplyAPI.url = "https://" + GlobalVar.ErplyAPI.clientCode + ".erply.com/api/";

                JObject jQeury;
                JToken status;
                //JToken records;
                Dictionary<string, object> inputParameters = new Dictionary<string, object>();
                inputParameters.Add("username", textBox_Username.Text);
                inputParameters.Add("password", textBox_Password.Text);
                jQeury = GlobalVar.ErplyAPI.sendRequest("verifyUser", inputParameters);
                inputParameters.Clear();

                status = jQeury["status"];

                if (status["responseStatus"].ToString() != "ok")
                {
                    this.Cursor = Cursors.Default;
                    string errMsg = String.Format("user verification failed, error code {0}", status["errorCode"].Value<int>());
                    MessageBox.Show(errMsg);
                }
                else
                {
                    GlobalVar.UserName = textBox_Username.Text;
                    GlobalVar.LogonDateTime = DateTime.Now;
                    GetWarehouse2ndPosInfo();
                    this.Cursor = Cursors.Default;
                    //关闭登录窗口，显示主窗口
                    this.Close();
                    new System.Threading.Thread(() =>
                    {
                        Application.Run(new MainForm());
                    }).Start();
                }

            }
            catch (Exception error)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(error.Message);
            }
        }
        public void GetWarehouse2ndPosInfo()
        {
            //获取WarehouseInfo
            JObject jQeury;
            JToken status, records;
            Dictionary<string, object> inputParameters = new Dictionary<string, object>();
            int recordsTotal = 0, recordsOnPage = 0, pageNo = 1;

            do
            {
                inputParameters.Add("recordsOnPage", 500);
                inputParameters.Add("pageNo", pageNo);
                jQeury = GlobalVar.ErplyAPI.sendRequest("getPointsOfSale", inputParameters);
                //  string strVATResult = jQeury.ToString();  //测试所有搜索结果
                inputParameters.Clear();

                status = jQeury["status"];
                if (status["responseStatus"].ToString() != "ok")
                {
                    MessageBox.Show(String.Format("Search failed: error code {0}", status["errorCode"].Value<int>()));
                    return;
                }
                else
                {
                    if (recordsTotal == 0)
                        recordsTotal = status["recordsTotal"].Value<int>();

                    recordsOnPage += status["recordsInResponse"].Value<int>();

                    records = jQeury["records"];
                    foreach (var record in records)
                    {
                        bool nodeFound = false;
                        int wareHouseID = -1, posID = -1;

                        if (!JsonExtensions.IsNullOrEmpty(record["warehouseID"]))
                            wareHouseID = record["warehouseID"].Value<int>();

                        if (!JsonExtensions.IsNullOrEmpty(record["pointOfSaleID"]))
                            posID = record["pointOfSaleID"].Value<int>();

                        foreach (var warehouseInfo in GlobalVar.ListWarehouseInfo)
                        {
                            if (wareHouseID == warehouseInfo.WarehouseID)  //warehouse节点已经存在
                            {
                                //将PosID加入该节点
                                KeyValuePair<int, string> posPair = new KeyValuePair<int, string>(posID, record["name"].ToString());
                                warehouseInfo.ListPosInfo.Add(posPair);
                                nodeFound = true;
                            }
                        }

                        if (!nodeFound)  //节点不存在， 创建节点
                        {
                            WarehouseInfo whinfo = new WarehouseInfo();
                            whinfo.WarehouseID = wareHouseID;
                            whinfo.WarehouseName = record["warehouseName"].ToString();
                            whinfo.address = record["address"].ToString();
                            KeyValuePair<int, string> posPair = new KeyValuePair<int, string>(posID, record["name"].ToString());
                            whinfo.ListPosInfo.Add(posPair);

                            GlobalVar.ListWarehouseInfo.Add(whinfo);
                        }
                    }
                }
                pageNo++;
            }
            while (recordsOnPage < recordsTotal);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            GlobalVar.ErplyAPI = new erplyAPI.EAPI();
            GlobalVar.ListWarehouseInfo = new List<WarehouseInfo>();

            /*textBox_CustomerCode.Text = "422458";
            textBox_Username.Text = "JoeK";
            textBox_Password.Text = "55555555";*/

            textBox_CustomerCode.Text = "428814";
            textBox_Username.Text = "registerwatch";
            textBox_Password.Text = "99999100";

            GlobalVar.ErplyAPI.clientCode = textBox_CustomerCode.Text;
            GlobalVar.ErplyAPI.username = textBox_Username.Text;
            GlobalVar.ErplyAPI.password = textBox_Password.Text;

            GlobalVar.ErplyAPI.url = "https://" + GlobalVar.ErplyAPI.clientCode + ".erply.com/api/";
        }
    }
}
