
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var previews = {};
    Csw.nodeHoverIn = Csw.nodeHoverIn || 
        Csw.register('nodeHoverIn', function (event, nodeid, nodekey, delay) {
            'use strict';
            var previewopts = {
                name: nodeid + '_preview',
                nodeid: nodeid,
                nodekey: nodekey,
                eventArg: event
            };
            if (false === Csw.isNullOrEmpty(nodeid) || false === Csw.isNullOrEmpty(nodekey)) {
                if (Csw.number(delay, -1) >= 0) {
                    previewopts.openDelay = delay;
                }
                previews[nodeid] = Csw.nbt.nodePreview(Csw.main.body, previewopts);
                previews[nodeid].open();
            }
        }); // Csw.nodeHoverIn 


    Csw.nodeHoverOut = Csw.nodeHoverOut ||
        Csw.register('nodeHoverOut', function (event, nodeid) {
            'use strict';
            if (false === Csw.isNullOrEmpty(nodeid) &&
                false === Csw.isNullOrEmpty(previews[nodeid])) {
                previews[nodeid].close(); 
                previews[nodeid] = undefined;
            }
    }); // Csw.nodeHoverOut


    Csw.nbt.nodePreview = Csw.nbt.nodePreview ||
        Csw.nbt.register('nodePreview', function (cswParent, options) {
            'use strict';
            
            var cswPrivate = {
                name: '',
                nodeid: '',
                nodekey: '',
                eventArg: {},
                openDelay: 1500,
                closeDelay: 500
            };
            Csw.extend(cswPrivate, options);
                
            var cswPublic = {};

            cswPrivate.fixDimensions = function() {
                if (cswPrivate.div) {
                    // Make sure preview div is within the window
                    var windowX = $(window).width() - 10;
                    var windowY = $(window).height() - 10;
                    var divwidth = cswPrivate.div.$.width();
                    var divheight = cswPrivate.div.$.height();
                    var X = cswPrivate.eventArg.pageX + 20; // move it to the right of the cursor, to keep from preventing click events
                    var Y = cswPrivate.eventArg.pageY;

                    if (X + divwidth > windowX) X = windowX - divwidth;
                    // this doesn't work with page scrolling
                    // if(Y + divheight > windowY) Y = windowY - divheight;

                    cswPrivate.div.css({
                        maxWidth: windowX,
                        maxHeight: windowY,
                        top: Y + 'px',
                        left: X + 'px'
                    });
                    cswPrivate.div.css('z-index', '100');
                }
            };// fixDimensions()

            cswPrivate.loadPreview = function() {
                if (cswPrivate.div) {
                    cswPrivate.div.show();

                    cswPrivate.previewTabsAndProps = Csw.layouts.tabsAndProps(cswPrivate.div, {
                        name: cswPrivate.name + 'tabs',
                        globalState: {
                            currentNodeId: cswPrivate.nodeid,
                            currentNodeKey: cswPrivate.nodekey,
                            ShowAsReport: false
                        },
                        tabState: {
                            EditMode: Csw.enums.editMode.Preview,
                            showSaveButton: false
                        },
                        AjaxWatchGlobal: false,
                        onInitFinish: function(AtLeastOneProp) {
                            cswPrivate.loadingDiv.remove();
                            if (AtLeastOneProp) {
                                cswPrivate.fixDimensions();
                            } else {
                                cswPrivate.div.hide();
                            }
                        }
                    });
                }
            }; // loadPreview()

            cswPrivate.hoverIn = function() {
                clearTimeout(cswPrivate.closeTimeoutHandle);
            }; // hoverIn()
            
            cswPrivate.hoverOut = function() {
                cswPublic.close();
            }; // hoverOut()

            
            cswPublic.open = function (options) {
                cswPrivate.openTimeoutHandle = Csw.defer(cswPrivate.loadPreview, cswPrivate.openDelay);
            }; // open()


            cswPublic.close = function () {
                clearTimeout(cswPrivate.openTimeoutHandle);
//                // Clear all node previews, in case other ones are hanging around
//                $('.CswNodePreview').remove();
                cswPrivate.closeTimeoutHandle = Csw.defer(cswPrivate.unBindThisPreview, cswPrivate.closeDelay);
            }; // close()

            // constructor
            (function() {

                cswPrivate.div = cswParent.div({
                        name: cswPrivate.name, 
                        cssclass: 'CswNodePreview' 
                    })
                    .css({
                        position: 'absolute',
                        overflow: 'auto',
                        border: '1px solid #003366',
                        padding: '2px',
                        backgroundColor: '#ffffff'
                    })
                    .hide();

                //Case 28397
                //1st: nuke any existing previews
                Csw.publish('CswUnbindOldPreviews');

                //2nd: declare the nuke _this_ preview
                cswPrivate.unBindThisPreview = function () {
                    if (cswPrivate.div) {
                        //cswPrivate.div.unbind('mouseenter mouseleave');
                        cswPrivate.div.remove();
                    }
                    if(cswPrivate.previewTabsAndProps) {
                        cswPrivate.previewTabsAndProps.tearDown();
                    }
                    Csw.unsubscribe('CswUnbindOldPreviews', null, cswPrivate.unBindThisPreview);
                };
                //3rd: bind to nuke _this_ preview
                Csw.subscribe('CswUnbindOldPreviews', cswPrivate.unBindThisPreview);
                
                cswPrivate.div.$.hover(cswPrivate.hoverIn, cswPrivate.hoverOut);

                cswPrivate.fixDimensions();

                cswPrivate.loadingDiv = cswPrivate.div.div({ 
                        text: '&nbsp;&nbsp;&nbsp;Loading...',
                        styles: { 
                            fontStyle: 'italic'
                        }
                    });
            } ()); // constructor

            return cswPublic;
    }); // Csw.controls.nodeLink
} ());
