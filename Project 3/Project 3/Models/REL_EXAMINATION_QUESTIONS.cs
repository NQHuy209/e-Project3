//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class REL_EXAMINATION_QUESTIONS
    {
        public int ExaminationId { get; set; }
        public int QuestionId { get; set; }
        public int TopicId { get; set; }
        public int ManagerId { get; set; }
    
        public virtual EXAMINATION EXAMINATION { get; set; }
        public virtual QUESTIONS QUESTIONS { get; set; }
    }
}