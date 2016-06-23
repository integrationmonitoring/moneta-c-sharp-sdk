using System;
using System.Collections.Generic;

namespace Moneta
{
    class MonetaSdkResult
    {
        public bool error = false;
        public String errorMessage = null;
	    public String xmlData = null;
        public String jsonData = null;
        // attributes
        public Dictionary<string, string> attributes = new Dictionary<string, string>();
        // pure data
        public Object response;
    }
}
