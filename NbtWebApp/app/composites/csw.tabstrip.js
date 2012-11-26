
(function () {
    'use strict';

    Csw.composites.tabStrip = Csw.composites.tabStrip ||
        Csw.composites.register('tabStrip', function (cswParent, options) {

            //#region Variables
            var cswPrivate = {
                name: '',
                tabPanel: {
                    title: 'Tabs',
                    height: 400,
                    width: 800,
                    resizable: true,
                    stateful: true,
                    stateId: 'CswRequestCart'
                },
                tabs: [],
                //    [{
                //    title: 'First tab',
                //    html: 'No content has been defined for this tab',
                //    tooltip: 'First tab of many',
                //    handler: function (tab, eventObj) {
                        
                //    }
                //}],
                onTabSelect: function(el, eventObj, callBack) {
                    
                },
                extTabs: []
            };
            var cswPublic = { };

            //#endregion Variables

            //#region Pre-ctor
            (function _pre() {
                Csw.extend(cswPrivate, options);

                cswParent.empty();
                cswPublic = cswParent.div();

            }());
                
            //#endregion Pre-ctor
               

            //#region Define Class Members

            //cswPrivate.method = function() {};

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
                    var extTab = cswPublic.tabPanel.add([tab])[0];
                    cswPrivate.extTabs.push(extTab);
                    return extTab;
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
                return cswPrivate.extTabs;
            };
            
            cswPublic.setActiveTab = function (tab) {
                /// <summary>
                /// Takes a tab instance, id or index and sets it as active/selected
                /// </summary>
                /// <returns type="Ext.tab">ExtJS tab instance</returns>
                tab = tab || 0;
                return cswPublic.tabPanel.setActiveTab(tab);
            };
                
            //#endregion Define Class Members
                

            //#region Post-ctor

            window.Ext.onReady(function() {

            (function _post() {
                
                    window.Ext.tip.QuickTipManager.init();

                    cswPublic.tabPanel = window.Ext.create('Ext.tab.Panel', {
                        height: cswPrivate.tabPanel.height,
                        width: cswPrivate.tabPanel.width,
                        renderTo: cswPublic.getId(),
                        title: cswPrivate.tabPanel.title,
                        resizable: cswPrivate.tabPanel.resizable,
                        stateful: cswPrivate.tabPanel.stateful,
                        stateId: cswPrivate.tabPanel.stateId,
                        plugins: [{
                            ptype: 'tabscrollermenu',
                            maxText: 15,
                            pageSize: 5
                        },
                        new window.Ext.ux.TabReorderer()
                        ]
                    });

                    Csw.each(cswPrivate.tabs, function(tabObj) {
                        cswPublic.addTab(tabObj);
                    });

            }());
            });
            //#endregion Post-ctor

            return cswPublic;

        });
} ());
