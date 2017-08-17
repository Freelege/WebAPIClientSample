using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Publicdefinition;
using System.Runtime.InteropServices;
using System.Globalization;

namespace ErplyPOSDemo
{
    public partial class MainForm : Form
    {
        public const int USER = 0x0400;  //Self-defined message
        public List<VatRate> ListVatRate;
        public List<SearchResult> ListSearchResult;
        public List<int> selectedPosIDs;
        public List<int> selectedWarehouseIDs;
        DateTime dateTimeFrom;
        DateTime dateTimeTo;

        public bool searching;
        public bool searchCancelled;
        private ManualResetEvent _stopSearchEvent;

        public int displayCount;

        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case USER + 1:  //Search completed
                    //string message = string.Format("收到自己消息的参数:{0},{1}", m.WParam, m.LParam);
                    //处理启动 函数MessageBox.Show(message);//显示一个消息框
                    SearchStopped();
                    break;

                default:
                    base.DefWndProc(ref m);//一定要调用基类函数，以便系统处理其它消息。
                    break;
            }
        }
        private void SearchStopped()
        {
            button_Search.Text = "Search";
            button_Search.Enabled = true;
            this.Cursor = Cursors.Default;
            listView_wareHouse.Enabled = true;
            listView_pos.Enabled = true;
            dateTime_From.Enabled = true;
            dateTime_To.Enabled = true;
            timer_progress.Stop();

            if (searchCancelled)
                label_searchOngoing.Text = "Search cancelled";
            else
                label_searchOngoing.Text = "Search completed";

            searchCancelled = false;
            searching = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-AU");

            ListVatRate = new List<VatRate>();
            ListSearchResult = new List<SearchResult>();
            selectedWarehouseIDs = new List<int>();
            selectedPosIDs = new List<int>();
            dateTime_From.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            dateTime_To.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
            searching = false;
            searchCancelled = false;
            _stopSearchEvent = new ManualResetEvent(false);

            //初始化WareHouse列表框以及POS列表框
            listView_wareHouse.Columns.Add("ID", 50, HorizontalAlignment.Left);
            listView_wareHouse.Columns.Add("Warehouse Name", 260, HorizontalAlignment.Left);
            listView_wareHouse.View = View.Details;
            listView_wareHouse.FullRowSelect = true;
            listView_wareHouse.HideSelection = false;  //让被选中的行一直处于选中状态
           // listView_wareHouse.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            listView_pos.Columns.Add("ID", 50, HorizontalAlignment.Left);
            listView_pos.Columns.Add("POS Name", 260, HorizontalAlignment.Left);
            listView_pos.View = View.Details;
            listView_pos.FullRowSelect = true;
            listView_pos.HideSelection = false;  //让被选中的行一直处于选中状态

            listView_wareHouse.BeginUpdate();
            ListViewItem lvi0_ware = new ListViewItem();
            lvi0_ware.Text = "";
            lvi0_ware.SubItems.Add("All warehouses");
            listView_wareHouse.Items.Add(lvi0_ware);

            listView_pos.BeginUpdate();
            ListViewItem lvi0_pos = new ListViewItem();
            lvi0_pos.Text = "";
            lvi0_pos.SubItems.Add("All POS");
            listView_pos.Items.Add(lvi0_pos);

            //Show all warehouses
            foreach (var warehouseInfo in GlobalVar.ListWarehouseInfo)
            {
                ListViewItem lvi_ware = new ListViewItem();
                lvi_ware.Text = String.Format("{0:000}", warehouseInfo.WarehouseID);
                lvi_ware.SubItems.Add(warehouseInfo.WarehouseName);
                listView_wareHouse.Items.Add(lvi_ware);
            }
            listView_pos.EndUpdate();
            listView_wareHouse.EndUpdate();
            listView_wareHouse.Items[0].Selected = true;  //初始化设置第一行被选中

            //初始化result列表框
            listView_result.VirtualMode = true;
            listView_result.VirtualListSize = 0;
            listView_result.Columns.Add(" ", 315, HorizontalAlignment.Left);  //Description
            listView_result.Columns.Add(" ", 80, HorizontalAlignment.Left);   //Qty
            listView_result.Columns.Add(" ", 140, HorizontalAlignment.Left);   //Price
            listView_result.Columns.Add(" ", listView_result.Size.Width - 495, HorizontalAlignment.Left);   //Total
            listView_result.View = View.Details;

            label_userName.Text = "User: " + GlobalVar.UserName;
            label_logonTime.Text = "Logon at: " + GlobalVar.LogonDateTime.ToString();

            //清空几个信息指示Label
            label_searchOngoing.Text = "";
            label_totalTransaction.Text = "";
            label_totalItems.Text = "";
            label_totalAmount.Text = "";
        }

        private void button_Search_Click(object sender, EventArgs e)
        {
             try
            {
                 if (searching)
                 {
                     _stopSearchEvent.Set();
                     button_Search.Text = "Stopping...";
                     button_Search.Enabled = false;
                     return;
                 }

                JObject jQeury;
                JToken status, records;
                Dictionary<string, object> inputParameters = new Dictionary<string, object>();
               
                //如果是第一次搜索， 查询VAT rate
                if (ListVatRate.Count == 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    inputParameters.Add("recordsOnPage", 500);
                    jQeury = GlobalVar.ErplyAPI.sendRequest("getVatRates", inputParameters);
                    //  string strVATResult = jQeury.ToString();  //测试所有搜索结果
                    inputParameters.Clear();

                    status = jQeury["status"];
                    if (status["responseStatus"].ToString() == "ok")
                    {
                        records = jQeury["records"];
                        foreach (var record in records)
                            ListVatRate.Add(new VatRate(record["id"].Value<int>(), record["name"].ToString()));
                    }
                }

                //获取所有被选中的POS
                selectedPosIDs.Clear();
                ListView.SelectedListViewItemCollection poses = listView_pos.SelectedItems;
                foreach (ListViewItem pos in poses)
                    selectedPosIDs.Add(int.Parse(pos.SubItems[0].Text));

                if (selectedPosIDs.Count == 0)
                {
                    MessageBox.Show("Please select at least one POS.");
                    searching = false;
                    this.Cursor = Cursors.Default;
                    return;
                }

                //获取所有被选中的warehouse
                selectedWarehouseIDs.Clear();
                ListView.SelectedListViewItemCollection selectedWarehouses = listView_wareHouse.SelectedItems;
                foreach (ListViewItem warehouse in selectedWarehouses)
                {
                    if (warehouse.SubItems[1].Text == "All warehouses")
                    {
                        selectedWarehouseIDs.Clear();
                        selectedWarehouseIDs.Add(9999);  //特殊ID 9999表示所有warehouse
                        break;
                    }
                    else
                        selectedWarehouseIDs.Add(int.Parse(warehouse.SubItems[0].Text));
                }

                dateTimeFrom = dateTime_From.Value;
                dateTimeTo = dateTime_To.Value;

                ListSearchResult.Clear();
                listView_result.Invalidate();

                button_Search.Text = "Stop searching";
                this.Cursor = Cursors.WaitCursor;
                listView_wareHouse.Enabled = false;
                listView_pos.Enabled = false;
                dateTime_From.Enabled = false;
                dateTime_To.Enabled = false;
                label_searchOngoing.Text = "";
                label_totalTransaction.Text = "";
                label_totalItems.Text = "";
                label_totalAmount.Text = "";
 
                 //Start search thread
                System.Threading.Thread searchThread = new System.Threading.Thread(PosSearch);
                searchThread.Start();
                //new System.Threading.Thread(PosSearch).Start();

                 //Start timer
                displayCount = 0;
                timer_progress.Interval = 1000;
                timer_progress.Start();

                /*searchThread.Join(); //Wait for possearch thread terminate
                searching= false;
                this.Cursor = Cursors.Default;*/
            }
            catch(Exception error)
            {
                this.Cursor = Cursors.Default;
                searching = false;
                MessageBox.Show(error.Message);
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            listView_wareHouse.Focus();
        }

        private void listView_wareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection wareHouses = listView_wareHouse.SelectedItems;
            listView_pos.Items.Clear();
            foreach (ListViewItem wareHouse in wareHouses)
            {
                string wareHouseName = wareHouse.SubItems[1].Text;
                if (wareHouseName == "All warehouses")
                {
                    //Show all POS
                    listView_pos.Sorting = SortOrder.Ascending;
                    foreach (var warehouseInfo in GlobalVar.ListWarehouseInfo)
                    {
                        foreach (var posInfo in warehouseInfo.ListPosInfo)
                        {
                            ListViewItem lvi_pos = new ListViewItem();
                            lvi_pos.Text = String.Format("{0:000}", posInfo.Key);
                            lvi_pos.SubItems.Add(posInfo.Value);
                            listView_pos.Items.Add(lvi_pos);
                        }
                    }
                    break;  //exit loop
                }
                else  //Only show POS in selected warehouses
                {
                    listView_pos.Sorting = SortOrder.None;
                    foreach (var warehouseInfo in GlobalVar.ListWarehouseInfo)
                    {
                        if (wareHouseName == warehouseInfo.WarehouseName)
                        {
                            foreach (var posInfo in warehouseInfo.ListPosInfo)
                            {
                                ListViewItem lvi_pos = new ListViewItem();
                                lvi_pos.Text = String.Format("{0:000}", posInfo.Key);
                                lvi_pos.SubItems.Add(posInfo.Value);
                                listView_pos.Items.Add(lvi_pos);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void listView_result_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= 0 && e.ItemIndex < ListSearchResult.Count)
            {
                SearchResult result = ListSearchResult.ElementAt(e.ItemIndex);
                ListViewItem lvi = new ListViewItem(); 	// create a listviewitem object
                lvi.Text = result.description;

                ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi1.Text = result.quantity;
                lvi.SubItems.Add(lvsi1); 			// assign subitem to item

                ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi2.Text = result.price;
                lvi.SubItems.Add(lvsi2); 			// assign subitem to item

                ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi3.Text = result.total;
                lvi.SubItems.Add(lvsi3); 			// assign subitem to item
                e.Item = lvi; 		// assign item to event argument's item-property
            }
            else
            {
                ListViewItem lvi = new ListViewItem(); 	// create a listviewitem object
                lvi.Text = "";

                ListViewItem.ListViewSubItem lvsi1 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi1.Text = "";
                lvi.SubItems.Add(lvsi1); 			// assign subitem to item

                ListViewItem.ListViewSubItem lvsi2 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi2.Text = "";
                lvi.SubItems.Add(lvsi2); 			// assign subitem to item

                ListViewItem.ListViewSubItem lvsi3 = new ListViewItem.ListViewSubItem(); // subitem
                lvsi3.Text = "";
                lvi.SubItems.Add(lvsi3); 			// assign subitem to item
                e.Item = lvi; 		// assign item to event argument's item-property
            }
        }

        private void PrintSaleReceipt(JToken record, DateTime dateTime, ref int totalItems, ref float totalAmount)
        {
            try
            {
                StringBuilder strLine = new StringBuilder();

                //Invoice number
                strLine.Append("Receipt number: ");
                strLine.Append(record["number"].ToString());
                ListSearchResult.Add(new SearchResult(strLine.ToString(), "", "", ""));

                //Date Time
                ListSearchResult.Add(new SearchResult(dateTime.ToString(), "", "", ""));
                //ListSearchResult.Add(new SearchResult());   //加空行

                //Ware house name
                strLine.Clear();
                strLine.Append("Warehouse name: ");
                strLine.Append(record["warehouseName"].ToString());
                ListSearchResult.Add(new SearchResult(strLine.ToString(), "", "", ""));

                //POS name
                strLine.Clear();
                strLine.Append("POS name: ");
                strLine.Append(record["pointOfSaleName"].ToString());
                ListSearchResult.Add(new SearchResult(strLine.ToString(), "", "", ""));

                //Customer 
                strLine.Clear();
               // ListSearchResult.Add(new SearchResult());   //加空行
                strLine.Append("Customer: ");
                strLine.Append(record["clientName"].ToString());
                ListSearchResult.Add(new SearchResult(strLine.ToString(), "", "", ""));

                //Seller
                strLine.Clear();
                strLine.Append("Seller: ");
                strLine.Append(record["employeeName"].ToString());
                ListSearchResult.Add(new SearchResult(strLine.ToString(), "", "", ""));

                //Items
                ListSearchResult.Add(new SearchResult());   //加空行
                ListSearchResult.Add(new SearchResult("Description", "Qty", "Price", "Total"));
                ListSearchResult.Add(new SearchResult("______________________________________",
                                                      "________", "________________", "_______"));
                var rows = record["rows"];
                foreach (var row in rows)
                {
                    SearchResult result = new SearchResult();
                    result.description = row["itemName"].ToString();
                    result.quantity= row["amount"].ToString();

                    if (!JsonExtensions.IsNullOrEmpty(row["amount"]))
                        totalItems += row["amount"].Value<int>();

                    if (!JsonExtensions.IsNullOrEmpty(row["finalPriceWithVAT"]))
                        result.price = String.Format("{0:0.00}", row["finalPriceWithVAT"].Value<float>());

                    if (!JsonExtensions.IsNullOrEmpty(row["rowTotal"]))
                        result.total = String.Format("{0:0.00}", row["rowTotal"].Value<float>());

                    ListSearchResult.Add(result);
                }
                ListSearchResult.Add(new SearchResult("______________________________________",
                                                      "________", "________________", "_______"));

                //Sub total
                strLine.Clear();
                if (!JsonExtensions.IsNullOrEmpty(record["total"]))
                    strLine.Append(String.Format("{0:0.00}", record["total"].Value<float>()));
                ListSearchResult.Add(new SearchResult("", "", "Sub total:", strLine.ToString()));

                ListSearchResult.Add(new SearchResult("",
                                                      "", "________________", "________"));

                //Net total
                strLine.Clear();
                if (!JsonExtensions.IsNullOrEmpty(record["netTotal"]))
                    strLine.Append(String.Format("{0:0.00}", record["netTotal"].Value<float>()));
                ListSearchResult.Add(new SearchResult("", "", "Net total:", strLine.ToString()));

                //GST total
                string gst = ""; ;
                strLine.Clear();
                strLine.Append("GST(");
                var vatTotals = record["vatTotalsByTaxRate"];
                int vatId;
                if (!JsonExtensions.IsNullOrEmpty(vatTotals[0]["vatrateID"]))
                {
                    vatId = vatTotals[0]["vatrateID"].Value<int>();
                    foreach (var vat in ListVatRate)
                    {
                        if (vatId == vat.Id)
                        {
                            strLine.Append(vat.VatRateName);
                            strLine.Append("):");
                            gst = strLine.ToString();
                            break;
                        }
                    }
                    strLine.Clear();
                    if (!JsonExtensions.IsNullOrEmpty(vatTotals[0]["total"]))
                        strLine.Append(String.Format("{0:0.00}", vatTotals[0]["total"].Value<float>()));
                }
                ListSearchResult.Add(new SearchResult("", "",  gst, strLine.ToString()));
                ListSearchResult.Add(new SearchResult("",
                                                      "", "________________", "________"));

                //Total and Paid
                strLine.Clear();
                if (!JsonExtensions.IsNullOrEmpty(record["total"]))
                {
                    strLine.Append(String.Format("{0:0.00}", record["total"].Value<float>()));
                    totalAmount += record["total"].Value<float>();
                }

                ListSearchResult.Add(new SearchResult("", "", "Total $:", strLine.ToString()));

                string payment;
                strLine.Clear();
                strLine.Append("Paid(");
                strLine.Append(record["paymentType"].ToString());
                strLine.Append("):");
                payment = strLine.ToString();
                strLine.Clear();
                var test2 = record["paid"];
                if (!JsonExtensions.IsNullOrEmpty(record["paid"]))
                    strLine.Append(String.Format("{0:0.00}", record["paid"].Value<float>()));
                ListSearchResult.Add(new SearchResult("", "", payment, strLine.ToString()));

                //notes
                if (!JsonExtensions.IsNullOrEmpty(record["notes"]))
                {
                    if (record["notes"].ToString() != "")
                        ListSearchResult.Add(new SearchResult(record["notes"].ToString(), "", "", ""));
                }

                ListSearchResult.Add(new SearchResult());   //加空行
                ListSearchResult.Add(new SearchResult());   //加空行
                ListSearchResult.Add(new SearchResult());   //加空行
                ListSearchResult.Add(new SearchResult());   //加空行
                ListSearchResult.Add(new SearchResult());   //加空行
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        public void PosSearch()
        {
            if (searching)  //avoid mistake (only one searching at one time)
                return;

            try
            {
                searching = true;
                int recordsTotal, recordsOnPage, pageNo;
                int nCount = 0;  //符合条件搜索结果计数
                int totalItems = 0; //卖出的总物品数
                float totalAmount = 0; //卖出的总金额
 
                JObject jQeury;
                JToken status, records;
                Dictionary<string, object> inputParameters = new Dictionary<string, object>();

                //获取搜索时间段所有的付款信息，并存放到相应的数据结构中
               /* inputParameters.Add("dateFrom", dateTimeFrom.Date.ToString("yyyy-MM-dd"));
                inputParameters.Add("dateTo", dateTimeTo.Date.ToString("yyyy-MM-dd"));
                jQeury = GlobalVar.ErplyAPI.sendRequest("getPayments", inputParameters);
                inputParameters.Clear(); */

                foreach (int warehouseID in selectedWarehouseIDs)
                {
                    recordsTotal = 0;
                    recordsOnPage = 0;
                    pageNo = 1;

                    if (searchCancelled)
                        break;

                    do
                    {
                        if (searchCancelled)
                            break;

                        inputParameters.Add("dateFrom", dateTimeFrom.Date.ToString("yyyy-MM-dd"));
                        inputParameters.Add("dateTo", dateTimeTo.Date.ToString("yyyy-MM-dd"));
                        if (warehouseID != 9999)
                            inputParameters.Add("warehouseID", warehouseID);
                        inputParameters.Add("getRowsForAllInvoices", 1);
                        inputParameters.Add("recordsOnPage", 100);
                        inputParameters.Add("pageNo", pageNo);

                        jQeury = GlobalVar.ErplyAPI.sendRequest("getSalesDocuments", inputParameters);
                        inputParameters.Clear();
                        // string strResult = jQeury.ToString();  //测试所有搜索结果
                        status = jQeury["status"];
                        if (status["responseStatus"].ToString() != "ok")  //Search失败
                        {
                            MessageBox.Show(String.Format("Search failed: error code {0}", status["errorCode"].Value<int>()));
                            searchCancelled = true;
                        }
                        else   //Search成功
                        {
                            if (recordsTotal == 0)
                                recordsTotal = status["recordsTotal"].Value<int>();

                            recordsOnPage += status["recordsInResponse"].Value<int>();

                            records = jQeury["records"];
                            foreach (var record in records.Reverse())
                            {
                                if (searchCancelled)
                                    break;

                                if (_stopSearchEvent.WaitOne(0) == true)
                                {
                                    _stopSearchEvent.Reset();
                                    searchCancelled = true; ;
                                }
                                else
                                {
                                    int PosID = record["pointOfSaleID"].Value<int>();
                                    bool posFound = false;
                                    foreach (int ID in selectedPosIDs)
                                    {
                                        if (PosID == ID)
                                        {
                                            posFound = true;
                                            break;
                                        }
                                    }
                                    if (posFound)   //符合搜索POS ID条件
                                    {
                                        StringBuilder strDateTime = new StringBuilder(record["date"].ToString());
                                        strDateTime.Append(" ");
                                        strDateTime.Append(record["time"].ToString());
                                        DateTime dateTime = Convert.ToDateTime(strDateTime.ToString());

                                        if (dateTime >= dateTimeFrom && dateTime <= dateTimeTo)  //符合时间日期条件
                                        {
                                            nCount++;
                                            PrintSaleReceipt(record, dateTime, ref totalItems, ref totalAmount);
                                        }
                                    }
                                }
                            }

                            //Refresh the result ListView and relative information
                            Action ListViewResultDelegate = () =>
                            {
                                listView_result.VirtualListSize = ListSearchResult.Count;
                                listView_result.Invalidate();
                                label_totalTransaction.Text = String.Format("{0} transactions found", nCount);
                                label_totalItems.Text = String.Format("Total items sold: {0}", totalItems);
                                label_totalAmount.Text = String.Format("Total amount sold: {0:C}", totalAmount);
                            };
                            this.Invoke(ListViewResultDelegate);
                        }
                        pageNo++;
                    }
                    while (recordsOnPage < recordsTotal);
                }

                searching = false;

                //Notify Mainform that search finished.
                Action SearchProgressDelegate = () =>
                {
                    MainForm.PostMessage(this.Handle, USER + 1, 0, 0);
                };
                this.Invoke(SearchProgressDelegate);

                //MessageBox放最后， 以免影响PostMessage.
                if (nCount == 0)
                    MessageBox.Show("No search result");
            }
            catch (Exception error)
            {
                Action noResultDelegate = () =>
                {
                    MessageBox.Show(error.Message);
                };
                this.Invoke(noResultDelegate);
             }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searching)
            {
                MessageBox.Show("POS search is ongoing, please stop searching at first");
                e.Cancel = true;
            }
        }

        private void timer_progress_Tick(object sender, EventArgs e)
        {
            string dotCount;
            switch (displayCount % 7)
            {
                case 0:
                    dotCount = "Searching      ";
                    break;
                case 1:
                    dotCount = "Searching.     ";
                    break;
                case 2:
                    dotCount = "Searching..    ";
                    break;
                case 3:
                    dotCount = "Searching...   ";
                    break;
                case 4:
                    dotCount = "Searching....  ";
                    break;
                case 5:
                    dotCount = "Searching..... ";
                    break;
                case 6:
                    dotCount = "Searching......";
                    break;
                default:
                    dotCount = "Searching      ";
                    break;
            }

            label_searchOngoing.Text = dotCount;
            displayCount++;
        }

        private void button_Search_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void button_Search_MouseLeave(object sender, EventArgs e)
        {
            if (searching)
                this.Cursor = Cursors.WaitCursor;
            else
                this.Cursor = Cursors.Default;
        }

        private void listView_wareHouse_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ListViewItem item = listView_wareHouse.SelectedItems[0];

                int wareHouseID;
                if (item.SubItems[0].Text != "")
                {
                    wareHouseID = Convert.ToInt32(item.SubItems[0].Text);
                    foreach (var warehouseInfo in GlobalVar.ListWarehouseInfo)
                    {
                        if (warehouseInfo.WarehouseID == wareHouseID)
                        {
                            if (warehouseInfo.address == "")
                            {
                                this.Cursor = Cursors.WaitCursor;
                                JObject jQeury;
                                JToken status;
                                Dictionary<string, object> inputParameters = new Dictionary<string, object>();

                                inputParameters.Add("warehouseID", wareHouseID);
                                jQeury = GlobalVar.ErplyAPI.sendRequest("getWarehouses", inputParameters);
                                inputParameters.Clear();
                                status = jQeury["status"];
                                if (status["responseStatus"].ToString() != "ok")  //Search失败
                                    MessageBox.Show(String.Format("Search warehouse ino failed: error code {0}", status["errorCode"].Value<int>()));
                                else   //Search成功
                                {
                                    var record = jQeury["records"][0];
                                    string address = record["address"].ToString();
                                    warehouseInfo.address = address;
                                }
                                this.Cursor = Cursors.Default;
                            }

                            //Show warehouse detail info dialog
                            WarehouseInfoForm info = new WarehouseInfoForm(Cursor.Position.X, Cursor.Position.Y, warehouseInfo.address);
                            info.ShowDialog();
                            break;
                        }
                    }
                }
                else  //Only show POS in selected warehouses
                {
                    MessageBox.Show("Sorry, can't get all warehouse information at one time");
                }
            }
        }
    }

    public class VatRate
    {
        public int Id;
        public string VatRateName;
        public VatRate(int id, string rateName)
        {
            this.Id = id;
            this.VatRateName = rateName;
        }
    };
}
