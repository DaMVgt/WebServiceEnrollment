using WebServicesEnrollment.Models;
using System.Data.SqlClient;
using System.Data;

namespace WebServicesEnrollment.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private SqlConnection connection = new SqlConnection("Server=localhost;Database=kalum_test;User Id=sa;Password=Inicio.2022;");
        private List<CarreraTecnica> carreras = new List<CarreraTecnica>();
        private List<Cargo> cargos = new List<Cargo>();
        public EnrollmentService()
        {
            carreras.Add(new CarreraTecnica() { CarreraId = "1", Carrera = "FullStack DOTNET Core" });
            carreras.Add(new CarreraTecnica() { CarreraId = "2", Carrera = "FullStack Java EE" });
            carreras.Add(new CarreraTecnica() { CarreraId = "3", Carrera = "Desarrollo Movil Android" });

            cargos.Add(new Cargo() { CargoId = "1", Descripcion = "Pago por inscripcion", Prefijo = "INSC", Monto = 1200.00, IsGeneraMora = false, PorcentajeMora = 0 });
            cargos.Add(new Cargo() { CargoId = "2", Descripcion = "Pago Carnet", Prefijo = "CARNE", Monto = 30.00, IsGeneraMora = false, PorcentajeMora = 0 });
            cargos.Add(new Cargo() { CargoId = "3", Descripcion = "Pago academico mensual", Prefijo = "PAGM", Monto = 800.00, IsGeneraMora = false, PorcentajeMora = 0 });
        }
        public EnrollmentResponse EnrollmentProcess(EnrollmentRequest request)
        {
            EnrollmentResponse respuesta = null;
            Aspirante aspirante = buscarAspirante(request.NoExpediente);
            if (aspirante == null)
            {
                respuesta = new EnrollmentResponse() { Codigo = 204, Respuesta = $"No Existe el aspiratne con el Expediente {request.NoExpediente}" };
            }
            else
            {
                respuesta = EjecutarProcedimiento(request);
            }
            return respuesta;
        }
        private EnrollmentResponse EjecutarProcedimiento(EnrollmentRequest request)
        {
            EnrollmentResponse respuesta = null;
            SqlCommand cmd = new SqlCommand("sp_EnrollmentProcess", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@NoExpediente", request.NoExpediente));
            cmd.Parameters.Add(new SqlParameter("@Ciclo", request.Ciclo));
            cmd.Parameters.Add(new SqlParameter("@MesInicioPago", request.MesInicioPago));
            cmd.Parameters.Add(new SqlParameter("@CarreraId", request.CarreraId));
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                respuesta = new EnrollmentResponse() { Codigo = 201, Respuesta = "Enrollment success!!!" };
            }
            catch
            {
                respuesta = new EnrollmentResponse() { Codigo = 503, Respuesta = "Error al momento de llamar al procedimiento almacenado" };
            }
            finally
            {
                connection.Close();
            }
            return respuesta;
        }
        private Aspirante buscarAspirante(string noExpediente)
        {
            Aspirante resultado = null;

            SqlDataAdapter daAspirante = new SqlDataAdapter($"SELECT * from Aspirante WHERE NoExpediente = '{noExpediente}';", connection);
            DataSet dsAspirante = new DataSet();
            daAspirante.Fill(dsAspirante, "Aspirante");
            if (dsAspirante.Tables["Aspirante"].Rows.Count > 0)
            {
                resultado = new Aspirante()
                {
                    NoExpediente = dsAspirante.Tables["Aspirante"].Rows[0][0].ToString(),
                    Apellidos = dsAspirante.Tables["Aspirante"].Rows[0][1].ToString(),
                    Nombres = dsAspirante.Tables["Aspirante"].Rows[0][2].ToString(),
                    Direccion = dsAspirante.Tables["Aspirante"].Rows[0][3].ToString(),
                    Telefono = dsAspirante.Tables["Aspirante"].Rows[0][4].ToString(),
                    Email = dsAspirante.Tables["Aspirante"].Rows[0][5].ToString(),
                    Estatus = dsAspirante.Tables["Aspirante"].Rows[0][6].ToString(),
                    CarreraId = dsAspirante.Tables["Aspirante"].Rows[0][7].ToString(),
                    JornadaId = dsAspirante.Tables["Aspirante"].Rows[0][9].ToString()
                };
            }
            return resultado;
        }
        public string Test(string s)
        {
            Console.WriteLine("Test method executed");
            return s;
        }
    }
}