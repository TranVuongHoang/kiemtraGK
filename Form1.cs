using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Linq;
using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace De02
{

    public partial class Form1 : Form
    {
        private ProductBUS productBUS;
        private bool isDataModified = false;
        public Form1()
        {
            InitializeComponent();
            productBUS = new ProductBUS();
        }
        private void SetControlState()
        {
            btnLuu.Enabled = isDataModified;
            btnKLuu.Enabled = isDataModified;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dgvSP.Columns.Add("MaSP", "Mã Sản Phẩm");
            dgvSP.Columns.Add("TenSP", "Tên Sản Phẩm");
            dgvSP.Columns.Add("NgayNhap", "Ngày Nhập");
            dgvSP.Columns.Add("LoaiSP", "Loại Sản Phẩm");

            // Sau đó tải danh sách sản phẩm
            LoadProductDataGridView();
            LoadProductDataGridView();
            LoadCategoryComboBox();
            SetControlState();
        }
        private void LoadProductDataGridView()
        {
            List<Sanpham> products = productBUS.GetAllProducts();
            dgvSP.Rows.Clear();

            foreach (Sanpham product in products)
            {
                int index = dgvSP.Rows.Add();
                dgvSP.Rows[index].Cells["MaSP"].Value = product.MaSP;
                dgvSP.Rows[index].Cells["TenSP"].Value = product.TenSP;
                dgvSP.Rows[index].Cells["NgayNhap"].Value = product.NgayNhap != null ? product.NgayNhap.Value.ToString("dd/MM/yyyy") : "";
                dgvSP.Rows[index].Cells["LoaiSP"].Value = product.LoaiSP != null ? product.LoaiSP.TenLoai : "";
            }
        }
        private void LoadCategoryComboBox()
        {
            List<LoaiSP> categories = productBUS.GetAllCategories();
            cbLoaiSP.DataSource = categories;
            cbLoaiSP.DisplayMember = "TenLoai";
            cbLoaiSP.ValueMember = "MaLoai";
        }

        private void dgvSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSP.Rows[e.RowIndex];
                txtMaSP.Text = selectedRow.Cells["MaSP"].Value?.ToString() ?? "";
                txtTenSP.Text = selectedRow.Cells["TenSP"].Value?.ToString() ?? "";
                dtpNgayNhap.Value = DateTime.TryParse(selectedRow.Cells["NgayNhap"].Value?.ToString(), out DateTime date) ? date : DateTime.Now;
                cbLoaiSP.Text = selectedRow.Cells["LoaiSP"].Value?.ToString() ?? "";
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult result = MessageBox.Show(
                    "Bạn muốn thêm sản phẩm này?",
                    "Đã thêm thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvSP.Rows.Add(txtMaSP.Text, txtTenSP.Text, dtpNgayNhap.Value.ToString("dd/MM/yyyy"), cbLoaiSP.Text);
                    isDataModified = true;
                    SetControlState();
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (ValidateInput() && dgvSP.CurrentRow != null)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn muốn sửa sản phẩm này?",
                    "Đã sửa thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DataGridViewRow row = dgvSP.CurrentRow;
                    row.Cells["MaSP"].Value = txtMaSP.Text;
                    row.Cells["TenSP"].Value = txtTenSP.Text;
                    row.Cells["NgayNhap"].Value = dtpNgayNhap.Value.ToString("dd/MM/yyyy");
                    row.Cells["LoaiSP"].Value = cbLoaiSP.Text;

                    isDataModified = true;
                    SetControlState();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSP.CurrentRow != null && !dgvSP.CurrentRow.IsNewRow)
            {
                // Hiển thị hộp thoại xác nhận xóa
                DialogResult result = MessageBox.Show(
                    "Bạn muốn xóa sản phẩm này?",
                    "Xác xóa thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // Nếu người dùng chọn "Yes", tiến hành xóa
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string maSP = dgvSP.CurrentRow.Cells["MaSP"].Value?.ToString();
                        if (!string.IsNullOrEmpty(maSP))
                        {
                            // Xóa sản phẩm khỏi cơ sở dữ liệu thông qua BUS
                            productBUS.DeleteProduct(maSP);

                            // Xóa sản phẩm khỏi DataGridView
                            dgvSP.Rows.Remove(dgvSP.CurrentRow);

                            // Cập nhật trạng thái
                            isDataModified = true;
                            SetControlState();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Không được xóa dòng trống hoặc đã lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
       "Bạn có muốn lưu các thay đổi?",
       "Đã lưu",
       MessageBoxButtons.YesNo,
       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvSP.Rows)
                {
                    if (row.IsNewRow) continue;

                    string maSP = row.Cells["MaSP"].Value?.ToString();
                    string tenSP = row.Cells["TenSP"].Value?.ToString();
                    if (!DateTime.TryParse(row.Cells["NgayNhap"].Value?.ToString(), out DateTime ngayNhap))
                    {
                        ngayNhap = DateTime.Now;
                    }
                    string loaiSP = row.Cells["LoaiSP"].Value?.ToString();

                    Sanpham product = new Sanpham
                    {
                        MaSP = maSP,
                        TenSP = tenSP,
                        NgayNhap = ngayNhap,
                        MaLoai = productBUS.GetCategoryIdByName(loaiSP)
                    };

                    productBUS.AddOrUpdateProduct(product);
                }

                isDataModified = false;
                SetControlState();
                LoadProductDataGridView();
            }
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn muốn bỏ qua các thay đổi?",
        "Không lưu thành công",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LoadProductDataGridView();
                isDataModified = false;
                SetControlState();
            }
        }
        private bool ValidateInput()
        {
            err.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                err.SetError(txtMaSP, "Mã sp không được trống.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                err.SetError(txtTenSP, "Tên sp không được trống.");
                isValid = false;
            }
            if (cbLoaiSP.SelectedIndex == -1)
            {
                err.SetError(cbLoaiSP, "Vui lòng chọn loại sp.");
                isValid = false;
            }

            return isValid;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (isDataModified)
            {
                DialogResult result = MessageBox.Show(
                    "Dữ liệu chưa lưu. Bạn có muốn thoát?",
                    "Đã thoát thành công ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có thực sự muốn thoát?",
                    "Xác nhận thoát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tenSP = txtTim.Text.Trim();

            if (!string.IsNullOrEmpty(tenSP))
            {
                List<Sanpham> products = productBUS.GetProductsByName(tenSP);

                if (products != null && products.Count > 0)
                {
                    dgvSP.Rows.Clear();

                    foreach (Sanpham product in products)
                    {
                        int index = dgvSP.Rows.Add();
                        dgvSP.Rows[index].Cells["MaSP"].Value = product.MaSP;
                        dgvSP.Rows[index].Cells["TenSP"].Value = product.TenSP;
                        dgvSP.Rows[index].Cells["NgayNhap"].Value = product.NgayNhap != null ? product.NgayNhap.Value.ToString("dd/MM/yyyy") : "";
                        dgvSP.Rows[index].Cells["LoaiSP"].Value = product.LoaiSP != null ? product.LoaiSP.TenLoai : "";
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
