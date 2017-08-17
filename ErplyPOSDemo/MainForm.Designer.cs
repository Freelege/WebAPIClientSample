namespace ErplyPOSDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label5 = new System.Windows.Forms.Label();
            this.button_Search = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTime_To = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTime_From = new System.Windows.Forms.DateTimePicker();
            this.listView_result = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.listView_wareHouse = new System.Windows.Forms.ListView();
            this.listView_pos = new System.Windows.Forms.ListView();
            this.timer_progress = new System.Windows.Forms.Timer(this.components);
            this.label_searchOngoing = new System.Windows.Forms.Label();
            this.label_totalTransaction = new System.Windows.Forms.Label();
            this.label_totalItems = new System.Windows.Forms.Label();
            this.label_totalAmount = new System.Windows.Forms.Label();
            this.label_userName = new System.Windows.Forms.Label();
            this.label_logonTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 807);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "To:";
            // 
            // button_Search
            // 
            this.button_Search.Location = new System.Drawing.Point(359, 769);
            this.button_Search.Name = "button_Search";
            this.button_Search.Size = new System.Drawing.Size(161, 60);
            this.button_Search.TabIndex = 12;
            this.button_Search.Text = "Search";
            this.button_Search.UseVisualStyleBackColor = true;
            this.button_Search.Click += new System.EventHandler(this.button_Search_Click);
            this.button_Search.MouseLeave += new System.EventHandler(this.button_Search_MouseLeave);
            this.button_Search.MouseHover += new System.EventHandler(this.button_Search_MouseHover);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 773);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "From:";
            // 
            // dateTime_To
            // 
            this.dateTime_To.CustomFormat = "dd-MM-yyyy HH:mm:ss";
            this.dateTime_To.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTime_To.Location = new System.Drawing.Point(54, 804);
            this.dateTime_To.Name = "dateTime_To";
            this.dateTime_To.Size = new System.Drawing.Size(211, 25);
            this.dateTime_To.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 377);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(127, 15);
            this.label7.TabIndex = 16;
            this.label7.Text = "Point of sales:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Warehouses:";
            // 
            // dateTime_From
            // 
            this.dateTime_From.CustomFormat = "dd-MM-yyyy HH:mm:ss ";
            this.dateTime_From.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTime_From.Location = new System.Drawing.Point(54, 769);
            this.dateTime_From.Name = "dateTime_From";
            this.dateTime_From.Size = new System.Drawing.Size(211, 25);
            this.dateTime_From.TabIndex = 10;
            // 
            // listView_result
            // 
            this.listView_result.Location = new System.Drawing.Point(359, 37);
            this.listView_result.Name = "listView_result";
            this.listView_result.Size = new System.Drawing.Size(639, 677);
            this.listView_result.TabIndex = 17;
            this.listView_result.UseCompatibleStateImageBehavior = false;
            this.listView_result.View = System.Windows.Forms.View.Details;
            this.listView_result.VirtualMode = true;
            this.listView_result.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView_result_RetrieveVirtualItem);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(356, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Search results:";
            // 
            // listView_wareHouse
            // 
            this.listView_wareHouse.FullRowSelect = true;
            this.listView_wareHouse.GridLines = true;
            this.listView_wareHouse.Location = new System.Drawing.Point(10, 37);
            this.listView_wareHouse.Name = "listView_wareHouse";
            this.listView_wareHouse.Size = new System.Drawing.Size(310, 320);
            this.listView_wareHouse.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_wareHouse.TabIndex = 20;
            this.listView_wareHouse.UseCompatibleStateImageBehavior = false;
            this.listView_wareHouse.View = System.Windows.Forms.View.Details;
            this.listView_wareHouse.SelectedIndexChanged += new System.EventHandler(this.listView_wareHouse_SelectedIndexChanged);
            this.listView_wareHouse.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_wareHouse_MouseClick);
            // 
            // listView_pos
            // 
            this.listView_pos.FullRowSelect = true;
            this.listView_pos.GridLines = true;
            this.listView_pos.Location = new System.Drawing.Point(10, 395);
            this.listView_pos.Name = "listView_pos";
            this.listView_pos.Size = new System.Drawing.Size(310, 320);
            this.listView_pos.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_pos.TabIndex = 21;
            this.listView_pos.UseCompatibleStateImageBehavior = false;
            this.listView_pos.View = System.Windows.Forms.View.Details;
            // 
            // timer_progress
            // 
            this.timer_progress.Tick += new System.EventHandler(this.timer_progress_Tick);
            // 
            // label_searchOngoing
            // 
            this.label_searchOngoing.AutoSize = true;
            this.label_searchOngoing.Location = new System.Drawing.Point(356, 731);
            this.label_searchOngoing.Name = "label_searchOngoing";
            this.label_searchOngoing.Size = new System.Drawing.Size(127, 15);
            this.label_searchOngoing.TabIndex = 24;
            this.label_searchOngoing.Text = "Search Oongoing";
            // 
            // label_totalTransaction
            // 
            this.label_totalTransaction.AutoSize = true;
            this.label_totalTransaction.Location = new System.Drawing.Point(617, 731);
            this.label_totalTransaction.Name = "label_totalTransaction";
            this.label_totalTransaction.Size = new System.Drawing.Size(151, 15);
            this.label_totalTransaction.TabIndex = 19;
            this.label_totalTransaction.Text = "Total transactions";
            // 
            // label_totalItems
            // 
            this.label_totalItems.AutoSize = true;
            this.label_totalItems.Location = new System.Drawing.Point(617, 756);
            this.label_totalItems.Name = "label_totalItems";
            this.label_totalItems.Size = new System.Drawing.Size(95, 15);
            this.label_totalItems.TabIndex = 25;
            this.label_totalItems.Text = "Total Items";
            // 
            // label_totalAmount
            // 
            this.label_totalAmount.AutoSize = true;
            this.label_totalAmount.Location = new System.Drawing.Point(617, 781);
            this.label_totalAmount.Name = "label_totalAmount";
            this.label_totalAmount.Size = new System.Drawing.Size(103, 15);
            this.label_totalAmount.TabIndex = 26;
            this.label_totalAmount.Text = "Total Amount";
            // 
            // label_userName
            // 
            this.label_userName.AutoSize = true;
            this.label_userName.Location = new System.Drawing.Point(617, 806);
            this.label_userName.Name = "label_userName";
            this.label_userName.Size = new System.Drawing.Size(79, 15);
            this.label_userName.TabIndex = 27;
            this.label_userName.Text = "User name";
            // 
            // label_logonTime
            // 
            this.label_logonTime.AutoSize = true;
            this.label_logonTime.Location = new System.Drawing.Point(617, 831);
            this.label_logonTime.Name = "label_logonTime";
            this.label_logonTime.Size = new System.Drawing.Size(87, 15);
            this.label_logonTime.TabIndex = 28;
            this.label_logonTime.Text = "Logon time";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 858);
            this.Controls.Add(this.label_logonTime);
            this.Controls.Add(this.label_userName);
            this.Controls.Add(this.label_totalAmount);
            this.Controls.Add(this.label_totalItems);
            this.Controls.Add(this.label_searchOngoing);
            this.Controls.Add(this.listView_pos);
            this.Controls.Add(this.listView_wareHouse);
            this.Controls.Add(this.label_totalTransaction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView_result);
            this.Controls.Add(this.dateTime_From);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_Search);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTime_To);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Text = "Erply POS";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_Search;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTime_To;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTime_From;
        private System.Windows.Forms.ListView listView_result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView_wareHouse;
        private System.Windows.Forms.ListView listView_pos;
        private System.Windows.Forms.Timer timer_progress;
        private System.Windows.Forms.Label label_searchOngoing;
        private System.Windows.Forms.Label label_totalTransaction;
        private System.Windows.Forms.Label label_totalItems;
        private System.Windows.Forms.Label label_totalAmount;
        private System.Windows.Forms.Label label_userName;
        private System.Windows.Forms.Label label_logonTime;
    }
}