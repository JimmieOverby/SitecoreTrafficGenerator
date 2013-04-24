using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace Dms.Jump.TrafficGenerator.Business.Utilities.Tools
{
    public class QueryStringBuilder
    {
        private NameValueCollection collection;
        
        public QueryStringBuilder() 
        { 
            collection = System.Web.HttpUtility.ParseQueryString(string.Empty); 
        } 
        
        public QueryStringBuilder Add(string key, string value) 
        {
            collection.Add(key, value); 
            return this; 
        } 
        
        public QueryStringBuilder Remove(string key) 
        { 
            collection.Remove(key); 
            return this; 
        } 
        
        public string this[string key] 
        { 
            get { return collection[key]; } 
            set { collection[key] = value; } 
        } 
        
        public override string ToString() {
            return collection.ToString(); 
        } 
    }
}
