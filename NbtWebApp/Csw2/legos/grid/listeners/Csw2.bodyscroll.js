/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _bodyscrollIIFE(nameSpace) {

    nameSpace.grids.listeners.lift('bodyscroll',
        /**
         * Create a new bodyscroll listener;
        */
        function listener(callBack) {
            'use strict';
            if (callBack) {
              
              /**
               * Undocumented listener method
              */
              return function bodyscroll() {
                  var args = arguments;
                  callBack.call(this, args);
              };
          }
      });


}(window.$om$));