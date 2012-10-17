
<!-- saved from url=(0055)http://d3eyf2cx8mbems.cloudfront.net/js/loggly-0.1.0.js -->
<html><head><meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"></head><body><pre style="word-wrap: break-word; white-space: pre-wrap;">/**
 * Castor - a cross site POSTing JavaScript logging library for Loggly
 * 
 * Copyright (c) 2011 Loggly, Inc.
 * All rights reserved.
 *
 * Author: Kord Campbell &lt;kord@loggly.com&gt;
 * Date: May 2, 2011
 * 
 * Uses methods from janky.post, copyright(c) 2011 Thomas Rampelberg &lt;thomas@saunter.org&gt;
 *  
 * Sample usage (replace with your own Loggly HTTP input URL):

  &lt;script src="/js/loggly.js" type="text/javascript"&gt;&lt;/script&gt;
  &lt;script type="text/javascript"&gt;
    window.onload=function(){
      castor = new loggly({ url: 'http://logs.loggly.com/inputs/a4e839e9-4227-49aa-9d28-e18e5ba5a818?rt=1', level: 'WARN'});
      castor.log("url="+window.location.href + " browser=" + castor.user_agent + " height=" + castor.browser_size.height);
    }
  &lt;/script&gt;

 */  

(function() {
  this.loggly = function(opts) {
    this.user_agent = get_agent();
    this.browser_size = get_size();
    log_methods = {'error': 5, 'warn': 4, 'info': 3, 'debug': 2, 'log': 1};
    if (!opts.url) throw new Error("Please include a Loggly HTTP URL.");
    if (!opts.level) { 
      this.level = log_methods['info'];
    } else {
      this.level = log_methods[opts.level];
    }
    this.log = function(data) {
      if (log_methods['log'] == this.level) { 
        opts.data = data;
        janky(opts); 
      }
    };
    this.debug = function(data) {
      if (log_methods['debug'] &gt;= this.level) { 
        opts.data = data;
        janky(opts); 
      }
    };
    this.info = function(data) {
      if (log_methods['info'] &gt;= this.level) { 
        opts.data = data;
        janky(opts); 
      }
    };
    this.warn = function(data) {
      if (log_methods['warn'] &gt;= this.level) { 
        opts.data = data;
        janky(opts); 
      }
    };
    this.error = function(data) {
      if (log_methods['error'] &gt;= this.level) { 
        opts.data = data;
        janky(opts); 
      }
    };
  };
  this.janky = function(opts) {
    janky._form(function(iframe, form) {
      form.setAttribute("action", opts.url);
      form.setAttribute("method", "post");
      janky._input(iframe, form, opts.data);
      form.submit();
	  setTimeout(function(){
        document.body.removeChild(iframe);              
      }, 2000);
    });
  };
  this.janky._form = function(cb) {
    var iframe = document.createElement("iframe");
    document.body.appendChild(iframe);
    iframe.style.display = "none";
    setTimeout(function() {
      var form = iframe.contentWindow.document.createElement("form");
      iframe.contentWindow.document.body.appendChild(form);
      cb(iframe, form);
    }, 0);
  };
  this.janky._input = function(iframe, form, data) {
    var inp = iframe.contentWindow.document.createElement("input");
    inp.setAttribute("type", "hidden");
    inp.setAttribute("name", "source");
    inp.value = "castor " + data;
    form.appendChild(inp);
  };
  this.get_agent = function () {
    return navigator.appCodeName + navigator.appName + navigator.appVersion;
  };
  this.get_size = function () {
    var width = 0; var height = 0;
    if( typeof( window.innerWidth ) == 'number' ) {
      width = window.innerWidth; height = window.innerHeight;
    } else if( document.documentElement &amp;&amp; ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
      width = document.documentElement.clientWidth; height = document.documentElement.clientHeight;
    } else if( document.body &amp;&amp; ( document.body.clientWidth || document.body.clientHeight ) ) {
      width = document.body.clientWidth; height = document.body.clientHeight;
    }
    return {'height': height, 'width': width};
  };
})();

</pre></body></html>