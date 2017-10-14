namespace barcode_printer
{
    partial class mainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrm));
            Janus.Windows.GridEX.GridEXLayout gridEX1_DesignTimeLayout = new Janus.Windows.GridEX.GridEXLayout();
            Janus.Windows.GridEX.GridEXLayout gridEX2_DesignTimeLayout = new Janus.Windows.GridEX.GridEXLayout();
            this.main_toolstrip = new System.Windows.Forms.ToolStrip();
            this.toolstrp_printtemple = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_codeprint = new System.Windows.Forms.ToolStripButton();
            this.manual_print = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_codecheck = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.page_up = new System.Windows.Forms.ToolStripButton();
            this.page_down = new System.Windows.Forms.ToolStripButton();
            this.tbJumpoffset = new System.Windows.Forms.ToolStripTextBox();
            this.jump_pageoffset = new System.Windows.Forms.ToolStripButton();
            this.print_minmax = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLanguageSwitch = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.serialport_iotrans = new System.IO.Ports.SerialPort(this.components);
            this.lblog = new System.Windows.Forms.Label();
            this.gridEX1 = new Janus.Windows.GridEX.GridEX();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbWarningMsg = new System.Windows.Forms.Label();
            this.btnCancelScan = new System.Windows.Forms.Button();
            this.gridEX2 = new Janus.Windows.GridEX.GridEX();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lbPrivate = new System.Windows.Forms.Label();
            this.lbLink = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbIN4 = new System.Windows.Forms.Label();
            this.lbIN3 = new System.Windows.Forms.Label();
            this.lbIN2 = new System.Windows.Forms.Label();
            this.lbIN1 = new System.Windows.Forms.Label();
            this.lbIN0 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbOUT4 = new System.Windows.Forms.Label();
            this.lbOUT3 = new System.Windows.Forms.Label();
            this.lbOUT2 = new System.Windows.Forms.Label();
            this.lbOUT1 = new System.Windows.Forms.Label();
            this.lbOUT0 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serialport_printer = new System.IO.Ports.SerialPort(this.components);
            this.timer_cheksn = new System.Windows.Forms.Timer(this.components);
            this.timer_check_ready = new System.Windows.Forms.Timer(this.components);
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.echo_msg1 = new barcode_printer.echo_msg();
            this.main_toolstrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEX1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEX2)).BeginInit();
            this.panelStatus.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // main_toolstrip
            // 
            this.main_toolstrip.BackColor = System.Drawing.SystemColors.Control;
            this.main_toolstrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.main_toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstrp_printtemple,
            this.toolstrip_codeprint,
            this.manual_print,
            this.toolstrip_codecheck,
            this.toolStripSeparator1,
            this.page_up,
            this.page_down,
            this.tbJumpoffset,
            this.jump_pageoffset,
            this.print_minmax,
            this.toolStripSeparator2,
            this.btnLanguageSwitch});
            resources.ApplyResources(this.main_toolstrip, "main_toolstrip");
            this.main_toolstrip.Name = "main_toolstrip";
            // 
            // toolstrp_printtemple
            // 
            this.toolstrp_printtemple.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolstrp_printtemple, "toolstrp_printtemple");
            this.toolstrp_printtemple.Name = "toolstrp_printtemple";
            this.toolstrp_printtemple.Click += new System.EventHandler(this.toolstrp_printtemple_Click);
            // 
            // toolstrip_codeprint
            // 
            this.toolstrip_codeprint.BackColor = System.Drawing.Color.Transparent;
            this.toolstrip_codeprint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolstrip_codeprint, "toolstrip_codeprint");
            this.toolstrip_codeprint.Name = "toolstrip_codeprint";
            this.toolstrip_codeprint.Click += new System.EventHandler(this.toolstrip_codeprint_Click);
            // 
            // manual_print
            // 
            this.manual_print.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.manual_print, "manual_print");
            this.manual_print.Name = "manual_print";
            this.manual_print.Click += new System.EventHandler(this.manual_print_Click);
            // 
            // toolstrip_codecheck
            // 
            this.toolstrip_codecheck.BackColor = System.Drawing.Color.Transparent;
            this.toolstrip_codecheck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolstrip_codecheck, "toolstrip_codecheck");
            this.toolstrip_codecheck.Name = "toolstrip_codecheck";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // page_up
            // 
            this.page_up.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.page_up, "page_up");
            this.page_up.Name = "page_up";
            this.page_up.Click += new System.EventHandler(this.page_up_Click);
            // 
            // page_down
            // 
            this.page_down.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.page_down, "page_down");
            this.page_down.Name = "page_down";
            this.page_down.Click += new System.EventHandler(this.page_down_Click);
            // 
            // tbJumpoffset
            // 
            this.tbJumpoffset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbJumpoffset.Name = "tbJumpoffset";
            resources.ApplyResources(this.tbJumpoffset, "tbJumpoffset");
            this.tbJumpoffset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbJumpoffset_KeyPress);
            // 
            // jump_pageoffset
            // 
            this.jump_pageoffset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.jump_pageoffset, "jump_pageoffset");
            this.jump_pageoffset.Name = "jump_pageoffset";
            this.jump_pageoffset.Click += new System.EventHandler(this.jump_pageoffset_Click);
            // 
            // print_minmax
            // 
            this.print_minmax.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.print_minmax, "print_minmax");
            this.print_minmax.Name = "print_minmax";
            this.print_minmax.Click += new System.EventHandler(this.print_minmax_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // btnLanguageSwitch
            // 
            this.btnLanguageSwitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.btnLanguageSwitch, "btnLanguageSwitch");
            this.btnLanguageSwitch.Name = "btnLanguageSwitch";
            this.btnLanguageSwitch.Click += new System.EventHandler(this.btnLanguageSwitch_Click);
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // serialport_iotrans
            // 
            this.serialport_iotrans.BaudRate = 115200;
            this.serialport_iotrans.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialport_iotrans_DataReceived);
            // 
            // lblog
            // 
            resources.ApplyResources(this.lblog, "lblog");
            this.lblog.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblog.Name = "lblog";
            // 
            // gridEX1
            // 
            this.gridEX1.AllowColumnDrag = false;
            resources.ApplyResources(this.gridEX1, "gridEX1");
            this.gridEX1.BorderStyle = Janus.Windows.GridEX.BorderStyle.Flat;
            resources.ApplyResources(gridEX1_DesignTimeLayout, "gridEX1_DesignTimeLayout");
            this.gridEX1.DesignTimeLayout = gridEX1_DesignTimeLayout;
            this.gridEX1.FocusStyle = Janus.Windows.GridEX.FocusStyle.Solid;
            this.gridEX1.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid;
            this.gridEX1.GroupByBoxVisible = false;
            this.gridEX1.Name = "gridEX1";
            this.gridEX1.RepeatHeaders = Janus.Windows.GridEX.InheritableBoolean.False;
            this.gridEX1.RowFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat;
            this.gridEX1.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.True;
            this.gridEX1.VisualStyle = Janus.Windows.GridEX.VisualStyle.VS2005;
            this.gridEX1.SizingColumn += new Janus.Windows.GridEX.SizingColumnEventHandler(this.gridEX1_SizingColumn);
            this.gridEX1.FormattingRow += new Janus.Windows.GridEX.RowLoadEventHandler(this.gridEX1_FormattingRow);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label3.Name = "label3";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.lbWarningMsg);
            this.panel3.Controls.Add(this.btnCancelScan);
            this.panel3.Name = "panel3";
            // 
            // lbWarningMsg
            // 
            resources.ApplyResources(this.lbWarningMsg, "lbWarningMsg");
            this.lbWarningMsg.AutoEllipsis = true;
            this.lbWarningMsg.ForeColor = System.Drawing.Color.Red;
            this.lbWarningMsg.Name = "lbWarningMsg";
            // 
            // btnCancelScan
            // 
            resources.ApplyResources(this.btnCancelScan, "btnCancelScan");
            this.btnCancelScan.Name = "btnCancelScan";
            this.btnCancelScan.UseVisualStyleBackColor = true;
            this.btnCancelScan.Click += new System.EventHandler(this.btnCancelScan_Click);
            // 
            // gridEX2
            // 
            resources.ApplyResources(this.gridEX2, "gridEX2");
            this.gridEX2.BorderStyle = Janus.Windows.GridEX.BorderStyle.Flat;
            resources.ApplyResources(gridEX2_DesignTimeLayout, "gridEX2_DesignTimeLayout");
            this.gridEX2.DesignTimeLayout = gridEX2_DesignTimeLayout;
            this.gridEX2.FocusStyle = Janus.Windows.GridEX.FocusStyle.Solid;
            this.gridEX2.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid;
            this.gridEX2.GroupByBoxVisible = false;
            this.gridEX2.Name = "gridEX2";
            this.gridEX2.VisualStyle = Janus.Windows.GridEX.VisualStyle.VS2005;
            // 
            // panelStatus
            // 
            resources.ApplyResources(this.panelStatus, "panelStatus");
            this.panelStatus.BackColor = System.Drawing.Color.White;
            this.panelStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStatus.Controls.Add(this.lbPrivate);
            this.panelStatus.Controls.Add(this.lbLink);
            this.panelStatus.Controls.Add(this.panel2);
            this.panelStatus.Controls.Add(this.panel1);
            this.panelStatus.Name = "panelStatus";
            // 
            // lbPrivate
            // 
            resources.ApplyResources(this.lbPrivate, "lbPrivate");
            this.lbPrivate.Name = "lbPrivate";
            // 
            // lbLink
            // 
            resources.ApplyResources(this.lbLink, "lbLink");
            this.lbLink.BackColor = System.Drawing.Color.Gray;
            this.lbLink.ForeColor = System.Drawing.Color.White;
            this.lbLink.Name = "lbLink";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.lbIN4);
            this.panel2.Controls.Add(this.lbIN3);
            this.panel2.Controls.Add(this.lbIN2);
            this.panel2.Controls.Add(this.lbIN1);
            this.panel2.Controls.Add(this.lbIN0);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Name = "panel2";
            // 
            // lbIN4
            // 
            resources.ApplyResources(this.lbIN4, "lbIN4");
            this.lbIN4.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbIN4.Name = "lbIN4";
            // 
            // lbIN3
            // 
            resources.ApplyResources(this.lbIN3, "lbIN3");
            this.lbIN3.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbIN3.Name = "lbIN3";
            // 
            // lbIN2
            // 
            resources.ApplyResources(this.lbIN2, "lbIN2");
            this.lbIN2.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbIN2.Name = "lbIN2";
            // 
            // lbIN1
            // 
            resources.ApplyResources(this.lbIN1, "lbIN1");
            this.lbIN1.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbIN1.Name = "lbIN1";
            // 
            // lbIN0
            // 
            resources.ApplyResources(this.lbIN0, "lbIN0");
            this.lbIN0.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbIN0.Name = "lbIN0";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.lbOUT4);
            this.panel1.Controls.Add(this.lbOUT3);
            this.panel1.Controls.Add(this.lbOUT2);
            this.panel1.Controls.Add(this.lbOUT1);
            this.panel1.Controls.Add(this.lbOUT0);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Name = "panel1";
            // 
            // lbOUT4
            // 
            resources.ApplyResources(this.lbOUT4, "lbOUT4");
            this.lbOUT4.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbOUT4.Name = "lbOUT4";
            // 
            // lbOUT3
            // 
            resources.ApplyResources(this.lbOUT3, "lbOUT3");
            this.lbOUT3.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbOUT3.Name = "lbOUT3";
            // 
            // lbOUT2
            // 
            resources.ApplyResources(this.lbOUT2, "lbOUT2");
            this.lbOUT2.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbOUT2.Name = "lbOUT2";
            // 
            // lbOUT1
            // 
            resources.ApplyResources(this.lbOUT1, "lbOUT1");
            this.lbOUT1.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbOUT1.Name = "lbOUT1";
            // 
            // lbOUT0
            // 
            resources.ApplyResources(this.lbOUT0, "lbOUT0");
            this.lbOUT0.Image = global::barcode_printer.Properties.Resources.gray;
            this.lbOUT0.Name = "lbOUT0";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // timer_cheksn
            // 
            this.timer_cheksn.Interval = 1000;
            this.timer_cheksn.Tick += new System.EventHandler(this.timer_cheksn_Tick);
            // 
            // timer_check_ready
            // 
            this.timer_check_ready.Enabled = true;
            this.timer_check_ready.Tick += new System.EventHandler(this.timer_check_ready_Tick);
            // 
            // timerClock
            // 
            this.timerClock.Interval = 50;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // echo_msg1
            // 
            resources.ApplyResources(this.echo_msg1, "echo_msg1");
            this.echo_msg1.BackColor = System.Drawing.Color.Transparent;
            this.echo_msg1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.echo_msg1.Name = "echo_msg1";
            // 
            // mainFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.echo_msg1);
            this.Controls.Add(this.gridEX1);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.gridEX2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblog);
            this.Controls.Add(this.main_toolstrip);
            this.KeyPreview = true;
            this.Name = "mainFrm";
            this.Load += new System.EventHandler(this.mainFrm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.Resize += new System.EventHandler(this.mainFrm_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mainFrm_KeyDown);
            this.main_toolstrip.ResumeLayout(false);
            this.main_toolstrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEX1)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridEX2)).EndInit();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip main_toolstrip;
        private System.Windows.Forms.ToolStripButton toolstrip_codeprint;
        private System.Windows.Forms.ToolStripButton toolstrip_codecheck;
        private System.Windows.Forms.ToolStripButton toolstrp_printtemple;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.IO.Ports.SerialPort serialport_iotrans;
        private System.Windows.Forms.ToolStripButton manual_print;
        private System.Windows.Forms.Label lblog;
        private Janus.Windows.GridEX.GridEX gridEX1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbWarningMsg;
        private System.Windows.Forms.Button btnCancelScan;
        private Janus.Windows.GridEX.GridEX gridEX2;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbIN4;
        private System.Windows.Forms.Label lbIN3;
        private System.Windows.Forms.Label lbIN2;
        private System.Windows.Forms.Label lbIN1;
        private System.Windows.Forms.Label lbIN0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbOUT4;
        private System.Windows.Forms.Label lbOUT3;
        private System.Windows.Forms.Label lbOUT2;
        private System.Windows.Forms.Label lbOUT1;
        private System.Windows.Forms.Label lbOUT0;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton page_down;
        private System.Windows.Forms.ToolStripButton page_up;
        private System.IO.Ports.SerialPort serialport_printer;
        private System.Windows.Forms.Timer timer_cheksn;
        private System.Windows.Forms.ToolStripTextBox tbJumpoffset;
        private System.Windows.Forms.ToolStripButton jump_pageoffset;
        private echo_msg echo_msg1;
        private System.Windows.Forms.Timer timer_check_ready;
        private System.Windows.Forms.Label lbLink;
        private System.Windows.Forms.ToolStripButton print_minmax;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.Label lbPrivate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnLanguageSwitch;
    }
}

