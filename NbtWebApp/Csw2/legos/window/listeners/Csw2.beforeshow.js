/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _beforeshowIIFE(nameSpace) {

   nameSpace.okna.listeners.lift('beforeshow',
         /**
          * Create a new render listener;
         */
        function (callBack) {
            'use strict';
            if (callBack) {
              //http://docs.sencha.com/extjs/4.1.3/#!/api/Ext.window.Window-event-beforeshow

              /**
               * BeforeShow event on the window.Window
               * @param extView {Ext.Component} usually the Ext Window
               * @param eOpts {Object} arbitrary Ext props
              */
              return function (extView, eOpts) {
                  'use strict';
                  callBack.call(extView, extView, eOpts);
              };
          }
      });


}(window.$om$));