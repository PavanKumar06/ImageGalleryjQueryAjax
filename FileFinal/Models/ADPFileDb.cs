//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FileFinal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ADPFileDb
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Nullable<int> FileSize { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
    }
}
