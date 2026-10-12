using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SegundoParcial
{
    public partial class Form1 : Form

    {
        // Realizo la Conexión a la Base de Datos
        
        private string cadenaConexion = "Server=.;Database=ParcialProgramacion1;Trusted_Connection=True;TrustServerCertificate=True;";

        public int idSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
       
        {
            
            if (!ValidarCampos()) return;

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                // Consulta SQL parametrizada para evitar inyecciones SQL
                string query = "INSERT INTO Productos (Nombre, Categoria, Precio, Existencias) VALUES (@Nombre, @Categoria, @Precio, @Existencias)";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                comando.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                comando.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text));
                comando.Parameters.AddWithValue("@Existencias", Convert.ToInt32(txtExistencias.Text));

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    ConsultarProductos(); // Refresca la tabla automáticamente
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnConsulta_Click(object sender, EventArgs e)

        {
            ConsultarProductos();
        }

        
        private void ConsultarProductos()
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "SELECT Id, Nombre, Categoria, Precio, Existencias FROM Productos";
                SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();

                try
                {
                    conexion.Open();
                    adaptador.Fill(dt);
                    dgvProductos.DataSource = dt; // Carga los datos reales de SQL Server en la tabla
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnActualizar_Click(object sender, EventArgs e)
      
        {
            
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "UPDATE Productos SET Nombre=@Nombre, Categoria=@Categoria, Precio=@Precio, Existencias=@Existencias WHERE Id=@Id";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", idSeleccionado);
                comando.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                comando.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                comando.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text));
                comando.Parameters.AddWithValue("@Existencias", Convert.ToInt32(txtExistencias.Text));

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    ConsultarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Solicita confirmación obligatoria al usuario
            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No) return;

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "DELETE FROM Productos WHERE Id=@Id";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", idSeleccionado);

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    ConsultarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
    
        {
            LimpiarCampos();
        }

        
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();
            idSeleccionado = 0;
            txtNombre.Focus();  
        }


        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        {
            // Verifica que se haga clic en una fila con datos y no en los encabezados
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
                idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtCategoria.Text = fila.Cells["Categoria"].Value.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
                txtExistencias.Text = fila.Cells["Existencias"].Value.ToString();
            }
        }

 
        
        }
        // 2. Método de Validación 
        private bool ValidarCampos()
        {
            // El nombre y la categoría son obligatorios
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("El nombre y la categoría son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // El precio debe ser numérico y mayor que cero
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número válido y mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Las existencias deben ser un número entero igual o mayor que cero
            if (!int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MessageBox.Show("Las existencias deben ser un número entero igual o mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

    }
}

//Integrantes
//Diego Angel Cerna Rodriguez 
//Diego Emnanuel Umaña Rivas

