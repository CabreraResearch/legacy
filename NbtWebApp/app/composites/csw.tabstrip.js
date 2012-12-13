
(function () {
    'use strict';

    Csw.composites.tabStrip = Csw.composites.tabStrip ||
        Csw.composites.register('tabStrip', function (cswParent, cswPrivate) {

            //#region Variables
            
            var cswPublic = { };

            //#endregion Variables

            //#region Pre-ctor
            (function _pre() {
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.tabPanel = cswPrivate.tabPanel || {};
                cswPrivate.tabPanel.title = cswPrivate.tabPanel.title || 'Tabs';
                cswPrivate.tabPanel.height = cswPrivate.tabPanel.height || 600;
                //cswPrivate.tabPanel.width = cswPrivate.tabPanel.width || 1000;
                cswPrivate.tabPanel.resizable = cswPrivate.tabPanel.resizable; // || true
                cswPrivate.tabPanel.stateful = cswPrivate.tabPanel.stateful;  // || true
                cswPrivate.tabPanel.stateId = cswPrivate.tabPanel.stateId || 'CswTabStrip';
                cswPrivate.tabs = cswPrivate.tabs || [];
                //    [{
                //    title: 'First tab',
                //    html: 'No content has been defined for this tab',
                //    tooltip: 'First tab of many'
                //}],
                cswPrivate.onTabSelect = cswPrivate.onTabSelect || function(el, eventObj, callBack) {

                };
                
                cswParent.empty();
                cswPublic = cswParent.div().css({width: '100%', height: '100%'});

            }());
                
            //#endregion Pre-ctor

            //#region Define Class Members

            cswPrivate.tabCollection = [];
            
            cswPublic.addTab = function(tab) {
            	/// <summary>
            	/// Add a single Ext tab based on a simple JS tab object.
            	/// </summary>
            	/// <param name="tab" type="Object">Definition containing title and HTML.</param>
            	/// <returns type="Ext.tab">ExtJS tab instance</returns>
                tab = tab || {
                    title: 'This is a tab',
                    html: 'Hi, I am tab ',
                    tooltip: title
                };
                tab.layout = 'fit';
                var extTab = cswPublic.tabPanel.add([tab])[0];

                var newTab = {
                    get csw() {
                        var ret = null;
                        if (extTab.el) {
                            ret = Csw.domNode({
                                ID: extTab.el.id,
                                el: extTab.el.dom
                            });
                        }
                        return ret;
                    },
                    set csw(val) {
                        //No setter.
                    }
                };
                newTab.name = tab.title;
                newTab.ext = extTab;
                
                cswPrivate.tabCollection.push(newTab);
                return newTab;
            };
            
            cswPublic.addTabs = function (tabs) {
                /// <summary>
                /// Add an array of Ext tabs based on an array of simple JS tab object.
                /// </summary>
                /// <param name="tabs" type="Array">Array of tab definitions, each containing a title and HTML.</param>
                /// <returns type="Array">Array of ExtJS tab instances</returns>
                tabs = tabs || cswPrivate.tabs;
                Csw.each(tabs, function (tabObj) {
                    cswPublic.addTab(tabObj);
                });
                return cswPrivate.tabCollection;
            };
            
            cswPublic.setActiveTab = function (tab) {
                /// <summary>
                /// Takes a tab instance, id or index and sets it as active/selected
                /// </summary>
                /// <returns type="Ext.tab">ExtJS tab instance</returns>
                tab = tab || 0;
                return cswPublic.tabPanel.setActiveTab(tab);
            };

            cswPublic.setTitle = function (title) {
                /// <summary>
                /// Sets the title of a Tab Panel
                /// </summary>
                /// <returns type="tabStrip">Csw tabstrip instance</returns>
                title = title || 'Tab Title';
                cswPublic.tabPanel.setTitle(title);
                return cswPublic;
            };

            cswPublic.getWidth = function () {
                /// <summary>
                /// Gets the width of the tab strip
                /// </summary>
                /// <returns type="Number">Width</returns>
                return cswPublic.tabPanel.getWidth();
            };

            cswPublic.setWidth = function (width) {
                /// <summary>
                /// Sets the width of the tab strip
                /// </summary>
                /// <returns type="tabStrip">Csw tabstrip instance</returns>
                cswPublic.tabPanel.setWidth(width);
                return cswPublic;
            };

            cswPublic.resetWidth = function () {
                /// <summary>
                /// Reset the tab strip to its original width
                /// </summary>
                /// <returns type="tabStrip">Csw tabstrip instance</returns>
                cswPublic.setWidth(cswPrivate.width);
                return cswPublic;
            };

            cswPublic.setSize = function(def) {
            	/// <summary>
            	/// Set the heigth and width of the tab strip
            	/// </summary>
            	/// <param name="def">Object with 'width' and 'height' properties</param>
                /// <returns type="tabStrip">Csw tabstrip instance</returns>
                def = def || {
                    width: 800,
                    height: 600
                };
                cswPublic.tabPanel.setSize(def);
                return cswPublic;
            };

            //#endregion Define Class Members
              

            //#region Post-ctor
            
                (function _post() {

                    window.Ext.tip.QuickTipManager.init();

                    cswPublic.tabPanel = window.Ext.create('Ext.tab.Panel', {
                        id: cswPrivate.ID,
                        layout: 'fit',
                        height: cswPrivate.tabPanel.height, //no height will render as 0px
                        //width: no width is fine
                        renderTo: cswPublic.getId(),
                        title: cswPrivate.tabPanel.title,
                        resizable: cswPrivate.tabPanel.resizable,
                        stateful: cswPrivate.tabPanel.stateful,
                        stateId: cswPrivate.tabPanel.stateId,
                        plugins: [{
                                ptype: 'tabscrollermenu',
                                maxText: 15,
                                pageSize: 5
                            }
                            //new window.Ext.ux.TabReorderer() //there is no ptype for this plugin. Pass in an instance instead.
                        ],
                        listeners: {
                            click: {
                                element: 'el', //bind to the underlying el property on the panel
                                fn: function(el, eventObj, callBack) {
                                    var tabName = el.target.innerHTML;
                                    Csw.tryExec(cswPrivate.onTabSelect, tabName, el, eventObj, callBack);
                                }
                            }
                        }
                    });
                    cswPrivate.width = cswPublic.getWidth();
                    cswPublic.addTabs(cswPrivate.tabs);

                }());
            
            //#endregion Post-ctor

            return cswPublic;

        });
} ());
