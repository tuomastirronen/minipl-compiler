using System;

namespace MiniPL {
	public class Source {

		public string path;
		public string content;

		public Source(string path) {
	        this.path = path;
	        this.content = System.IO.File.ReadAllText(this.path).Trim();
	    }
	}    
}