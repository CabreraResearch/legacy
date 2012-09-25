/**
 * Javascript jzebra Plugin.
 * 
 * This plugin provide to help usage of jzebra.jar.
 * 
 * I'have no write jzebra.jar class
 * 
 * I write a javascript library that use jzebra.jar java applet method can be real usefull for web developers!
 * This class it's tested on Zebra TLP2844-Z, but it can be use with other zebra printer becouse it's use ZPL
 * print code language developer by Zebra.
 * 
 * @author  Andrea Namici
 * @mailto  andrea.namici@gmail.com
 * 
 */

 window.jzebra = function(printer_name,appletOptions){
       //Applet
       this.def_applet_name       = "jzebra";       //Applet Default Name
       this.def_applet_width      = "0px";          //Applet Default Width
       this.def_applet_height     = "0px";          //Applet Default Height
       this.def_applet_url        = "jzebra.jar";   //Applet Default default url
       this.def_applet_visibility = "visible";      //Applet Default Visibility Style
       
       //Applet Params
       this.def_printer_name       = "zebra";       //Name of Zebra Printer
       
       //ZPL 
       this.zpl_code               = false;         //ZPL default Code
       
       //Javascript
       this.jzebra_istance         = false;         //Applet Istance of jzebra
       this.jzebra_istance_timeout = 5000;          //Applet Istance init default timeout. this time tell when init the jzebra reference in class
       this.nr_packed              = 35;            //Jzebra max print at once
       this.nr_append              = 0;             //Jzebra nr of print appended
       this.buffer                 = new Array();   //Jzebra Print Buffer, this parameter it's a print array buffer
       this.buffer_timeout         = 5000;          //Jzebra Time of each buffer print (setTimeout js)
       this.buffer_timeout_int     = false;         //Jzebra Reference of Timeout buffer print function
       
       /**
        * Class Costruct
        * 
        * Init of Java Applet
        * 
        * @return boolean
        */
        this.init = function(printer_name,appletOptions){

            var applet = document.createElement("applet");
            var param  = document.createElement("param");


            //Applet Attribute
            
            applet.setAttribute("code","jzebra.PrintApplet.class");
            
            if(typeof appletOptions.name!='undefined'){
               applet.setAttribute("name",appletOptions.name);
            }else{
               applet.setAttribute("name",this.def_applet_name);
            }
            
            if(typeof appletOptions.url != 'undefined'){
               applet.setAttribute("archive",appletOptions.url);
            }else{
               applet.setAttribute("archive",this.def_applet_url);
            }
            
            if(typeof appletOptions.width!='undefined'){
               applet.setAttribute("widht",appletOptions.width);
            }else{
               applet.setAttribute("widht",this.def_applet_width);
            }
            
            if(typeof appletOptions.height!='undefined'){
               applet.setAttribute("height",appletOptions.applet_height);
            }else{
               applet.setAttribute("height",this.def_applet_height);
            }
            
            
            if(typeof appletOptions.visibility!="undefined"){
               applet.style.visibility = appletOptions.visibility;
            }else{
               applet.style.visibility = this.def_applet_visibility;
            }
            
            //Applet Params Attribute
            param.setAttribute("name","printer");
            param.setAttribute("value",printer_name);
            
            applet.appendChild(param);
            document.getElementsByTagName("body")[0].appendChild(applet).appendChild(param);
            
            return this.initAppletIstance() > 0  ? true : false;
        }
        
        
        /**
         * Init Class jzebra_istance reference
         * 
         * Set class applet java reference aftern a seconds timeout.
         * This workaround it's usefull when loading a java applet  it's too slow.
         * 
         * @return Int
         */
        this.initAppletIstance = function(){
            
            return window.setTimeout(function(jzebraObj){
                    jzebraObj.jzebra_istance = document.getElementsByName("jzebra")[0]; 
                    return true;
            },this.jzebra_istance_timeout,this);
            
        }
        
        /**
         * Get generic ZPL to Print. 
         * Before print you can do sobstition using build_zpl_code()
         * 
         * @param Int type  You can load more than one type of zpl Code using this method
         * @see build_zpl_code Method
         * @return String
         */
        this.getZPL = function(type){
            
            var zpl_code = "";
            
            switch(type)
            {
                case 1:        //QrCode
                                zpl_code ="^XA";
                                zpl_code+="^PR5";
                                zpl_code+="^FO50,70";
                                zpl_code+="   ^A0N,20,20";
                                zpl_code+="   ^FD@pst_creation_datetime_locale@";
                                zpl_code+="^FS";
                                zpl_code+="   ^FO250,70";
                                zpl_code+="   ^A0N,20,20";
                                zpl_code+="   ^FD@aug_code@";
                                zpl_code+="^FS";
                                zpl_code+="^FO50,140";
                                zpl_code+="   ^A0N,20,20^CI0,21,36";
                                zpl_code+="   ^FDPrezzo: @pst_imp_iva@ $";
                                zpl_code+="^FS";
                                zpl_code+="^FO250,140";
                                zpl_code+="   ^A0N,20,20";
                                zpl_code+="   ^FDPeso: @pst_weight@g";
                                zpl_code+="^FS";
                                zpl_code+="   ^FO50,200";
                                zpl_code+="   ^GB300,0,4";
                                zpl_code+="^FS";
                                zpl_code+="   ^FO420,40";
                                zpl_code+="   ^BQN,5,5";
                                zpl_code+="   ^FDQA,@pst_code@";
                                zpl_code+="^FS";
                                zpl_code+="^XZ";
                            break;
                 
                 default:     
                                zpl_code ="^XA";
                                zpl_code+="^PR5";
                                zpl_code+="^FO50,70";
                                zpl_code+="   ^A0N,20,20";
                                zpl_code+="   ^FDHello ZPL Work!";
                                zpl_code+="^FS";
                                zpl_code+="^XZ";
                                
                             break;
                             
            }
            
            return zpl_code;
        }
        
        /**
         * Get ZPL Code to Print with sobstituion of parameter in zpl_code
         * 
         * @param String zpl_code ZPL code to use
         * @param Object options  fields to find and replace in zpl_code. Note that field must be like this "@fieldname@". the charater "@" it's important!
         * 
         * return String ZPL Code to print or FALSE if failure
         */
        this.BuildZPL = function(zpl_code,options){
            
            if(typeof options=='object')
            {
                for(attr in options)
                {
                    var regExp = new RegExp("@"+attr+"@");
                    zpl_code = zpl_code.replace(regExp,options[attr]);
                }
                
                return zpl_code;
            }
            
            return false;
        }

        /**
         * Return current java applet HTML reference of document
         * @return object
         */
        this.get_applet_istance = function(){
            return this.jzebra_istance;
        }


       /**
        * Append un job to jZebra.
        * 
        * You can use Buffer method or print all jobs append at the end using printJob()
        * 
        * @param Int     type       Type of ZPL code to Print
        * @param Boolean use_buffer If TRUE use buffer method, FALSE append only
        * @param Object  options    Fields to find and replace in zpl_code. Note that field must be like this "@fieldname@". the charater "@" it's important!
        * 
        * @see printJob()
        * 
        * @return Boolean TRUE on job Append, FALSE otherwise
        * 
        */
        this.appendJob = function(type,options,use_buffer){ 

            var use_buffer = typeof use_buffer=='undefined' ? false : use_buffer;
            
            if(use_buffer){
               if(this.nr_append==0){
                  this.initBufferTimeoutPrint();
               }
            }            
             
            var zpl_code  = this.getZPL(type);                  //get ZPL code to format
                zpl_code  = this.BuildZPL(zpl_code,options);    //format ZPL code to print
                
            this.nr_append++;
            
            if(use_buffer){   
               return this.buffer.push(zpl_code);   //Append zpl_code to buffer que
            }else{
               return this.append(zpl_code);        //Append direct
            }
        }
                
        /**
         * Init Buffer Print at first time that ivoke appendJob Method.
         * 
         * @return INT setTimeout int
         */
        this.initBufferTimeoutPrint = function(){
            
            this.buffer_timeout_int = window.setInterval(function(jzebraObj){ 
                                        if(!jzebraObj.printBuffer()){
                                            window.clearTimeout(jzebraObj.buffer_timeout_int); //Clear interval appena buffer finito
                                        }
                                      },this.buffer_timeout,this);
            
            return this.buffer_timeout_int;
            
        }
        
        /**
         * Append a jzebra print
         * 
         * @param zpl_code String ZPL to print
         * 
         * @return Boolean
         */
        this.append = function(zpl_code){             
            this.jzebra_istance.append(zpl_code);
            return true;
        }       
        
        //Print Method ***********************************************************
        //
        //
        
        /**
         * Print Current Buffer Array
         * 
         * @return Boolean
         */
        this.printBuffer = function(){
            
            if(this.buffer.length>0)
            {
                i=0;
                while(i<this.nr_packed && (zpl_code = this.buffer.shift()))
                {
                    this.jzebra_istance.append(zpl_code);
                    i++;
                }

                if(i>0){
                    this.printJob();
                    return true;
                }
            }
            
            return false;
        }
        
        /**
         * Print all Jobs Appended
         * 
         * @return Void
         */
        this.printJob = function(){
           return this.jzebra_istance.print();
        }
        
        /**
         * Print Single ZPL code.
         * 
         * This method append a single string and print directly
         * 
         * @param String code ZPL code to Print
         * 
         * @return Void
         */
        this.printZPLCode = function(code){   
            this.jzebra_istance.append(code);
            this.jzebra_istance.print();
        }
        
        //
        //**********************************************************************
        
        //Class Costruct ************************       
            
        var printer_name  = typeof printer_name    == 'undefined' ? this.def_printer_name   : printer_name;

        var appletOptions = typeof appletOptions   == 'undefined' ? 
                                                                    { 
                                                                      url        : this.def_applet_url,
                                                                      width      : this.def_applet_width,
                                                                      height     : this.def_applet_height,
                                                                      name       : this.def_applet_name,
                                                                      visibility : this.def_applet_visibility
                                                                    }
                                                                  
                                                                  : appletOptions;
                                                                  
        return this.init(printer_name,appletOptions);

        // ***********************************
};
