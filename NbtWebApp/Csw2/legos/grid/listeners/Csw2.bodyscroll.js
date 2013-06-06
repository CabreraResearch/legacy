/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _bodyscrollIIFE() {

     /**
      * Create a new bodyscroll listener;
     */
      Csw2.grids.listeners.lift('bodyscroll', function (callBack){
          if (callBack) {
             
              /**
               * Undocumented listener method
              */
              return function () {
                  callBack();
              };
          }
      });


      }());