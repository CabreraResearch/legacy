/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _itemdbclickIIFE(nameSpace) {

    
    nameSpace.trees.listeners.lift('itemdblclick',
         /**
          * Create a new render listener;
         */
        function listeners(callBack) {
            'use strict';
            if (callBack) {
              //http://docs.sencha.com/extjs/4.1.3/#!/api/Ext.tree.Panel-event-itemdblclick

              /**
               * AfterRender event on the tree panel
               * @param extView {Ext.Component} usually the Ext Panel
               * @param record {Ext.data.Model} The record object
               * @param item {HTMLElement} The DOM node
               * @param e {Ext.EventObject} The event object
               * @param eOpts {Object} arbitrary Ext props
              */
              return function itemdblclick(extView, record, item, index, e, eOpts) {
                  'use strict';
                  callBack(extView, record, item, index, e, eOpts);
              };
          }
      });


}(window.$om$));