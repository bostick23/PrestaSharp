//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrestaSharp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ps_warehouse
    {
        public long id_warehouse { get; set; }
        public long id_currency { get; set; }
        public long id_address { get; set; }
        public long id_employee { get; set; }
        public string reference { get; set; }
        public string name { get; set; }
        public string management_type { get; set; }
        public bool deleted { get; set; }
    }
}