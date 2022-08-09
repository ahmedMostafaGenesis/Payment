using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ingenico.GUI
{




	public class ImgRcptViewer : Form
	{
		private List<byte[]> m_merchantCopyImg;

		private List<byte[]> m_customerCopyImg;

		private List<byte[]> m_formattedRcptCopyImg;

		private PictureBox merchantPictureBox;

		private PictureBox custPictureBox;

		private PictureBox formattedRcptPictureBox;

		private int merchIndex;

		private int custIndex;

		private int rcptIndex;

		private const int merchTabPageIndex = 0;

		private const int custTabPageIndex = 1;

		private const int rcptTabPageIndex = 0;

		public string formattedRcptName;

		private IContainer components;

		private TabControl tabControlReceipt;

		private TabPage tabPageMerchant;

		private TabPage tabPageCustomer;

		private TabPage tabPageFormattedRcpt;

		private Button nextBtn;

		private Button saveButton;

		private Button PreviousBtn;

		public List<byte[]> MerchantCopyImg
		{
			get { return m_merchantCopyImg; }
			set { m_merchantCopyImg = value; }
		}

		public List<byte[]> CustomerCopyImg
		{
			get { return m_customerCopyImg; }
			set { m_customerCopyImg = value; }
		}

		public List<byte[]> FormattedRcptCopyImg
		{
			get { return m_formattedRcptCopyImg; }
			set { m_formattedRcptCopyImg = value; }
		}

		public ImgRcptViewer()
		{
			InitializeComponent();
		}

		private void DrawImage(ref PictureBox imgPictureBox, List<byte[]> listImg, int imgIndex, int selectedPageIndex)
		{
			if (imgPictureBox != null)
			{
				imgPictureBox.Hide();
			}

			imgPictureBox = new PictureBox();
			imgPictureBox.Size = new Size(410, 1000);
			tabControlReceipt.TabPages[selectedPageIndex].Controls.Add(imgPictureBox);
			tabControlReceipt.SelectedIndex = selectedPageIndex;
			using MemoryStream stream = new MemoryStream(listImg[imgIndex]);
			Image image = Image.FromStream(stream);
			if (image.Size.Height > tabControlReceipt.TabPages[selectedPageIndex].Height)
			{
				tabControlReceipt.TabPages[selectedPageIndex].AutoScroll = true;
			}

			imgPictureBox.Show();
			imgPictureBox.Image = image;
		}

		private void ImgRcpt_Load(object sender, EventArgs e)
		{
			if (m_merchantCopyImg != null && m_merchantCopyImg.Count != 0)
			{
				DrawImage(ref merchantPictureBox, m_merchantCopyImg, merchIndex, 0);
			}
			else
			{
				tabControlReceipt.TabPages.Remove(tabPageMerchant);
			}

			if (m_customerCopyImg != null && m_customerCopyImg.Count != 0)
			{
				if (tabControlReceipt.TabCount > 1)
				{
					DrawImage(ref custPictureBox, m_customerCopyImg, custIndex, 1);
				}
				else
				{
					DrawImage(ref custPictureBox, m_customerCopyImg, custIndex, 0);
				}
			}
			else
			{
				tabControlReceipt.TabPages.Remove(tabPageCustomer);
			}

			if (m_formattedRcptCopyImg != null && m_formattedRcptCopyImg.Count != 0)
			{
				tabControlReceipt.Controls.Add(tabPageFormattedRcpt);
				tabPageFormattedRcpt.Text = formattedRcptName;
				nextBtn.Visible = false;
				PreviousBtn.Visible = false;
				DrawImage(ref formattedRcptPictureBox, m_formattedRcptCopyImg, rcptIndex, 0);
			}
			else
			{
				tabControlReceipt.TabPages.Remove(tabPageFormattedRcpt);
			}
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			saveFileDialog.Title = "Save Receipt ...";
			saveFileDialog.Filter = "PNG files(*.png; *.PNG)| *.png; *.PNG";
			if (tabControlReceipt.SelectedTab == tabPageCustomer)
			{
				for (int i = 0; i < m_customerCopyImg.Count; i++)
				{
					saveFileDialog.FileName = "CustomerReceipt_" + (i + 1);
					if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
					{
						string fileName = saveFileDialog.FileName;
						if (File.Exists(fileName))
						{
							File.Delete(fileName);
						}

						FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
						fileStream.Write(m_customerCopyImg[i], 0, m_customerCopyImg[i].Length);
						fileStream.Close();
					}
				}

				return;
			}

			if (tabControlReceipt.SelectedTab == tabPageMerchant)
			{
				for (int j = 0; j < m_merchantCopyImg.Count; j++)
				{
					saveFileDialog.FileName = "MerchantReceipt_" + (j + 1);
					if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
					{
						string fileName2 = saveFileDialog.FileName;
						if (File.Exists(fileName2))
						{
							File.Delete(fileName2);
						}

						FileStream fileStream2 = new FileStream(fileName2, FileMode.Create, FileAccess.Write);
						fileStream2.Write(m_merchantCopyImg[j], 0, m_merchantCopyImg[j].Length);
						fileStream2.Close();
					}
				}

				return;
			}

			for (int k = 0; k < m_formattedRcptCopyImg.Count; k++)
			{
				saveFileDialog.FileName = "RqstFormatReceipt_1";
				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					string fileName3 = saveFileDialog.FileName;
					if (File.Exists(fileName3))
					{
						File.Delete(fileName3);
					}

					FileStream fileStream3 = new FileStream(fileName3, FileMode.Create, FileAccess.Write);
					fileStream3.Write(m_formattedRcptCopyImg[k], 0, m_formattedRcptCopyImg[k].Length);
					fileStream3.Close();
				}
			}
		}

		private void nextBtn_Click(object sender, EventArgs e)
		{
			if (tabControlReceipt.SelectedIndex == 0)
			{
				merchIndex++;
				if (merchIndex < m_merchantCopyImg.Count && m_merchantCopyImg[merchIndex] != null)
				{
					DrawImage(ref merchantPictureBox, m_merchantCopyImg, merchIndex, 0);
				}
				else
				{
					merchIndex = m_merchantCopyImg.Count - 1;
				}
			}

			if (tabControlReceipt.SelectedIndex == 1)
			{
				custIndex++;
				if (custIndex < m_customerCopyImg.Count && m_customerCopyImg[custIndex] != null)
				{
					DrawImage(ref custPictureBox, m_customerCopyImg, custIndex, 1);
				}
				else
				{
					custIndex = m_customerCopyImg.Count - 1;
				}
			}
		}

		private void PreviousBtn_Click(object sender, EventArgs e)
		{
			if (tabControlReceipt.SelectedIndex == 0 && merchIndex > 0)
			{
				merchIndex--;
				if (merchIndex < m_merchantCopyImg.Count && m_merchantCopyImg[merchIndex] != null)
				{
					DrawImage(ref merchantPictureBox, m_merchantCopyImg, merchIndex, 0);
				}
			}

			if (tabControlReceipt.SelectedIndex == 1 && custIndex > 0)
			{
				custIndex--;
				if (custIndex < m_customerCopyImg.Count && m_customerCopyImg[custIndex] != null)
				{
					DrawImage(ref custPictureBox, m_customerCopyImg, custIndex, 1);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tabControlReceipt = new System.Windows.Forms.TabControl();
			this.tabPageMerchant = new System.Windows.Forms.TabPage();
			this.tabPageCustomer = new System.Windows.Forms.TabPage();
			this.tabPageFormattedRcpt = new System.Windows.Forms.TabPage();
			this.nextBtn = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.PreviousBtn = new System.Windows.Forms.Button();
			this.tabControlReceipt.SuspendLayout();
			base.SuspendLayout();
			this.tabControlReceipt.Controls.Add(this.tabPageMerchant);
			this.tabControlReceipt.Controls.Add(this.tabPageCustomer);
			this.tabControlReceipt.Location = new System.Drawing.Point(11, 3);
			this.tabControlReceipt.Name = "tabControlReceipt";
			this.tabControlReceipt.SelectedIndex = 0;
			this.tabControlReceipt.Size = new System.Drawing.Size(410, 927);
			this.tabControlReceipt.TabIndex = 0;
			this.tabPageMerchant.Location = new System.Drawing.Point(4, 22);
			this.tabPageMerchant.Name = "tabPageMerchant";
			this.tabPageMerchant.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMerchant.Size = new System.Drawing.Size(402, 901);
			this.tabPageMerchant.TabIndex = 0;
			this.tabPageMerchant.Text = "Merchant Copy";
			this.tabPageMerchant.UseVisualStyleBackColor = true;
			this.tabPageCustomer.Location = new System.Drawing.Point(4, 22);
			this.tabPageCustomer.Name = "tabPageCustomer";
			this.tabPageCustomer.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCustomer.Size = new System.Drawing.Size(402, 901);
			this.tabPageCustomer.TabIndex = 1;
			this.tabPageCustomer.Text = "Customer Copy";
			this.tabPageCustomer.UseVisualStyleBackColor = true;
			this.tabPageFormattedRcpt.Location = new System.Drawing.Point(4, 22);
			this.tabPageFormattedRcpt.Name = "tabPageFormattedRcpt";
			this.tabPageCustomer.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCustomer.Size = new System.Drawing.Size(402, 901);
			this.tabPageFormattedRcpt.TabIndex = 2;
			this.tabPageFormattedRcpt.UseVisualStyleBackColor = true;
			this.nextBtn.Location = new System.Drawing.Point(107, 933);
			this.nextBtn.Name = "nextBtn";
			this.nextBtn.Size = new System.Drawing.Size(75, 23);
			this.nextBtn.TabIndex = 1;
			this.nextBtn.Text = "Next";
			this.nextBtn.UseVisualStyleBackColor = true;
			this.nextBtn.Click += new System.EventHandler(nextBtn_Click);
			this.saveButton.Location = new System.Drawing.Point(319, 933);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 2;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(saveButton_Click);
			this.PreviousBtn.Location = new System.Drawing.Point(17, 933);
			this.PreviousBtn.Name = "PreviousBtn";
			this.PreviousBtn.Size = new System.Drawing.Size(75, 23);
			this.PreviousBtn.TabIndex = 3;
			this.PreviousBtn.Text = "Previous";
			this.PreviousBtn.UseVisualStyleBackColor = true;
			this.PreviousBtn.Click += new System.EventHandler(PreviousBtn_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.ClientSize = new System.Drawing.Size(434, 962);
			base.Controls.Add(this.PreviousBtn);
			base.Controls.Add(this.saveButton);
			base.Controls.Add(this.nextBtn);
			base.Controls.Add(this.tabControlReceipt);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImgRcptViewer";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Viewer";
			base.Load += new System.EventHandler(ImgRcpt_Load);
			this.tabControlReceipt.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}