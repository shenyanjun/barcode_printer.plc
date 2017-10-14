namespace barcode_printer {
    partial class echo_msg {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(echo_msg));
            this.lbMsg = new System.Windows.Forms.Label();
            this.timer_cleanup = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbMsg
            // 
            resources.ApplyResources(this.lbMsg, "lbMsg");
            this.lbMsg.AutoEllipsis = true;
            this.lbMsg.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbMsg.Name = "lbMsg";
            // 
            // timer_cleanup
            // 
            this.timer_cleanup.Enabled = true;
            this.timer_cleanup.Interval = 3000;
            this.timer_cleanup.Tick += new System.EventHandler(this.timer_cleanup_Tick);
            // 
            // echo_msg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbMsg);
            this.Name = "echo_msg";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.Timer timer_cleanup;
    }
}
