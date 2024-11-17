using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedSoft.Utility
{
    public static class SD
    {
        #region Roluri Utilizatori
        public const string Rol_Admin = "Admin";
        public const string Rol_Medic = "Medic";
        public const string Rol_Pacient = "Pacient";
        #endregion

        #region Status Consultatie
        public const string ConsultStatusPending = "In asteptare";
        public const string ConsultStatusApproved = "Confirmata";
        public const string ConsultStatusCompleted = "Finalizata";
        public const string ConsultStatusCanceled = "Anulata";

        public static IEnumerable<SelectListItem> GetConsultStatusStrings()
        {
            yield return new SelectListItem("In asteptare", "In asteptare");
            yield return new SelectListItem("Confirmata", "Confirmata");
            yield return new SelectListItem("Finalizata", "Finalizata");
            yield return new SelectListItem("Anulata", "Anulata");
        }
        #endregion 
    }
}
