/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _afterrenderIIFE() {

     /**
      * Create a new show listener;
     */
    Csw2.okna.listeners.lift('show', function (callBack) {
          if (callBack) {
              //http://docs.sencha.com/extjs/4.1.3/#!/api/Ext.window.Window-event-show

              /**
               * Show event on the tree panel
               * @param extView {Ext.Component} usually the Ext Window
               * @param eOpts {Object} arbitrary Ext props
              */
              return function (extView, eOpts) {
                  callBack.call(extView, extView, eOpts);
              };
          }
      });


      }());