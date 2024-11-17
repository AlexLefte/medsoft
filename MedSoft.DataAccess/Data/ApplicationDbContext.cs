using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MedSoft.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MedSoft.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        #region Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        #endregion

        #region Properties
        public DbSet<Administrator> Administrator { get; set; }
        public DbSet<Medic> Medic { get; set; }
        public DbSet<MedicCount> MedicCount { get; set; }
        public DbSet<Pacient> Pacient { get; set; }
        public DbSet<Medicamente> Medicamente { get; set; }
        public DbSet<Consultatie> Consultatie { get; set; }
        public DbSet<Specializare> Specializare { get; set; }
        public DbSet<IdentityUser> AspNetUsers { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Metoda ce adauga valori implicite in diversele tabele ale bazei de date
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicCount>().HasNoKey();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Specializare>().HasData(
                new Specializare
                {
                    SpecializareID = 1,
                    Nume = "Cardiologie",
                },
                new Specializare
                {
                    SpecializareID = 2,
                    Nume = "Chirurgie",
                },
                new Specializare
                {
                    SpecializareID = 3,
                    Nume = "Dermatologie",
                },
                new Specializare
                {
                    SpecializareID = 4,
                    Nume = "Neurologie",
                },
                new Specializare
                {
                    SpecializareID = 5,
                    Nume = "Oftalmologie",
                });

            /*modelBuilder.Entity<Medic>().HasData(
                new Medic
                {
                    MedicID = 1,
                    NumeMedic = "Ionescu",
                    PrenumeMedic = "Andrei",
                    Specializare = 2,
                    PretAbonament = 2000.0
                },
                new Medic
                {
                    MedicID = 2,
                    NumeMedic = "Petrescu",
                    PrenumeMedic = "Adrian",
                    Specializare = 1,
                    PretAbonament = 1200.0
                },
                new Medic
                {
                    MedicID = 3,
                    NumeMedic = "Vasile",
                    PrenumeMedic = "Georgian",
                    Specializare = 3,
                    PretAbonament = 200.0
                },
                new Medic
                {
                    MedicID = 4,
                    NumeMedic = "Cristea",
                    PrenumeMedic = "Mihai",
                    Specializare = 5,
                    PretAbonament = 500
                });          

            modelBuilder.Entity<Pacient>().HasData(
                new Pacient
                {
                    PacientID = 1,
                    CNP = "5020416780018",
                    NumePacient = "Ion",
                    PrenumePacient = "Vasile",
                    Adresa = "Bucuresti",
                    Asigurare = 3000
                },
                new Pacient
                {
                    PacientID = 2,
                    CNP = "5011023560017",
                    NumePacient = "Gheorghe",
                    PrenumePacient = "Cristian",
                    Adresa = "Constanta",
                    Asigurare = 2500
                },
                new Pacient
                {
                    PacientID = 3,
                    CNP = "6040606120021",
                    NumePacient = "Ionescu",
                    PrenumePacient = "Mirela",
                    Adresa = "Tulcea",
                    Asigurare = 1000
                });*/

            modelBuilder.Entity<Medicamente>().HasData(
                new Medicamente
                {
                    MedicamentID = 1,
                    Denumire = "Paracetamol"
                },
                new Medicamente
                {
                    MedicamentID = 2,
                    Denumire = "Ibuprofen"
                },
                new Medicamente
                {
                    MedicamentID = 3,
                    Denumire = "Aspirină"
                },
                new Medicamente
                {
                    MedicamentID = 4,
                    Denumire = "Amoxicilin"
                },
                new Medicamente
                {
                    MedicamentID = 5,
                    Denumire = "Omeprazol"
                },
                new Medicamente
                {
                    MedicamentID = 6,
                    Denumire = "Aciclovir"
                });
        }

        #region Stored Procedures Calls
        public void Call_SP_Adauga_Consultatie(DateTime data,
            string medicID, 
            string pacientID,
            string? diagnostic,
            long? medicamentID, 
            string? doza, 
            string status,
            out string errorMessage,
            out bool errorFlag)
        {
            // Input
            var dataParameter = new SqlParameter("@Data", data);
            var medicIDParameter = new SqlParameter("@MedicID", medicID);
            var pacientIDParameter = new SqlParameter("@PacientID", pacientID);
            var diagnosticParameter = new SqlParameter("@Diagnostic", diagnostic ?? (object)DBNull.Value);
            var medicamentIDParameter = new SqlParameter("@MedicamentID", medicamentID ?? (object)DBNull.Value);
            var dozaParameter = new SqlParameter("@Doza", doza ?? (object)DBNull.Value);
            var statusParameter = new SqlParameter("@Status", status);

            // Output
            var errorMessageParameter = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, -1);
            errorMessageParameter.Direction = ParameterDirection.Output;

            var errorFlagParameter = new SqlParameter("@ErrorFlag", SqlDbType.Bit);
            errorFlagParameter.Direction = ParameterDirection.Output;

            // Apelul procedurii stocate
            Database.ExecuteSqlRaw("EXEC SP_Adauga_Consultatie @Data, @MedicID, @PacientID, @Diagnostic, @MedicamentID, @Doza, @Status, " +
                        "@ErrorMessage OUTPUT, @ErrorFlag OUTPUT",
                dataParameter, medicIDParameter, pacientIDParameter, diagnosticParameter, medicamentIDParameter, dozaParameter, statusParameter,
                    errorMessageParameter, errorFlagParameter);

            // Setare parametri de iesire
            errorMessage = errorMessageParameter.Value?.ToString();
            errorFlag = Convert.ToBoolean(errorFlagParameter.Value);
        }

        public void Call_SP_Modifica_Consultatie(long consultatieID, 
            DateTime data, 
            string medicID, 
            string pacientID, 
            string? diagnostic, 
            long? medicamentID, 
            string? doza, 
            string status,
            out string errorMessage,
            out bool errorFlag)
        {
            // Input
            var consultatieParameter = new SqlParameter("@ConsultatieID", consultatieID);
            var dataParameter = new SqlParameter("@Data", data);
            var medicIDParameter = new SqlParameter("@MedicID", medicID);
            var pacientIDParameter = new SqlParameter("@PacientID", pacientID);
            var diagnosticParameter = new SqlParameter("@Diagnostic", diagnostic ?? (object)DBNull.Value);
            var medicamentIDParameter = new SqlParameter("@MedicamentID", medicamentID ?? (object)DBNull.Value);
            var dozaParameter = new SqlParameter("@Doza", doza ?? (object)DBNull.Value);
            var statusParameter = new SqlParameter("@Status", status);

            // Output
            var errorMessageParameter = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, -1);
            errorMessageParameter.Direction = ParameterDirection.Output;

            var errorFlagParameter = new SqlParameter("@ErrorFlag", SqlDbType.Bit);
            errorFlagParameter.Direction = ParameterDirection.Output;

            // Apelul procedurii stocate
            Database.ExecuteSqlRaw("EXEC SP_Modifica_Consultatie @ConsultatieID, @Data, @MedicID, @PacientID, @Diagnostic, @MedicamentID, @Doza, @Status, " +
                        "@ErrorMessage OUTPUT, @ErrorFlag OUTPUT",
                consultatieParameter, dataParameter, medicIDParameter, pacientIDParameter, diagnosticParameter, medicamentIDParameter, dozaParameter, statusParameter,
                    errorMessageParameter, errorFlagParameter);

            // Setare parametri de iesire
            errorMessage = errorMessageParameter.Value?.ToString();
            errorFlag = Convert.ToBoolean(errorFlagParameter.Value);
        }
        #endregion
        #endregion
    }
}