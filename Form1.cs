using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SchoolDB
{

    public partial class Form1 : Form
    {
        private BindingSource bindingSource;
        public Form1()
        {
            InitializeComponent();
            bindingSource = new BindingSource();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ và Tên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Vui lòng nhập tuổi hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMajor.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn Ngành học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nếu tất cả dữ liệu hợp lệ, thêm sinh viên vào cơ sở dữ liệu
            using (var context = new SchoolContext())
            {
                try
                {
                    var student = new Student
                    {
                        FullName = txtFullName.Text,
                        Age = age,
                        Major = cmbMajor.SelectedItem.ToString()
                    };

                    context.Students.Add(student);
                    context.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu

                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Sau khi thêm, tải lại danh sách sinh viên để cập nhật DataGridView
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Xóa các trường nhập liệu sau khi thêm thành công
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            txtFullName.Clear();
            txtAge.Clear();
            cmbMajor.SelectedIndex = -1;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolContext())
            {
                // Kiểm tra nếu có hàng được chọn trong DataGridView
                if (dataGridViewStudents.SelectedRows.Count > 0)
                {
                    // Lấy StudentId từ hàng được chọn
                    int studentId = (int)dataGridViewStudents.SelectedRows[0].Cells["StudentId"].Value;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    var student = context.Students.FirstOrDefault(s => s.StudentId == studentId);

                    if (student != null)
                    {
                        // Xác nhận trước khi xóa
                        DialogResult result = MessageBox.Show(
                            $"Bạn có chắc chắn muốn xóa sinh viên {student.FullName} không?",
                            "Xác nhận xóa",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result == DialogResult.Yes)
                        {
                            // Xóa sinh viên nếu người dùng xác nhận
                            context.Students.Remove(student);
                            context.SaveChanges();

                            // Tải lại danh sách sinh viên sau khi xóa
                            LoadStudents();

                            // Hiển thị thông báo xóa thành công
                            MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        // Hiển thị thông báo nếu không tìm thấy sinh viên
                        MessageBox.Show("Không tìm thấy sinh viên trong cơ sở dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Hiển thị thông báo nếu không có hàng nào được chọn
                    MessageBox.Show("Vui lòng chọn một sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolContext())
            {
                if (dataGridViewStudents.SelectedRows.Count > 0)
                {
                    int studentId = (int)dataGridViewStudents.SelectedRows[0].Cells[0].Value;
                    var student = context.Students.FirstOrDefault(s => s.StudentId == studentId);

                    if (student != null)
                    {
                        student.FullName = txtFullName.Text;
                        student.Age = int.Parse(txtAge.Text);
                        student.Major = cmbMajor.SelectedItem.ToString();

                        context.SaveChanges();
                        LoadStudents();
                    }
                }
            }
        }
        private void LoadStudents()
        {
            using (var context = new SchoolContext())
            {
                var students = context.Students.ToList();
                dataGridViewStudents.DataSource = students;
            }
        }
        private void ConfigureDataGridView()
        {
            // Xóa các cột cũ
            dataGridViewStudents.Columns.Clear();

            // Cột StudentId
            DataGridViewTextBoxColumn colStudentId = new DataGridViewTextBoxColumn();
            colStudentId.Name = "StudentId";
            colStudentId.HeaderText = "Mã Sinh Viên";
            colStudentId.DataPropertyName = "StudentId";  // Liên kết với thuộc tính "StudentId" của đối tượng Student
            dataGridViewStudents.Columns.Add(colStudentId);

            // Cột FullName
            DataGridViewTextBoxColumn colFullName = new DataGridViewTextBoxColumn();
            colFullName.Name = "FullName";
            colFullName.HeaderText = "Họ và Tên";
            colFullName.DataPropertyName = "FullName";  // Liên kết với thuộc tính "FullName"
            dataGridViewStudents.Columns.Add(colFullName);

            // Cột Age
            DataGridViewTextBoxColumn colAge = new DataGridViewTextBoxColumn();
            colAge.Name = "Age";
            colAge.HeaderText = "Tuổi";
            colAge.DataPropertyName = "Age";  // Liên kết với thuộc tính "Age"
            dataGridViewStudents.Columns.Add(colAge);

            // Cột Major
            DataGridViewTextBoxColumn colMajor = new DataGridViewTextBoxColumn();
            colMajor.Name = "Major";
            colMajor.HeaderText = "Ngành Học";
            colMajor.DataPropertyName = "Major";  // Liên kết với thuộc tính "Major"
            dataGridViewStudents.Columns.Add(colMajor);
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.CurrentRow != null)
            {
                // Hiển thị thông tin sinh viên trong các TextBox
                txtFullName.Text = dataGridViewStudents.CurrentRow.Cells["FullName"].Value.ToString();
                txtAge.Text = dataGridViewStudents.CurrentRow.Cells["Age"].Value.ToString();

                // Hiển thị ngành học trong ComboBox
                var selectedStudent = (Student)dataGridViewStudents.CurrentRow.DataBoundItem;
                if (selectedStudent != null && selectedStudent.Major != null)
                {
                    cmbMajor.SelectedItem = selectedStudent.Major;
                }
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            using (SchoolContext dbContext = new SchoolContext())
            {
                var students = dbContext.Students.ToList();
                bindingSource.DataSource = students;
                dataGridViewStudents.DataSource = bindingSource;
            }
            txtStudentID.DataBindings.Add("Text", bindingSource, "StudentID");
            txtFullName.DataBindings.Add("Text", bindingSource, "FullName");
            txtAge.DataBindings.Add("Text", bindingSource, "Age");
            cmbMajor.DataBindings.Add("Text", bindingSource, "Major");

        }

        private void txtStudentID_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtStudentID.Text, out int studentId))
            {
                using (var context = new SchoolContext())
                {
                    // Tìm sinh viên với StudentID tương ứng
                    var student = context.Students.FirstOrDefault(s => s.StudentId == studentId);

                    if (student != null)
                    {
                        // Hiển thị thông tin sinh viên nếu tìm thấy
                        txtFullName.Text = student.FullName;
                        txtAge.Text = student.Age.ToString();
                        cmbMajor.SelectedItem = student.Major;
                    }
                    else
                    {
                        // Xóa các TextBox và ComboBox nếu không tìm thấy sinh viên
                        txtFullName.Clear();
                        txtAge.Clear();
                        cmbMajor.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                // Xóa các TextBox và ComboBox nếu StudentID không hợp lệ
                txtFullName.Clear();
                txtAge.Clear();
                cmbMajor.SelectedIndex = -1;
            }
        }
    

        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listNganh = cmbMajor.Items;
            foreach (var item in listNganh)
            {
                cmbMajor.SelectedItem = item;
            }



            if (cmbMajor.SelectedItem != null)
            {
                // Thực hiện hành động khi người dùng chọn ngành học
                string selectedMajor = cmbMajor.SelectedItem.ToString();
                MessageBox.Show("Ngành học được chọn: " + selectedMajor, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void LoadMajorData()
        {
            using (var context = new SchoolContext())
            {
                // Lấy danh sách ngành học từ cơ sở dữ liệu (loại bỏ các mục trùng lặp)
                var majorList = context.Students
                                       .Select(s => s.Major)
                                       .Distinct()
                                       .ToList();

                // Thiết lập dữ liệu cho ComboBox
                cmbMajor.DataSource = majorList;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bindingSource.Position < bindingSource.Count - 1)
            {
                bindingSource.MoveNext();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bindingSource.Position > 0)
            {
                bindingSource.MovePrevious();
            }
        }
    }
    }

    
    


