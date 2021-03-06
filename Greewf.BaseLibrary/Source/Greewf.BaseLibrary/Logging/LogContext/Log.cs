//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Greewf.BaseLibrary.Logging
{
    using System;
    using System.Collections.Generic;
    
    
    public partial class Log
    {
        public Log()
        {
            this.LogDetails = new HashSet<LogDetail>();
        }
    
    	
        
        public long Id { get; set; }
    	
        
        public System.DateTime DateTime { get; set; }
    	
        
        public string Key { get; set; }
    	
        
        public string Code { get; set; }
    	
        
        public string Text { get; set; }
    	
        
        public string Ip { get; set; }
    	
        
        public string MachineName { get; set; }
    	
        
        public string Browser { get; set; }
    	
        
        public string Username { get; set; }
    	
        
        public string UserFullname { get; set; }
    	
        
        public string RequestUrl { get; set; }
    	
        
        public string Querystring { get; set; }
    	
        
        public Nullable<bool> IsMobile { get; set; }
    	
        
        public string UserAgent { get; set; }
    	
               
    	
        
        public string RequestMethod { get; set; }
    	
        
        public string RequestHeaders { get; set; }
    	
        
        public Nullable<bool> FromInternet { get; set; }
    	
        
        public Nullable<int> Checksum { get; set; }
    	
        
        public string ServerMachineName { get; set; }
    	
        
        public string ServerProcessName { get; set; }
    	
        
        public Nullable<int> ServerProcessId { get; set; }
    
        
        public virtual ICollection<LogDetail> LogDetails { get; set; }
    }
}
